const mongoose = require('mongoose');
const express = require('express');
const app = express();

const axios = require('axios').default;
const fs = require('fs');
const bodyParser = require("body-parser");
const { parse } = require('csv-parse');

const { insertData } = require('./database_utility/mogodb_help');
const { Currency} = require("./Models/currency");

const dotenv = require('dotenv')
dotenv.config();

//const uri = "mongodb://mongodb:27017/CurrencyDB?directConnection=true";

const uri = "mongodb+srv://Vukasin21:Veselinovic1@cluster0.rsqe9xv.mongodb.net/CurrencyDB";

mongoose.connect(uri).then(() => {
    console.log('Connection to database successfull.')
    //fillDatabase().then(() => {
    //    console.log('Database filled successfully.');
    //})
    .catch(err => {
        console.log(err);
        console.log('Error occured while populating the database...');
    });
}).catch(err => {
    console.log(err);
    console.log('An error occured while connecting to the database.');
});

async function fillDatabase(){
    
    const currencyCount = await mongoose.connection.db.collection('currencies').countDocuments();
    if(currencyCount == 0){
        console.log('Currency collection is empty... Inserting currency...');
        insertData('./dataset/exchange_rates START.csv', Currency);
    }
    
}

app.get('/Test', (req, res /*po nekad moze i next da ima */) => {

    console.log("Test");
    res.send("Test");
}) 


module.exports = app;