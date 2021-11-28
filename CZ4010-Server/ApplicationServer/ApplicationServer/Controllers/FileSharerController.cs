using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ApplicationServer.Models;
using ApplicationServer.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ApplicationServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileSharerController : ControllerBase
    {

        private readonly IdentityRepository _identity;
        private readonly FileRepository _file;
        private readonly CoreDbContext _db;


        public FileSharerController(IdentityRepository identity, FileRepository file, CoreDbContext db)
        {
            _identity = identity;
            _file = file;
            _db = db;
        }

        [HttpGet("file")]
        public async Task<ActionResult<GetFileResponse>> GetFile([FromQuery]string url, [FromQuery]string taggedUsername, [FromQuery]string signature)
        {
            var req = new GetFileRequest(url, taggedUsername);
            var hash = req.GetSHA256();
            if (!await CheckSignature(hash, Convert.FromBase64String(signature), taggedUsername)) return Unauthorized();
            var res = await _file.GetFile(req);
            if (res is null) return NotFound();
            await StoreLog(req.ToLog());
            return Ok(res);
        }

        [HttpPost]
        public async Task<ActionResult<SubmitFileResponse>> SubmitFile(SubmitFileSignedRequest request)
        {
            var (req, signature) = request;
            var hash = req.GetSHA256();
            if (!await CheckSignature(hash, Convert.FromBase64String(signature), req.TaggedUsername))
                return Unauthorized();
            var res = await _file.StoreFile(req);
            if (res is null) return StatusCode(500);
            var log = request.ToLog();
            log.URL = res.URL;
            await StoreLog(log);
            return Ok(res);
        }
        
        [HttpPut("file")]
        public async Task<ActionResult<SubmitFileResponse>> UpdateFile(UpdateFileSignedRequest request)
        {
            var (req, signature) = request;
            var hash = req.GetSHA256();
            if (!await CheckSignature(hash, Convert.FromBase64String(signature), req.TaggedUsername))
                return Unauthorized();
            var res = await _file.UpdateFile(req);
            if (res is null) return StatusCode(500);
            await StoreLog(request.ToLog());
            return Ok(res);
        }

        [HttpDelete("file")]
        public async Task<ActionResult> DeleteFile([FromBody]DeleteFileSignedRequest request)
        {
            var (req, signature) = request;
            var hash = req.GetSHA256();
            if (!await CheckSignature(hash, Convert.FromBase64String(signature), req.TaggedUsername))
                return Unauthorized();
            var res = await _file.DeleteFile(req);
            if (!res) return NotFound();
            await StoreLog(request.ToLog());
            return StatusCode(204);
        }

        [HttpPost("share")]
        public async Task<ActionResult<SharingFileResponse>> ShareFile([FromBody]SharingFileSignedRequest request)
        {
            var (req, signature) = request;
            var hash = req.GetSHA256();
            if (!await CheckSignature(hash, Convert.FromBase64String(signature), req.CallerTaggedUsername))
                return Unauthorized();
            var res = await _file.ShareFile(req);
            if (res is null) return StatusCode(500);
            await StoreLog(request.ToLog());
            return Ok(res);
        }

        [HttpDelete("unshare")]
        public async Task<ActionResult<SharingFileResponse>> UnshareFile([FromBody]UnsharingFileSignedRequest request)
        {
            var (req, signature) = request;
            var hash = req.GetSHA256();
            if (!await CheckSignature(hash, Convert.FromBase64String(signature), req.CallerTaggedUsername))
                return Unauthorized();
            var res = await _file.UnshareFile(req);
            if (res is null) return NotFound();
            await StoreLog(request.ToLog());
            return Ok(res);
        }

        [HttpGet("logs")]
        public async Task<ActionResult<IEnumerable<AuditLogModel>>> GetLogs([FromQuery]string taggedUsername, [FromQuery]string url, [FromQuery]LogType? type, [FromQuery]int count = 10)
        {
            IQueryable<AuditLogModel> query = _db.Logs;
            if (taggedUsername is not null && taggedUsername != "")
            {
                query = query.Where(x => x.Caller == taggedUsername);
            }

            if (url is not null && url != "")
            {
                query = query.Where(x => x.URL == url);
            }

            if (type is not null)
            {
                query = query.Where(x => x.Type == type);
            }

            if (count > 100)
            {
                count = 100;
            }

            var logs = await query.Take(count).ToListAsync();
            return Ok(logs);
        }

        private async Task<bool> CheckSignature(byte[] hash, byte[] signature,string taggedUsername)
        {
            var key = await _identity.GetPublicKey(taggedUsername);
            if (key is null) return false;
            var rsa = RSA.Create();
            rsa.ImportParameters(key.ToRsaParams());
            var deformatter = new RSAPKCS1SignatureDeformatter();
            deformatter.SetKey(rsa);
            deformatter.SetHashAlgorithm("SHA256");
            return deformatter.VerifySignature(hash, signature);
        }

        private async Task<bool> StoreLog(AuditLogModel log)
        {
            _db.Logs.Add(log);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}