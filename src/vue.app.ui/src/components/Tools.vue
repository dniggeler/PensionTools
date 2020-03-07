<template>
    <v-container grid-list-xs>
        <v-content>
            <v-list v-for="(tool, i) in tools" :key="i">
                <v-card>
                    <v-card-title primary-title>
                        {{tool}}
                    </v-card-title>
                    <v-card-text>
                        {{descriptions[i]}}
                    </v-card-text>
                </v-card>
            </v-list>
        </v-content>
    </v-container>
</template>

<script>
export default {

  data () {
    return {
        tools: [
          'Income and Wealth Tax Calculator',
          'Capital Benefits Tax Calculator',
        ],
        descriptions: [
          'Calculates the overall tax amount based \
on a person taxable income in a given tax year.',
          'Calculates the overall tax amount based \
          on a person capital benefit from the second and/or third pillar in a given tax year.',
        ],
    }
  }
}
</script>