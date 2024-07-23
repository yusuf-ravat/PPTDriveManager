using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PPTApi.Model;
using System.IO;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using System.Threading;
using Google.Apis.Util.Store;
using System.Linq;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3.Data;

namespace PPTApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PPTUploadController : ControllerBase
    {
        private readonly PptDbContext _context;
        private readonly string _uploadFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadsPPT");

        // Google Drive credentials
        private const string CLIENT_ID = "";
        private const string CLIENT_SECRET = "";
        private const string REFRESH_TOKEN = "";

        public PPTUploadController(PptDbContext context)
        {
            _context = context;
            if (!Directory.Exists(_uploadFolderPath))
            {
                Directory.CreateDirectory(_uploadFolderPath);
            }
        }

    
        private async Task<byte[]> FileToByteArrayAsync(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }


        [HttpGet("getpptnames")]
        public IActionResult GetPPTNames()
        {
            var pptDetails = _context.PptDetails
                                     .Select(p => new
                                     {
                                         Id = p.Id,
                                         PptFileName = Path.GetFileName(p.PptFilePath),
                                         DriveFileId = p.DriveFileId,
                                         DriveFilePath = p.DriveFilepath
                                     })
                                     .ToList();

            return Ok(pptDetails);
        }




        //[HttpGet("getpptfile/{filename}")]
        //public async Task<IActionResult> GetPptFile(string filename)
        //{
        //    var filePath = Path.Combine("path/to/your/files", filename);
        //    if (!System.IO.File.Exists(filePath))
        //    {
        //        return NotFound();
        //    }

        //    var memory = new MemoryStream();
        //    using (var stream = new FileStream(filePath, FileMode.Open))
        //    {
        //        await stream.CopyToAsync(memory);
        //    }
        //    memory.Position = 0;
        //    return File(memory, "application/vnd.openxmlformats-officedocument.presentationml.presentation", filename);
        //}

        [HttpPost("upload")]
public async Task<IActionResult> UploadPPT(List<IFormFile> pptFiles)
{
    if (pptFiles == null || pptFiles.Count == 0)
    {
        return BadRequest("No files uploaded.");
    }

    var uploadResults = new List<object>();

    foreach (var pptFile in pptFiles)
    {
        var filePath = Path.Combine(_uploadFolderPath, pptFile.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await pptFile.CopyToAsync(stream);
        }

        var pptDetails = new PPTDetails
        {
            PptFilePath = filePath,
            PptFileData = await FileToByteArrayAsync(pptFile)
        };

        _context.PptDetails.Add(pptDetails);
        await _context.SaveChangesAsync();

        // Upload to Google Drive
        var driveFile = await UploadFileToGoogleDrive(filePath);

        if (driveFile == null)
        {
            return StatusCode(500, "Error uploading file to Google Drive.");
        }

        pptDetails.DriveFileId = driveFile.Id;
        pptDetails.DriveFilepath = $"https://docs.google.com/presentation/d/{driveFile.Id}/edit";
        _context.PptDetails.Update(pptDetails);
        await _context.SaveChangesAsync();

        uploadResults.Add(new
        {
            Id = pptDetails.Id,
            FilePath = pptDetails.PptFilePath,
            DriveFileId = driveFile.Id,
            GoogleSlidesLink = pptDetails.DriveFilepath
        });
    }

    return Ok(uploadResults);
}

        private async Task<Google.Apis.Drive.v3.Data.File> UploadFileToGoogleDrive(string filePath)
        {
            try
            {
                var secrets = new ClientSecrets
                {
                    ClientId = CLIENT_ID,
                    ClientSecret = CLIENT_SECRET
                };

                var tokenResponse = new TokenResponse
                {
                    RefreshToken = REFRESH_TOKEN
                };

                var credential = new UserCredential(
                    new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = secrets
                    }),
                    "user",
                    tokenResponse);

                var service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "PPTUpload"
                });

                var fileMetadata = new Google.Apis.Drive.v3.Data.File
                {
                    Name = Path.GetFileName(filePath)
                };

                FilesResource.CreateMediaUpload request;

                using (var stream = new FileStream(filePath, FileMode.Open))
                {
                    request = service.Files.Create(fileMetadata, stream, "application/vnd.openxmlformats-officedocument.presentationml.presentation");
                    request.Fields = "id";
                    await request.UploadAsync();
                }

                var uploadedFile = request.ResponseBody;

                // Set file permissions
                await SetFilePermissions(service, uploadedFile.Id);

                return uploadedFile;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }

        private async Task SetFilePermissions(DriveService service, string fileId)
        {
            var permission = new Permission
            {
                Role = "reader",
                Type = "anyone"
            };

            await service.Permissions.Create(permission, fileId).ExecuteAsync();
        }

        [HttpGet("getDriveFilePath/{driveFileId}")]
        public IActionResult GetDriveFilePath(string driveFileId)
        {
            var pptDetail = _context.PptDetails.FirstOrDefault(p => p.DriveFileId == driveFileId);
            if (pptDetail == null)
            {
                return NotFound("Drive file not found.");
            }

            return Ok(new { DriveFilePath = pptDetail.DriveFilepath });
        }


        //delete fileboth side
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeletePPT(int id)
        {
            var pptDetails = await _context.PptDetails.FindAsync(id);
            if (pptDetails == null)
            {
                return NotFound("File not found.");
            }

            // Delete from Google Drive
            var success = await DeleteFileFromGoogleDrive(pptDetails.DriveFileId);
            if (!success)
            {
                return StatusCode(500, "Error deleting file from Google Drive.");
            }

            // Delete the file record from the database
            _context.PptDetails.Remove(pptDetails);
            await _context.SaveChangesAsync();

            return Ok("File deleted successfully.");
        }

        private async Task<bool> DeleteFileFromGoogleDrive(string fileId)
        {
            try
            {
                var secrets = new ClientSecrets
                {
                    ClientId = CLIENT_ID,
                    ClientSecret =CLIENT_SECRET
                };

                var tokenResponse = new TokenResponse
                {
                    RefreshToken = REFRESH_TOKEN
                };

                var credential = new UserCredential(
                    new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
                    {
                        ClientSecrets = secrets
                    }),
                    "user",
                    tokenResponse);

                var service = new DriveService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "PPTUpload"
                });

                await service.Files.Delete(fileId).ExecuteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

    }
}
