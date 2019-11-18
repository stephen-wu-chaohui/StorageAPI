using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Storage.Core;
using Storage.S3;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace StorageAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : Controller
    {
        private readonly ICloudStorage cloudStorage;

        public FilesController(ICloudStorage cloudStorage)
        {
            this.cloudStorage = cloudStorage;
        }

        // GET: api/files
        [HttpGet]
        [Route("{bucketName}")]
        public async Task<ActionResult<IResponse>> ListAsync(string bucketName)
        {
            var result = await cloudStorage.ListAsync(bucketName);
            return result.StatusCode == ServiceStatusCode.OK ? Ok(result.Files) : ErrorResult(result);
        }

        // GET: api/files
        [HttpGet]
        [Route("{bucketName}/{key}")]
        public async Task<ActionResult<IResponse>> GetObjectAsync(string bucketName, string key)
        {
            var result = await cloudStorage.ListAsync(bucketName, new string[] { key });
            if (result.StatusCode == ServiceStatusCode.OK)
            {
                if (result.Files.Any())
                {
                    return Ok(result.Files.First());
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return ErrorResult(result);
            }
        }

        // POST api/files
        [HttpPost]
        [Route("{bucketName}/add")]
        public async Task<ActionResult<IResponse>> AddFiles(string bucketName, IList<IFormFile> formFiles)
        {
            var uploadItems = formFiles.Select(ff => new UploadItem(ff)).Cast<IUploadItem>().ToList();
            var result = await cloudStorage.UploadAsync(bucketName, uploadItems);
            return result.StatusCode == ServiceStatusCode.OK? Ok(result) : ErrorResult(result);
        }

        private ActionResult<IResponse> ErrorResult(IResponse result)
        {
            return result.StatusCode switch
            {
                ServiceStatusCode.OK => Ok(),
                ServiceStatusCode.BucketDoesNotExist => BadRequest("Bucket doesn't exist"),
                ServiceStatusCode.InvalidBucketName => BadRequest("Bucket name is invalid"),
                ServiceStatusCode.HttpStatusCode => StatusCode(StatusCodes.Status500InternalServerError, result),
                ServiceStatusCode.InvalidSettings => StatusCode(StatusCodes.Status500InternalServerError, "Invalid settings"),
                _ => StatusCode(StatusCodes.Status500InternalServerError, "Unexpect internal status"),
            };
        }
    }
}
