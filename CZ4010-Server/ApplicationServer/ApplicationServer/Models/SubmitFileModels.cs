using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ApplicationServer.Models
{
    public record SubmitFileSignedRequest(SubmitFileRequest Request, string Signature){
        public AuditLogModel ToLog()
        {
            return new AuditLogModel(this.ToJson(), LogType.UploadFile, Request.TaggedUsername, null);
        }
    };

    public record SubmitFileRequest(string EncryptedFile, string EncryptedKey, string TaggedUsername);

    public record SubmitFileResponse(string URL);

}