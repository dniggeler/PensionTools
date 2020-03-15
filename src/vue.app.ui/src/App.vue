<template>
  <v-app>
    <v-app-bar app>
      <v-toolbar>
        <v-app-bar-nav-icon @click="drawer=!drawer" />
        <v-toolbar-title>Swiss Tax and Pension Tools</v-toolbar-title>

        <v-toolbar-items>
          <v-btn text to="/">
            <v-icon left>mdi-home</v-icon>
            <span>Home</span>
          </v-btn>
          <v-btn text to="/tools">
            <v-icon left>mdi-hammer-screwdriver</v-icon>
            <span>Tools</span>
          </v-btn>

          <v-btn text to="/municipalities">
            <v-icon left>mdi-database</v-icon>
            <span>Data</span>
          </v-btn>
        </v-toolbar-items>

        <v-spacer />

        <v-btn icon right href="https://github.com/dniggeler/PensionTools" target="_blank">
          <v-icon>mdi-github</v-icon>
        </v-btn>
      </v-toolbar>
    </v-app-bar>
    <v-navigation-drawer v-model="drawer" app>
      <v-expansion-panels flat :multiple="true" class="pr-1">
        <v-expansion-panel
          focusable
          v-for="(link,i) in links"
          :key="i"
          :tile="true"
          router
          :to="link.route"
        >
          <v-expansion-panel-header :hide-actions="!link.sublinks">
            <div>
              <v-icon class="pr-1">{{ link.icon }}</v-icon>
              <span>{{link.text}}</span>
            </div>
          </v-expansion-panel-header>
          <v-expansion-panel-content>
            <v-list>
              <v-list-item v-for="(sublink,i) in link.sublinks" :key="i">
                <div>
                  <v-icon class="pr-1">{{ sublink.icon }}</v-icon>
                  <span>{{sublink.text}}</span>
                </div>
              </v-list-item>
            </v-list>
          </v-expansion-panel-content>
        </v-expansion-panel>
      </v-expansion-panels>
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
    multiple: true,
    links: [
      { key: "home", text: "Home", icon: "mdi-home", route: "/" },
      {
        key: "tools",
        text: "Tools",
        icon: "mdi-hammer-screwdriver",
        sublinks: [
          { key: "IncomeTax", text: "Income Tax", icon: "mdi-calculator" },
          { key: "BenefitTax", text: "Benefit Tax", icon: "mdi-calculator" }
        ],
        route: "/tools"
      },
      {
        key: "tools",
        text: "Data",
        icon: "mdi-database",
        route: "/municipalities"
      }
    ]
  })
};
</script>
