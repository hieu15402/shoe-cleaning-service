namespace TP4SCS.Library.Models.Response.General
{
    public class Pagination
    {
        public Pagination(int totalItems, int pageSize, int currentPage, int totalPages)
        {
            TotalItems = totalItems;
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalPages = totalPages;
        }

        public int TotalItems { get; set; }

        public int PageSize { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }
    }
}
