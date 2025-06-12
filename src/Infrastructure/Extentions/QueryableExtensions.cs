using TwitterAPI.Responses;
using Microsoft.EntityFrameworkCore;

namespace Domain.Extentions
{
    public static class QueryableExtensions
    {
        public static async Task<(List<T>, PaginationData)> ToPaginatedListAsync<T>(this IQueryable<T> queryable, int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber <= 0)
            {
                pageNumber = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 10;
            }

            if (pageSize > 40)
            {
                pageSize = 40;
            }


            var paginationMetadata = await queryable.GetPaginationMetadataAsync(pageNumber, pageSize);
            queryable = queryable.ApplyPagination(pageNumber, pageSize);
            var entidades = await queryable.ToListAsync();

            return (entidades, paginationMetadata);
        }

        private static async Task<PaginationData> GetPaginationMetadataAsync<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            if (pageSize <= 0)
            {
                pageSize = 1;
            }

            int total = await query.CountAsync();

            var isPageNumberLessThanOne = pageNumber < 1;
            pageNumber = isPageNumberLessThanOne ? 1 : pageNumber;

            var isPageSizeLessThanZero = pageSize < 0;
            pageSize = isPageSizeLessThanZero ? 0 : pageSize;

            return new PaginationData()
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = total,
                TotalPages = (int)Math.Ceiling((double)total / pageSize),
            };
        }

        private static IQueryable<T> ApplyPagination<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            return query.Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize);
        }
    }
}
