using System;
using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text.Json.Serialization;

namespace ApplicationServer.Models
{
    public class RSAPubKey
    {
        [JsonPropertyName("modulus")]
        public string Modulus { get; set; }
        [JsonPropertyName("exponent")]
        public string Exponent { get; set; }
        public RSAParameters ToRsaParams()
        {
            return new RSAParameters { Modulus = Convert.FromBase64String(Modulus), Exponent = Convert.FromBase64String(Exponent) };
        }
    }
}