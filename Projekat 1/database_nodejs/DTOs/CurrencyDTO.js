class CurrencyDTO {
    constructor(currencyId, title, currency, value, date) {
        this.currencyId = currencyId
        this.title = title
        this.currency = currency
        this.value = value
        this.date = date
    }
}

module.exports = { CurrencyDTO }