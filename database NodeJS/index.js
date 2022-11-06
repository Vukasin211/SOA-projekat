const mongoose = require('mongoose');
const express = require('express');
const app = express();

const axios = require('axios').default;
const fs = require('fs');
const bodyParser = require("body-parser");
const { parse } = require('csv-parse');

const { insertData } = require('./database_utility/mogodb_help');
const { Currency} = require("./Models/currency");

mongoose.connect("mongodb://localhost/currencyDB", () => {
    console.log(" Connected to currencyDB ");
    
    fillDatabase().then( () => {
        console.log(" Filled database ");
    });
    
});

async function fillDatabase(){
    const currencyCount = await mongoose.connection.db.collection('currencies').countDocuments();
    if(currencyCount == 0){
        console.log('Currency collection is empty... Inserting currency...');
        insertData('./dataset/exchange_rates.csv', Currency);
    }
}

/*
function readData(firstLine, lastLine) {
    fs.createReadStream('./dataset/exchange_rates.csv', { encoding : 'utf-8' })
        .pipe(parse({ delimiter : ',', from_line: firstLine, to_line: lastLine, 
        columns: ["currencyId" ,"title" ,"currency" ,"value" ,"date"]}))
        .on('data', (chunk) => {
            axios.post('http://localhost:3000/currency/newCurrency', chunk)
              .catch(function (error) {
                console.log(error.code);
              });
        });
}

function main(offset, maxline, freq){
  let firstLine=2;
  let lastLine=firstLine+offset;
  setInterval(() => {
      readData(firstLine,lastLine);    
      firstLine+=offset+1
      lastLine=lastLine+offset+1< maxline ? lastLine+offset+1 : maxline      
  }, freq);
}

const maxLine=400000;
const offset=5;
const freq = 200;
main(offset, maxLine, freq);
*/



app.get('/Test', (req, res /*po nekad moze i next da ima */) => {

    console.log("Test");
    res.send("Test");
}) 


module.exports = app;