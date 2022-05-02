using BardMidiApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BardMidiApi
{
    public static class EnumerableExtensions
    {
        public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageNumber, int rowsPerPage)
        {
            return query.Skip(pageNumber * rowsPerPage).Take(rowsPerPage);
        }

        public static async Task<ApiPaginatedList<T>> AsPaginatedListAsync<T>(this IQueryable<T> query, int numResults, int pageNumber, int rowsPerPage)
        {
            var list = await query.Paginate(pageNumber, rowsPerPage).ToListAsync();
            return list.AsPaginated(numResults, pageNumber, rowsPerPage);
        }
        
        public static ApiPaginatedList<T> AsPaginated<T>(this List<T> target, int count, int page, int rowsPerPage)
        {
            return new ApiPaginatedList<T>(target, count, page, rowsPerPage);
        }
    }
}
