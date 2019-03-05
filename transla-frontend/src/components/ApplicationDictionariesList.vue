<template>
  <div>
    <md-card>
      <md-card-content>
        <md-table @md-selected="onSelect">
          <md-table-toolbar>
            <div class="md-toolbar-section-start">
              <h1 class="md-title">{{ application.alias }}</h1>
            </div>

            <md-field md-clearable class="md-toolbar-section-end">
              <md-input
                placeholder="Search by alias..."
                v-model="searchTerm"
                @input="searchOnTable"
              />
            </md-field>
          </md-table-toolbar>

          <md-table-row>
            <md-table-head>Alias</md-table-head>
            <md-table-head
              v-for="culture in cultures"
              v-bind:key="culture.cultureName"
              ><CultureFlag v-bind:cultureName="culture.cultureName"
            /></md-table-head>
          </md-table-row>

          <md-table-row
            v-for="item in searched"
            v-bind:key="item.alias"
            v-on:click="onSelect(item)"
          >
            <md-table-cell>{{ item.alias }}</md-table-cell>
            <md-table-cell
              v-for="culture in cultures"
              v-bind:key="culture.cultureName"
            >
              <md-icon
                v-if="
                  isAvailableForCulture(item.dictionaries, culture.cultureName)
                "
                >check_circle</md-icon
              >
              <md-icon v-else>error_outline</md-icon>
            </md-table-cell>
          </md-table-row>
        </md-table>

        <md-empty-state
          v-show="searchTerm && searched.length < 1"
          md-label="No dictionaries found"
          :md-description="
            `No dictionaries found for this '${searchTerm}' query. Try a different search term or create a new dictionary.`
          "
        >
          <md-button
            class="md-primary md-raised"
            v-on:click="showCreateNewItem = true"
            >Create New Dictionary Item</md-button
          >
        </md-empty-state>
      </md-card-content>
    </md-card>
    <EditDictionary v-bind:grouped-dictionary="selectedGroupedDictionary" />
    <CreateDictionary
      v-bind:application-alias="application.alias"
      v-bind:is-active="showCreateNewItem"
      v-on:closed="showCreateNewItem = false"
      v-on:saved="showCreateNewItem = false"
    />
  </div>
</template>

<script>
import CultureFlag from "./CultureFlag.vue";
import EditDictionary from "./EditDictionary.vue";
import CreateDictionary from "./CreateDictionary.vue";
export default {
  name: "ApplicationDictionariesList",
  components: {
    CultureFlag,
    EditDictionary,
    CreateDictionary
  },
  props: ["application", "cultures"],
  computed: {},
  data() {
    return {
      searchTerm: null,
      searched: [],
      selectedGroupedDictionary: null,
      showCreateNewItem: false
    };
  },
  methods: {
    createNewDictionaryItem() {
      this.showCreateNewItem = true;
    },
    onSelect(selectedItem) {
      this.selectedGroupedDictionary = {
        application: this.application.alias,
        item: selectedItem
      };
    },
    isAvailableForCulture(dictionaries, cultureName) {
      var self = this;
      if (!dictionaries) {
        return false;
      }

      return (
        dictionaries.filter(
          dictionary =>
            dictionary.cultureName === cultureName &&
            !self.isEmptyOrWhiteSpace(dictionary.value)
        ).length > 0
      );
    },
    isEmptyOrWhiteSpace(str) {
      return str === null || str.match(/^ *$/) !== null;
    },
    toLower(text) {
      return text.toString().toLowerCase();
    },
    searchOnTable() {
      var self = this;
      if (self.searchTerm) {
        self.searched = self.application.dictionaries.filter(item =>
          self.toLower(item.alias).includes(self.toLower(self.searchTerm))
        );
        return;
      }
      self.searched = self.application.dictionaries;
    }
  },
  created() {
    this.searched = this.application.dictionaries;
  }
};
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped lang="less"></style>
