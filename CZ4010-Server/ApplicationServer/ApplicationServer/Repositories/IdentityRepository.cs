using System;
using System.Net.Http;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using ApplicationServer.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace ApplicationServer.Repositories
{
    public class IdentityRepository
    {
        private readonly IMemoryCache _cache;

        private HttpClient _client = new HttpClient();
        
        private readonly string IdentityServerAddress = Environment.GetEnvironmentVariable("IDENTITY_SERVER_HOST")??"identity";

        public IdentityRepository(IMemoryCache cache)
        {
            _cache = cache;
            
        }

        private readonly MemoryCacheEntryOptions _options = new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.High,
            SlidingExpiration = TimeSpan.FromMinutes(5)
        };
        
        public async Task<RSAPubKey> GetPublicKey(string taggedUsername)
        {
            if (_cache.TryGetValue(taggedUsername, out RSAPubKey pubKey)) return pubKey;
            taggedUsername = UrlEncoder.Default.Encode(taggedUsername);
            var response =
                await _client.GetAsync($"http://{IdentityServerAddress}/Identity/GetIdentity?taggedUsername={taggedUsername}");
            if (!response.IsSuccessStatusCode) return null;
            var array = await response.Content.ReadAsByteArrayAsync();
            var key = JsonSerializer.Deserialize<RSAPubKey>(array);
            _cache.Set(taggedUsername, key, _options);
            return key;
        }
    }
}