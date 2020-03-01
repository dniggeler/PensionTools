<template>
  <v-container fluid>
    <v-content>
      <v-col cols="6">
        <v-row v-for="record in records" :key="record.id">
          <v-col cols="1">{{ record.id }}</v-col>
          <v-col cols="5">{{ record.value }}</v-col>
        </v-row>
      </v-col>
    </v-content>
  </v-container>
</template>
  
<script>
import api from "@/services/MunicipalityApiService";

export default {
  data() {
    return {
      loading: false,
      records: [],
      model: {}
    };
  },
  async created() {
    this.getAll();
  },
  methods: {
    async getAll() {
      this.loading = true;

      try {
        var items = await api.getAll();
        this.records = items.map(item => {
          return {
            id: item.bfsNumber,
            value: item.name + " (" + item.canton + ")"
          };
        });
      } finally {
        this.loading = false;
      }
    }
  }
};
</script>
