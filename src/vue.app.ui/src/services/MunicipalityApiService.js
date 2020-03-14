import axios from 'axios'

const client = axios.create({
  baseURL: process.env.VUE_APP_ROOT_API,
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
      console.log(process.env);
      return this.execute('get', 'api/calculators/tax/municipality/2019')
    }
}