<template>
  <div id="app" class="page-container">
    <md-app md-waterfall md-mode="fixed-last">
      <md-app-toolbar class="md-large md-dense md-primary">
        <div class="md-toolbar-row">
          <div class="md-toolbar-section-start">
            <span class="md-title">Transla</span>
          </div>
          <div class="md-toolbar-section-end">
            <md-button class="md-primary" v-on:click="showCultureCreation"
              >Add culture</md-button
            >
            <md-button class="md-primary" v-on:click="showApplicationCreation"
              >Add application</md-button
            >
            <md-button class="md-primary">Add dictionary</md-button>
          </div>
        </div>

        <div class="md-toolbar-row">
          <md-tabs class="md-primary" md-sync-route>
            <md-tab md-label="Cultures" to="/cultures"></md-tab>
            <md-tab md-label="Applications" to="/applications"></md-tab>
            <md-tab md-label="Dictionaries" to="/"></md-tab>
          </md-tabs>
        </div>
      </md-app-toolbar>

      <md-app-content>
        <router-view />
      </md-app-content>
    </md-app>

    <!-- creation of culture -->
    <md-dialog-prompt
      :md-active.sync="culture_creation.dialogIsActive"
      v-model="culture_creation.cultureName"
      md-title="Create new culture"
      md-input-maxlength="5"
      md-input-placeholder="Type culture name (ISO 639 standard)"
      md-confirm-text="Save"
      @md-cancel="culture_creation.dialogIsActive = false"
      @md-confirm="onCultureCreationConfirm"
    />

    <!-- creation of application -->
    <md-dialog-prompt
      :md-active.sync="application_creation.dialogIsActive"
      v-model="application_creation.alias"
      md-title="Create new application"
      md-input-maxlength="20"
      md-input-placeholder="Type application name"
      md-confirm-text="Save"
      @md-cancel="application_creation.dialogIsActive = false"
      @md-confirm="onApplicationCreationConfirm"
    />
  </div>
</template>

<script>
export default {
  data: () => ({
    menuVisible: false,
    culture_creation: {
      dialogIsActive: false,
      cultureName: null
    },
    application_creation: {
      dialogIsActive: false,
      alias: null
    }
  }),
  methods: {
    // culture
    showCultureCreation() {
      this.culture_creation.cultureName = null;
      this.culture_creation.dialogIsActive = true;
    },
    onCultureCreationConfirm() {
      var self = this;
      this.$store
        .dispatch("createCulture", {
          cultureName: self.culture_creation.cultureName
        })
        .then(function() {
          // emit event
          self.culture_creation.dialogIsActive = false;
        });
    },
    // application
    showApplicationCreation() {
      this.application_creation.alias = null;
      this.application_creation.dialogIsActive = true;
    },
    onApplicationCreationConfirm() {
      var self = this;
      this.$store
        .dispatch("createApplication", {
          alias: self.application_creation.alias
        })
        .then(function() {
          // emit event
          self.application_creation.dialogIsActive = false;
        });
    }
  },
  created() {
    var self = this;
    this.$store.dispatch("loadApplications");
    this.$store.dispatch("loadCultures").then(function() {
      self.$store.dispatch("loadDictionaries").then(function() {
        self.loading = false;
      });
    });
  }
};
</script>

<style lang="less">
// css
@import "../node_modules/vue-material/dist/vue-material.css";
@import "../node_modules/vue-material/dist/theme/default.css"; // This line here
#app {
  font-family: "Avenir", Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}
#nav {
  padding: 30px;
  a {
    font-weight: bold;
    color: #2c3e50;
    &.router-link-exact-active {
      color: #42b983;
    }
  }
}

.md-card {
  margin-top: 15px;
  margin-bottom: 15px;
}

.md-table {
  text-align: left;
}

.md-dialog {
  width: 60%;

  form {
    padding: 15px;
  }
}
</style>
