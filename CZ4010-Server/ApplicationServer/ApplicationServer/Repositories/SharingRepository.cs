using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationServer.Models;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace ApplicationServer.Repositories
{
    public class SharingRepository
    {
        private readonly CoreDbContext _dbContext;

        public SharingRepository(CoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<string> GetEncryptedKey(string url, string taggedUsername)
        {
            var key = await _dbContext.Sharing
                .Where(x => x.URL == url && x.TaggedUsername == taggedUsername)
                .Select(x => x.EncryptedKey)
                .FirstOrDefaultAsync();
            return key;
        }

        public async Task<bool> CreateNewFileSharing(string url, string taggedUsername, string encryptedKey)
        {
            var line = new SharingDataModel(url, taggedUsername, true, encryptedKey);
            _dbContext.Sharing.Add(line);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<string>> AddEncryptedKeys(SharingFileRequest model)
        {
            if (!await IsOwner(model.URL, model.CallerTaggedUsername)) return null;
            var lines = model.TaggedUsernamesToShareWith.Select(x =>
                new SharingDataModel(model.URL, x.TaggedUsername, false, x.EncryptedKey))
                .ToList();
            await _dbContext.Sharing.AddRangeAsync(lines);
            await _dbContext.SaveChangesAsync();
            return lines.Select(x => x.TaggedUsername);
        }

        public async Task<IEnumerable<string>> RemoveEncryptedKeys(UnsharingFileRequest request)
        {
            if (!await IsOwner(request.URL, request.CallerTaggedUsername)) return null;
            await _dbContext.Sharing
                .Where(x => x.URL == request.URL && request.TaggedUsernamesToRemove.Contains(x.TaggedUsername))
                .DeleteAsync();
            return request.TaggedUsernamesToRemove;
        }
        
        public async Task<bool> DeleteKeys(string url, string taggedUsername)
        {
            if (!await IsOwner(url, taggedUsername)) return false;
            await _dbContext.Sharing.Where(x => x.URL == url).DeleteAsync();
            return true;
        }

        public async Task<bool> IsOwner(string url, string taggedUsername)
        {
            var key = await _dbContext.Sharing
                .Where(x => x.URL == url && x.TaggedUsername == taggedUsername)
                .Select(x => x.IsOwner)
                .FirstOrDefaultAsync();
            return key;
        }
    }
}