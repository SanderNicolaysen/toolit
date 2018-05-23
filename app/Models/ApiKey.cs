using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using Microsoft.ApplicationInsights.Extensibility.Implementation;

namespace app.Models
{
    public class ApiKey
    {
        public string Id { get; set; }

        [Required]
        [DisplayName("Name")]
        public string Name { get; set; }

        [DisplayName("Key")]
        public string Key { get; set; }

        public static string GenerateApiKey()
        {
            var bytes = new byte[32];
            using (var rand = RandomNumberGenerator.Create())
            {
                rand.GetBytes(bytes);
            }
            string key = Convert.ToBase64String(bytes);

            return key;
        }
    }
}