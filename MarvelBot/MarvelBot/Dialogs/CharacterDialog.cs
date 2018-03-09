using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using MarvelBot.Properties;
using CharacterParam = MarvelBot.Models.Parameters.Character;
using System.Net.Http;
using Newtonsoft.Json;
using MarvelBot.Models.Responses;
using MarvelBot.Classes;
using System.Collections.Generic;
using System.Configuration;

namespace MarvelBot.Dialogs
{
    [Serializable]
    public class CharacterDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var reply = activity.CreateReply();

            if (activity.Text == "nuevo idioma")
            {
                await context.PostAsync("¡Solo di la palabra!");
                ConfigurationManager.AppSettings["DesiredLanguage"] = String.Empty;
                context.Call(new TranslateDialog(), CallBack);
            }
            else
            {
                string newCharacter = await SearchCharacter(activity, reply);
                await context.PostAsync(reply);
                context.Wait(MessageReceivedAsync);
            }
        }

        private async Task<string> SearchCharacter(Activity activity, Activity reply)
        {
            string newCharacter = activity.Text;

            //https://stackoverflow.com/a/5788920/294804
            var parameters = new CharacterParam(Resources.PrivateKey, Resources.PublicKey)
            {
                Name = newCharacter
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
            string selectedCharacterId = serializedEntity.Data.Results[0].Id.ToString();
            string name = serializedEntity.Data.Results[0].Name;
            string content = await Translator.TranslateSentenceAsync(serializedEntity.Data.Results[0].Description, ConfigurationManager.AppSettings["DesiredLanguage"].ToString());
            var thumbnail = serializedEntity.Data.Results[0].Thumbnail;
            HeroCard myCard = await HeroCardCreation(name, content, thumbnail);

            reply.Attachments.Add(myCard.ToAttachment());
            return newCharacter;
        }

        private async Task<HeroCard> HeroCardCreation(string name, string content, Thumbnail thumbnail)
        {
            HeroCard myCard = new HeroCard
            {
                Title = name,
                Subtitle = content
            };

            List<CardImage> imageList = new List<CardImage>();
            List<CardAction> buttonsList = new List<CardAction>();
            CardImage characterImage = new CardImage(thumbnail.CreatePath("portrait_uncanny"));
            imageList.Add(characterImage);
            myCard.Images = imageList;

            CardAction seriesButton = new CardAction();
            seriesButton.Title = await Translator.TranslateSentenceAsync("Most important series", ConfigurationManager.AppSettings["DesiredLanguage"].ToString());
            seriesButton.Type = ActionTypes.ImBack;
            seriesButton.Value = "series";
            buttonsList.Add(seriesButton);

            CardAction comicsButton = new CardAction();
            comicsButton.Title = await Translator.TranslateSentenceAsync("Most important comics", ConfigurationManager.AppSettings["DesiredLanguage"].ToString());
            comicsButton.Type = ActionTypes.ImBack;
            comicsButton.Value = "comics";
            buttonsList.Add(comicsButton);
            myCard.Buttons = buttonsList;
            return myCard;
        }

        private async Task CallBack(IDialogContext context, IAwaitable<object> result)
        {
            context.Done("");
        }
    }
}