<template>
  <v-container fill-height>
    <v-layout align="center" justify="center">
      <v-flex xs12 sm8 md6 lg4>
        <v-card class="elevation-12">
          <v-toolbar dark color="primary">
            <v-toolbar-title>Login</v-toolbar-title>
          </v-toolbar>
          <v-card-text>
            <v-form>
              <v-text-field
                label="Email"
                v-model="email"
                prepend-icon="mdi-account"
                required
              ></v-text-field>
              <v-text-field
                label="Password"
                type="password"
                v-model="password"
                prepend-icon="mdi-lock"
                required
              ></v-text-field>
              <v-btn block color="primary" @click="login">Login</v-btn>
            </v-form>
          </v-card-text>
          <v-divider></v-divider>
          <v-card-actions>
            <v-spacer></v-spacer>
            <v-btn color="blue" dark @click="loginWithFacebook">
              <v-icon left>fab fa-facebook</v-icon>
              Login with Facebook
            </v-btn>
            <v-btn color="red" dark @click="loginWithGoogle">
              <v-icon left>fab fa-google</v-icon>
              Login with Google
            </v-btn>
          </v-card-actions>
        </v-card>
      </v-flex>
    </v-layout>
  </v-container>
</template>

<script>
export default {
  data() {
    return {
      email: '',
      password: ''
    }
  },
  methods: {
    login() {
      // Your login logic here
    },
    loginWithFacebook() {
      // Your Facebook login logic here
    },
    loginWithGoogle() {
      // Your Google login logic here
    }
  }
}
</script>

<style scoped>
.v-card__title {
  padding: 24px;
}
.v-card__text {
  padding: 24px;
}
.v-card__actions {
  padding: 16px;
}
</style>