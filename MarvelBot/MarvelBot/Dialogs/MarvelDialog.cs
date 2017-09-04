using MarvelBot.Classes;
using MarvelBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MarvelBot.Dialogs
{
    [Serializable]
    public class MarvelDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            string newCharacter = activity.Text;
            string basicUrl = Helpers.FirstPathBuilder("characters", "9", "name=" + newCharacter);
            HttpClient request = new HttpClient();
            var responseString = await request.GetStringAsync(basicUrl);

            var serializedEntity = JsonConvert.DeserializeObject<Rootobject>(responseString);
            string name = serializedEntity.data.results[0].name;
            string content = serializedEntity.data.results[0].description;
            var thumbnail = serializedEntity.data.results[0].thumbnail;


            try
            {
                var reply = activity.CreateReply();
                reply.Attachments = new List<Attachment>();

                reply.Attachments.Add(new Attachment
                {
                    Name = name,
                    Content = content,
                    ContentUrl = Helpers.ImagePathBuilder(thumbnail.path, thumbnail.extension, "portrait_uncanny"),
                    ContentType = "image/jpg"
                });


                await context.PostAsync(reply);
            }
            catch (Exception)
            {

                throw;
            }


            context.Wait(MessageReceivedAsync);
        }
    }
}