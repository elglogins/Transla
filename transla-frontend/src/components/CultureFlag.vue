<template>
  <div>
    <img v-bind:src="countryFlagImage" />
    <md-tooltip md-direction="top">{{ cultureName }}</md-tooltip>
  </div>
</template>

<script>
export default {
  name: "CultureFlag",
  props: ["cultureName"],
  computed: {
    countryFlagImage() {
      return `https://www.countryflags.io/${
        this.cultureName.split("-")[1]
      }/shiny/24.png`;
    }
  }
};
</script>

<style scoped lang="less"></style>
