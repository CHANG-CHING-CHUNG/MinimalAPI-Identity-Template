<script setup>
import { inject, ref } from "vue";
import { useI18n } from "vue-i18n";
const { t } = useI18n({ useScope: "global" });
const axios = inject("axios");
const csrfToken = ref("");
const cookie = ref("");
const successData = ref({});
const errorData = ref({});
const validateSuccessData = ref({});
const validateErrorData = ref({});

const getCsrfToken = async () => {
  axios
    .get("/antiforgery/token")
    .then((res) => {
      csrfToken.value = document.cookie
        .split("; ")
        .find((row) => row.startsWith("XSRF-TOKEN="))
        .split("=")[1];
      cookie.value = document.cookie;
    })
    .catch((errors) => {
      if (errors.response.data) {
        errorData.value = errors.response.data;
      } else {
        errorData.value = { status: "Failure", errors: errors.message };
      }
    });
};

const validateCsrfToken = async () => {
  validateSuccessData.value = {};
  validateErrorData.value = {};
  axios
    .get("https://localhost:7281/", {
      headers: { "X-XSRF-TOKEN": csrfToken.value },
    })
    .then((res) => (validateSuccessData.value = res.data))
    .catch((errors) => {
      validateErrorData.value = errors.response.data;
      if (errors.response.data) {
        validateErrorData.value = errors.response.data;
      } else {
        validateErrorData.value = { status: "Failure", errors: errors.message };
      }
    });
};
</script>

<template>
  <main>
    <div class="mb-4">
      <button @click="getCsrfToken" class="my-1 mx-1 py-1 px-3 bg-green-600 text-white rounded">
        {{ t("buttons.getCsrfToken") }}
      </button>
      <p class="text-xl">{{ t("pages.csrfToken.title") }}</p>
      <div>
        <ul>
          <li>
            Token: <span>{{ csrfToken }}</span>
          </li>
          <li>
            Cookies: <span>{{ cookie }}</span>
          </li>
        </ul>
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
      <button
        @click="validateCsrfToken"
        class="my-1 mx-1 py-1 px-3 bg-green-600 text-white rounded"
      >
      {{ t("buttons.validateToken") }}
      </button>

      <div>
        <ul>
          Error Status:
          <span>{{ validateErrorData.status }}</span>
          <li>{{ validateErrorData.errors }}</li>
        </ul>
      </div>
      <div>
        <ul>
          Success Status:
          <span>{{ validateSuccessData.status }}</span>
          <li>
            {{ validateSuccessData.data }}
          </li>
        </ul>
      </div>
    </div>
  </main>
</template>
