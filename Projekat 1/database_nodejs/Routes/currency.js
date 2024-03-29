
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
    res.status(200).json(`Currency  ${savedCurrency.title} successfully added!`);
  } catch (err) {
    res.json({ message: err });
  }
});

//editCurrencyBySign
router.put("/Update=:sign", async (req, res) => {

    const updatedCurrency = await Currency.findOneAndUpdate({ currency: req.params.sign });
    updatedCurrency.value = req.body.value;
    if (updatedCurrency) {
      console.log(updatedCurrency.value);
      res
        .status(200)
        .json("Currency " + req.params.sign + ` has been successfully updated to ${req.body.value} !`);
    } else {
      res.status(400).json("Currency not found!");
    }
  updatedCurrency.save().catch(err=> res.status(500).send(err.message));
});

//DeleteCurrencyByID
router.delete("/DeleteId=:id", async(req, res)=>{
  Currency.deleteOne({currencyId: req.params.id})
  .then(result=>{
      if (result.deletedCount===0)
          return res.status(404).send(`DELETE Request failed! Currency with id ${req.params.id} not found`)
      return res.send("Currency deleted successfully")
  })
  .catch(err => res.status(500).send(err.message)) 
})

//DeleteCurrencyBySign
router.delete("/DeleteSign=:Sign", async(req, res)=>{
  Currency.deleteOne({currency: req.params.Sign})
  .then(result=>{
      if (result.deletedCount===0)
          return res.status(404).send(`DELETE Request failed! Currency with SIGN ${req.params.Sign} not found`)
      return res.send("Currency deleted successfully")
  })
  .catch(err => res.status(500).send(err.message)) 
})


module.exports = router;