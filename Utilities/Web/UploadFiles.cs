
using FluentFTP;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace EMRMS.Utilities.Web
{
    public static class UploadFiles
    {
        private static string ftpUrl = "64.62.151.106";
        private static string username = "bucket";
        private static string password = "t?I4Bxr%4zwv2bgP";
        private static string remoteDir = "httpdocs/";
        /*
        public static void UploadFile(string filePath, string remoteDirectory, string newFileName)
        {
            remoteDir += remoteDirectory;
            using (var client = new FtpClient(ftpUrl, username, password))
            {
                client.Connect();

                if (!client.DirectoryExists(remoteDir))
                {
                    client.CreateDirectory(remoteDir);
                }

                string remoteFilePath = Path.Combine(remoteDir, newFileName);
                client.UploadFile(filePath, remoteFilePath);

                client.Disconnect();
            }
        }*/
        public static async Task UploadFileAsync(string filePath, string remoteDirectory, string newFileName,
            CancellationToken cancellationToken = default)
        {
            string fullRemoteDir = Path.Combine(remoteDir, remoteDirectory);
            using (var client = new AsyncFtpClient(ftpUrl, username, password))
            {
                await client.Connect(cancellationToken);

                if (!await client.DirectoryExists(fullRemoteDir, cancellationToken))
                {
                    await client.CreateDirectory(fullRemoteDir, cancellationToken);
                }

                string remoteFilePath = Path.Combine(fullRemoteDir, newFileName);

                await client.UploadFile(filePath, remoteFilePath, token: cancellationToken);

                await client.Disconnect();
            }
        }
    }
}
