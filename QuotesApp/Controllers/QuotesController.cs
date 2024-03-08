using Microsoft.AspNetCore.Mvc;
using Quotes.Repository;
using System.ComponentModel.DataAnnotations;

namespace Quotes.Controllers
{ 
    [ApiController]
    [Route("[controller]")]
    public class QuotesController : ControllerBase
    { 
        private readonly ILogger<QuotesController> _logger;

        public QuotesController(ILogger<QuotesController> logger)
        {
            _logger = logger;
        }


        [HttpGet]
        public ActionResult<Quote> Get(
            [FromServices] IQuotesRepository quotesRepository,
            int id)
        {
            return quotesRepository.Get(id);
        }

        [HttpGet("Count")]
        public ActionResult<int> Count([FromServices] IQuotesRepository quotesRepository)
        {
            return quotesRepository.Count();
        }


        [HttpGet("Pairs")]
        public ActionResult<int> GetPairCount(
         [FromServices] IQuotesRepository quotesRepository,
         int max)
        {
            return quotesRepository.GetNumberOfPairQuotes(max);
        }


        [HttpDelete]
        public ActionResult<bool> Delete([FromServices] IQuotesRepository quotesRepository, int id)
        {
            var current = quotesRepository.Get(id);

            if (current == null)
                return NotFound();

            return quotesRepository.Delete(current);
        }

        [HttpPut]
        public ActionResult<bool> Delete([FromServices] IQuotesRepository quotesRepository, Quote quote)
        {
            var current = quotesRepository.Get(quote.Id);

            if (current == null)
                return NotFound();

            return quotesRepository.Update(quote);
        }

        [HttpPost]
        public ActionResult<Quote> Post(
            [FromServices] IQuotesRepository quotesRepository,
            [FromBody] Quote quote)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var created = quotesRepository.Add(quote);

                if (created == null)
                {
                    return StatusCode(500, "A problem happened while handling your request.");
                }

                return Ok(created);
            } 
             catch (Exception ex)
            {
                return StatusCode(500, $"A problem happened while handling your request");
            }
        }
    }
}
