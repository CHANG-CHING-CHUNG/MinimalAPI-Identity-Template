<script setup>
import { useI18n } from "vue-i18n";
const { t } = useI18n({ useScope: "global" });
</script>

<template>
  <main>
    <a href="https://localhost:7281/account/google-login?returnUrl=/current-user" class="py-3 px-8 bg-[#04AA6D] text-white rounded">{{ t("buttons.googleLogin") }}</a>
  </main>
</template>
