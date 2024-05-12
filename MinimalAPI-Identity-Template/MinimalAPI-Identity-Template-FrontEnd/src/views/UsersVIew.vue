<script setup>
import { inject, ref } from "vue";
import { useI18n } from "vue-i18n";
const { t } = useI18n({ useScope: "global" });

const axios = inject("axios");
const successData = ref({});
const errorData = ref({});
const updateErrorData = ref({});

const getUserList = async () => {
  successData.value = {};
  errorData.value = {};
  axios
    .get("/api/identity/user-manage/users")
    .then((res) => (successData.value = res.data))
    .catch((errors) => {
      errorData.value = errors.response.data;
    });
};

const updateNickname = async (user) => {
  if (!(successData.value.data && successData.value.data.length)) return;
  updateErrorData.value = {};
  axios
    .patch(`/api/identity/user-manage/users/${user.id}`, {
      nickname: user.newNickname,
    })
    .then((res) => {
      alert("Update nickname successed")
    })
    .catch((errors) => {
      alert(`Update nickname failed ${errors}`)
    });
};

const deleteUser = async (userId) => {
  if (!(successData.value.data && successData.value.data.length)) return;
  axios
    .delete(`/api/identity/user-manage/users/${userId}`)
    .then((res) => {
      console.log("Delete user successed")
      successData.value.data = successData.value.data.filter(user => {
        return user.id !== userId
      })
    })
    .catch((errors) => {
      console.log("Delete user failed", errors.response.data)
    });
}
</script>

<template>
  <main>
    <div>
      <div>
        <button @click="getUserList" class="my-1 mx-1 py-1 px-3 bg-green-600 text-white rounded">
          {{ t("buttons.getUserList") }}
        </button>
      </div>
      <p class="text-xl">{{ t("pages.userList.title") }}</p>
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
          <li v-for="user in successData.data">
            <div>
              <p>{{ user }}</p>
              <input v-model="user.newNickname" placeholder="輸入新的暱稱" />
              <button
                @click="updateNickname(user)"
                class="my-1 mx-1 py-1 px-3 bg-green-600 text-white rounded"
              >
              {{ t("buttons.edit") }}
              </button>
              <button @click="deleteUser(user.id)" class="my-1 mx-1 py-1 px-3 bg-red-400 text-white rounded">{{ t("buttons.delete") }}</button>
            </div>
          </li>
        </ul>
      </div>
    </div>
  </main>
</template>
