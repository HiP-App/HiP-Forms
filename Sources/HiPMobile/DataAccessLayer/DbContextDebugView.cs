using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.EntityFrameworkCore
{
    /// <summary>
    /// Provides properties that allow the developer to drill into the current
    /// database data and change tracking information during a debug session.
    /// </summary>
    [DebuggerDisplay("🔧 Inspect data and tracked entities")]
    public class DbContextDebugView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DbContext db;

        public DbContextDebugView(DbContext db) => this.db = db ?? throw new ArgumentNullException(nameof(db));

        public DbContextDataDebugView Data => new DbContextDataDebugView(db);

        public DbContextChangeTrackingDebugView ChangeTracking => new DbContextChangeTrackingDebugView(db);

        public IReadOnlyList<IEntityType> Model => db.Model.GetEntityTypes().ToList();
    }

    [DebuggerDisplay("{Description}")]
    public class DbContextDataDebugView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DbContext db;

        public DbContextDataDebugView(DbContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IReadOnlyList<EntityTypeCollectionDebugView> EntityTypes => db.Model.GetEntityTypes()
            .OrderBy(type => type.ClrType.Name)
            .Select(type => new EntityTypeCollectionDebugView(db, type))
            .ToList();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string Description => $"{EntityTypes.Count} entity type(s)";
    }

    /// <summary>
    /// Lists all database entities of a certain type.
    /// </summary>
    [DebuggerDisplay("{Description}")]
    public class EntityTypeCollectionDebugView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DbContext db;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IEntityType entityType;

        public EntityTypeCollectionDebugView(DbContext db, IEntityType entityType)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.entityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IReadOnlyList<EntityDebugView> Entities
        {
            get
            {
                var set = (IQueryable<object>)db.GetType().GetMethod("Set")
                    .MakeGenericMethod(entityType.ClrType)
                    .Invoke(db, null);

                return set
                    .AsNoTracking()
                    .Select(entity => new EntityDebugView(db, entity))
                    .ToList();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string Description => entityType.ClrType.Name;
    }

    /// <summary>
    /// Lists all entities currently attached to a <see cref="DbContext"/>, grouped by <see cref="EntityState"/>.
    /// </summary>
    [DebuggerDisplay("{Description}")]
    public class DbContextChangeTrackingDebugView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DbContext db;

        public DbContextChangeTrackingDebugView(DbContext db)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IReadOnlyList<TrackedEntityCollectionDebugView> States => Enum.GetValues(typeof(EntityState))
            .Cast<EntityState>()
            .Select(state => new TrackedEntityCollectionDebugView(db, state))
            .ToList();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string Description => string.Join(", ", States
            .Where(s => s.Entries.Count > 0)
            .Select(s => $"{s.Entries.Count} {s.State}"));
    }

    /// <summary>
    /// Lists all entities that are currently being tracked in a certain state by a <see cref="DbContext"/>.
    /// </summary>
    [DebuggerDisplay("{Description}")]
    public class TrackedEntityCollectionDebugView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DbContext db;

        public TrackedEntityCollectionDebugView(DbContext db, EntityState state)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            State = state;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public EntityState State { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IReadOnlyList<TrackedEntityDebugView> Entries => db.ChangeTracker.Entries()
            .Where(e => e.State == State)
            .OrderBy(e => e.Entity.GetType().Name)
            .Select(e => new TrackedEntityDebugView(db, e))
            .ToList();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string Description
        {
            get
            {
                var groups = Entries
                    .GroupBy(e => e.TrackingInfo.Entity.GetType())
                    .Select(g => $"{g.Count()}x {g.First().TrackingInfo.Entity.GetType().Name}")
                    .ToList();

                return $"{State}: {Entries.Count}" + (groups.Count > 0 ? $" ({string.Join(", ", groups)})" : "");
            }
        }
    }

    [DebuggerDisplay("{Description}")]
    public class TrackedEntityDebugView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DbContext db;

        public TrackedEntityDebugView(DbContext db, EntityEntry trackingInfo)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            TrackingInfo = trackingInfo ?? throw new ArgumentNullException(nameof(trackingInfo));
        }

        public EntityEntry TrackingInfo { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public EntityDebugView Entity => new EntityDebugView(db, TrackingInfo.Entity);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string Description => $"{TrackingInfo.Entity.GetType().Name} ({TrackingInfo.Entity})";
    }

    [DebuggerDisplay("{Entity}")]
    public class EntityDebugView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DbContext db;

        public EntityDebugView(DbContext db, object entity)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            Entity = entity;
        }

        // Note: 'DebuggerBrowsableState.HideRoot' causes 'References' to be missing in Xamarin
        public object Entity { get; set; }

        public NavigationCollectionDebugView References => new NavigationCollectionDebugView(db, Entity);
    }

    /// <summary>
    /// Lists the navigation properties of an entity and loads them on demand.
    /// </summary>
    [DebuggerDisplay("🔗 References")]
    public class NavigationCollectionDebugView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DbContext db;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object entity;

        public NavigationCollectionDebugView(DbContext db, object entity)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.entity = entity;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IReadOnlyList<NavigationDebugView> References => (entity == null)
            ? ImmutableList<NavigationDebugView>.Empty
            : db.Entry(entity).Navigations
                .Select(nav => new NavigationDebugView(db, nav))
                .ToList() as IReadOnlyList<NavigationDebugView>;
    }

    /// <summary>
    /// Displays a navigation property and loads it on demand.
    /// </summary>
    [DebuggerDisplay("{Description}")]
    public class NavigationDebugView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DbContext db;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly NavigationEntry nav;

        public NavigationDebugView(DbContext db, NavigationEntry nav)
        {
            this.db = db ?? throw new ArgumentNullException(nameof(db));
            this.nav = nav ?? throw new ArgumentNullException(nameof(nav));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object Value
        {
            get
            {
                // For nav.Query() we need to ensure that the entity is (at least temporarily) attached
                var isDetached = nav.EntityEntry.State == EntityState.Detached;

                if (isDetached)
                    nav.EntityEntry.State = EntityState.Unchanged;

                try
                {
                    switch (nav)
                    {
                        // Note: Query() won't attach to change tracker

                        // TODO: This doesn't seem to work correctly

                        case ReferenceEntry _:
                            return new EntityDebugView(db, nav.IsLoaded
                                ? nav.CurrentValue
                                : nav.Query().Cast<object>().FirstOrDefault());

                        case CollectionEntry _:
                            var targets = nav.IsLoaded
                                ? (IEnumerable<object>)nav.CurrentValue
                                : nav.Query().Cast<object>();
                            return targets.Select(e => new EntityDebugView(db, e)).ToList();

                        default:
                            throw new NotSupportedException();
                    }
                }
                finally
                {
                    if (isDetached)
                        nav.EntityEntry.State = EntityState.Detached;
                }
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string Description => $"🔗 {nav.EntityEntry.Entity.GetType().Name}.{nav.Metadata.Name}";
    }
}
