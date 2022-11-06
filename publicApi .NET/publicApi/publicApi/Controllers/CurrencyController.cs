using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using publicApi.DTOs;
using publicApi.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using Newtonsoft.Json;
using MQTTnet;
using MQTTnet.Client;
using System.Threading;

namespace publicApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private int currencyCount;

        public CurrencyController(IConfiguration configuration)
        {
            Configuration = configuration;
        }
         
        [HttpGet]
        [Route("getRandomCurrency")]
        public async Task<IActionResult> getRandomCurrency()
        {
            var returnValue = new Response();
            using (var httpClient = new HttpClient())
            {
                //host.docker.internal
                using (var response = await httpClient.GetAsync("http://host.docker.internal:3000/currency/"))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        returnValue.Currency = await response.Content.ReadFromJsonAsync<Currency>();
                    }
                    else
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        return new JsonResult(
                            new
                            {
                                response = apiResponse
                            }
                        );
                    }
                }
            }
            /*
            var apiLib = new ApiLib(Configuration["IMDBapiKey"]);
            var data = await apiLib.SearchMovieAsync(returnValue?.Movie?.Title);
            var ratings = await apiLib.RatingsAsync(data.Results[0].Id);
            returnValue!.Reviews = new
            {
                imdb = ratings.IMDb == "" ? "?" + "/10" : ratings.IMDb + "/10",
                fimAffinity = ratings.FilmAffinity == "" ? "?" + "/10" : ratings.FilmAffinity + "/10",
                theMovieDb = ratings.TheMovieDb == "" ? "?" + "/10" : ratings.TheMovieDb + "/10",
                metacritic = ratings.Metacritic == "" ? "?" + "/100" : ratings.Metacritic + "/100",
                rottenTomatoes = ratings.RottenTomatoes == "" ? "?" + "/100" : ratings.RottenTomatoes + "/100"
            };

            */
            return new JsonResult(returnValue);
        }

        [HttpGet]
        [Route("getCurrencyByID/{ID}")]
        public async Task<IActionResult> getCurrencyByID(int ID)
        {
            var returnValue = new Response();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("http://host.docker.internal:3000/currency/ID=" + ID))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        returnValue.Currency = await response.Content.ReadFromJsonAsync<Currency>();
                    }
                    else
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        return new JsonResult(
                            new
                            {
                                response = apiResponse
                            }
                        );
                    }
                }
            }
            return new JsonResult(returnValue);
        }

        [HttpGet]
        [Route("getCurrencyByTitle/{Title}")]
        public async Task<IActionResult> getCurrencyByTitle(string Title)
        {
            var returnValue = new Response();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("http://host.docker.internal:3000/currency/Sign=" + Title))
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        returnValue.Currency = await response.Content.ReadFromJsonAsync<Currency>();
                    }
                    else
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        return new JsonResult(
                            new
                            {
                                response = apiResponse
                            }
                        );
                    }
                }
            }
            return new JsonResult(returnValue);
        }

        [HttpPost]
        [Route("postCurrency")]
        public async Task<IActionResult> postMovie([FromBody] Currency newCurrency)
        {
            using (var httpClient = new HttpClient())
            {
                var c = JsonConvert.SerializeObject(newCurrency);
                StringContent content = new StringContent(c, Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("http://host.docker.internal:3000/currency/newCurrency", content))
                {
                    currencyCount++;
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    var mqtt = new
                    {
                        currencyId = newCurrency.currencyId,
                        title = newCurrency.title,
                        currency = newCurrency.currency,
                        value = newCurrency.value,
                        data = newCurrency.data

                    };
                    await publishMqtt(JsonConvert.SerializeObject(mqtt));
                    return new JsonResult(
                        new
                        {
                            response = apiResponse
                        }
                    );
                }
            }
        }

        [HttpPut]
        [Route("updateCurrency/{title}")]
        public async Task<IActionResult> updateMovie(string title, UpdateCurrency updatedCurrency)
        {
            using (var httpClient = new HttpClient())
            {
                var c = JsonConvert.SerializeObject(updatedCurrency);
                StringContent content = new StringContent(c, Encoding.UTF8, "application/json");
                using (var response = await httpClient.PutAsync("http://host.docker.internal:3000/currency/Update=" + title, content))
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    return new JsonResult(
                        new
                        {
                            response = apiResponse
                        }

                    );
                }
            }
        }


        static async Task publishMqtt(string payload)
        {
            var mqttFactory = new MqttFactory();
            using (var mqttClient = mqttFactory.CreateMqttClient())
            {
                var options = new MqttClientOptionsBuilder()
                .WithTcpServer("broker.emqx.io", 1883)
                .Build();

                await mqttClient.ConnectAsync(options, CancellationToken.None);

                var message = new MqttApplicationMessageBuilder()
                .WithTopic("inputMQTT")
                .WithPayload(payload)
                .Build();

                await mqttClient.PublishAsync(message, CancellationToken.None);
                // await mqttClient.DisconnectAsync();
            }

        }

    }

}

