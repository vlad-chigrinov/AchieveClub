<template>
  <header>
    <div class="log">
      <img src="./assets/LOGO.png" alt="" />
    </div>
    <div>
      <p class="txt"><span>Клуб</span> It_Club</p>
    </div>
  </header>
  <section>
    <div class="form-register">
      <a id="href" href="SignForm.vue">Войти в систему</a>
      <br /><br />
      <div class="b"></div>
      <p style="font-size: 28px; margin-top: 2%">Зарегистрироватся</p>
      <div class="form">
        <label id="l1">Имя</label>
        <input type="text" placeholder="Имя" v-model="this.firstname" />
        <div class="error" v-if="firstname.length < 2">
          <p>Имя должно содержать больше 2 символов</p>
        </div>

        <label>Фамилия</label>
        <input type="text" placeholder="Фамилия" v-model="this.lastname" />
        <div class="error" v-if="lastname.length < 3">
          <p>Фамилия должна содержать больше 3 символов</p>
        </div>
        <label>Email</label>
        <input type="email" placeholder="Email" v-model="this.email" />
        <select class="input">
          <option></option>
          <option></option>
        </select>
        <label>Паполь</label>
        <input type="password" placeholder="Пароль" v-model="this.password" />
        <label>Потверждение пароля</label>
        <input type="password" placeholder="Подтверждение пароля" v-model="this.repeatedPassword" />
        <div class="error" v-if="this.password != this.repeatedPassword">
          <p>Пароли должны совпадать</p>
        </div>
        <button id="btn-r" @click="Add()">Регистрация</button>
      </div>
    </div>
  </section>
</template>
<script lang="ts">


//import formOne from 'SignForm.vue'
type ClubTitle = {title:string, id:number}
    export default{
        data(){
            return{
                firstname:'',
                lastname:'',
                clubId:null,
                email:null,
                password:null,
                repeatedPassword: null,
                Acc:{},
                clubTitles:Array<ClubTitle>
               
               


            }
        },
        methods:{
            Add(){
              this.Acc = {
                "firstname":this.firstname,
                "lastname":this.lastname,
                "clubId":this.clubId,
                "email":this.email,
                "password":this.password,
                "avatarURL":'/'
              }
              fetch("/api/auth/registration",{
                method:'POST',
                body: JSON.stringify(this.Acc),
                headers: {
                'Content-Type': 'application/json',
          },

          })

        },
        async GetClubId(){
            var data = await fetch("/api/clubs/titles");
            var json = await data.json();
            this.clubTitles = json;
            console.log(this.clubTitles);
        }
    },
    mounted(){
        this.GetClubId();
    }
}
</script>
<style>
#l1{
  margin-top:5%;
}
label{
  font-size:15px;
}
.error {
  width: 100%;
  height: 3vh;
  font-size: 10px;
  color: red;
  border: 1px solid red;
  margin-top: 2%;
  padding: 1%;
}
@font-face {
  font-family: Play;
  src: url("./fonts/Play-Bold.ttf");
}
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
  font-family: Play;
  text-decoration: none;
}
header {
  width: 100%;
  background-color: #0d4e81;
  border-radius: 0px 0px 30px 30px;
  display: flex;
  justify-content: center;
  align-items: center;
  gap: 2%;
  padding: 1%;
}
.txt {
  color: aliceblue;
  font-size: 25px;
  word-spacing: 3%;
}
span {
  font-size: 40px;
  font-weight: 700;
}
.log {
  background-color: aliceblue;
  border-radius: 40px;
  padding: 1vh;
}
img {
  width: 100px;
  height: 100px;
}
section {
  display: flex;
  justify-content: center;
  flex-direction: center;
  padding: 5%;
  gap: 5%;
}
#href {
  color: rgba(124, 124, 124, 1);
  font-size: 26px;
}
.b {
  border-bottom: 2px solid rgba(124, 124, 124, 1);
}
.form {
  display: flex;
  flex-direction: column;
  gap: 2%;
}
#btn-r {
  height: 5vh;
  background-color: rgba(78, 155, 218, 1);
  border: none;
  border-radius: 10px;
  color: aliceblue;
  margin-top: 2%;
}
input {
  width: 100%;
  height: 4vh;
  margin-top: 2%;
}
.input {
  width: 100%;
  height: 4vh;
  margin-top: 2%;
}
</style>
