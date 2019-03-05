import Vue from "vue";
import VueMaterial from "vue-material";
import App from "./App.vue";
import router from "./router";
import store from "./store";
import Vuex from "vuex";

Vue.config.productionTip = false;
Vue.use(VueMaterial);
Vue.use(Vuex);

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount("#app");
