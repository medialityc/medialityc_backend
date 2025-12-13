namespace Medialityc.Utils.Extensions
{
    public static class PaginatedExtensions
    {
        public static IEnumerable<T> PaginatePage<T>(this IEnumerable<T> source, int page, int pageSize)
        {
            if (page <= 0) 
                page = 1;
            if (pageSize <= 0) 
                pageSize = 10;
            return source.Skip((page - 1) * pageSize).Take(pageSize);
        }
    }
}