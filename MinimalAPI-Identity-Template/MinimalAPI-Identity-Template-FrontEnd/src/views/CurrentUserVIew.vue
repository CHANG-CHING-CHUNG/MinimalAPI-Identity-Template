<script setup>
import { inject, ref } from "vue";
import { useI18n } from "vue-i18n";
const { t } = useI18n({ useScope: "global" });

const axios = inject("axios");
const successData = ref({});
const errorData = ref({});
const updateSuccessData = ref({});
const updateErrorData = ref({});
const updateNicknameModel = ref("")

const getCurrentUser = async () => {
  successData.value = {};
  errorData.value = {};
  axios
    .get("/api/identity/user-manage/user")
    .then((res) => (successData.value = res.data))
    .catch((errors) => {
      errorData.value = errors.response.data;
    });
};

const updateNickname = async () => {
  if (!(successData.value.data && successData.value.data[0])) return
  updateSuccessData.value = {};
  updateErrorData.value = {};
  console.log("successData",successData.value.data[0].id);
  axios
    .patch(`/api/identity/user-manage/users/${successData.value.data[0].id}`, {nickname:updateNicknameModel.value})
    .then((res) => (updateSuccessData.value = res.data))
    .catch((errors) => {
      updateErrorData.value = errors.response.data;
    })
};
</script>

<template>
  <main>
    <div class="divide-y-2">
      <div class="mb-2">
        <div>
          <button @click="getCurrentUser" class="my-1 mx-1 py-1 px-3 bg-green-600 text-white rounded">
            {{ t("buttons.getCurrentLoggedInUserData")}}
          </button>
        </div>
        <p class="text-xl">{{ t("pages.currentUser.title") }}</p>
  
        <div>
          <ul>
            Error Status:
            <span>{{ errorData.status }}</span>
            <li v-for="error of errorData.errors">Error: {{ error.description }}</li>
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
      <div class="flex flex-col gap-y-2">
        <div class="flex flex-col gap-y-2">
          <label for="nickname">{{ t("pages.currentUser.nicknameLabel") }}</label>
          <input type="text" id="nickname" v-model="updateNicknameModel">
          <div>
            <button @click="updateNickname" class="my-1 mx-1 py-1 px-3 bg-green-600 text-white rounded">{{ t("buttons.edit")}}</button>
          </div>
        </div>
        <div>
          <ul>
            UpdateError Status:
            <span>{{ updateErrorData.status }}</span>
            <li v-for="error of updateErrorData.errors">Error: {{ error.description }}</li>
          </ul>
        </div>
        <div>
          <ul>
            Update Success Status:
            <span>{{ updateSuccessData.status }}</span>
            <li>
              {{ updateSuccessData.data }}
            </li>
          </ul>
        </div>
      </div>
    </div>
  </main>
</template>
