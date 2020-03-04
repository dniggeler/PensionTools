import Vue from 'vue'
import Router from 'vue-router'
import Start from '@/components/Start'
import Municipalities from '@/components/SupportedMunicipalities'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/municipalities',
      name: 'Start',
      component: Start
    },
    {
      path: '/',
      name: 'Municipalities',
      component: Municipalities
    }
  ]
})
