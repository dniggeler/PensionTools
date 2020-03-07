import Vue from 'vue'
import Router from 'vue-router'
import Start from '@/components/Start'
import Municipalities from '@/components/SupportedMunicipalities'
import Tools from '@/components/Tools'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'Start',
      component: Start
    },
    {
      path: '/tools',
      name: 'Tools',
      component: Tools
    },
    {
      path: '/municipalities',
      name: 'Municipalities',
      component: Municipalities
    }
  ]
})
