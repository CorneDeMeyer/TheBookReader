using Microsoft.AspNetCore.Http;
using TheBookReader.Domain.Models;

namespace TheBookReader.DomainLogic.Interface
{
    public interface IBookReaderService
    {
        Task<ResultModel<FileFilter, Dictionary<string, int>>> UploadAndGetBookStatistics(IFormFile file, FileFilter filter);
    }
}
