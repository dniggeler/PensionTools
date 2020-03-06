<template>
<v-container>
  <v-content>
  <v-carousel
    cycle
    height="400"
    show-arrows-on-hover
    light
    hide-delimiter-background
  >
    <v-carousel-item
      v-for="(slide, i) in slides"
      :key="i"
    >
      <v-sheet
        color="grey lighten-5"
        height="100%"
        >
          <v-row
              justify="center"
              align="baseline"
              style="height: 250px;"
          >
            <div class="body-2 pt-10">{{ descriptions[i] }}</div>
          </v-row>
            <v-row
              justify="center"
            >
            <div class="display-3">{{ slide }}</div>
            </v-row>

            <v-row
              justify="center"
            >
              <div class="body-1">{{ descriptions[i] }}</div>
            </v-row>
      </v-sheet>  
    </v-carousel-item>
  </v-carousel>
  </v-content>
</v-container>
</template>

<script>
export default {
  data () {
    return {
        slides: [
          'Tax Calculators',
          'Tax Comparers',
          'Pension Calculators',
          'Scenario Analysis',
          'Municipalities'
        ],
        descriptions: [
          'Calculators for various tax types.',
          'Calculators to compare tax against other municipalities.',
          'Calculators for second and third pillar.',
          'Combine calculators and change tax parameters over time.',
          'Information about the set of municipalities supported by the calculators'
        ],
    }
  }
}
</script>