<template>
  <div class="md-layout md-alignment-center-center">
    <div class="md-layout-item md-size-50">
      <div v-if="savedUsers.length > 0">
        <h2>Select existing user</h2>
        <md-list>
          <md-list-item
            v-for="savedUser in savedUsers"
            v-bind:key="savedUser.baseUrl"
          >
            <md-icon>person</md-icon>
            <span class="md-list-item-text" @click="selectUser(savedUser)">{{
              savedUser.baseUrl
            }}</span>
            <md-button
              @click="removeUser(savedUser)"
              class="md-icon-button md-list-action md-accent"
            >
              <md-icon>delete</md-icon>
            </md-button></md-list-item
          >
        </md-list>
        <md-divider></md-divider>
      </div>

      <form @submit.prevent="saveNewCredentials">
        <h2>Add user</h2>
        <div v-if="errors.length">
          <b>Please correct the following error(s):</b>
          <ul>
            <li v-for="error in errors" v-bind:key="error">{{ error }}</li>
          </ul>
        </div>
        <md-field>
          <label>Base url</label>
          <md-input
            v-model="creationData.baseUrl"
            placeholder="Base url of Transla API service"
            maxlength="80"
          ></md-input>
        </md-field>
        <md-field>
          <label>Api key</label>
          <md-input
            v-model="creationData.apiKey"
            placeholder="Api key for accessing Transla API service"
            maxlength="80"
          ></md-input>
        </md-field>

        <md-button type="submit" class="md-raised md-primary">Save</md-button>
      </form>
    </div>
  </div>
</template>

<script>
// @ is an alias to /src
import { userService } from "../services/UserService";
export default {
  name: "login",
  components: {},
  data() {
    return {
      creationData: {
        apiKey: null,
        baseUrl: null
      },
      errors: [],
      savedUsers: []
    };
  },
  methods: {
    loadUsers() {
      this.savedUsers = userService.getSavedUsers();
    },
    removeUser(user) {
      userService.remove(user.baseUrl);
      this.loadUsers();
    },
    selectUser(user) {
      var self = this;
      userService.login(user.baseUrl, user.apiKey);

      // load data
      self.$store.dispatch("loadApplications");
      self.$store.dispatch("loadCultures").then(function() {
        self.$store.dispatch("loadDictionaries");
      });

      self.$router.push("home");
    },
    validateNewCredentialsForm(e) {
      e.preventDefault();
      if (this.creationData.baseUrl && this.creationData.apiKey) {
        return true;
      }

      // validation
      this.errors = [];
      if (!this.creationData.baseUrl) {
        this.errors.push("Base url required.");
      }
      if (!this.creationData.apiKey) {
        this.errors.push("API key required.");
      }

      return false;
    },
    saveNewCredentials(e) {
      var self = this;
      if (this.validateNewCredentialsForm(e)) {
        userService.create(this.creationData.baseUrl, this.creationData.apiKey);
        userService.login(this.creationData.baseUrl, this.creationData.apiKey);
        self.selectUser(this.creationData);
      }
    }
  },
  created() {
    this.loadUsers();
  }
};
</script>

<style scoped lang="less">
.md-list-item-content {
  i,
  span {
    cursor: pointer;
  }
}
</style>
