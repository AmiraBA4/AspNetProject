using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace StudentAdminPortal.API.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        #region READ
        /// <summary>
        /// Get entity by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetById(int id);
        /// <summary>
        /// Get entity by Id Async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> GetByIdAsync(Guid id);
        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <returns>The all dataset.</returns>
        IEnumerable<T> GetAll();
        /// <summary>
        /// Find entity
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        IEnumerable<T> Find(Expression<Func<T, bool>> expression);
        /// <summary>
        /// Gets the first or default entity based on a predicate, orderby and children inclusions.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">Navigation properties separated by a comma.</param>
        /// <param name="disableTracking">A boolean to disable entities changing tracking.</param>
        /// <returns>The first element satisfying the condition.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        Task<T> GetFirstOrDefault(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            bool disableTracking = true
            );
        /// <summary>
        /// Gets the entities based on a predicate, orderby and children inclusions.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <param name="orderBy">A function to order elements.</param>
        /// <param name="include">A function to include navigation properties</param>
        /// <param name="disableTracking">A boolean to disable entities changing tracking.</param>
        /// <returns>A list of elements satisfying the condition.</returns>
        /// <remarks>This method default no-tracking query.</remarks>
        Task<IEnumerable<T>> GetMuliple(
           Expression<Func<T, bool>> predicate = null,
           Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
           Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
           bool disableTracking = true
       );
        #endregion

        #region CREATE
        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        void Add(T entity);
        /// <summary>
        /// Inserts a range of entities.
        /// </summary>
        /// <param name="entities">The entities to insert.</param>
        void AddRange(IEnumerable<T> entities);
        #endregion

        #region DELETE
        /// <summary>
        /// Deletes the entity by the specified primary key.
        /// </summary>
        /// <param name="id">The primary key value.</param>
        Task<T> Delete(object id);
        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity to delete.</param>
        void Delete(T entityToDelete);
        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        /// <param name="entities">The entities to delete.</param>
        void Delete(IEnumerable<T> entities);
        #endregion

        #region OTHER
        /// <summary>
        /// Check if an element exists for a condition.
        /// </summary>
        /// <param name="predicate">A function to test each element for a condition.</param>
        /// <returns>A boolean</returns>
        bool Exists(Expression<Func<T, bool>> predicate);
        #endregion
    }
}
