import Vue from "vue";
import Router from "vue-router";
import Home from "./views/Home.vue";
import { userService } from "./services/UserService";

Vue.use(Router);

// https://github.com/vuematerial/vue-material/issues/1977
Vue.component("router-link", Vue.options.components.RouterLink);
Vue.component("router-view", Vue.options.components.RouterView);

const router = new Router({
  mode: "history",
  base: process.env.BASE_URL,
  routes: [
    {
      path: "/home",
      name: "home",
      component: Home
    },
    {
      path: "/cultures",
      name: "cultures",
      // route level code-splitting
      // this generates a separate chunk (about.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () =>
        import(/* webpackChunkName: "cultures" */ "./views/Cultures.vue")
    },
    {
      path: "/applications",
      name: "applications",
      // route level code-splitting
      // this generates a separate chunk (about.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () =>
        import(/* webpackChunkName: "cultures" */ "./views/Applications.vue")
    },
    {
      path: "/",
      name: "login",
      component: () =>
        import(/* webpackChunkName: "cultures" */ "./views/Login.vue"),
      meta: {
        guest: true
      }
    }
  ]
});

router.beforeEach((to, from, next) => {
  if (to.matched.some(record => record.meta.guest)) {
    next();
  } else {
    // must be logged in to access page
    if (userService.isLoggedIn()) {
      next();
    } else {
      next({ name: "login" });
    }
  }
});

export default router;
