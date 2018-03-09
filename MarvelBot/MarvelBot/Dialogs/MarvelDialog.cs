using MarvelBot.Classes;
using MarvelBot.Models.Responses;
using MarvelBot.Properties;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CharacterParam = MarvelBot.Models.Parameters.Character;

namespace MarvelBot.Dialogs
{
    [Serializable]
    public class MarvelDialog : IDialog<object>
    {
        string languageCode = string.Empty;
        string newCharacter = string.Empty;
        string selectedCharacterId = string.Empty;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var reply = activity.CreateReply();

            if (String.IsNullOrEmpty(languageCode))
            {
                await DefineLanguage(activity, reply);
            }
            else if (activity.Text == "new language")
            {
                reply.Text = "Just say the word!";
                languageCode = string.Empty;
            }
            else if (activity.Text == "series" || activity.Text == "comics")
            {
                if (activity.Text == "comics")
                {
                    var basicUrl = string.Format("http://gateway.marvel.com/v1/public/characters/{0}/comics?apikey=(tu API key)&ts=(tu timestamp)&hash=(tu hash)", selectedCharacterId);

                    HttpClient request = new HttpClient();
                    var responseString = await request.GetStringAsync(basicUrl);

                    var serializedEntity = JsonConvert.DeserializeObject<Models.Rootobject>(responseString);

                    HeroCard myCard = new HeroCard
                    {
                        Title = "Comics",
                        Subtitle = await Translator.TranslateSentenceAsync(String.Format("Most important comics {0} has appeared in", newCharacter), languageCode)
                    };
                    List<CardImage> imageList = new List<CardImage>();

                }
                else if (activity.Text == "series")
                {
                    var basicUrl = string.Format("http://gateway.marvel.com/v1/public/characters/{0}/series?apikey=(tu API key)&ts=(tu timestamp)&hash=(tu hash)", selectedCharacterId);

                    HttpClient request = new HttpClient();
                    var responseString = await request.GetStringAsync(basicUrl);

                    var serializedEntity = JsonConvert.DeserializeObject<Models.Rootobject>(responseString);

                    HeroCard myCard = new HeroCard
                    {
                        Title = "Series",
                        Subtitle = await Translator.TranslateSentenceAsync(String.Format("Most important series {0} has appeared in", newCharacter), languageCode)
                    };
                    List<CardImage> imageList = new List<CardImage>();
                }
                
            }
            else
            {
                newCharacter = await SearchCharacter(activity, reply);
            }

            await context.PostAsync(reply);
            context.Wait(MessageReceivedAsync);
        }

        private async Task DefineLanguage(Activity activity, Activity reply)
        {
            languageCode = await Translator.GetDesiredLanguageAsync(activity.Text);
            reply.Text = await Translator.TranslateSentenceAsync("Hi! About which 'Marvel' character you would like to learn ?", languageCode);
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
            selectedCharacterId = serializedEntity.Data.Results[0].Id.ToString();
            string name = serializedEntity.Data.Results[0].Name;
            string content = await Translator.TranslateSentenceAsync(serializedEntity.Data.Results[0].Description, languageCode);
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
            seriesButton.Title = await Translator.TranslateSentenceAsync("Most important series", languageCode);
            seriesButton.Type = ActionTypes.ImBack;
            seriesButton.Value = "series";
            buttonsList.Add(seriesButton);

            CardAction comicsButton = new CardAction();
            comicsButton.Title = await Translator.TranslateSentenceAsync("Most important comics", languageCode);
            comicsButton.Type = ActionTypes.ImBack;
            comicsButton.Value = "comics";
            buttonsList.Add(comicsButton);
            myCard.Buttons = buttonsList;
            return myCard;
        }
    }
}