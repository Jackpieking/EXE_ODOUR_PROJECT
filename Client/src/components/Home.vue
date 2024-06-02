<script setup lang="ts"></script>

<template>
  <v-carousel height="600" class="mb-5">
    <v-carousel-item
      src="https://cdn.vuetifyjs.com/images/cards/docks.jpg"
      cover
    ></v-carousel-item>
    <v-carousel-item
      src="https://cdn.vuetifyjs.com/images/cards/hotel.jpg"
      cover
    ></v-carousel-item>
    <v-carousel-item
      src="https://cdn.vuetifyjs.com/images/cards/sunshine.jpg"
      cover
    ></v-carousel-item>
  </v-carousel>
  <v-container class="bg-white container pt-5 pb-5 height-500">
    <v-row justify="center" class="pt-5 pb-5">
      <p class="text-big h0">Categories</p>
    </v-row>
    <v-row justify="center" class="text-center mt-5">
      <v-col cols="4" v-for="n in 3" :key="n">
        <v-col>
          <v-row justify="center">
            <img class="rounded-circle w-75" :src="images[n]" />
            <v-col cols="12"
              ><h2>{{ categories[n] }}</h2></v-col
            >
            <v-btn color="red">Go Shop</v-btn>
          </v-row>
        </v-col>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
export default {
  data() {
    return {
      categories: ["", "For Men", "For Ladies", "Gift Set"],
      images: [
        "",
        "/src/assets/cate-men.jpg",
        "/src/assets/cate-ladies.jpg",
        "/src/assets/cate-gift.jpg",
      ],
    };
  },
};
</script>

<style>
.h0 {
  font-size: 50px;
}
.height-500 {
  height: 700px;
}
img {
  height: 300px;
}
</style>
