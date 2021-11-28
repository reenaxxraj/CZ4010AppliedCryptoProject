namespace ApplicationServer.Models
{
    public class FileDataModel
    {
        public FileDataModel(string url, string encryptedFile)
        {
            URL = url;
            EncryptedFile = encryptedFile;
        }

        public FileDataModel()
        {
        }

        public string URL { get; set; }
        public string EncryptedFile { get; set; }
    }

    public record SharingDataModel(string URL, string TaggedUsername, bool IsOwner, string EncryptedKey);
}