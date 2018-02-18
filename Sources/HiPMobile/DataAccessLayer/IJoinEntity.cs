using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer
{
    /// <summary>
    /// Supports relational many-to-many relationships. A many-to-many relationship between
    /// two types A and B is usually modeled through a third "join type/table" J containing
    /// one row for each connection between A and B. This type J should implement this
    /// interface twice, once for type A and once for type B.
    /// </summary>
    public interface IJoinEntity<TEntity>
    {
        TEntity Navigation { get; set; }
    }

    /// <summary>
    /// Supports relational many-to-many relationships of the same entity type. Such a
    /// many-to-many relationship between is usually modeled through a "join type/table"
    /// J containing one row for each connection. This type J should implement this interface.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TOtherEntity"></typeparam>
    /// <typeparam name="TJoinEntity">The join type</typeparam>
    public interface IJoinEntitySameType<TEntity>
    {
        TEntity this[JoinSide side] { get; set; }
    }

    public enum JoinSide { A, B }

    /// <summary>
    /// Supports relational many-to-many relationships of the same entity type. Such a
    /// many-to-many relationship between is usually modeled through a "join type/table"
    /// J containing one row for each connection. This facade allows to get, establish
    /// and remove connections without the need to deal with the join type J.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TOtherEntity"></typeparam>
    /// <typeparam name="TJoinEntity">The join type</typeparam>
    public struct JoinCollectionFacade<TEntity, TJoinEntity> : ICollection<TEntity>
        where TJoinEntity : IJoinEntitySameType<TEntity>, new()
    {
        private readonly TEntity _ownerEntity;
        private readonly ICollection<TJoinEntity> _collection;
        private readonly JoinSide _navigationTarget;
        private static int target;

        public JoinCollectionFacade(TEntity ownerEntity, ICollection<TJoinEntity> collection, JoinSide navigationTarget)
        {
            _ownerEntity = ownerEntity;
            _collection = collection;
            _navigationTarget = navigationTarget;
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            var target = _navigationTarget;
            return _collection.Select(e => e[target]).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TEntity item)
        {
            var entity = new TJoinEntity();
            entity[_navigationTarget] = item;
            entity[_navigationTarget == JoinSide.A ? JoinSide.B : JoinSide.A] = _ownerEntity;
            _collection.Add(entity);
        }

        public void Clear() => _collection.Clear();

        public bool Contains(TEntity item)
        {
            var target = _navigationTarget;
            return _collection.Any(e => Equals(item, e, target));
        }

        public void CopyTo(TEntity[] array, int arrayIndex) => this.ToList().CopyTo(array, arrayIndex);

        public bool Remove(TEntity item)
        {
            var target = _navigationTarget;
            return _collection.Remove(_collection.FirstOrDefault(e => Equals(item, e, target)));
        }

        public int Count => _collection.Count;

        public bool IsReadOnly => _collection.IsReadOnly;

        private static bool Equals(TEntity item, TJoinEntity e, JoinSide target) => Equals(e[target], item);
    }

    /// <summary>
    /// Supports relational many-to-many relationships. A many-to-many relationship between
    /// two types A and B is usually modeled through a third "join type/table" J containing
    /// one row for each connection between A and B. This facade allows to get, establish
    /// and remove A-B-connections without the need to deal with the join type J.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TOtherEntity"></typeparam>
    /// <typeparam name="TJoinEntity">The join type</typeparam>
    public struct JoinCollectionFacade<TEntity, TOtherEntity, TJoinEntity> : ICollection<TEntity>
        where TJoinEntity : IJoinEntity<TEntity>, IJoinEntity<TOtherEntity>, new()
    {
        private readonly TOtherEntity _ownerEntity;
        private readonly ICollection<TJoinEntity> _collection;

        public JoinCollectionFacade(TOtherEntity ownerEntity, ICollection<TJoinEntity> collection)
        {
            _ownerEntity = ownerEntity;
            _collection = collection;
        }

        public IEnumerator<TEntity> GetEnumerator() =>
            _collection.Select(e => ((IJoinEntity<TEntity>)e).Navigation).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TEntity item)
        {
            var entity = new TJoinEntity();
            ((IJoinEntity<TEntity>)entity).Navigation = item;
            ((IJoinEntity<TOtherEntity>)entity).Navigation = _ownerEntity;
            _collection.Add(entity);
        }

        public void Clear() => _collection.Clear();

        public bool Contains(TEntity item) => _collection.Any(e => Equals(item, e));

        public void CopyTo(TEntity[] array, int arrayIndex) => this.ToList().CopyTo(array, arrayIndex);

        public bool Remove(TEntity item) => _collection.Remove(_collection.FirstOrDefault(e => Equals(item, e)));

        public int Count => _collection.Count;

        public bool IsReadOnly => _collection.IsReadOnly;

        private static bool Equals(TEntity item, TJoinEntity e) => Equals(((IJoinEntity<TEntity>)e).Navigation, item);
    }
}
