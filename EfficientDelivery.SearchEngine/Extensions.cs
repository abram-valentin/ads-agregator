using System.Linq;
using System.Web;

namespace EfficientDelivery.SearchEngine
{
    public static class Extensions
    {
        public static string RemoveEscapes(this string text)
        {
            var formatedInfo = string.Empty;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == '\"' || text[i] == '\n')
                    formatedInfo += " ";
                else
                    formatedInfo += text[i];
            }

            var str = formatedInfo.Split(" ");

            formatedInfo = string.Join(" ", str.Where(s => !string.IsNullOrEmpty(s)));

            return formatedInfo;
        }


    }
}
