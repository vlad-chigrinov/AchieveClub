import { createApp } from "vue";
import App from "./App.vue";
import "./assets/css/app.css";
import { createWebHistory, createRouter } from "vue-router";

import CurrentUserPage from "./components/CurrentUserPage.vue";
import LoginPage from "./components/LoginPage.vue";
import RegistrationPage from "./components/RegistrationPage.vue";

const routes = [
  { path: "/", component: CurrentUserPage },
  { path: "/login", component: LoginPage },
  { path: "/registration", component: RegistrationPage },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

createApp(App).use(router).mount("#app");
