<template>
  <v-card color="grey-lighten-4">
    <v-container>
      <v-row justify="center" text-align="center">
        <v-col cols="5" align="center">
          <v-img height="400" cover src="/src/assets/cate-men.jpg"></v-img>
          <v-sheet
            class="mx-auto mt-5"
            max-width="500"
            max-height="150"
            color="grey-lighten-4"
          >
            <v-slide-group
              v-model="model"
              color="grey"
              class="pa-4"
              center-active
              show-arrows
            >
              <v-slide-group-item
                v-for="n in item.image.length"
                :key="n"
                v-slot="{ isSelected, toggle }"
              >
                <v-card
                  :color="isSelected ? 'grey-lighten-2' : 'grey-lighten-1'"
                  class="ma-4"
                  height="100"
                  width="100"
                  @click="toggle"
                >
                  <v-img
                    height="200"
                    aspect-ratio="16/9"
                    cover
                    src="/src/assets/cate-men.jpg"
                  ></v-img>
                </v-card>
              </v-slide-group-item>
            </v-slide-group>
          </v-sheet>
        </v-col>
        <v-col cols="7" align="center">
          <v-card>{{ item.name }}</v-card>
        </v-col>
      </v-row>
    </v-container>
  </v-card>
</template>

<style>
#app {
  background-color: red;
}
</style>

<script>
export default {
  props: {
    item: {
      type: Object,
      required: true,
    },
  },
  data: () => ({
    model: null,
  }),
};
</script>
