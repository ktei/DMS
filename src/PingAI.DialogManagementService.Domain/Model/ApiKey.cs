using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace PingAI.DialogManagementService.Domain.Model
{
    public class ApiKey : ValueObject
    {
        private readonly string? _key;
        private const int KeyLength = 32;

        public string? Key => _key;
        
        private ApiKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException($"{nameof(key)} must not be empty");
            if (key.Trim().Length != KeyLength)
                throw new ArgumentException($"{nameof(key)} must have length of {KeyLength}");
            _key = key.Trim();
        }

        private ApiKey()
        {
        }
        
        public static readonly ApiKey Empty = new ApiKey();

        public bool IsEmpty => string.IsNullOrEmpty(_key);

        public static ApiKey GenerateNew()
        {
            var key = new byte[KeyLength];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(key);
            var keyStr = string.Join("", Convert.ToBase64String(key).Take(32));
            return new ApiKey(keyStr);
        }
        
        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return _key ?? string.Empty;
        }

        public static implicit operator string?(ApiKey apiKey) => apiKey?.Key;

        public static explicit operator ApiKey(string apiKey) => new ApiKey(apiKey);
    }
}