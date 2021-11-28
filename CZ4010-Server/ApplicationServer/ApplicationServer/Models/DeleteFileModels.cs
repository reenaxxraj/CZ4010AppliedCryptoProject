namespace ApplicationServer.Models
{
    public record DeleteFileSignedRequest(DeleteFileRequest Request, string Signature)
    {
        public AuditLogModel ToLog()
        {
            return new AuditLogModel(this.ToJson(), LogType.DeleteFile, Request.TaggedUsername, Request.URL);
        }
    }
    public record DeleteFileRequest(string URL, string TaggedUsername);
}