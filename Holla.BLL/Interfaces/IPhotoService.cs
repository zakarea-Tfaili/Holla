using System.IO;
using System.Threading.Tasks;
using CloudinaryDotNet.Actions;

namespace Holla.BLL.Interfaces
{
    public interface IPhotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(Stream fileStream, string fileName);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}