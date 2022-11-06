const http = require('http');
const app = require('./index');
const bodyParser = require("body-parser");

const port = process.env.PORT || 3000;
app.set('port', port);
const server = http.createServer(app);

const currencyRoute = require("./Routes/currency");
const { options } = require("./Routes/currency");

app.use(bodyParser.json());
app.use("/currency", currencyRoute);

const currencyRouter = require('./Routes/currency');
app.use('/currency', currencyRoute);

server.listen(port, () => {
    console.log(`Server started on port ${port}...`);
});