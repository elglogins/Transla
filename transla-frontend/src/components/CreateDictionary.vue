<template>
  <md-dialog :md-active.sync="showDialog">
    <md-dialog-title>Create new dictionary</md-dialog-title>
    <md-dialog-content>
      <form @submit="save">
        <md-field>
          <label for="movie">Application</label>
          <md-select v-model="temporaryFormData.application">
            <md-option
              v-bind:value="application.alias"
              v-for="application in applications"
              v-bind:key="application.alias"
              >{{ application.alias }}</md-option
            >
          </md-select>
        </md-field>

        <md-field>
          <label>Alias</label>
          <md-input
            v-model="temporaryFormData.alias"
            placeholder="YOUR_DICTIONARY_ALIAS"
            maxlength="30"
          ></md-input>
        </md-field>

        <md-field
          v-for="dictionary in temporaryFormData.dictionaries"
          v-bind:key="dictionary.cultureName"
        >
          <label> {{ dictionary.cultureName }}</label>
          <md-textarea v-model="dictionary.value"></md-textarea>
        </md-field>
      </form>
    </md-dialog-content>
    <md-dialog-actions>
      <md-button class="md-primary" v-on:click="cancel()">Close</md-button>
      <md-button class="md-primary" v-on:click="save()">Save</md-button>
    </md-dialog-actions>
  </md-dialog>
</template>

<script>
export default {
  name: "CreateDictionary",
  props: ["applicationAlias", "isActive"],
  computed: {
    cultures() {
      return this.$store.getters.getCultures;
    },
    applications() {
      return this.$store.getters.getApplications;
    }
  },
  data() {
    return {
      showDialog: false,
      temporaryFormData: {}
    };
  },
  watch: {
    isActive: function(newVal) {
      var self = this;
      var model = {
        alias: null,
        application: self.applicationAlias,
        dictionaries: []
      };
      // ensure all cultures are present
      this.cultures.forEach(function(culture) {
        model.dictionaries.push({
          cultureName: culture.cultureName,
          value: ""
        });
      });

      this.temporaryFormData = model;
      self.showDialog = newVal;
    }
  },
  methods: {
    save() {
      var self = this;
      self.temporaryFormData.dictionaries.forEach(function(item) {
        item.application = self.temporaryFormData.application;
        item.alias = self.temporaryFormData.alias;
      });
      /*eslint-disable */
      // console.log(self.temporaryFormData);
      /*eslint-enable */
      self.$store
        .dispatch(
          "editAliasGroupedDictionary",
          self.temporaryFormData.dictionaries
        )
        .then(function() {
          // emit event
          self.temporaryFormData = {};
          self.$emit("saved");
        });
    },
    cancel() {
      var self = this;
      self.$emit("closed");
    }
  }
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="less">
form {
  padding: 15px;
}

.md-dialog {
  width: 60%;
}
</style>
