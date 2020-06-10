using System;
using Microsoft.AspNetCore.Http;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using Amazon.S3.Util;
using Amazon;


namespace inr.Models
{
  public class S3Response
  {
    public HttpStatusCode Status { get; set;}
    public string Message { get; set; }
  }
  public class Uploader
  {
    public string email {get; set;}
    public IFormFile file {get; set;}

    public Uploader(string getEmail, IFormFile getFile)
    {
      email = getEmail;
      file = getFile;
    }

    public static async Task<S3Response> UploadFileToS3(IFormFile file)
    {
      Console.WriteLine("Started the function");
      try
      {
        using(var client = new AmazonS3Client("AKIAJ2LLUB3Z4IYCWB2Q", "v+jYIxOXSjnc06Mer4ynSKMrSvy2GsS95DFpy62q", RegionEndpoint.USWest2))
        {
          using (var newMemoryStream = new MemoryStream())
          {
            file.CopyTo(newMemoryStream);

            var uploadRequest = new TransferUtilityUploadRequest
            {
              InputStream = newMemoryStream,
              Key = file.FileName,
              BucketName = "j-farkas",
              CannedACL = S3CannedACL.PublicRead
            };
            var fileTransferUtility = new TransferUtility(client);
            await fileTransferUtility.UploadAsync(uploadRequest);
          }
        }
      }
      catch (AmazonS3Exception e)
      {
        Console.WriteLine(e.Message);
        return new S3Response
        {
          Message = e.Message,
          Status = e.StatusCode
        };
      }

      return new S3Response
      {
        Message = "upload succesful",
        Status = HttpStatusCode.OK
      };
    }
  }
}
