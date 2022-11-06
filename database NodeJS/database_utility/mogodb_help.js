const fs = require('fs');
const { parse } = require('csv-parse');
const mongoose = require("mongoose");

function insertData(filename, model){
    
    let currency = [];
    fs.createReadStream(filename, { encoding : 'utf-8' })   //TODO: encoding mozda treba da bude ascii, vidi ako pravi problem
        .pipe(parse({ delimiter : ',', from_line: 1, columns: true }))
        .on('data', chunk => {
            currency.push(chunk);
            if(currency.length > 50){
                populate(currency, model);
                currency = [];
                //console.log(currency);
            }
        })
        .on('end', () => {
            if(currency.length > 0){
                //console.log(currency);
                populate(currency, model);
            }
        });
}
        
function populate(data, model){
    //console.log( "Ovo je model : " + model);
    console.log("OVDE SMO")
        model.insertMany(data).then(() => {}).catch((err) => {
            console.log(err);
            console.log('Insert many caused an error...');
        });

/*
        data.forEach(element => {
            model.collection.insertOne(element);
       });
*/
}

module.exports = { insertData: insertData }