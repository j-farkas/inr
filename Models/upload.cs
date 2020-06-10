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
using System.Net.Mail;
//using System.Windows.Forms;


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
    public static void sendMail(string url, string email)
    {
      MailMessage mail = new MailMessage();
      SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

      mail.From = new MailAddress("jaredmfarkasaws@gmail.com");
      mail.To.Add(email);
      mail.Subject = "Your Presigned Url";
      mail.Body = url;
      SmtpServer.Port = 587;
      SmtpServer.Credentials = new System.Net.NetworkCredential("hidden", "hidden");
      SmtpServer.EnableSsl = true;

      SmtpServer.Send(mail);
    }

    static string GeneratePreSignedURL(IFormFile file, string email){
      String urlString = "";
      using(var client = new AmazonS3Client("hidden", "hidden", RegionEndpoint.USWest2))
      {
        GetPreSignedUrlRequest request = new GetPreSignedUrlRequest
        {
          BucketName = "j-farkas",
          Key = file.FileName,
          Expires = DateTime.Now.AddMinutes(10)
        };
        urlString = client.GetPreSignedURL(request);
      }
        sendMail(urlString, email);
        return urlString;
    }
    public static async Task<String> UploadFileToS3(IFormFile file, string email)
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
            url = GeneratePreSignedURL(file, email);
          }
        }
      return url;
    }
  }
}
