using System;
using System.Collections.Generic;
using System.Linq;

namespace PingAI.DialogManagementService.Api.Models
{
    public class ErrorsDto
    {
        public List<string> Errors { get; set; } = new List<string>();
        public string? ErrorCode { get; set; }
        
        public ErrorsDto(IEnumerable<string> errors, string? errorCode = default(string?))
        {
            Errors = (errors ?? throw new ArgumentNullException(nameof(errors))).ToList();
            ErrorCode = errorCode;
        }

        public ErrorsDto(string error) : this(new[]{error})
        {
            
        }
        
        public ErrorsDto(string error, string? errorCode) : this(new[]{error}, errorCode)
        {
            
        }

        public ErrorsDto()
        {
            
        }
    }
}