using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using ImService;

namespace Handlers
{
    public class ImageHandler{
        private readonly DiscordSocketClient _client;

        private readonly IConfigurationRoot _config;


        public ImageHandler(DiscordSocketClient client, IConfigurationRoot config){
            _client = client;
            _config = config;
            _client.MessageReceived += HandleMessageReceived;
        }

        public async Task InitializeAsync(){
            _client.MessageReceived += HandleMessageReceived;
        }
        private async Task HandleMessageReceived(SocketMessage msg){
            
            if(msg.Attachments.Count != 0){
                var imageAttachments = msg.Attachments.Where(x =>
                x.Filename.EndsWith(".jpg")||
                x.Filename.EndsWith(".png"));

                if(imageAttachments.Any()){
                    var credentialsPath = "authtoken.json";
                    var folderId = _config["folderID"];
                    var _tokenSource = new CancellationTokenSource();
                    var cancelToken = _tokenSource.Token;
                    var service = new ImageService();


                    //First try for getting credentials and service. Keeping it here for now, in case I try with this again.
                    //var clientSecrets = await GoogleClientSecrets.FromFileAsync(credentialsPath);
                   // .CreateScoped(DriveService.ScopeConstants.Drive)
                   // .UnderlyingCredential as UserCredential;

                   //var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                   // clientSecrets.Secrets,
                   // new[] { DriveService.ScopeConstants.DriveFile },
                    //"user",
                    //CancellationToken.None);

                    //var service = new DriveService(new BaseClientService.Initializer(){
                    //    HttpClientInitializer = credential
                    //});

                    foreach (var attachment in imageAttachments){
                        using (var client = new HttpClient()){
                            var fileBytes = await client.GetByteArrayAsync(attachment.Url);
                            var fileMetadata = new Google.Apis.Drive.v3.Data.File(){
                                Name = attachment.Filename,
                                Parents = new List<string> { folderId }
                            };

                        using (var stream = new MemoryStream(fileBytes)){
                            await service.UploadFile(stream, attachment.Filename, attachment.ContentType,folderId,attachment.Filename);
                            await msg.Channel.SendMessageAsync($"Image '{attachment.Filename}' uploaded to Google Drive.");
                        }
                        }
                    }
                }
            }


        }
    }
}