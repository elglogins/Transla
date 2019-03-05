import Vue from "vue";
import Vuex from "vuex";
import axios from "axios";

Vue.use(Vuex);

export default new Vuex.Store({
  state: {
    applicationsGroupedDictionaries: [],
    cultures: [],
    applications: []
  },
  getters: {
    getApplicationsGroupedDictionaries(state) {
      return state.applicationsGroupedDictionaries;
    },
    getCultures(state) {
      return state.cultures;
    },
    getApplications(state) {
      return state.applications;
    }
  },
  mutations: {
    // dictionaries
    setApplicationsGroupedDictionaries(state, dictionaries) {
      state.applicationsGroupedDictionaries = dictionaries;
    },
    editAliasGroupedDictionary(state, aliasGroupedDictionaries) {
      if (!aliasGroupedDictionaries && aliasGroupedDictionaries.length <= 0) {
        return false;
      }
      var application = state.applicationsGroupedDictionaries.find(
        f => f.alias == aliasGroupedDictionaries[0].application
      );
      var dictionaryGroup = application.dictionaries.find(
        f => f.alias == aliasGroupedDictionaries[0].alias
      );
      aliasGroupedDictionaries.forEach(function(element) {
        var existing = dictionaryGroup.dictionaries.find(
          f => f.cultureName == element.cultureName
        );
        if (existing) {
          Object.assign(existing, element);
        } else {
          dictionaryGroup.dictionaries.push(element);
        }
      });
    },
    // cultures
    setCultures(state, cultures) {
      state.cultures = cultures;
    },
    createCulture(state, culture) {
      state.cultures.push(culture);
    },
    deleteCulture(state, culture) {
      var existing = state.cultures.find(c => {
        return c.cultureName == culture.cultureName;
      });

      if (existing) {
        var index = state.cultures.indexOf(existing);
        state.cultures.splice(index, 1);
      }
    },
    // applications
    setApplications(state, applications) {
      state.applications = applications;
    },
    createApplication(state, application) {
      state.applications.push(application);
    },
    deleteApplication(state, application) {
      var existing = state.applications.find(a => {
        return a.alias == application.alias;
      });

      if (existing) {
        var index = state.applications.indexOf(existing);
        state.applications.splice(index, 1);
      }
    }
  },
  actions: {
    // dictionaries
    loadDictionaries(context) {
      return axios
        .get("http://localhost:5000/application-grouped")
        .then(response => {
          context.commit("setApplicationsGroupedDictionaries", response.data);
        });
    },
    editAliasGroupedDictionary(context, aliasGroupedDictionaries) {
      return new Promise((resolve, reject) => {
        axios
          .post(
            "http://localhost:5000/api/Dictionary",
            aliasGroupedDictionaries
          )
          .then(
            function() {
              // http success, call the mutator and change something in state
              context.commit(
                "editAliasGroupedDictionary",
                aliasGroupedDictionaries
              );
              resolve(aliasGroupedDictionaries); // Let the calling function know that http is done. You may send some data back
            },
            error => {
              // http failed, let the calling function know that action did not work out
              reject(error);
            }
          );
      });
    },
    // applications
    loadApplications(context) {
      return axios
        .get("http://localhost:5000/api/Application")
        .then(response => {
          context.commit("setApplications", response.data);
        });
    },
    createApplication(context, application) {
      return new Promise((resolve, reject) => {
        axios.post(`http://localhost:5000/api/Application`, application).then(
          function() {
            // http success, call the mutator and change something in state
            context.commit("createApplication", application);
            resolve(application); // Let the calling function know that http is done. You may send some data back
          },
          error => {
            // http failed, let the calling function know that action did not work out
            reject(error);
          }
        );
      });
    },
    deleteApplication(context, application) {
      return new Promise((resolve, reject) => {
        axios
          .delete(`http://localhost:5000/api/Application/${application.alias}`)
          .then(
            function() {
              // http success, call the mutator and change something in state
              context.commit("deleteApplication", application);
              resolve(application); // Let the calling function know that http is done. You may send some data back
            },
            error => {
              // http failed, let the calling function know that action did not work out
              reject(error);
            }
          );
      });
    },
    // ciltures
    loadCultures(context) {
      return axios.get("http://localhost:5000/api/Culture").then(response => {
        context.commit("setCultures", response.data);
      });
    },
    createCulture(context, culture) {
      return new Promise((resolve, reject) => {
        axios.post(`http://localhost:5000/api/Culture`, culture).then(
          function() {
            // http success, call the mutator and change something in state
            context.commit("createCulture", culture);
            resolve(culture); // Let the calling function know that http is done. You may send some data back
          },
          error => {
            // http failed, let the calling function know that action did not work out
            reject(error);
          }
        );
      });
    },
    deleteCulture(context, culture) {
      return new Promise((resolve, reject) => {
        axios
          .delete(`http://localhost:5000/api/Culture/${culture.cultureName}`)
          .then(
            function() {
              // http success, call the mutator and change something in state
              context.commit("deleteCulture", culture);
              resolve(culture); // Let the calling function know that http is done. You may send some data back
            },
            error => {
              // http failed, let the calling function know that action did not work out
              reject(error);
            }
          );
      });
    }
  }
});
