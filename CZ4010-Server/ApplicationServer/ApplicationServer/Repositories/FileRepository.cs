using System;
using System.Linq;
using System.Threading.Tasks;
using ApplicationServer.Models;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace ApplicationServer.Repositories
{
    public class FileRepository
    {
        private readonly SharingRepository _repository;
        private readonly CoreDbContext _dbContext;

        public FileRepository(SharingRepository repository, CoreDbContext dbContext)
        {
            _repository = repository;
            _dbContext = dbContext;
        }
        public async Task<GetFileResponse> GetFile(GetFileRequest request)
        {
            var key = await _repository.GetEncryptedKey(request.Url, request.TaggedUsername);
            if (key is null) return null;
            var file = await _dbContext.Files.FindAsync(request.Url);
            if (file is null) return null;
            return new GetFileResponse(file.EncryptedFile, key);
        }

        public async Task<SubmitFileResponse> StoreFile(SubmitFileRequest request)
        {
            var url = await GetNewURL();
            if (!await _repository.CreateNewFileSharing(url, request.TaggedUsername, request.EncryptedKey)) return null;
            _dbContext.Files.Add(new FileDataModel(url, request.EncryptedFile));
            await _dbContext.SaveChangesAsync();
            return new SubmitFileResponse(url);
        }
        
        public async Task<UpdateFileResponse> UpdateFile(UpdateFileRequest request)
        {
            var (url, encryptedFile, taggedUsername) = request;
            if (!await _repository.IsOwner(url, taggedUsername)) return null;
            await _dbContext.Files.Where(x => x.URL == url)
                .UpdateAsync(x => new FileDataModel{EncryptedFile = encryptedFile});
            return new UpdateFileResponse(url);
        }

        public async Task<bool> DeleteFile(DeleteFileRequest request)
        {
            if (!await _repository.DeleteKeys(request.URL, request.TaggedUsername)) return false;
            await _dbContext.Files.Where(x => x.URL == request.URL).DeleteAsync();
            return true;
        }

        public async Task<SharingFileResponse> ShareFile(SharingFileRequest request)
        {
            var models = await _repository.AddEncryptedKeys(request);
            if (models is null) return null;
            return new SharingFileResponse(request.URL, models);
        }
        
        public async Task<SharingFileResponse> UnshareFile(UnsharingFileRequest request)
        {
            var models = await _repository.RemoveEncryptedKeys(request);
            if (models is null) return null;
            return new SharingFileResponse(request.URL, models);
        }

        private async Task<string> GetNewURL()
        {
            var url = GenerateURL();
            while (await _dbContext.Files.AnyAsync(x => x.URL == url))
            {
                url = GenerateURL();
            }
            return url;
        }

        private string GenerateURL()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[10];
            var random = new Random();

            for (var i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new string(stringChars);
            return finalString;
        }
    }
}