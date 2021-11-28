using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ApplicationServer
{
    public static class ObjectExtensionMethods
    {
        public static string ToJson(this object x)
        {
            return JsonSerializer.Serialize(x);
        }
        public static byte[] ToBytes(this object x)
        {
            return Encoding.ASCII.GetBytes(x.ToJson());
        }

        public static byte[] GetSHA256(this object x)
        {
            return SHA256.HashData(x.ToBytes());
        }
    }
}