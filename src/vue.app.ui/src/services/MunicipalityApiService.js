import axios from 'axios'

const client = axios.create({
  baseURL: 'http://localhost:54405/',//'https://pensiontools.herokuapp.com',
  json: true
})

export default {
    async execute(method, resource, data) {
      return client({
        method,
        url: resource,
        data
      }).then(req => {
        return req.data
      })
    },
    getAll() {
      return this.execute('get', 'api/calculators/tax/municipality/2019')
    }
}