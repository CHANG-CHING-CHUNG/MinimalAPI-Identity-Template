<script setup>
import { inject, ref } from "vue";
import { useRoute } from "vue-router";
import { useI18n } from "vue-i18n";
const { t } = useI18n({ useScope: "global" });
const route = useRoute();
const axios = inject("axios");
const email = ref("");
const userId = ref(route.query.userId || "");
const token = ref(route.query.token || "");
const newPassword = ref("");
const successData = ref({});
const errorData = ref({});
const resetSuccessData = ref({});
const resetErrorData = ref({});
const sendForgotPasswordEmail = async () => {
  successData.value = {};
  errorData.value = {};
  axios
    .post("/api/identity/user-manage/forgot-password", {
      email: email.value,
    })
    .then((res) => (successData.value = res.data))
    .catch((errors) => {
      if (errors.response.data) {
        errorData.value = errors.response.data;
      } else {
        errorData.value = { status: "Failure", errors: errors.message };
      }
    });
};
const resetNewPassword = async () => {
  resetSuccessData.value = {};
  resetErrorData.value = {};
  axios
    .post("/api/identity/user-manage/confirm-reset-password", {
      userId: userId.value,
      token: token.value,
      newPassword: newPassword.value,
    })
    .then((res) => (resetSuccessData.value = res.data))
    .catch((errors) => {
      if (errors.response.data) {
        resetErrorData.value = errors.response.data;
      } else {
        resetErrorData.value = { status: "Failure", errors: errors.message };
      }
    });
};
</script>

<template>
  <main>
    <div class="mb-4">
      <p class="text-xl">{{ t("pages.forgotPassword.getResetLinkTitle") }}</p>
      <div>
        <div class="flex flex-col gap-y-2">
          <label for="email">Email</label>
          <input v-model="email" type="text" id="email" />
          <button
            @click="sendForgotPasswordEmail"
            class="my-1 mx-1 py-1 px-3 bg-green-600 text-white rounded"
          >
          {{ t("buttons.submit") }}
          </button>
        </div>
      </div>
      <div>
        <ul>
          Error Status:
          <span>{{ errorData.status }}</span>
          <li>{{ errorData.errors }}</li>
        </ul>
      </div>
      <div>
        <ul>
          Success Status:
          <span>{{ successData.status }}</span>
          <li>
            {{ successData.data }}
          </li>
        </ul>
      </div>
    </div>
    <div>
      <p class="text-xl">{{ t("pages.forgotPassword.resetPasswordTitle") }}</p>
      <div>
        <div class="flex flex-col gap-y-2">
          <input v-model="userId" type="text" id="userId" />
          <input v-model="token" type="text" id="token" />
          <label for="newPassword">New Password</label>
          <input v-model="newPassword" type="text" id="newPassword" />
          <button
            @click="resetNewPassword"
            class="my-1 mx-1 py-1 px-3 bg-green-600 text-white rounded"
          >
            {{ t("buttons.submit") }}
          </button>
        </div>
      </div>
      <div>
        <ul>
          Error Status:
          <span>{{ resetErrorData.status }}</span>
          <li>{{ resetErrorData.errors }}</li>
        </ul>
      </div>
      <div>
        <ul>
          Success Status:
          <span>{{ resetSuccessData.status }}</span>
          <li>
            {{ resetSuccessData.data }}
          </li>
        </ul>
      </div>
    </div>
  </main>
</template>
