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
        private readonly DbContext _db;

        public DbContextDebugView(DbContext db) => _db = db ?? throw new ArgumentNullException(nameof(db));

        public DbContextDataDebugView Data => new DbContextDataDebugView(_db);

        public DbContextChangeTrackingDebugView ChangeTracking => new DbContextChangeTrackingDebugView(_db);

        public IReadOnlyList<IEntityType> Model => _db.Model.GetEntityTypes().ToList();
    }

    [DebuggerDisplay("{_description}")]
    public class DbContextDataDebugView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DbContext _db;

        public DbContextDataDebugView(DbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IReadOnlyList<EntityTypeCollectionDebugView> EntityTypes => _db.Model.GetEntityTypes()
            .OrderBy(type => type.ClrType.Name)
            .Select(type => new EntityTypeCollectionDebugView(_db, type))
            .ToList();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _description => $"{EntityTypes.Count} entity type(s)";
    }

    /// <summary>
    /// Lists all database entities of a certain type.
    /// </summary>
    [DebuggerDisplay("{_description}")]
    public class EntityTypeCollectionDebugView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DbContext _db;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly IEntityType _entityType;

        public EntityTypeCollectionDebugView(DbContext db, IEntityType entityType)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _entityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IReadOnlyList<EntityDebugView> Entities
        {
            get
            {
                var set = (IQueryable<object>)_db.GetType().GetMethod("Set")
                    .MakeGenericMethod(_entityType.ClrType)
                    .Invoke(_db, null);

                return set
                    .AsNoTracking()
                    .Select(entity => new EntityDebugView(_db, entity))
                    .ToList();
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _description => _entityType.ClrType.Name;
    }

    /// <summary>
    /// Lists all entities currently attached to a <see cref="DbContext"/>, grouped by <see cref="EntityState"/>.
    /// </summary>
    [DebuggerDisplay("{_description}")]
    public class DbContextChangeTrackingDebugView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DbContext _db;

        public DbContextChangeTrackingDebugView(DbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IReadOnlyList<TrackedEntityCollectionDebugView> States => Enum.GetValues(typeof(EntityState))
            .Cast<EntityState>()
            .Select(state => new TrackedEntityCollectionDebugView(_db, state))
            .ToList();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _description => string.Join(", ", States
            .Where(s => s.Entries.Count > 0)
            .Select(s => $"{s.Entries.Count} {s.State}"));
    }

    /// <summary>
    /// Lists all entities that are currently being tracked in a certain state by a <see cref="DbContext"/>.
    /// </summary>
    [DebuggerDisplay("{_description}")]
    public class TrackedEntityCollectionDebugView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DbContext _db;

        public TrackedEntityCollectionDebugView(DbContext db, EntityState state)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            State = state;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public EntityState State { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IReadOnlyList<TrackedEntityDebugView> Entries => _db.ChangeTracker.Entries()
            .Where(e => e.State == State)
            .OrderBy(e => e.Entity.GetType().Name)
            .Select(e => new TrackedEntityDebugView(_db, e))
            .ToList();

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _description
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

    [DebuggerDisplay("{_description}")]
    public class TrackedEntityDebugView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DbContext _db;

        public TrackedEntityDebugView(DbContext db, EntityEntry trackingInfo)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            TrackingInfo = trackingInfo ?? throw new ArgumentNullException(nameof(trackingInfo));
        }

        public EntityEntry TrackingInfo { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public EntityDebugView Entity => new EntityDebugView(_db, TrackingInfo.Entity);

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _description => $"{TrackingInfo.Entity.GetType().Name} ({TrackingInfo.Entity})";
    }

    [DebuggerDisplay("{Entity}")]
    public class EntityDebugView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DbContext _db;

        public EntityDebugView(DbContext db, object entity)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            Entity = entity;
        }

        // Note: 'DebuggerBrowsableState.HideRoot' causes 'References' to be missing in Xamarin
        public object Entity { get; set; }

        public NavigationCollectionDebugView References => new NavigationCollectionDebugView(_db, Entity);
    }

    /// <summary>
    /// Lists the navigation properties of an entity and loads them on demand.
    /// </summary>
    [DebuggerDisplay("🔗 References")]
    public class NavigationCollectionDebugView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DbContext _db;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly object _entity;

        public NavigationCollectionDebugView(DbContext db, object entity)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _entity = entity;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public IReadOnlyList<NavigationDebugView> References => (_entity == null)
            ? ImmutableList<NavigationDebugView>.Empty
            : _db.Entry(_entity).Navigations
                .Select(nav => new NavigationDebugView(_db, nav))
                .ToList() as IReadOnlyList<NavigationDebugView>;
    }

    /// <summary>
    /// Displays a navigation property and loads it on demand.
    /// </summary>
    [DebuggerDisplay("{_description}")]
    public class NavigationDebugView
    {
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly DbContext _db;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly NavigationEntry _nav;

        public NavigationDebugView(DbContext db, NavigationEntry nav)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _nav = nav ?? throw new ArgumentNullException(nameof(nav));
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public object Value
        {
            get
            {
                // For nav.Query() we need to ensure that the entity is (at least temporarily) attached
                var isDetached = _nav.EntityEntry.State == EntityState.Detached;

                if (isDetached)
                    _nav.EntityEntry.State = EntityState.Unchanged;

                try
                {
                    switch (_nav)
                    {
                        // Note: Query() won't attach to change tracker

                        // TODO: This doesn't seem to work correctly

                        case ReferenceEntry _:
                            return new EntityDebugView(_db, _nav.IsLoaded
                                ? _nav.CurrentValue
                                : _nav.Query().Cast<object>().FirstOrDefault());

                        case CollectionEntry _:
                            var targets = _nav.IsLoaded
                                ? (IEnumerable<object>)_nav.CurrentValue
                                : _nav.Query().Cast<object>();
                            return targets.Select(e => new EntityDebugView(_db, e)).ToList();

                        default:
                            throw new NotSupportedException();
                    }
                }
                finally
                {
                    if (isDetached)
                        _nav.EntityEntry.State = EntityState.Detached;
                }
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string _description => $"🔗 {_nav.EntityEntry.Entity.GetType().Name}.{_nav.Metadata.Name}";
    }
}
