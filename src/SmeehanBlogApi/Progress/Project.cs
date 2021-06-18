using Amazon.DynamoDBv2.DataModel;

namespace SmeehanBlogApi.Progress
{
    [DynamoDBTable("Progress")]
    public class Project
    {
        [DynamoDBHashKey]
        public int Id { get; set; }
        [DynamoDBRangeKey]
        public bool Active { get; set; }
        [DynamoDBProperty]
        public string Title { get; set; }
        [DynamoDBProperty]
        public int Type { get; set; }
        [DynamoDBProperty]
        public string Series { get; set; }
        [DynamoDBProperty]
        public int Status { get; set; }

        //public override string ToString()
        //{
        //    return string.Format(@"{0} – {1} Actors: {2}", Title, ReleaseYear, string.Join(", ", ActorNames.ToArray()));
        //}

    }
}
