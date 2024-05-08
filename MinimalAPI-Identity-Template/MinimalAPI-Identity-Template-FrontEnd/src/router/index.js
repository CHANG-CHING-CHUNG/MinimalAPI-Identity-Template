import { createRouter, createWebHistory } from 'vue-router'
import HomeView from '../views/HomeView.vue'
import RegisterVIew from '@/views/RegisterVIew.vue'
import LoginVIew from '@/views/LoginVIew.vue'
import LogoutView from '@/views/LogoutView.vue'
import GoogleLoginView from '@/views/GoogleLoginView.vue'
import CurrentUserVIew from '@/views/CurrentUserVIew.vue'
import UsersVIew from '@/views/UsersVIew.vue'
import GetCsrfToken from '@/views/GetCsrfToken.vue'
import ForgotPassword from '@/views/ForgotPassword.vue'
import EmailLogin from '@/views/EmailLogin.vue'
import GoogleLogoutView from '@/views/GoogleLogoutView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'home',
      component: HomeView
    },
    {
      path: '/register',
      name: 'register',
      component: RegisterVIew
    },
    {
      path: '/login',
      name: 'login',
      component: LoginVIew
    },
    {
      path: '/logout',
      name: 'logout',
      component: LogoutView
    },
    {
      path: '/google-login',
      name: 'google-login',
      component: GoogleLoginView
    },
    {
      path: '/current-user',
      name: 'current-user',
      component: CurrentUserVIew
    },
    {
      path: '/users',
      name: 'users',
      component: UsersVIew
    },
    {
      path: '/get-csrf-token',
      name: 'get-csrf-token',
      component: GetCsrfToken
    },
    {
      path: '/forgot-password',
      name: 'forgot-password',
      component: ForgotPassword
    },
    {
      path: '/email-login',
      name: 'email-login',
      component: EmailLogin
    },
    {
      path: '/google-logout',
      name: 'google-logout',
      component: GoogleLogoutView
    },
  ]
})

export default router
