using System;
using System.Collections.Generic;
using System.Linq;

namespace PingAI.DialogManagementService.Api.Models
{
    public class ErrorsDto
    {
        public List<string> Errors { get; set; } = new List<string>();

        public ErrorsDto(IEnumerable<string> errors)
        {
            Errors = (errors ?? throw new ArgumentNullException(nameof(errors))).ToList();
        }

        public ErrorsDto(string error) : this(new[]{error})
        {
            
        }

        public ErrorsDto()
        {
            
        }
    }
}