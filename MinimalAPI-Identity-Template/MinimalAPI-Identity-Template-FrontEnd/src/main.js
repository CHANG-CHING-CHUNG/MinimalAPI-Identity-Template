import "./index.css";
import { createApp } from "vue";
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

const app = createApp(App);
app.provide("axios", axiosInstance);
app.use(createPinia());
app.use(router);

app.mount("#app");
