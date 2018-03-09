using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using MarvelBot.Classes;
using System.Configuration;

namespace MarvelBot.Dialogs
{
    [Serializable]
    public class TranslateDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await StartConversation(context);
        }

        private async Task StartConversation(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            string responseString = string.Empty;
            var query = activity.Text.ToString();
            string languageCode = ConfigurationManager.AppSettings["DesiredLanguage"].ToString();

            if (String.IsNullOrEmpty(languageCode))
            {
                await DefineLanguage(activity, context);
            }
        }

        private async Task DefineLanguage(Activity activity, IDialogContext reply)
        {
            ConfigurationManager.AppSettings["DesiredLanguage"] = await Translator.GetDesiredLanguageAsync(activity.Text);
            var finalReply = await Translator.TranslateSentenceAsync("Hi! About which 'Marvel' character you would like to learn ?", ConfigurationManager.AppSettings["DesiredLanguage"].ToString());
            await reply.PostAsync(finalReply);
            reply.Call(new CharacterDialog(), CallBack);
        }

        private async Task CallBack(IDialogContext context, IAwaitable<object> result)
        {
            context.Done("");
        }
    }
}