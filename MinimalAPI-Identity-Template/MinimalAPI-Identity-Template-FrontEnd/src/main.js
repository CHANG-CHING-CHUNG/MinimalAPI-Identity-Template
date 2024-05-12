import "./index.css";
import { createApp } from "vue";
import { createI18n } from "vue-i18n";
import { createPinia } from "pinia";

import App from "./App.vue";
import router from "./router";
import axios from "axios";
const httpsBaseURL = "https://localhost:7281";
const httpBaseURL = "http://localhost:5247";
const axiosInstance = axios.create({
  baseURL: httpsBaseURL,
  timeout: 8000,
  withCredentials: true,
});
const i18n = createI18n({
  legacy: false,
  locale: "zh_tw",
  fallbackLocale: "en",
  messages: {
    en: {
      langSelection: {
        switchLang: "Switch Language",
        en: "English",
        zh_tw: "Chinese (Traditional)",
      },
      headerMenuBtn: {
        register: "Register",
        login: "Login",
        logout: "Logout",
        googleLogout: "Google Logout",
        googleLogin: "Google Login",
        currentUserData: "Current User Data",
        userList: "User List",
        getCsrfTokenCookie: "Get CSRF Token Cookie",
        forgotPassword: "Forgot Password",
        emailLogin: "Email Login Without Password",
      },
      buttons: {
        submit: "Submit",
        googleLogin: "Google Login",
        getCurrentLoggedInUserData: "Get Current logged in User Data",
        edit: "Edit",
        getUserList: "Get User List",
        delete: "Delete",
        getCsrfToken: "Get CSRF Token",
        validateToken: "Validate Token",
      },
      pages: {
        logout: {
          title: "Current Login Status",
        },
        googleLogout: {
          title: "Google Current Login Status",
        },
        currentUser: {
          title: "Current Logged In User Info",
          nicknameLabel: "New Nickname",
        },
        userList: {
          title: "User List",
        },
        csrfToken: {
          title: "Received CSRF Token",
        },
        forgotPassword: {
          getResetLinkTitle: "Forgot Password: Get Password Reset Link",
          resetPasswordTitle: "Forgot Password: Reset Password",
        },
        emailLogin: {
          title: "Get Email Login Link",
        },
      },
    },
    zh_tw: {
      langSelection: {
        switchLang: "切換語言",
        en: "English",
        zh_tw: "繁體中文",
      },
      headerMenuBtn: {
        register: "註冊",
        login: "登入",
        logout: "登出",
        googleLogout: "Google 登出",
        googleLogin: "Google 登入",
        currentUserData: "當前使用者資料",
        userList: "使用者列表",
        getCsrfTokenCookie: "取得 CSRF TOKEN Cookie",
        forgotPassword: "忘記密碼",
        emailLogin: "Email 無密碼登入",
      },
      buttons: {
        submit: "送出",
        googleLogin: "Google 登入",
        getCurrentLoggedInUserData: "取得目前登入的使用者資料",
        edit: "編輯",
        getUserList: "取得使用者列表",
        delete: "刪除",
        getCsrfToken: "取得 Token",
        validateToken: "驗證 Token",
      },
      pages: {
        logout: {
          title: "目前登入狀態",
        },
        googleLogout: {
          title: "Google 目前登入狀態",
        },
        currentUser: {
          title: "目前登入的使用者資料",
          nicknameLabel: "新的暱稱",
        },
        userList: {
          title: "使用者列表",
        },
        csrfToken: {
          title: "已取得的 CSRF Token",
        },
        forgotPassword: {
          getResetLinkTitle: "忘記密碼：取得重設密碼連結",
          resetPasswordTitle: "忘記密碼：重設密碼",
        },
        emailLogin: {
          title: "Email 取得無密碼登入連結",
        },
      },
    },
  },
});
const app = createApp(App);
app.provide("axios", axiosInstance);
app.use(createPinia());
app.use(router);
app.use(i18n);
app.mount("#app");

if (window.navigator.language === "zh-TW") {
  i18n.global.locale.value = "zh_tw";
} else {
  i18n.global.locale.value = "en";
}
