using System;
using System.Collections.Generic;
using System.Linq;

namespace PingAI.DialogManagementService.Api.Models
{
    public class ErrorsDto
    {
        private readonly List<string> _errors;

        public ErrorsDto(IEnumerable<string> errors)
        {
            _errors = (errors ?? throw new ArgumentNullException(nameof(errors))).ToList();
        }

        public ErrorsDto(string error) : this(new[]{error})
        {
            
        }

        public string[] Errors => _errors.ToArray();
    }
}