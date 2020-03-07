<template>
  <v-container fluid>
    <v-content>
      <v-card>
        <v-card-title
          class="headline lighten-3"
        >Search for Swiss municipality
        </v-card-title>
        <v-card-text>
      Find your municipality of interest to calculate various
      figures ranging from tax to complex scenarios.
        </v-card-text>
        <v-card-text>
          <v-autocomplete
            v-model="model"
            :items="items"
            :loading="isLoading"
            :search-input.sync="search"
            item-text="description"
            item-value="id"
            color="white"
            hide-no-data
            hide-selected
            label="Supported Municipalities"
            placeholder="Start typing to Search"
            prepend-icon="mdi-api"
            return-object>
          </v-autocomplete>
        </v-card-text>
        <v-divider></v-divider>
    <v-expand-transition>
      <v-list v-if="model">
        <v-list-item
          v-for="(field, i) in fields"
          :key="i"
        >
          <v-list-item-content>
            <v-list-item-title v-text="field.value"></v-list-item-title>
            <v-list-item-subtitle v-text="field.key"></v-list-item-subtitle>
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-expand-transition>
    <v-card-actions>
      <v-spacer></v-spacer>
      <v-btn
        :disabled="!model"
        @click="model = null">
        Clear
        <v-icon right>mdi-close-circle</v-icon>
      </v-btn>
    </v-card-actions>
      </v-card>
          </v-content>
  </v-container>
</template>
  
<script>
import api from "@/services/MunicipalityApiService";

export default {
  data() {
    return {
      descriptionLimit: 60,
      entries: [],
      isLoading: false,
      model: null,
      search: null,
    };
  },

  async created() {
    this.entries = await this.getAll();
  },

  computed: {
      fields () {
        if (!this.model) return []

        return Object.keys(this.model).map(key => {
          return {
            key,
            value: this.model[key] || 'n/a',
          }
        })
      },
      items () {
        return this.entries.map(entry => {
          const description = entry.name + " (" + entry.canton + ")";
          const id = entry.bfsNumber;

          const item = Object.assign({}, entry, { id, description })
          return item;
        })
      },
    },

    watch: {
      search () {
        // Items have already been loaded
        if (this.items.length > 0) return

        // Items have already been requested
        if (this.isLoading) return

        // Lazily load input items
        return {
          count: this.items.length,
          entries: this.items
        };
      },
    },
  
  methods: {
    async getAll() {
      this.isLoading = true;

      try {
        return await api.getAll();
      } 
      catch(err) {
        console.log(err)
      }
      finally {
        this.isLoading = false;
      }
    }
  }
};
</script>
