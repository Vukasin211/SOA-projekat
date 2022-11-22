using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using MQTTnet.Client;
using MQTTnet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using Newtonsoft.Json;

namespace monitoring
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "monitoring", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "monitoring v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });




            // Configure the HTTP request pipeline.
            var mqttFactory = new MqttFactory();
            var mqttClient = mqttFactory.CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer("broker.emqx.io", 1883)
                .WithCleanSession()
                .Build();

            mqttClient.ConnectedAsync += async (e) =>
            {
                var topicFilter = new MqttTopicFilterBuilder()
                .WithTopic("projekatIII")
                .Build();

                await mqttClient.SubscribeAsync(topicFilter);
            };

            mqttClient.ApplicationMessageReceivedAsync += async (e) =>
            {
                var limits = app.ApplicationServices.GetRequiredService<Limits>();
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);

                dynamic toJson = JObject.Parse(payload);
                var name = toJson.readings[0].name.ToString();
                var value = toJson.readings[0].value;


                byte[] bytes = Convert.FromBase64String(value.ToString());
                Array.Reverse(bytes, 0, 8);
                var newValue = BitConverter.ToDouble(bytes);
                float floatValue = (float)newValue;

                string color = "green";

                //Slanje komande
                if (floatValue > limits.valueLimits[name][1])
                {
                    color = "blue";
                }
                else if (floatValue < limits.valueLimits[name][0])
                {
                    color = "red";
                }
                Console.WriteLine(color);


                using (var httpClient = new HttpClient())
                {
                    var paramss = new
                    {
                        color = color,
                        parameterName = name
                    };
                    var c = JsonConvert.SerializeObject(paramss);
                    StringContent content = new StringContent(c, Encoding.UTF8, "application/json");
                    using (var response = await httpClient.PutAsync("http://host.docker.internal:48082/api/v1/device/e77135b3-c62f-4829-a172-2f0a47ad577e/command/18354ec7-9870-4d7a-941b-4a732a581b7d", content))
                    {
                    }
                }
            };

            mqttClient.ConnectAsync(options, CancellationToken.None);

            app.UseHttpsRedirection();

            app.UseAuthorization();

        }
    }
}
