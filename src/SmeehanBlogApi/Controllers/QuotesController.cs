using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
        public QuotesController(
            IQuoteStore quoteStore,
            IOptions<QuoteOptions> options,
            ILogger<QuotesController> logger)
        {
            _quoteStore = quoteStore ?? throw new ArgumentNullException(nameof(quoteStore), "The provided IQuoteStore was null");
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options), "The options object was not configured");
            _logger = logger ?? throw new ArgumentNullException(nameof(logger), "The provided ILogger was null");
        }

        private IQuoteStore _quoteStore;
        private QuoteOptions _options;
        private ILogger<QuotesController> _logger;

        [HttpGet]
        [Route("random/{numberOfQuotes}")]
        public async Task<IActionResult> GetRandomQuoteAsync(int numberOfQuotes)
        {
            var message = $"{numberOfQuotes} quotes were requested";
            _logger.LogInformation(message);

            var endingId = 0;

            var tableDescriptionResponse = await _quoteStore.GetTableDescription().ConfigureAwait(false);
            _logger.LogTrace("retrieved table description");

            if (tableDescriptionResponse.Table.ItemCount < int.MaxValue)
            {
                endingId = Convert.ToInt32(tableDescriptionResponse.Table.ItemCount) + (_options.BeginingId - 1);
                message = $"{endingId} the ending id for the quote range";
                _logger.LogTrace(message);
            }
            else
            {
                _logger.LogWarning("There are too many guotes in the database");
                throw new ArgumentOutOfRangeException("There are too many quotes in the database");
            }

            _logger.LogTrace("Retrieving quotes from database");
            return Ok(await _quoteStore.GetRandomQuotesAsync(numberOfQuotes, endingId));
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
