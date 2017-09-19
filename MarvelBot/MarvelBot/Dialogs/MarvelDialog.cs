using MarvelBot.Classes;
using MarvelBot.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using MarvelBot.Models.Responses;
using MarvelBot.Properties;
using CharacterParam = MarvelBot.Models.Parameters.Character;

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
            //string basicUrl = Helpers.FirstPathBuilder("characters", "9", "name=" + newCharacter);

            var parameters = new CharacterParam(Resources.PrivateKey, Resources.PublicKey)
            {
                Name = newCharacter,
                TimeStamp = 1
            };

            var section = "characters";
            //https://stackoverflow.com/a/14517976/294804
            var uriBuilder = new UriBuilder($"http://gateway.marvel.com/v1/public/{section}")
            {
                Query = parameters.ToString()
            };
            var basicUrl = uriBuilder.ToString();

            HttpClient request = new HttpClient();
            var responseString = await request.GetStringAsync(basicUrl);

            var serializedEntity = JsonConvert.DeserializeObject<Response<Character>>(responseString);
            string name = serializedEntity.Data.Results[0].Name;
            string content = serializedEntity.Data.Results[0].Description;
            var thumbnail = serializedEntity.Data.Results[0].Thumbnail;


            try
            {
                var reply = activity.CreateReply();
                reply.Attachments = new List<Attachment>
                {
                    new Attachment
                    {
                        Name = name,
                        Content = content,
                        ContentUrl = Helpers.ImagePathBuilder(thumbnail.Path, thumbnail.Extension, "portrait_uncanny"),
                        ContentType = "image/jpg"
                    }
                };



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