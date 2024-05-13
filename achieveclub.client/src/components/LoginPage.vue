<template>
    <header>
        <div class="header-wrapper">
            <div class="img">
                <img src="../assets/images/gllg.png" alt="">
            </div>
            <div class="text-wrapper">
                <h3>
                    <b>Система достижений</b>
                </h3>
            </div>
        </div>
    </header>
    <main>
        <div class="content-wrapper">
            <div class="headingWrapper">
                <h3>Войти в систему</h3>
            </div>
            <div class="registerForm">
                <hr />
                <RouterLink to="/registration">Зарегистрироваться</RouterLink>
            </div>
            <div class="emailForm">
                <span>Email</span>
                <input type="email" id="emailInput" v-model="loginParams.Email"/>
            </div>
            <div class="passwordForm">
                <span>Пароль</span>
                <input type="password" id="passwordInput" v-model="loginParams.Password">
            </div>
            <div class="submitButton">
                <button type="submit" @click="Login">Войти</button>
            </div>
            <p v-if="errors">{{errors}}</p>
        </div>
    </main>
</template>

<script setup lang="ts">

import { debug } from "console";
import { METHODS } from "http";
import { json } from "stream/consumers";
import { reactive, ref } from "vue";
import { useRouter } from "vue-router";

class LoginParams{
    Email: string = "";
    Password: string = "";
    constructor(email: string, password: string) {
        this.Email = email;
        this.Password = password;
    }
}

const loginParams = reactive(new LoginParams("", ""));
const errors = ref("");
const router = useRouter();

async function Login(){
    console.log(loginParams)
    let f = await fetch("api/auth/login", {method: "POST", body: JSON.stringify({email:loginParams.Email, password: loginParams.Password}), headers: {"Content-Type":"text/json"}});

    if(f.status == 200){
        router.push("/");
    }else{
        errors.value = f.statusText;
    }
}

</script>

<style scoped>
header {
    background-color: #0D4E81;
    border-radius: 0px 0px 50px 50px;
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
    height: 300px;
    padding: 5%;
}

.registerForm p {
    cursor: pointer;
}

.registerForm hr {
    margin: 0;
    color: #8F8F8F;
    width: 108%;
}

.emailForm input {
    padding: 10px;
    border-radius: 5px;
    border: 1px solid #8F8F8F;
}

.emailForm {
    color: #8F8F8F;
    font-size: 14px;
    width: 100%;
}

.passwordForm input {
    padding: 10px;
    border-radius: 5px;
    border: 1px solid #8F8F8F;
}

.passwordForm {
    font-size: 14px;
    color: #8F8F8F;
    width: 100%;
}

.headingWrapper {
    margin: 0;
    width: 100%;
}

.submitButton {
    width: 98%;
}

.submitButton button {
    width: 100%;
    background: #4E9BDA;
    border-radius: 5px;
    border: 1px solid #4E9BDA;
    color: white;
    font-family: 'Play', sans-serif;
    height: 45px;
    font-size: large;
    font-weight: bold;
    padding: 0;
    cursor: pointer;
}

.registerForm p {
    text-decoration: none;
    color: #8F8F8F;
    font-weight: bold;
    font-size: 14px;
    cursor: pointer;
}

.registerForm {
    margin: 0;
}
</style>