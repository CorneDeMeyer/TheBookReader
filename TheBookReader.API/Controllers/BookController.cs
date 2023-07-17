using TheBookReader.DomainLogic.Interface;
using TheBookReader.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace TheBookReader.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookController : Controller
    {
        private readonly IBookReaderService _bookReaderService;

        public BookController(IBookReaderService bookReaderService) => _bookReaderService = bookReaderService;

        /// <summary>
        /// Upload Book and Get sum of words used
        /// </summary>
        /// <param name="file">Uploaded PDF File</param>
        /// <param name="retrieveWordsMinLenght">Minimum Character Length Required (Default 6 Characters)</param>
        /// <param name="retrieveTopXWords">Only Show Top X words in result (Default 50 Words)</param>
        /// <returns>Result in format of descending order for count and time took the do execute</returns>
        [ProducesResponseType(200)]
        [ProducesResponseType(501)]
        [RequestSizeLimit(52428800)]
        [HttpPost("GetBookStatistics")]
        public async Task<IActionResult> GetBookStats([FromForm] IFormFile file, [FromQuery] int retrieveWordsMinLength = 6, [FromQuery] int retrieveTopXWords = 50)
        {
            // Just some validation to ensure everything will work
            if (retrieveTopXWords <= 0)
            {
                return BadRequest("Please specify select top X words to return.");
            }
            else if (retrieveWordsMinLength <= 0)
            {
                return BadRequest("Please specify Minimum Word length to count.");
            }
            else if (!file.ContentType.Equals("application/pdf") && !file.ContentType.Equals("text/plain"))
            {
                return BadRequest("Only PDF documents type accepted.");
            }

            var result = await _bookReaderService.UploadAndGetBookStatistics(file, new FileFilter
            {
                 MinLength = retrieveWordsMinLength,
                 TopRecords = retrieveTopXWords
            });

            if (result != null)
            {
                return Ok(result);
            }

            return BadRequest($"Book details for {file.FileName}, could not be retrieved.");
        }
    }
}
