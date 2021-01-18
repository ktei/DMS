using System;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using PingAI.DialogManagementService.Application.Interfaces.Services.Storage;

namespace PingAI.DialogManagementService.Infrastructure.Services.Storage
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3;
        private static readonly TimeSpan UploadTimeout = TimeSpan.FromMinutes(10);

        public S3Service(IAmazonS3 s3)
        {
            _s3 = s3;
        }
        
        public string GetPreSignedUploadUrl(string bucket, string contentType, string key)
        {
            AWSConfigsS3.UseSignatureVersion4 = true;
            var preSignedUrl = _s3.GetPreSignedURL(new GetPreSignedUrlRequest
            {
                BucketName = bucket,
                Key = key,
                ContentType = contentType,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.Add(UploadTimeout)
            });

            return preSignedUrl;
        }
    }
}
