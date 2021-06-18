using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmeehanBlogApi.DynamoDB
{
    /// <summary>
    /// This configures the code to work with DynamoDB.
    /// </summary>
    public class DynamoDBOptions
    {
        /// <summary>
        /// Specifies the AccessKey for local development.
        /// </summary>
        public string AccessKey { get; set; }

        /// <summary>
        /// Specifies the SecretKey for local development.
        /// </summary>
        public string SecretKey { get; set; }
    }
}
