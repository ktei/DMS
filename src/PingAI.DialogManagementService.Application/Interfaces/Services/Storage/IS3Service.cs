using System.Threading.Tasks;

namespace PingAI.DialogManagementService.Application.Interfaces.Services.Storage
{
    public interface IS3Service
    {
        string GetPreSignedUploadUrl(string bucket, string key);
    }
}