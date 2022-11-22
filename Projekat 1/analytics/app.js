//ovde treba subscribe da ide
const mqtt = require("mqtt");
const express = require("express");
const grpc = require("@grpc/grpc-js");
const protoLoader = require("@grpc/proto-loader");
const PROTO_PATH = __dirname + "/greet.proto";
const { InfluxDB } = require("@influxdata/influxdb-client");

// You can generate an API token from the "API Tokens Tab" in the UI

const token =
  "zl2j-17eUfggn5sn4CXxjG9-2Y1wyDAvzthKwnOfaxOq02oRMCmnSyeFPANuJlylkVmrmwuVplfJg8y4cZ8KqQ=="; 
const org = "ELFAK";
const bucket = "CurrencyDB";


const client = new InfluxDB({
  url: "http://host.docker.internal:8086",
  token: token,
});

const { Point } = require("@influxdata/influxdb-client");
const writeApi = client.getWriteApi(org, bucket);
writeApi.useDefaultTags({ host: "host1" });

const packageDefinition = protoLoader.loadSync(PROTO_PATH, {
  keepCase: true,
  longs: String,
  enums: String,
  defaults: true,
  oneofs: true,
});

let greet_proto = grpc.loadPackageDefinition(packageDefinition).greet;
const grpcClient = new greet_proto.Greeter(
  "host.docker.internal:5011",
  grpc.credentials.createInsecure()
);

const app = express();

var mqttClient = mqtt.connect("tcp://broker.emqx.io:1883");

mqttClient.on("connect", () => {
  mqttClient.subscribe("outputMQTT", (err) => {
    if (err) {
      console.log("Error while trying to subscribe to the topic", err);
    } else {
      console.log("Mqtt client subscribed to the topic");
    }
  });
});
mqttClient.on("message", function (topic, message) {
  console.log(
    "mqttOutput recieved filtered currency from eKuiper : " + message//.toString()
  );

  var e = JSON.parse(message.toString());
  var point = new Point("Currency")
    .intField("currencyId", parseInt(e[0].currencyId))
    .stringField("title", e[0].title)
    .stringField("currency", e[0].currency)
    .floatField("value", parseFloat(e[0].value))
    .stringField("data", e[0].data);
  writeApi.writePoint(point);

  grpcClient.Notify(e[0], (err) => {
    if (err) {
      console.log("Grpc error: ", err);
    }
  });
});

app.listen(5003);
