<template>
  <v-container>
    <v-row>
      <v-col cols="2">
        <v-col cols="12" @click="filterItems('All')" class="hover-link"
          ><p>All</p></v-col
        >
        <v-col cols="12" @click="filterItems('For Men')" class="hover-link"
          ><p>For Men</p></v-col
        >
        <v-col cols="12" @click="filterItems('For Ladies')" class="hover-link"
          ><p>For Ladies</p></v-col
        >
        <v-col cols="12" @click="filterItems('Gift Set')" class="hover-link"
          ><p>Gift Set</p></v-col
        >
      </v-col>
      <v-col cols="10">
        <v-row> <h1>Shop</h1> </v-row>
        <v-row>
          <v-col
            v-for="item in this.filteredItems"
            :key="item.id"
            :item="item"
            cols="4"
          >
            <v-card class="mx-auto rounded">
              <v-img
                max-height="400px"
                src="https://cdn.vuetifyjs.com/images/cards/sunshine.jpg"
                cover
              ></v-img>
              <v-card-title> Top western road trips </v-card-title>
              <v-card-subtitle> 1,000 miles of wonder </v-card-subtitle>
              <v-card-actions>
                <v-btn
                  color="orange-lighten-2"
                  text="Explore"
                  @click="onProductClick(item)"
                ></v-btn>
              </v-card-actions>
            </v-card>
          </v-col>
        </v-row>
      </v-col>
    </v-row>
  </v-container>
</template>

<style>
.hover-link:hover {
  cursor: pointer;
  color: red;
}
</style>

<script>
export default {
  data() {
    return {
      items: [
        { id: 205, category: "For Men", name: "Banana", price: 1, image: [1,2,3,4] },
        { id: 148, category: "For Ladies", name: "Orange", price: 2, image: [1,2,3,4] },
        { id: 248, category: "Gift Set", name: "Apple", price: 1, image: [1,2,3,4] },
      ],
      filteredItems: [],
    };
  },
  methods: {
    onProductClick(items) {
      this.$emit("load-product", items);
    },
    filterItems(selectedCategory = 'All') {
      if (selectedCategory == "All") this.filteredItems = this.items;
      else
        this.filteredItems = this.items.filter(
          (item) => item.category === selectedCategory
        );
    },
  },
  mounted() {
    this.filterItems();
  },
};
</script>
