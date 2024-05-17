<template>
  <!-----------------------------------nav----------------------------------------->
  <header>
    <div class="header-wrapper">
      <div class="img">
        <img src="../assets/images/gllg.png" alt="" />
      </div>
      <div class="text-wrapper">
        <h3>
          <b>Система достижений</b>
        </h3>
      </div>
    </div>
  </header>
  <!-----------------------------------main----------------------------------------->
  <main>
    <div class="content-wrapper">
      <div class="headingWrapper">
        <h3>Зарегистрироваться</h3>
      </div>
      <div class="registerForm">
        <hr />
        <p><a @onclick="() => {}">Войти в систему</a></p>
      </div>
      <div class="emailForm">
        <span>Имя</span>
        <input type="text" v-model="registrationParams.firstName"/>
      </div>
      <div class="emailForm">
        <span>Фамилия</span>
        <input type="text" v-model="registrationParams.lastName"/>
      </div>
      <div class="emailForm">
        <span>Email</span>
        <input type="email" v-model="registrationParams.email"/>
      </div>
      <div class="emailForm">
        <span>Место обучения</span>
        <select v-model="registrationParams.clubId">
          <option
            :value="clubTitle.id"
            v-for="clubTitle in clubTitles"
            :key="clubTitle.id"
          >
            {{ clubTitle.title }}
          </option>
        </select>
      </div>
      <div class="passwordForm">
        <span>Пароль</span>
        <input type="password" v-model="registrationParams.password"/>
      </div>
      <div class="passwordForm">
        <span>Подтверждение пароля</span>
        <input type="password" />
      </div>
      <div class="submitButton">
        <button type="submit" @click="console.log(registrationParams)">Регистрация</button>
      </div>
      <p style="color: red">Errors</p>
    </div>
  </main>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue";

class ClubTitle {
  id: number = 0;
  title: string = "";
}

class RegistrationParams{
    firstName: string = "";
    lastName: string = "";
    clubId: number = 0;
    email: string = "";
    password: string = "";
    avatarURL: string = "";
}

const clubTitles = ref<Array<ClubTitle>>();
const registrationParams = ref<RegistrationParams>(new RegistrationParams())

onMounted(() => LoadTitles());

async function LoadTitles() {
  let f = await fetch("/api/clubs/titles");
  if (f.status == 200) {
    clubTitles.value = (await f.json()) as Array<ClubTitle>;
  }
}
</script>

<style scoped>
header {
  background-color: #0d4e81;
  border-radius: 0px 0px 50px 50px;
}

.input__file-cont {
  width: 100% !important;
  margin: 0;
}

#input__file {
  display: none;
}

.input__file-icon-wrapper {
  height: 60px;
  width: 60px !important;
  display: -webkit-box;
  display: -ms-flexbox;
  display: flex;
  -webkit-box-align: center;
  -ms-flex-align: center;
  align-items: center;
  -webkit-box-pack: center;
  -ms-flex-pack: center;
  justify-content: center;
  border-right: 1px solid #fff;
}

.input__file-icon-wrapper i {
  font-size: 30px;
}

.input__file-button-text {
  line-height: 1;
  margin-top: 1px;
}

.input__file-button {
  width: 100% !important;
  height: 38px;
  background: #4e9bda;
  color: #fff;
  text-align: center;
  display: -webkit-box;
  display: -ms-flexbox;
  display: flex;
  -webkit-box-align: center;
  -ms-flex-align: center;
  align-items: center;
  -webkit-box-pack: start;
  -ms-flex-pack: start;
  justify-content: flex-start;
  border-radius: 3px;
  cursor: pointer;
  margin: 0 auto;
}

.header-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
  color: white;
  height: 100px;
}

.img img {
  width: 60px;
  background: white;
  object-fit: cover;
  margin-right: 15px;
  padding: 3px;
  border-radius: 10px;
}

main {
  display: flex;
  justify-content: center;
  align-items: center;
  vertical-align: middle;
  margin-top: 50px;
  height: inherit;
}

.content-wrapper * {
  width: 90%;
  margin-top: 5px;
  margin-bottom: 5px;
}

.content-wrapper {
  display: flex;
  border: 1px solid white;
  background: white;
  border-radius: 10px;
  justify-content: center;
  text-align: left;
  flex-direction: column;
  width: 300px;
  padding: 5%;
}

.registerForm p {
  cursor: pointer;
}

.registerForm hr {
  margin: 0;
  color: #8f8f8f;
  width: 108%;
}

.emailForm input {
  padding: 10px;
  border-radius: 5px;
  border: 1px solid #8f8f8f;
  width: 278px;
}

.emailForm select {
  padding: 10px;
  border-radius: 5px;
  border: 1px solid #8f8f8f;
  width: 300px;
  background: white;
}

.emailForm select option {
  background: white;
}

.emailForm {
  color: #8f8f8f;
  font-size: 14px;
  width: 100%;
}

.passwordForm input {
  padding: 10px;
  border-radius: 5px;
  border: 1px solid #8f8f8f;
  width: 278px;
}

.passwordForm {
  font-size: 14px;
  color: #8f8f8f;
  width: 100%;
}

#inputFileImg-cont {
  display: flex;
  justify-content: center;
  width: 100%;
}

.headingWrapper {
  margin: 0;
  width: 100%;
}

.submitButton {
  width: 100%;
}

.submitButton button {
  width: 100%;
  background: #4e9bda;
  border-radius: 5px;
  border: 1px solid #4e9bda;
  color: white;
  font-family: "Play", sans-serif;
  height: 45px;
  font-size: large;
  font-weight: bold;
  padding: 0;
  cursor: pointer;
}

.registerForm p a {
  text-decoration: none;
  color: #8f8f8f;
  font-weight: bold;
  font-size: 14px;
}

.registerForm {
  margin: 0;
}

.registerForm hr {
  width: 300px;
}
</style>
