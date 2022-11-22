using CsvHelper;
using System.IO;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;


using (var streamReader = new StreamReader(@"C:\Users\vukia\Desktop\SOA proj\Projekat\Moje\Projekat 1\helper_app\exchange_rates.csv"))
{
    using (var csvReader = new CsvReader(streamReader, CultureInfo.InvariantCulture))
    {

        var records = csvReader.GetRecords<Currency>().ToList();
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        var count = 0;
        while (await timer.WaitForNextTickAsync())
        {

            var serializedObject = JsonConvert.SerializeObject(records[count]);
            var currency = new StringContent(serializedObject, Encoding.UTF8, "application/json");
            using (var httpClient = new HttpClient())
            {
                await httpClient.PostAsync($"https://localhost:49157/Currency/postCurrency", currency);
            }
            Console.WriteLine(count + " / 63");
            count++;

            if (count > records.Count - 1)
            {
                timer.Dispose();
            }
        }
    }
}


public class Currency
{
    [Name("currencyId")]
    public int currencyId { get; set; }
    [Name("title")]
    public string title { get; set; }
    [Name("currency")]
    public string currency { get; set; }
    [Name("value")]
    public float value { get; set; }
    [Name("date")]
    public string data { get; set; }
}


