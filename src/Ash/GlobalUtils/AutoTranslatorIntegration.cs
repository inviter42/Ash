using XUnity.AutoTranslator.Plugin.Core;

namespace Ash.GlobalUtils
{
    internal static class AutoTranslatorIntegration
    {
        internal static string Translate(string text) {
            var translated = text;

            AutoTranslator.Default.TranslateAsync(text, result => {
                translated = result.Succeeded
                    ? result.TranslatedText
                    : result.ErrorMessage;
            });

            return translated;
        }
    }
}
