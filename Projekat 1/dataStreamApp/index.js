const fs = require('fs');
const { parse } = require('csv-parse');
const axios = require('axios');




function readData(firstLine, lastLine) {
    fs.createReadStream('./exchange_rates.csv', { encoding : 'utf-8' })
        .pipe(parse({ delimiter : ',', from_line: firstLine, to_line: lastLine, 
        columns: ["currencyId" ,"title" ,"currency" ,"value" ,"date"]}))
        .on('data', (chunk) => {

          axios.post('https://host.docker.internal:49153/Currency/postCurrency', chunk)
          .then((res) => {
              console.log(chunk)
              console.log(`Status: ${res.status}`);
              console.log('Body: ', res.data);
          }).catch((err) => {
              console.error(err);
          });

/*
          axios({
            method: 'get',
            url: 'https://localhost:49153/Currency/getRandomCurrency',
            responseType: 'stream',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/txt;charset=UTF-8'
            }
          })
            .then( (response) => {console.log(response)});*/
          /*
          axios.post('https://host.docker.internal:49153/Currency/postCurrency', chunk)
          .then(res => {
            console.log(`Status: ${res.status}`)
            console.log('Body: ', res.data)
          })
          .catch(err => {
            console.error(err)
          })*/
          /* 
            axios.post('https://host.docker.internal:49153/Currency/postCurrency', chunk ,
            {headers: {
              'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8'
           }})
              .catch(function (error) {
                console.log("Greska: " + error.code);
              });*/
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

