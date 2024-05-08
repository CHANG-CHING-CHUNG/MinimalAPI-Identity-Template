<script setup>
import { inject, ref } from "vue";
const axios = inject("axios");

const successData = ref({});
const errorData = ref({});

const logoutUser = async () => {
  successData.value = {};
  errorData.value = {};
  axios
    .post("/api/identity/logout", {})
    .then((res) => (successData.value = res.data))
    .catch((errors) => {
      if (errors.response.data) {
        errorData.value = errors.response.data;
      } else {
        errorData.value = { status: "Failure", errors: errors.message };
      }
    });
};
</script>

<template>
  <main>
    <p class="text-xl">目前登入狀態</p>
    <button @click="logoutUser" class="my-1 mx-1 py-1 px-3 bg-green-600 text-white rounded">
      送出
    </button>
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
