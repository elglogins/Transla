<template>
  <div v-if="!loading" v-cloak>
    <ApplicationDictionariesList
      v-for="application in applications"
      v-bind:application="application"
      v-bind:key="application.alias"
      v-bind:cultures="cultures"
      v-on:edit="selectedGroupedDictionary = $event"
      v-on:create="selectedCreationServiceAlias = $event"
    />
    <!-- <EditDictionary v-bind:grouped-dictionary="selectedGroupedDictionary" />
    <CreateDictionary v-bind:service-alias="selectedCreationServiceAlias" /> -->
  </div>
</template>

<script>
// @ is an alias to /src
import ApplicationDictionariesList from "@/components/ApplicationDictionariesList.vue";

export default {
  name: "home",
  components: {
    ApplicationDictionariesList
  },
  data() {
    return {
      loading: true,
      selectedGroupedDictionary: null,
      selectedCreationServiceAlias: null
    };
  },
  computed: {
    applications() {
      return this.$store.getters.getApplicationsGroupedDictionaries;
    },
    cultures() {
      return this.$store.getters.getCultures;
    }
  },
  methods: {}
};
</script>

<style scoped>
.md-progress-spinner {
  margin: 24px;
}
</style>
