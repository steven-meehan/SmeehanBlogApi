using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.Runtime;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SmeehanBlogApi.DynamoDB
{
    public static class DynamoDBExtentions
    {
        public static IServiceCollection AddDynamoDBConnection(
            this IServiceCollection services,
            IConfiguration config,
            IWebHostEnvironment env)
        {
            services.Configure<DynamoDBOptions>(config.GetSection("DynamoDB"));

            var dynamoDBSection = config.GetSection("DynamoDB");
            var dynamoDBOptions = new DynamoDBOptions();
            dynamoDBSection.Bind(dynamoDBOptions);

            if (env.IsDevelopment()) //This will use a specified user in AWS IAM, credentials are pulled from secrets
            {
                var credentials = new BasicAWSCredentials(dynamoDBOptions.AccessKey, dynamoDBOptions.SecretKey);
                var amazonDynamoDBConfig = new AmazonDynamoDBConfig()
                {
                    RegionEndpoint = RegionEndpoint.USEast1
                };
                var amazonDynamoDBClient = new AmazonDynamoDBClient(credentials, amazonDynamoDBConfig);
                var dynamoDBContext = new DynamoDBContext(amazonDynamoDBClient, new DynamoDBContextConfig
                {
                    //setting the ConsistentRead property to true ensures you recieve the latest values
                    ConsistentRead = true,
                    SkipVersionCheck = true
                });

                services.AddSingleton<IAmazonDynamoDB>(amazonDynamoDBClient);
                services.AddSingleton<IDynamoDBContext>(dynamoDBContext);
            }
            else //This will be executed on Lambda and use the service attached to the stack
            {
                var amazonDynamoDBClient = new AmazonDynamoDBClient();
                var dynamoDBContext = new DynamoDBContext(amazonDynamoDBClient, new DynamoDBContextConfig
                {
                    //setting the ConsistentRead property to true ensures you recieve the latest values
                    ConsistentRead = true,
                    SkipVersionCheck = true
                });

                services.AddSingleton<IAmazonDynamoDB>(amazonDynamoDBClient);
                services.AddSingleton<IDynamoDBContext>(dynamoDBContext);
            }

            return services;
        }
    }
}
