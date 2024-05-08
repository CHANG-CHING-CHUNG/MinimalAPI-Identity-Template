<script setup>
import { RouterLink, RouterView } from 'vue-router'
</script>

<template>
  <header>
    <div class="bg-slate-200 p-5">
      <nav class="flex justify-between flex-wrap gap-2">
        <RouterLink active-class="text-purple-600" class="bg-white px-2 py-2" to="/register">註冊</RouterLink>
        <RouterLink active-class="text-purple-600" class="bg-white px-2 py-2" to="/login">登入</RouterLink>
        <RouterLink active-class="text-purple-600" class="bg-white px-2 py-2" to="/logout">登出</RouterLink>
        <RouterLink active-class="text-purple-600" class="bg-white px-2 py-2" to="/google-logout">Google 登出</RouterLink>
        <RouterLink active-class="text-purple-600" class="bg-white px-2 py-2" to="/google-login">Google 登入</RouterLink>
        <RouterLink active-class="text-purple-600" class="bg-white px-2 py-2" to="/current-user">當前使用者資料</RouterLink>
        <RouterLink active-class="text-purple-600" class="bg-white px-2 py-2" to="/users">使用者列表</RouterLink>
        <RouterLink active-class="text-purple-600" class="bg-white px-2 py-2" to="/get-csrf-token">取得 CSRF TOKEN Cookie</RouterLink>
        <RouterLink active-class="text-purple-600" class="bg-white px-2 py-2" to="/forgot-password">忘記密碼</RouterLink>
        <RouterLink active-class="text-purple-600" class="bg-white px-2 py-2" to="/email-login">Email 無密碼登入</RouterLink>

      </nav>
    </div>
  </header>

  <div class="p-5 bg-slate-200">
    <RouterView />
  </div>
</template>