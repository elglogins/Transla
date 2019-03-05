<template>
  <div>
    <md-table md-card>
      <md-table-row>
        <md-table-head>Culture name</md-table-head>
        <md-table-head>Actions</md-table-head>
      </md-table-row>

      <md-table-row
        v-for="culture in cultures"
        v-bind:key="culture.cultureName"
      >
        <md-table-cell>
          <CultureFlag v-bind:cultureName="culture.cultureName" />
          {{ culture.cultureName }}
        </md-table-cell>
        <md-table-cell>
          <md-menu
            md-direction="bottom-end"
            :mdCloseOnClick="true"
            :mdCloseOnSelect="true"
          >
            <md-button md-menu-trigger><md-icon>settings</md-icon></md-button>

            <md-menu-content>
              <md-menu-item @click="requestDeletion(culture)"
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
import CultureFlag from "@/components/CultureFlag.vue";
export default {
  name: "cultures",
  data: () => ({
    deletion: {
      dialogIsActive: false,
      culture: {
        cultureName: null
      }
    }
  }),
  components: {
    CultureFlag
  },
  computed: {
    cultures() {
      return this.$store.getters.getCultures;
    },
    deletionPromptTitle() {
      return `Deletion of ${this.deletion.culture.cultureName} culture?`;
    },
    deletionPromptText() {
      return `By deleting <strong>${
        this.deletion.culture.cultureName
      }</strong> culture all its dictionary items are going to be deleted as well.`;
    }
  },
  methods: {
    requestDeletion(culture) {
      this.deletion.culture = culture;
      this.deletion.dialogIsActive = true;
    },
    onDeletionConfirm() {
      var self = this;
      this.$store
        .dispatch("deleteCulture", self.deletion.culture)
        .then(function() {
          // emit event
          self.deletion.dialogIsActive = false;
        });
    }
  }
};
</script>
