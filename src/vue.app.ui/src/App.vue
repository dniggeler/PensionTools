<template>
  <v-app>
    <v-app-bar app>
      <v-toolbar>
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
    <router-view />
    <v-footer></v-footer>
  </v-app>
</template>

<script>
export default {
  name: "App",

  data: () => ({
    drawer: false
  })
};
</script>
