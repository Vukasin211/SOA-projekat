
const express = require("express");
const router = express.Router();


const { Currency } = require("../Models/currency")

//getCurrencyLatest
router.get("/latest", (req, res) => {
  Currency.findOne().sort('-_id').then(
    response => {
      res.status(200).json(response);
    }
  );
});

//getCurrencyAll
router.get("/all", (req, res) => {
    Currency.find({})
    .then(response =>{
        const dtoResponse = []
        response.forEach(el=> dtoResponse.push(Currency.toDTO(el)))
        return res.send(dtoResponse)
    })
    .catch(err=> res.status(400).send(err.message))
});

//getCurrencyBySIGN
router.get("/Sign=:currency", async (req, res) => {
    try {
      const currency = await Currency.findOne({ currency: req.params.currency });
      if (currency) {
        res.status(200).json(currency);
      } else {
        res.status(404).json("Currency not found!");
        console.log(req.params.currency);
      }
    } catch (err) {
      res.json({ message: err });
    }
});

//getCurrencyByID
router.get("/ID=:id", async (req, res) => {
    try {
      const currency = await Currency.findOne({ currencyId: req.params.id });
      if (currency) {
        console.log(req.params.id);
        res.status(200).json(currency);
      } else {
        res.status(404).json("Currency not found!");
        console.log(req.params.id);
      }
    } catch (err) {
      res.json({ message: err });
    }
});

//getRandomCurrency
router.get("/", async (req, res) => {
  try {
    const currency = await Currency.aggregate([{ $sample: { size: 1 } }]);
    if (currency) {
      res.status(200).json(currency[0]);
    } else {
      res.status(404).json("Currency not found");
    }
  } catch (error) {
    res.json({ message: error });
  }
});

//postNewCurrency
router.post("/newCurrency", async (req, res) => {
  const currency = new Currency({
    currencyId: req.body.currencyId,
    title: req.body.title,
    currency: req.body.currency,
    value: req.body.value,
    date: req.body.date
  });
  try {
    const savedCurrency = await currency.save();
    res.status(200).json(`Currency  ${savedCurrency.Title} successfully added!`);
  } catch (err) {
    res.json({ message: err });
  }
});

//editCurrency
router.put("/Update=:title", async (req, res) => {
  try {
    const updatedCurrency = await Currency.findOneAndUpdate(
      { title: req.params.title },
      { $set: { 
        title: req.body.title,  
        currency: req.body.currency,
        value: req.body.value,
        date: req.body.date
       } }
    );
    if (updatedCurrency) {
      res
        .status(200)
        .json("Currency " + req.body.title + " has been successfully updated!");
    } else {
      res.status(400).json("Currency not found!");
    }
  } catch (error) {
    res.json({ message: error });
  }
});



module.exports = router;