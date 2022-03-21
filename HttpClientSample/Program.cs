using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using HttpClientSample.Models;
namespace HttpClientSample
{
    internal class Program
    {
        static HttpClient client = new HttpClient();
        static void Main(string[] args)
        {
            client.BaseAddress = new Uri("https://localhost:44339");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
            string jsonString = String.Empty;
            Payloads payloads = new Payloads();

            Payload payload = new Payload();
            payload.Load = 480;
            Fuels fuels = new Fuels();

            fuels.gas = 13.4;
            fuels.kerosine = 50.8;
            fuels.co2 = 20;
            fuels.wind = 60;
            payload.fuels = fuels;
            List<Powerplants> powerpalants = new List<Powerplants>();

            Powerplants powerplant1 = new Powerplants();
            powerplant1.name = "gasfiredbig1";
            powerplant1.type = "gasfired";
            powerplant1.efficiency = 0.53;
            powerplant1.pmin = 100;
            powerplant1.pmax = 460;
            powerpalants.Add(powerplant1);

            Powerplants powerplant2 = new Powerplants();
            powerplant2.name = "gasfiredbig2";
            powerplant2.type = "gasfired";
            powerplant2.efficiency = 0.53;
            powerplant2.pmin = 100;
            powerplant2.pmax = 460;
            powerpalants.Add(powerplant2);

            Powerplants powerplant3 = new Powerplants();
            powerplant3.name = "gasfiredsomewhatsmaller";
            powerplant3.type = "gasfired";
            powerplant3.efficiency = 0.37;
            powerplant3.pmin = 40;
            powerplant3.pmax = 210;
            powerpalants.Add(powerplant3);

            Powerplants powerplant4 = new Powerplants();
            powerplant4.name = "tj1";
            powerplant4.type = "turbojet";
            powerplant4.efficiency = 0.3;
            powerplant4.pmin = 0;
            powerplant4.pmax = 16;
            powerpalants.Add(powerplant4);

            Powerplants powerplant5 = new Powerplants();
            powerplant5.name = "windpark1";
            powerplant5.type = "windturbine";
            powerplant5.efficiency = 1;
            powerplant5.pmin = 0;
            powerplant5.pmax = 150;
            powerpalants.Add(powerplant5);

            Powerplants powerplant6 = new Powerplants();
            powerplant6.name = "windpark2";
            powerplant6.type = "windturbine";
            powerplant6.efficiency = 1;
            powerplant6.pmin = 0;
            powerplant6.pmax = 36;
            powerpalants.Add(powerplant6);
            payload.powerplants = powerpalants;
            payloads.payloads.Add(payload);
            jsonString = JsonSerializer.Serialize(payload);


            Console.WriteLine("Load : " + payload.Load);
            Console.WriteLine("Gas : " + fuels.gas);
            Console.WriteLine("kerosine : " + fuels.kerosine);
            Console.WriteLine("co2 : " + fuels.co2);
            Console.WriteLine("wind : " + fuels.wind);
            int counter = 5;
            powerpalants.ForEach(p =>
            {
                Console.WriteLine("name : " + p.name);
                Console.WriteLine("type : " + p.type);
                Console.WriteLine("eficiency: " + p.efficiency);
                Console.WriteLine("pmin : " + p.pmin);
                Console.WriteLine("pmax : " + p.pmax);
                counter++;
            });
            Console.WriteLine("************************************");
            Console.WriteLine();
            Console.WriteLine("pres enter to run the example");
            Console.WriteLine();
            //Console.SetCursorPosition(0, counter +3);
            string result = Console.ReadLine();
                    
            string strPost = PostAsync(jsonString).GetAwaiter().GetResult();

            List<Plant_Power>  plant_Powers = JsonSerializer.Deserialize<List<Plant_Power>>(strPost);

            Console.WriteLine();
            Console.WriteLine("RESULT");
            Console.WriteLine();

            plant_Powers.ForEach(p =>
            {
                Console.WriteLine("name : " + p.name + " , " + "p" + ":" + Math.Round(p.p ,2));
            });

            Console.WriteLine();
            Console.WriteLine("example_payloads");
            Console.WriteLine();

            string strFormPL = FormatJson(jsonString);
            

            Console.WriteLine(strFormPL);
            Console.WriteLine();
            Console.WriteLine("example_response");
            Console.WriteLine();


            string strFormR = FormatJson(strPost);

            Console.WriteLine(strFormR);
            Console.SetCursorPosition(0, 0);
            Console.ReadLine();
        }
        static async Task<string> PostAsync(string jsonString)
        {
            string Str = string.Empty;
            StringContent httpContent = new StringContent(jsonString, System.Text.Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(new Uri("https://localhost:44339/api/Powerloads"), httpContent);
            if (response.IsSuccessStatusCode)
            {
                Str = await response.Content.ReadAsAsync<string>();
            }
            return Str;
        }


        private const string INDENT_STRING = "    ";

        static string FormatJson(string json)
        {
            int indentation = 0;
            int quoteCount = 0;
            var result =
                from ch in json
                let quotes = ch == '"' ? quoteCount++ : quoteCount
                let lineBreak = ch == ',' && quotes % 2 == 0 ? ch + Environment.NewLine + String.Concat(Enumerable.Repeat(INDENT_STRING, indentation)) : null
                let openChar = ch == '{' || ch == '[' ? ch + Environment.NewLine + String.Concat(Enumerable.Repeat(INDENT_STRING, ++indentation)) : ch.ToString()
                let closeChar = ch == '}' || ch == ']' ? Environment.NewLine + String.Concat(Enumerable.Repeat(INDENT_STRING, --indentation)) + ch : ch.ToString()
                select lineBreak == null
                            ? openChar.Length > 1
                                ? openChar
                                : closeChar
                            : lineBreak;

            return String.Concat(result);
        }
    }
}
//Math.Round(ch,2)