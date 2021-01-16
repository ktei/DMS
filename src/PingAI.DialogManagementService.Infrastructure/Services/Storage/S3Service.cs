using System;
using Amazon.S3;
using Amazon.S3.Model;
using PingAI.DialogManagementService.Application.Interfaces.Services.Storage;

namespace PingAI.DialogManagementService.Infrastructure.Services.Storage
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3;
        private static readonly TimeSpan UploadTimeout = TimeSpan.FromDays(60);

        public S3Service(IAmazonS3 s3)
        {
            _s3 = s3;
        }
        
        public string GetPreSignedUploadUrl(string bucket, string key)
        {
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucket,
                Key        = key,
                ContentType = "text/plain",
                Verb       = HttpVerb.PUT,
                Expires    = DateTime.UtcNow.Add(UploadTimeout)
            };

            string url = _s3.GetPreSignedURL(request);
            return url;
        }
    }
}
