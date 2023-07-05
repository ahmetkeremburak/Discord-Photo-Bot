using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

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

        private async Task HandleMessageReceived(SocketMessage msg){
            
            if(msg.Attachments.Count != 0){
                var imageAttachments = msg.Attachments.Where(x =>
                x.Filename.EndsWith(".jpg")||
                x.Filename.EndsWith(".png"));

                if(imageAttachments.Any()){
                    var credentialsPath = _config["credentials"];
                    var folderId = _config["folderID"]; // Replace with actual Google Drive folder ID

                    var credential = GoogleCredential.FromFile(credentialsPath)
                    .CreateScoped(DriveService.ScopeConstants.Drive)
                    .UnderlyingCredential as UserCredential;

                    var service = new DriveService(new BaseClientService.Initializer(){
                        HttpClientInitializer = credential
                    });

                    foreach (var attachment in imageAttachments){
                        using (var client = new HttpClient()){
                            var fileBytes = await client.GetByteArrayAsync(attachment.Url);
                            var fileMetadata = new Google.Apis.Drive.v3.Data.File(){
                                Name = attachment.Filename,
                                Parents = new List<string> { folderId }
                            };

                        using (var stream = new MemoryStream(fileBytes)){
                            var request = service.Files.Create(fileMetadata, stream, attachment.ContentType);
                            request.Fields = "id";
                            var file = await request.UploadAsync();
                            //file.ToString file.id yerine geldi. ne kadar işe yarar bilmiyorum.
                            await msg.Channel.SendMessageAsync($"Image '{attachment.Filename}' uploaded to Google Drive. File ID: {file.ToString}");
                        }
                        }
                    }
                }
            }


        }
    }
}