using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmeehanBlogApi.Quotes
{
    public static class QuoteExtentions
    {

        public static IServiceCollection AddQuotes(
            this IServiceCollection services, 
            IConfiguration config)
        {
            var quoteSection = config.GetSection("Quote");
            var quoteOptions = new QuoteOptions();
            quoteSection.Bind(quoteOptions);

            services.Configure<QuoteOptions>(config.GetSection("Quote"));

            if (quoteOptions.MockStore)
            {
                services.AddScoped<IQuoteStore, MockQuoteStore>();
            }
            else
            {
                services.AddScoped<IQuoteStore, QuoteStore>();
            }

            return services;
        }
    }
}
