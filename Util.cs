using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExtraConcentratedJuice.BreakAndEnter
{
    public static class Util
    {
        public static string Translate(string TranslationKey, params object[] Placeholders) =>
            BreakAndEnter.instance.Translations.Instance.Translate(TranslationKey, Placeholders);
    }
}
