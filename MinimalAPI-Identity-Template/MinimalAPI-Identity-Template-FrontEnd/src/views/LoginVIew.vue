<script setup>
import { inject, ref } from "vue";
import { useI18n } from "vue-i18n";
const { t } = useI18n({ useScope: "global" });
const axios = inject("axios");
const email = ref("");
const password = ref("");

const successData = ref({});
const errorData = ref({});

const loginUser = async () => {
  successData.value = {};
  errorData.value = {};
  axios
    .post("/api/identity/login?useCookies=true", {
      email: email.value,
      password: password.value,
    })
    .then((res) => (successData.value = res.data))
    .catch((errors) => {
      errorData.value = errors.response.data;
    });
};
</script>

<template>
  <main>
    <div class="flex flex-col gap-y-2">
      <label for="email">Email</label>
      <input type="text" id="email" v-model="email" />
      <label for="nickname">Password</label>
      <input type="password" id="password" v-model="password" />
      <button @click="loginUser" class="my-1 mx-1 py-1 px-3 bg-green-600 text-white rounded">
        {{ t("buttons.submit") }}
      </button>
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
  </main>
</template>
