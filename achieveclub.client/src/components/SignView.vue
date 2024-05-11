
<template>
<header>
       <div class="log">
           <img src="./assets/LOGO.png" alt="">
       </div>
       <div>
           <p class="txt"><span>Клуб</span> It_Club</p>
       </div>
    </header>
    <section>
       <div class="form-register">
           <p style="font-size: 48px;">Войти в систему</p>
           <div class="form">
               <input v-model="email" type="email" placeholder="Email">
               <input v-model="password" type="password" placeholder="Пароль">
               <button id="btn-r">Войти</button>
            </div>
            <div class="b"></div>
            <p id="href">Зарегистрироватся</p>
       </div>
    </section>
</template>

<script lang="ts">
import {defineComponent} from 'vue'
export default defineComponent({
   data(){
      return{
         email:'',
         password:'',
         ck:{}
      }
   },
   methods:{
      Add(){
              this.ck={
               email:this.email,
               password:this.password
              }
              fetch("/api/auth/registration",{method:'POST',body: JSON.stringify(this.ck),headers: {'Content-Type': 'application/json',},})}
   }
})

</script>

<style scoped>
    @font-face {
   font-family: Play;
   src: url("./fonts/Play-Bold.ttf");
}
*{
   margin:0;
   padding:0;
   box-sizing: border-box;
   font-family:Play;
}
header{
   width:100%;
   background-color: #0D4E81;
   border-radius:0px 0px 30px 30px;
   display: flex;
   justify-content: center;
   align-items: center;
   gap:2%;
   padding:1%;
}
.txt{
   color:aliceblue;
   font-size: 25px;
   word-spacing: 3%;
}
span{
   font-size:40px;
   font-weight: 700;
}
.log{
   background-color: aliceblue;
   border-radius: 40px;
   padding:1vh;
}
img{
   width:100px;
   height:100px;
}
section{
   display:flex;
   justify-content: center;
   flex-direction: center;
   padding:5%;
   gap:5%;
}
#href{
   color:rgba(124, 124, 124, 1);
   font-size:36px;
}
.b{
   border-bottom:2px solid rgba(124, 124, 124, 1);
   margin-top:2%;
}
.form{
   display:flex;
   flex-direction: column;
   gap:2%;
}
#btn-r{
   height:5vh;
   background-color: rgba(78, 155, 218, 1);
   border:none;
   border-radius: 10px;
   color:aliceblue;
   margin-top: 2%;
}
input{
   width:100%;
   height:8vh;
   margin-top: 2%;
}
</style>
