<template>
  <div>
    <md-table md-card>
      <md-table-row>
        <md-table-head>Application name</md-table-head>
        <md-table-head>Actions</md-table-head>
      </md-table-row>

      <md-table-row
        v-for="application in applications"
        v-bind:key="application.alias"
      >
        <md-table-cell>
          {{ application.alias }}
        </md-table-cell>
        <md-table-cell>
          <md-menu
            md-direction="bottom-end"
            :mdCloseOnClick="true"
            :mdCloseOnSelect="true"
          >
            <md-button md-menu-trigger><md-icon>settings</md-icon></md-button>

            <md-menu-content>
              <md-menu-item @click="requestDeletion(application)"
                >Delete</md-menu-item
              >
            </md-menu-content>
          </md-menu>
        </md-table-cell>
      </md-table-row>
    </md-table>

    <md-dialog-confirm
      :md-active.sync="deletion.dialogIsActive"
      :md-title="deletionPromptTitle"
      :md-content="deletionPromptText"
      md-confirm-text="Ok"
      md-cancel-text="Cancel"
      @md-cancel="deletion.dialogIsActive = false"
      @md-confirm="onDeletionConfirm"
    />
  </div>
</template>

<script>
export default {
  name: "applications",
  data: () => ({
    deletion: {
      dialogIsActive: false,
      application: {
        alias: null
      }
    }
  }),
  computed: {
    applications() {
      return this.$store.getters.getApplications;
    },
    deletionPromptTitle() {
      return `Deletion of ${this.deletion.application.alias} application?`;
    },
    deletionPromptText() {
      return `By deleting <strong>${
        this.deletion.application.alias
      }</strong> application all its dictionary items are going to be deleted as well.`;
    }
  },
  methods: {
    requestDeletion(application) {
      this.deletion.application = application;
      this.deletion.dialogIsActive = true;
    },
    onDeletionConfirm() {
      var self = this;
      this.$store
        .dispatch("deleteApplication", self.deletion.application)
        .then(function() {
          // emit event
          self.deletion.dialogIsActive = false;
        });
    }
  }
};
</script>
