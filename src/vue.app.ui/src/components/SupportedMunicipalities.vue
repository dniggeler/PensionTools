<template>
  <div class="container-fluid mt-4">
    <h1 class="h1">Supported Municipalities</h1>
    <v-alert :show="loading" variant="info">Loading...</v-alert>
    <b-row>
      <b-col>
        <table class="table table-striped">
          <thead>
            <tr>
              <th>BFS Number</th>
              <th>Municipality</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="record in records" :key="record.id">
              <td>{{ record.id }}</td>
              <td>{{ record.value }}</td>
            </tr>
          </tbody>
        </table>
      </b-col>
    </b-row>
  </div>
</template>

<script>
  import api from '@/services/MunicipalityApiService';

  export default {
    data() {
      return {
        loading: false,
        records: [],
        model: {}
      };
    },
    async created() {
      this.getAll()
    },
    methods: {
      async getAll() {
        this.loading = true

        try {
          var items = await api.getAll();
          this.records = items.map(item => {
            return {
              id: item.bfsNumber,
              value: item.name + ' (' + item.canton + ')'
            }
          })
        } finally {
          this.loading = false
        }
      }
    }
  }
</script>
