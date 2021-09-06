namespace jwt_identity_api.Helpers.Paging
{
    public class MetaData
    {
        public int CurrentPage { get; set; }
        public int Total { get; set; }
        public int PageSize { get; set; }
        public int Count { get; set; }
        public bool HasPrevious => CurrentPage > 1;
        public bool HasNext => CurrentPage < PageSize;
    }
}