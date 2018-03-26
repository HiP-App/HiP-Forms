using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PaderbornUniversity.SILab.Hip.Mobile.Shared.DataAccessLayer
{
    // Currently, EF Core does not have built-in support for many-to-many relationships (such as between exhibits and
    // pages), so these need to be manually modeled through two one-to-many relationships and an intermediate so-called
    // join type, which makes it awkward to program against our model classes, though.
    //
    // The types in this file make it easier to deal with such relationships by "bridging" the intermediate join entities.
    // For example, this allows us to write "myExhibit.Pages.Add(myPage)" without needing to construct the join entity
    // that holds the foreign keys to "myExhibit" and "myPage".
    //
    // Code in this file is mostly taken from the following blog post:
    // https://blog.oneunicorn.com/2017/09/25/many-to-many-relationships-in-ef-core-2-0-part-4-a-more-general-abstraction/
    // It has been extended to support many-to-many relationships between entities of the same type.
    // The feature request for "automatic" many-to-many relationship mapping is being tracked here:
    // https://github.com/aspnet/EntityFrameworkCore/issues/1368

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
    /// many-to-many relationship is usually modeled through a "join type/table" J containing
    /// one row for each connection. This type J should implement this interface.
    /// </summary>
    public interface IJoinEntitySameType<TEntity>
    {
        TEntity this[JoinSide side] { get; set; }
    }

    public enum JoinSide { A, B }

    /// <summary>
    /// Supports relational many-to-many relationships of the same entity type. Such a
    /// many-to-many relationship is usually modeled through a "join type/table" J
    /// containing one row for each connection. This facade is a "shortcut" which allows
    /// to get, establish and remove connections without the need to deal with the join type J.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TJoinEntity">The join type</typeparam>
    public struct JoinCollectionFacade<TEntity, TJoinEntity> : ICollection<TEntity>
        where TJoinEntity : IJoinEntitySameType<TEntity>, new()
    {
        private readonly TEntity ownerEntity;
        private readonly ICollection<TJoinEntity> collection;
        private readonly JoinSide navigationTarget;

        public JoinCollectionFacade(TEntity ownerEntity, ICollection<TJoinEntity> collection, JoinSide navigationTarget)
        {
            this.ownerEntity = ownerEntity;
            this.collection = collection;
            this.navigationTarget = navigationTarget;
        }

        public IEnumerator<TEntity> GetEnumerator()
        {
            var target = navigationTarget;
            return collection.Select(e => e[target]).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TEntity item)
        {
            var entity = new TJoinEntity();
            entity[navigationTarget] = item;
            entity[navigationTarget == JoinSide.A ? JoinSide.B : JoinSide.A] = ownerEntity;
            collection.Add(entity);
        }

        public void Clear() => collection.Clear();

        public bool Contains(TEntity item)
        {
            var target = navigationTarget;
            return collection.Any(e => Equals(item, e, target));
        }

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if (collection.Count > array.Length - arrayIndex)
                throw new ArgumentException("Insufficient space in target array");

            var i = 0;
            var target = navigationTarget;

            foreach (var item in collection)
            {
                array[arrayIndex + i] = item[target];
                i++;
            }
        }

        public bool Remove(TEntity item)
        {
            var target = navigationTarget;
            return collection.Remove(collection.FirstOrDefault(e => Equals(item, e, target)));
        }

        public int Count => collection.Count;

        public bool IsReadOnly => collection.IsReadOnly;

        private static bool Equals(TEntity item, TJoinEntity e, JoinSide target) => Equals(e[target], item);
    }

    /// <summary>
    /// Supports relational many-to-many relationships. A many-to-many relationship between
    /// two types A and B is usually modeled through a third "join type/table" J containing
    /// one row for each connection between A and B. This facade is a "shortcut" which allows
    /// to get, establish and remove A-B-connections without the need to deal with the join type J.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TOtherEntity"></typeparam>
    /// <typeparam name="TJoinEntity">The join type</typeparam>
    public struct JoinCollectionFacade<TEntity, TOtherEntity, TJoinEntity> : ICollection<TEntity>
        where TJoinEntity : IJoinEntity<TEntity>, IJoinEntity<TOtherEntity>, new()
    {
        private readonly TOtherEntity ownerEntity;
        private readonly ICollection<TJoinEntity> collection;

        public JoinCollectionFacade(TOtherEntity ownerEntity, ICollection<TJoinEntity> collection)
        {
            this.ownerEntity = ownerEntity;
            this.collection = collection;
        }

        public IEnumerator<TEntity> GetEnumerator() =>
            collection.Select(e => ((IJoinEntity<TEntity>)e).Navigation).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public void Add(TEntity item)
        {
            var entity = new TJoinEntity();
            ((IJoinEntity<TEntity>)entity).Navigation = item;
            ((IJoinEntity<TOtherEntity>)entity).Navigation = ownerEntity;
            collection.Add(entity);
        }

        public void Clear() => collection.Clear();

        public bool Contains(TEntity item) => collection.Any(e => Equals(item, e));

        public void CopyTo(TEntity[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentNullException(nameof(array));

            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));

            if (collection.Count > array.Length - arrayIndex)
                throw new ArgumentException("Insufficient space in target array");

            var i = 0;
            foreach (var item in collection)
                array[arrayIndex + i++] = ((IJoinEntity<TEntity>)item).Navigation;
        }

        public bool Remove(TEntity item) => collection.Remove(collection.FirstOrDefault(e => Equals(item, e)));

        public int Count => collection.Count;

        public bool IsReadOnly => collection.IsReadOnly;

        private static bool Equals(TEntity item, TJoinEntity e) => Equals(((IJoinEntity<TEntity>)e).Navigation, item);
    }
}
