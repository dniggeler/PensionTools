<template>
  <v-app>
    <v-app-bar app>

      <v-toolbar>
        <v-app-bar-nav-icon @click="drawer=!drawer"/>
        <v-toolbar-title>Swiss Tax and Pension Tools</v-toolbar-title>

        <v-toolbar-items>
          <v-btn text to="/">
            <v-icon left>mdi-home</v-icon>
            <span>Home</span>
          </v-btn>
          <v-menu offset-y>
            <template v-slot:activator="{ on }">
              <v-btn slot="activator" v-on="on">
              <v-icon left>mdi-hammer-screwdriver</v-icon>
              <span>Tools</span>
              </v-btn>
            </template>
            <v-list>
              <v-list-item v-for="link in links[1].sublinks" :key="link.key" @click="callback">
                <v-list-item-title>{{ link.key }}</v-list-item-title>
              </v-list-item>
            </v-list>
          </v-menu>
          <v-btn text to="/tools">
            <v-icon left>mdi-hammer-screwdriver</v-icon>
            <span>Tools</span>
          </v-btn>
          <v-btn text to="/municipalities">
            <v-icon left>mdi-database</v-icon>
            <span>Data</span>
          </v-btn>
        </v-toolbar-items>

        <v-spacer/>

        <v-btn icon
            right
            href="https://github.com/dniggeler/PensionTools" 
            target="_blank"
          >
          <v-icon>mdi-github-circle</v-icon>
          </v-btn>
      </v-toolbar>

    </v-app-bar>
    <v-navigation-drawer 
      v-model="drawer"
      app
    >
      <v-list>
        <v-subheader class="title">Application</v-subheader>
        <v-list-item v-for="(link,i) in links" :key="i" router :to="link.route">
          <v-list-item-action>
            <v-icon>
              {{ link.icon }}
            </v-icon>
          </v-list-item-action>
          <v-list-item-content>
            <v-list-item-title>
              {{ link.text }}
            </v-list-item-title>
          </v-list-item-content>
          <v-list-item-action>
            <v-icon>mdi-menu-down</v-icon>
          </v-list-item-action>
        </v-list-item>
      </v-list>

    </v-navigation-drawer>
      
    <router-view />
    <v-footer></v-footer>
  </v-app>
</template>

<script>
export default {
  name: "App",

  data: () => ({
    drawer: false,
    links: [
      { text: 'Home', icon: 'mdi-home', sublinks: [], route: '/' },
      { text: 'Tools', icon: 'mdi-hammer-screwdriver', sublinks: [
        { key: 'IncomeTax', text: 'Income Tax' },
        { key: 'BenefitTax', text: 'Benefit Tax' }], route: '/tools' },
      { text: 'Data', icon: 'mdi-database', sublinks: [], route: '/municipalities' }
    ]
  })
};
</script>
