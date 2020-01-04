using System;
using System.Collections.Generic;
using System.Text;

namespace AdsAgregator.Core.Utilities
{
    public static class Extensions
    {
        public static string RemoveEscapes(this string text)
        {
            var formatedInfo = string.Empty;


            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\"' || text[i] == '\n')
                    continue;

                formatedInfo += text[i];
            }

            return formatedInfo;
        }
    }
}
