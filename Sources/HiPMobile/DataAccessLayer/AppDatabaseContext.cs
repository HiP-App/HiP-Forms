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
using PaderbornUniversity.SILab.Hip.Mobile.Shared.BusinessLayer.Models;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer
{
    public class AppDatabaseContext : DbContext
    {
        // Note: EF Core includes all types declared as DbSet<> here as well as all types found by recursively
        // exploring their navigation properties. For example, having a DbSet<Page> will also include
        // ImagePage/TextPage/etc. because Page has properties of type ImagePage/TextPage/etc.

        public DbSet<Exhibit> Exhibits { get; set; }
        public DbSet<Route> Routes { get; set; }

        public DbSet<Page> Pages { get; set; }

        public DbSet<Audio> Audios { get; set; }
        public DbSet<Image> Images { get; set; }

        public DbSet<ExhibitsVisitedAchievement> ExhibitsVisitedAchievements { get; set; }
        public DbSet<RouteFinishedAchievement> RouteFinishedAchievements { get; set; }

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