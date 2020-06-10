using System;
using Microsoft.AspNetCore.Http;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using Amazon.S3.Util;
using Amazon;
using Amazon.S3.Model;


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

    static string GeneratePreSignedURL(IFormFile file){
      String urlString = "";
      using(var client = new AmazonS3Client("AKIAJ2LLUB3Z4IYCWB2Q", "v+jYIxOXSjnc06Mer4ynSKMrSvy2GsS95DFpy62q", RegionEndpoint.USWest2))
      {
        GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
        {
          BucketName = "j-farkas",
          Key = file.FileName,
          Expires = DateTime.Now.AddMinutes(10)
        };
        urlString = client.GetPreSignedURL(request);
      }
        return urlString;
    }
    public static async Task<String> UploadFileToS3(IFormFile file)
    {
      String url = "";
      Console.WriteLine("Started the function");
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
            url = GeneratePreSignedURL(file);
          }
        }
      return url;
    }
  }
}
