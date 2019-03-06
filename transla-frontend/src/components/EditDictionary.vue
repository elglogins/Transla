<template>
  <md-dialog :md-active.sync="showDialog">
    <md-dialog-title
      >{{ temporaryFormData.application }} :
      {{ temporaryFormData.alias }}</md-dialog-title
    >
    <md-dialog-content>
      <form>
        <md-field
          v-for="dictionary in temporaryFormData.dictionaries"
          v-bind:key="dictionary.cultureName"
        >
          <label> {{ dictionary.cultureName }}</label>
          <md-textarea v-model="dictionary.value"></md-textarea>
        </md-field>
      </form>
    </md-dialog-content>
    <md-dialog-actions align="end">
      <md-button class="md-accent" v-on:click="cancel()">Delete</md-button>
      <span class="spacer"></span>
      <md-button class="md-primary" v-on:click="cancel()">Close</md-button>
      <md-button class="md-primary" v-on:click="save()">Save</md-button>
    </md-dialog-actions>
  </md-dialog>
</template>

<script>
export default {
  name: "EditDictionary",
  props: ["groupedDictionary"],
  computed: {
    cultures() {
      return this.$store.getters.getCultures;
    }
  },
  data() {
    return {
      showDialog: false,
      temporaryFormData: {}
    };
  },
  watch: {
    groupedDictionary: function(newVal) {
      // watch it
      var data = JSON.parse(JSON.stringify(newVal));
      var model = data.item;
      model.application = data.application;

      this.showDialog = newVal !== null;
      // ensure all cultures are present
      this.cultures.forEach(function(culture) {
        var exists = model.dictionaries.filter(
          dictionary => dictionary.cultureName === culture.cultureName
        );

        if (exists.length < 1) {
          // doesn't exist
          model.dictionaries.push({
            cultureName: culture.cultureName,
            alias: model.alias,
            application: model.application,
            value: ""
          });
        }
      });

      this.temporaryFormData = model;
    }
  },
  methods: {
    save() {
      var self = this;
      this.$store
        .dispatch(
          "editAliasGroupedDictionary",
          this.temporaryFormData.dictionaries
        )
        .then(function() {
          // emit event
          self.temporaryFormData = {};
          self.showDialog = false;
        });
    },
    cancel() {
      this.temporaryFormData = {};
      this.showDialog = false;
    }
  }
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="less">
.spacer {
  flex: 1 1 auto;
}
</style>
