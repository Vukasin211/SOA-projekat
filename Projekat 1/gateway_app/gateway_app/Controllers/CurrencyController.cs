using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using gateway_app.DTOs;
using gateway_app.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using Newtonsoft.Json;
using MQTTnet;
using MQTTnet.Client;
using System.Threading;
using Microsoft.AspNetCore.Http;

namespace gateway_app.Controllers
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
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => { return true; };
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
        [Route("getCurrencyBySign/{sign}")]
        public async Task<IActionResult> getCurrencyByTitle(string sign)
        {
            var returnValue = new Response();
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.GetAsync("http://host.docker.internal:3000/currency/Sign=" + sign))
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> postCurrency([FromBody] Currency newCurrency)
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
                        currencyId = (int)newCurrency.currencyId,
                        title = newCurrency.title,
                        currency = newCurrency.currency,
                        value = (float)newCurrency.value,
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
        [Route("updateCurrency/{sign}")]
        public async Task<IActionResult> updateCurrency(string sign, UpdateCurrency updatedCurrency)
        {
            using (var httpClient = new HttpClient())
            {
                var c = JsonConvert.SerializeObject(updatedCurrency);
                StringContent content = new StringContent(c, Encoding.UTF8, "application/json");

                using (var response = await httpClient.PutAsync("http://host.docker.internal:3000/currency/Update=" + sign, content))
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

        [HttpGet]
        [Route("getLiveValueToEUR/{sign}")]
        public async Task<IActionResult> getLiveValueToEUR(string sign)
        {
            using (var httpClient = new HttpClient())
            {

                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://currency-exchange.p.rapidapi.com/exchange?from=EUR&to=" + sign + "&q=1.0"),
                    Headers =
                {
                    { "X-RapidAPI-Key", "03bdbe7653msh61cc5aa4979f1b8p113e0djsn23fbea4b2dcb" },
                    { "X-RapidAPI-Host", "currency-exchange.p.rapidapi.com" },
                },
                };

                UpdateCurrency updatedCurrency = new UpdateCurrency();

                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    updatedCurrency.value = float.Parse(body);
                }


                var c = JsonConvert.SerializeObject(updatedCurrency);
                StringContent content = new StringContent(c, Encoding.UTF8, "application/json");


                using (var response = await httpClient.PutAsync("http://host.docker.internal:3000/currency/Update=" + sign, content))
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

        [HttpDelete]
        [Route("deleteCurrencyById/{id}")]
        public async Task<IActionResult> deleteCurrencyById(int id)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync("http://host.docker.internal:3000/currency/DeleteId=" + id))
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
                    return new JsonResult(
                        new
                        {
                            response = apiResponse
                        }
                    );

                }
            }
        }

        [HttpDelete]
        [Route("deleteCurrencyBySign/{sign}")]
        public async Task<IActionResult> deleteCurrencyBySign(string sign)
        {
            using (var httpClient = new HttpClient())
            {
                using (var response = await httpClient.DeleteAsync("http://host.docker.internal:3000/currency/DeleteSign=" + sign))
                {
                    var apiResponse = await response.Content.ReadAsStringAsync();
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

