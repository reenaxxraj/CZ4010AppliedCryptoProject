using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using IdentityServer.Models;

namespace IdentityServer
{
    public record CreateIdentitySignedRequest(CreateIdentityRequest Request, string Signature);

    public record CreateIdentityRequest(RSAPubKey PublicKey, string Username)
    {
        public byte[] ToBytes()
        {
            return Encoding.ASCII.GetBytes(JsonSerializer.Serialize(this));
        }

        public byte[] GetSHA256()
        {
            return SHA256.HashData(ToBytes());
        }
    };

    public record CreateIdentityResponse(RSAPubKey PublicKey, string TaggedUsername);
}