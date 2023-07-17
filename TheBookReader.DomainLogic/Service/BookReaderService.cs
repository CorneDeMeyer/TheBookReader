using TheBookReader.DomainLogic.Interface;
using Microsoft.Extensions.Logging;
using TheBookReader.Domain.Models;
using iTextSharp.text.pdf.parser;
using Microsoft.AspNetCore.Http;
using iTextSharp.text.pdf;
using System.Diagnostics;
using System.Text;

namespace TheBookReader.DomainLogic.Service
{
    public class BookReaderService : IBookReaderService
    {
        private readonly ILogger _logger;


        public BookReaderService(ILoggerFactory factory)
        {
            _logger = factory.CreateLogger<BookReaderService>();
        }


        public async Task<ResultModel<FileFilter, Dictionary<string, int>>> UploadAndGetBookStatistics(IFormFile file, FileFilter filter)
        {
            // Get Duration of Calculation
            var timer = new Stopwatch();
            timer.Start();

            var content = file.FileName.Contains(".pdf") 
                ? await GetAllPdfContent(file)
                : await GetAllTextContent(file);
            if (!string.IsNullOrEmpty(content.Key) && content.Value > 0)
            {
                var wordCounts = new CalculationService(content.Key).CalculateWordUsage(filter.MinLength, filter.TopRecords);
                timer.Stop();
                return new ResultModel<FileFilter, Dictionary<string, int>>
                {
                    Filter = filter,
                    ResultSet =  wordCounts,
                    Duration = timer.Elapsed,
                    BookTitle = file.FileName,
                    NumberOfPages = content.Value,
                };
            } 
            else
            {
                timer.Stop();
                return new ResultModel<FileFilter, Dictionary<string, int>>
                {
                    Filter = filter,
                    Duration = timer.Elapsed,
                    BookTitle = file.FileName,
                    Errors = new string[] { content.Key }
                };
            }
        }

        private async Task<KeyValuePair<string, int>> GetAllPdfContent(IFormFile file)
        {
            var content = string.Empty;
            var noPages = 0;
            try
            {
                // Create Memory Stream to read in the PDF's content
                using (var fileStream = new MemoryStream())
                {
                    await file.CopyToAsync(fileStream);
                    // Move Stream to correct posistion
                    fileStream.Position = 0;
                    // This could be improved, never worked with opening and reading a file like this before
                    StringBuilder text = new StringBuilder();
                    using (PdfReader reader = new PdfReader(fileStream))
                    {
                        noPages = reader.NumberOfPages;
                        // Reading page by page
                        for (int i = 1; i <= reader.NumberOfPages; i++)
                        {
                            text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
                        }
                    }
                    content = text.ToString();
                }
            }
            // Just incase we have issues, log the error atleast
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reading book content in file: {file.Name}.");
                content = ex.Message;
                noPages = -1;
            }

            // Return data relevant to the pdf
            return new KeyValuePair<string, int>(content, noPages);
        }

        private async Task<KeyValuePair<string, int>> GetAllTextContent(IFormFile file)
        {
            var content = string.Empty;
            var noPages = 1;

            try
            {
                using (var fileStream = new MemoryStream())
                {
                    await file.CopyToAsync(fileStream);
                    fileStream.Position = 0;
                    using (var sr = new StreamReader(fileStream))
                    {
                        // Read the stream as a string, and write the string to the console.
                        content = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error reading file content in file: {file.Name}.");
                content = ex.Message;
                noPages = -1;
            }

            // Return data relevant to the pdf
            return new KeyValuePair<string, int>(content, noPages);
        }
    }
}
