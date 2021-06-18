using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using SmeehanBlogApi.Quotes;
using System;
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
            _quoteStore = quoteStore ?? throw new ArgumentNullException(nameof(quoteStore), "The provided IQuoteStore was null");
        }

        private IQuoteStore _quoteStore;

        [HttpGet]
        [Route("random/{numberOfQuotes}")]
        public async Task<IActionResult> GetRandomQuoteAsync(int numberOfQuotes, int beginingId = 1001)
        {
            var endingId = 0;

            var tableDescriptionResponse = await _quoteStore.GetTableDescription().ConfigureAwait(false);

            if (tableDescriptionResponse.Table.ItemCount < int.MaxValue)
            {
                endingId = Convert.ToInt32(tableDescriptionResponse.Table.ItemCount) + (beginingId - 1);
            }
            else
            {
                throw new ArgumentOutOfRangeException("There are too many quotes in the database");
            }

            return Ok(await _quoteStore.GetRandomQuotesAsync(numberOfQuotes));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetQuoteAsync(int id)
        {
            var result = await _quoteStore.GetItem(id);
            
            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}
