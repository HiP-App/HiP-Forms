// Copyright (C) 2017 History in Paderborn App - Universität Paderborn
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models.JoinClasses;
using System.Collections;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer
{
    public partial class AppDatabaseContext : DbContext
    {
        // Note: EF Core includes all types declared as DbSet<> here as well as all types found by recursively
        // exploring their navigation properties.

        public DbSet<Exhibit> Exhibits { get; set; }
        public DbSet<Route> Routes { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Media> Media { get; set; }
        public DbSet<AchievementBase> Achievements { get; set; }
        public DbSet<AchievementPendingNotification> AchievementPendingNotifications { get; set; }

        public object DebugView => new DbContextDebugView { Db = this };

        public AppDatabaseContext(QueryTrackingBehavior changeTrackingBehavior)
        {
            ChangeTracker.QueryTrackingBehavior = changeTrackingBehavior;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=" + EFCoreDataAccess.DbPath);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Route>()
                .HasMany(r => r.Waypoints).WithOne(w => w.Route);

            // configure inheritance hierarchies
            // (see https://docs.microsoft.com/en-us/ef/core/modeling/relational/inheritance)
            modelBuilder.Entity<Media>()
                .HasDiscriminator<string>("Type")
                .HasValue<Image>(nameof(Image))
                .HasValue<Audio>(nameof(Audio));

            modelBuilder.Entity<AchievementBase>()
                .HasDiscriminator<string>("Type")
                .HasValue<ExhibitsVisitedAchievement>(nameof(ExhibitsVisitedAchievement))
                .HasValue<RouteFinishedAchievement>(nameof(RouteFinishedAchievement));

            modelBuilder.Entity<Page>()
                .HasDiscriminator<PageType>(nameof(Page.PageType))
                .HasValue<AppetizerPage>(PageType.AppetizerPage)
                .HasValue<TextPage>(PageType.TextPage)
                .HasValue<ImagePage>(PageType.ImagePage)
                .HasValue<TimeSliderPage>(PageType.TimeSliderPage);

            // configure composite primary keys of join tables
            // (see https://docs.microsoft.com/en-us/ef/core/modeling/keys)
            modelBuilder.Entity<JoinExhibitPage>()
                .HasKey(j => new { j.ExhibitId, j.PageId });

            modelBuilder.Entity<JoinPagePage>()
                .HasKey(j => new { j.PageId, j.AdditionalInformationPageId });

            modelBuilder.Entity<JoinRouteTag>()
                .HasKey(j => new { j.RouteId, j.TagId });

            modelBuilder.Entity<Waypoint>()
                .HasKey(j => new { j.RouteId, j.ExhibitId });

            // configure many-to-many-relationships of the same type
            modelBuilder.Entity<JoinPagePage>(page =>
            {
                page.HasOne(j => j.Page).WithMany(p => p.AdditionalInformationPagesRefs);
                page.HasOne(j => j.AdditionalInformationPage);
            });
        }

        /// <summary>
        /// Saves all pending changes and then clears the list of tracked entities.
        /// </summary>
        public void SaveChangesAndDetach()
        {
            SaveChanges();

            foreach (var entry in ChangeTracker.Entries().ToList())
                entry.State = EntityState.Detached;
        }
    }
}