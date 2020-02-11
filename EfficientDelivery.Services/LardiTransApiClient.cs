using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace EfficientDelivery.Services
{
    public class LardiTransApiClient
    {
        private async Task<string> GetSig()
        {
            HttpClient httpClient = new HttpClient();

            var login = "abram.valentin@outlook.com";
            
            var encodedPass = "736504f7948cfca9bed550659fad0eaa";

            var responce = await httpClient
                .GetAsync($"http://api.lardi-trans.com/api/?method=auth&login={login}&password={encodedPass}");

            var content = await responce
                .Content
                .ReadAsStringAsync();

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(content);

            return  document
                .DocumentNode
                .Descendants(0)
                .Where(n => n.Name == "sig")
                .FirstOrDefault()
                ?.InnerText;
        }


        public async Task<double> CalculateRoute(string pointA, string pointB)
        {
            var sig = await GetSig();

            HttpClient httpClient = new HttpClient();


            var responce = await httpClient
                .GetAsync($"http://api.lardi-trans.com/api/?method=distance.calc&sig={sig}&towns=Луганск|Донецк");

            var content = await responce
                .Content
                .ReadAsStringAsync();

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(content);

            var distance = document
                .DocumentNode
                .Descendants(0)
                .Where(n => n.Name == "total_range")
                .FirstOrDefault()
                ?.InnerText;

            return double.Parse(distance);
        }
    }
}
