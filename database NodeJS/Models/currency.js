const mongoose = require("mongoose");
const { CurrencyDTO } = require("../DTOs/CurrencyDTO")

const CurrencySchema = mongoose.Schema({
  currencyId: {
    type: Number,
    required: true,
    index: { unique: true }
  },  
  title: String,
  currency: String,
  value: Number,
  date: String
});

CurrencySchema.statics.toDTO = function (modelObject) {
  return new CurrencyDTO(
      modelObject.currencyId,
      modelObject.title,
      modelObject.currency,
      modelObject.value,
      modelObject.date)
}
//CurrencySchema.plugin(uniqueValidator);


const Currency = mongoose.model('Currency', CurrencySchema);

module.exports = { Currency : Currency };
