<script setup>
import { inject, ref } from "vue";
const axios = inject("axios");
const email = ref("");
const successData = ref({});
const errorData = ref({});

const sendLoginEmail = async () => {
  successData.value = {};
  errorData.value = {};
  axios
    .post("/api/identity/send-login-email", {
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
</script>

<template>
  <main>
    <div>
      <p class="text-xl">Email 取得無密碼登入連結</p>
      <div>
        <div class="flex flex-col gap-y-2">
          <label for="email">Email</label>
          <input v-model="email" type="text" id="email" />
          <button @click="sendLoginEmail" class="my-1 mx-1 py-1 px-3 bg-green-600 text-white rounded">送出</button>
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
  </main>
</template>
