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
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer
{
    /// <summary>
    /// Provides properties that allow the developer to drill into the current
    /// database data and change tracking information during a debug session.
    /// </summary>
    [DebuggerDisplay("🔧 Inspect data and tracked entities")]
    public class DbContextDebugView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public DbContext Db { get; set; }

        public DataView Data => new DataView
        {
            Db = Db,
            EntityTypes = Db.Model.GetEntityTypes()
                .OrderBy(type => type.ClrType.Name)
                .Select(type => new DataView.EntityTypeView
                {
                    Db = Db,
                    EntityType = type
                })
                .ToList()
        };

        public ChangeTrackingView ChangeTracking => new ChangeTrackingView
        {
            States = Enum.GetValues(typeof(EntityState))
                .Cast<EntityState>()
                .Select(state => new ChangeTrackingView.EntityStateView
                {
                    State = state,
                    Entries = Db.ChangeTracker.Entries()
                        .Where(e => e.State == state)
                        .OrderBy(e => e.Entity.GetType().Name)
                        .Select(e => new ChangeTrackingView.EntityStateView.EntryView { Db = Db, TrackingInfo = e })
                        .ToList()
                })
                .ToList()
        };

        [DebuggerDisplay("{Description}")]
        public class DataView
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public DbContext Db { get; set; }

            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public string Description => $"{EntityTypes.Count} entity type(s)";

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public IReadOnlyList<EntityTypeView> EntityTypes { get; set; }

            [DebuggerDisplay("{EntityTypeName}")]
            public class EntityTypeView
            {
                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                public DbContext Db { get; set; }

                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                public IEntityType EntityType { get; set; }

                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                public string EntityTypeName => EntityType.ClrType.Name;

                [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
                public IReadOnlyList<object> Entities
                {
                    get
                    {
                        var set = (IQueryable<object>)Db.GetType().GetMethod("Set")
                            .MakeGenericMethod(EntityType.ClrType)
                            .Invoke(Db, null);

                        return set
                            .Select(entity => new EntityView
                            {
                                Db = Db,
                                Entity = entity
                            }).ToList();
                    }
                }
            }
        }

        [DebuggerDisplay("{Description}")]
        public class ChangeTrackingView
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public string Description => string.Join(", ", States
                .Where(s => s.Entries.Count > 0)
                .Select(s => $"{s.Entries.Count} {s.State}"));

            [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
            public IReadOnlyList<EntityStateView> States { get; set; }

            [DebuggerDisplay("{Description}")]
            public class EntityStateView
            {
                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                public EntityState State { get; set; }

                [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                public string Description => $"{State}: {Entries.Count}";

                [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
                public IReadOnlyList<EntryView> Entries { get; set; }

                [DebuggerDisplay("{Description}")]
                public class EntryView
                {
                    public DbContext Db { get; set; }

                    public EntityEntry TrackingInfo { get; set; }

                    public EntityView Entity => new EntityView
                    {
                        Db = Db,
                        Entity = TrackingInfo.Entity
                    };

                    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                    public string Description => $"{TrackingInfo.Entity.GetType().Name} ({TrackingInfo.Entity})";
                }
            }
        }

        [DebuggerDisplay("{Entity}")]
        public class EntityView
        {
            [DebuggerBrowsable(DebuggerBrowsableState.Never)]
            public DbContext Db { get; set; }

            public object Entity { get; set; }

            public ReferenceListView References => new ReferenceListView
            {
                References = Db.Entry(Entity).Navigations
                    .Select(nav =>
                    {
                        nav.Load();
                        return new ReferenceListView.ReferenceView
                        {
                            Name = $"🔗 {Entity.GetType().Name}.{nav.Metadata.Name}",
                            Value = (nav.CurrentValue is IEnumerable<object> collection)
                                ? collection.Select(entity => new EntityView { Db = Db, Entity = entity }).ToList()
                                : new EntityView { Db = Db, Entity = nav.CurrentValue } as object
                        };
                    }).ToList()
            };

            [DebuggerDisplay("🔗 References")]
            public class ReferenceListView
            {
                [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
                public IReadOnlyList<ReferenceView> References { get; set; }

                [DebuggerDisplay("{Name}")]
                public class ReferenceView
                {
                    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
                    public string Name { get; set; }
                    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
                    public object Value { get; set; }
                }
            }
        }
    }
}
