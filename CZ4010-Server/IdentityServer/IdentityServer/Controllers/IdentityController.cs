using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using IdentityServer.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly IdentityDbContext _dbContext;

        public IdentityController(IdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        [HttpGet("GetUsernames")]
        public async Task<ActionResult<IEnumerable<string>>> GetMultiIdentity([FromQuery] string username)
        {
            if (!IsAlphaNumeric(username, new List<char> { '~' }))
                return BadRequest("Query string should be alphanumeric or '~'!");
            var entries = await _dbContext.Identities
                .Where(x => EF.Functions.Like(x.TaggedUsername, "%"+username+"%"))
                .Select(x => x.TaggedUsername)
                .ToListAsync();
            return entries;
        }
        
        [HttpGet("GetMultiIdentity")]
        public async Task<ActionResult<IEnumerable<IdentityModel>>> GetMultiIdentity([FromQuery] string[] taggedUsernames)
        {
            var entries = await _dbContext.Identities
                .Where(x => taggedUsernames.Contains(x.TaggedUsername))
                .ToListAsync();
            return entries;
        }

        [HttpGet("GetIdentity")]
        public async Task<ActionResult<RSAPubKey>> GetIdentity([FromQuery] string taggedUsername)
        {
            var key = (await _dbContext.Identities.FindAsync(taggedUsername))?.PublicKey;
            return key is not null? Ok(key) : NotFound();
        }
        
        [HttpPost("CreateIdentity")]
        public async Task<ActionResult<CreateIdentityResponse>> CreateIdentity([FromBody] CreateIdentitySignedRequest request)
        {
            if (!VerifyRequest(request)) return Unauthorized(request.Request.ToBytes());
			if(request.Request.Username is null) return BadRequest("Username was empty!");
            if (!IsAlphaNumeric(request.Request.Username)) return BadRequest("Username was not alphanumeric!");
            var taggedUsername = request.Request.Username + GenerateTag();
            while (await _dbContext.Identities.AnyAsync(x => x.TaggedUsername == taggedUsername))
            {
                taggedUsername = request.Request.Username + GenerateTag();
            }
            await _dbContext.Identities.AddAsync(new IdentityModel(taggedUsername, request.Request.PublicKey));
            await _dbContext.SaveChangesAsync();
            return Ok(new CreateIdentityResponse(request.Request.PublicKey, taggedUsername));
        }

        private static bool IsAlphaNumeric(string s, ICollection<char> chars = null)
        {
			if (s is null) return true;
            return s.All(x => char.IsLetterOrDigit(x) || (chars?.Contains(x)??false));
        }

        private static bool VerifyRequest(CreateIdentitySignedRequest request)
        {
            var (createIdentityRequest, signature) = request;
            var publicKey = createIdentityRequest.PublicKey.ToRsaParams();
            var rsa = RSA.Create();
            rsa.ImportParameters(publicKey);
            var deformatter = new RSAPKCS1SignatureDeformatter(rsa);
            deformatter.SetHashAlgorithm("SHA256");
            var hash = createIdentityRequest.GetSHA256();
            return deformatter.VerifySignature(hash, Convert.FromBase64String(signature));
        }

        private static string GenerateTag()
        {
            var rng = new Random();
            return $"~{rng.Next(9999):D4}";
        }
    }
}