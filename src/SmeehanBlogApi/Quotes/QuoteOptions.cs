namespace SmeehanBlogApi.Quotes
{
    public class QuoteOptions
    {
        public bool MockStore { get; set; } = false;
        public int BeginingId { get; } = 1001;
        public string TableName { get; set; } = "Quote";
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
    }
}
