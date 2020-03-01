<template>
  <v-app>
    <v-app-bar app>
      <v-toolbar>
        <v-toolbar-title>Swiss Tax and Pension Tools</v-toolbar-title>
        <v-toolbar-items>
          <v-btn text>Home</v-btn>
          <v-btn text>Tools</v-btn>
          <v-btn text>Municipalities</v-btn>
        </v-toolbar-items>
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
