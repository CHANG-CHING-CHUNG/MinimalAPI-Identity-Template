<script setup>
import { RouterLink, RouterView } from "vue-router";
import { useI18n } from "vue-i18n";
const { t, locale, availableLocales } = useI18n({ useScope: "global" });
</script>

<template>
  <div class="h-dvh bg-slate-200">
    <header>
      <div class="p-4">
        <div class="max-w-sm h-20">
          <label for="locale" class="block mb-2 text-sm font-medium text-gray-900 dark:text-white">{{
            t("langSelection.switchLang")
          }}</label>
          <select
            id="locale"
            class="bg-gray-50 border border-gray-300 text-gray-900 text-sm rounded-lg focus:ring-blue-500 focus:border-blue-500 block w-full p-2.5 dark:bg-gray-700 dark:border-gray-600 dark:placeholder-gray-400 dark:text-white dark:focus:ring-blue-500 dark:focus:border-blue-500"
            v-model="locale"
          >
            <option
              v-for="locale in $i18n.availableLocales"
              :key="`locale-${locale}`"
              :value="locale"
            >
              {{ t(`langSelection.${locale}`) }}
            </option>
          </select>
        </div>
      </div>
      <div class="p-5">
        <nav class="flex justify-between flex-wrap gap-2">
          <RouterLink active-class="text-purple-600" class="bg-white px-2 py-2" to="/register">{{
            t("headerMenuBtn.register")
          }}</RouterLink>
          <RouterLink active-class="text-purple-600" class="bg-white px-2 py-2" to="/login">{{
            t("headerMenuBtn.login")
          }}</RouterLink>
          <RouterLink active-class="text-purple-600" class="bg-white px-2 py-2" to="/logout">{{
            t("headerMenuBtn.logout")
          }}</RouterLink>
          <RouterLink active-class="text-purple-600" class="bg-white px-2 py-2" to="/google-logout">{{
            t("headerMenuBtn.googleLogout")
          }}</RouterLink>
          <RouterLink active-class="text-purple-600" class="bg-white px-2 py-2" to="/google-login">{{
            t("headerMenuBtn.googleLogin")
          }}</RouterLink>
          <RouterLink active-class="text-purple-600" class="bg-white px-2 py-2" to="/current-user">{{
            t("headerMenuBtn.currentUserData")
          }}</RouterLink>
          <RouterLink active-class="text-purple-600" class="bg-white px-2 py-2" to="/users">{{
            t("headerMenuBtn.userList")
          }}</RouterLink>
          <RouterLink
            active-class="text-purple-600"
            class="bg-white px-2 py-2"
            to="/get-csrf-token"
            >{{ t("headerMenuBtn.getCsrfTokenCookie") }}</RouterLink
          >
          <RouterLink
            active-class="text-purple-600"
            class="bg-white px-2 py-2"
            to="/forgot-password"
            >{{ t("headerMenuBtn.forgotPassword") }}</RouterLink
          >
          <RouterLink active-class="text-purple-600" class="bg-white px-2 py-2" to="/email-login">{{
            t("headerMenuBtn.emailLogin")
          }}</RouterLink>
        </nav>
      </div>
    </header>
  
    <div class="p-5">
      <RouterView />
    </div>
  </div>
</template>
