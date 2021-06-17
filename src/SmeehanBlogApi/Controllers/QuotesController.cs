using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmeehanBlogApi.Quotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmeehanBlogApi.Controllers
{
    [Produces("application/json")]
    [Route("api/quote")]
    [EnableCors("MyPolicy")]
    public class QuotesController : Controller
    {
        public QuotesController(IQuoteStore quoteStore)
        {
            _quoteStore = quoteStore;
        }

        private IQuoteStore _quoteStore;

        [HttpGet]
        [Route("random/{numberOfQuotes}")]
        public async Task<IEnumerable<Quote>> GetRandomQuoteAsync(int numberOfQuotes)
        {
            return await _quoteStore.GetRandomQuotesAsync(numberOfQuotes);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<Quote> GetQuoteAsync(int id)
        {
            return await _quoteStore.GetItem(id);
        }
    }
}
