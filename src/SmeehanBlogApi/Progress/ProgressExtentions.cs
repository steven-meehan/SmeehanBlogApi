using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace SmeehanBlogApi.Progress
{
    public static class ProgressExtentions
    {
        public static IServiceCollection AddProgress(
            this IServiceCollection services,
            IConfiguration config,
            IWebHostEnvironment env)
        {
            services.Configure<ProgressOptions>(config.GetSection("Progress"));

            var progressSection = config.GetSection("Progress");
            var progressOptions = new ProgressOptions();
            progressSection.Bind(progressOptions);

            if (env.IsDevelopment()) //This will allow the use of the MockQuoteStore
            {
                if (progressOptions.MockStore)
                {
                    services.AddScoped<IProgressStore, MockProgressStore>();
                }
                else
                {
                    services.AddScoped<IProgressStore, ProgressStore>();
                }
            }
            else //This will be executed on Lambda and use the service attached to the stack
            {
                services.AddScoped<IProgressStore, ProgressStore>();
            }

            return services;
        }
    }
}
