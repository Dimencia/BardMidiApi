namespace BardMidiApi.Models
{
    public class ApiPaginatedList<T>
    {
        public ApiPaginatedList(List<T> items, int count, int pageIndex, int rowsPerPage)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)rowsPerPage);
            TotalItems = count;
            Items = items;
        }

        public int PageIndex { get; set; }

        public int TotalPages { get; set; }

        public int TotalItems { get; set; }

        public List<T> Items { get; set; }
    }
}
