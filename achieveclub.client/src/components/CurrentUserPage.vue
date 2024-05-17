<template>
    <header>
        <div class="exitLink">
            <RouterLink to="/login">
                <img id="exitImg" src="../assets/images/back_icon.png" />
            </RouterLink>
        </div>


        <div class="heading-wrapper">
        </div>

        <div class="userProfile">
            <div class="userImage">
                <img id="userProfileImg" :src="currentUser.avatar">
            </div>
            <div class="changePhotoContainer">
                <input type="file" id="changePhotoInput" class="changePhotoInput"
                    accept="image/jpeg,image/png,image/webp" />
                <label for="changePhotoInput" class="changePhotoText">
                    Изменить аватарку
                </label>
            </div>
            <div class="userId">
                <p>
                    <!--First Name-->
                    {{ currentUser.firstName }}

                    <!--Last Name-->
                    {{ currentUser.lastName }}
                    <br>
                    <a id="locate" href="#">Клуб Дворец</a>
                </p>
            </div>
        </div>
    </header>
    <main>
        <div class="generalInfo">
            <!--XP SUM-->
            <div class="xp">
                <h1 id="XPLevel">{{ currentUser.xpSum }} ХР</h1>
                <p id="XPSlogan">Общее количество опыта</p>
            </div>
            <hr id="hr">
            <div class="task">
                <!--Completed Count-->
                <h1 id="allTasks">Заданий выполнено: 100 из 500
                </h1>
                <!--Completed Ratio (%)-->
                <p id="tasksSlogan">Выполнено 42%</p>
            </div>
        </div>
        <div class="completed" id="completed">
            <h1 id="completedTasks">Выполненные </h1>
        </div>

        <!------------task completed------------------------->
        <div class="completedTask" id="completedTask">
            <div class="taskImg">
                <img id="logoTask" src="../assets/images/gllg.png" alt="">
            </div>
            <div class="aboutTask">
                <div class="mark">
                    <h1 id="nameTask">
                        Лучший программист | 100 XP
                    </h1>
                </div>
                <p class="infoTask" id="infoTask">
                    Начать писать фронт на Vue
                </p>
            </div>
            <div class="taskXP">
                <img class="userPerscentIcon" src="../assets/images/back_icon.png" alt="" />
                <p>
                    42%
                </p>
            </div>
        </div>
        <!------------task completed------------------------->
        <div class="notCompleted" id="notCompleted">
            <h1 id="notCompletedTasks">Невыполненные </h1>
        </div>

        <!---------------------Not completed------------------>
        <div class="notCompletedTask" id="notCompletedTask" @click="console.log('click')">
            <div class="taskImg">
                <img id="logoTask" src="../assets/images/gllg.png" alt="">
            </div>
            <div class="aboutTask">
                <div class="mark">
                    <h1 id="nameTask">
                        Лучший программист | 100 XP
                    </h1>
                </div>
                <p class="notInfoTask" id="notInfoTask">
                    Начать писать фронт на Vue
                </p>
            </div>
            <div class="taskXP taskXPNotCompleted">
                <img class="userPerscentIcon" src="../assets/images/back_icon.png"
                    style="background-color: black; border-radius: 25%;" alt="" />
                <p>
                    42%
                </p>
            </div>
        </div>
    </main>
</template>

<script setup lang="ts">
import { onMounted, ref } from "vue";
import { useRouter } from "vue-router";

const router = useRouter()

onMounted(TryLoadUser)

class User {
    id: number = 0;
    firstName: string = "";
    lastName: string = "";
    avatar: string = "";
    clubId: number = 0;
    clubName: string = "";
    clubLogo: string = "";
    xpSum: number = 0;
}

const currentUser = ref(new User())

async function TryLoadUser() {
    let result = await TryRefresh()
    if (result == false)
        router.push("/login");

    let f = await fetch("/api/users/current");
    currentUser.value = await f.json() as User;
}

async function TryRefresh(): Promise<boolean> {
    let f = await fetch("api/auth/refresh");
    return f.status == 200;
}

</script>

<style scoped>
header {
    background-color: #0D4E81;
    border-radius: 0px 0px 50px 50px;
    margin: 0;
}

nav {
    background: white;
    position: fixed;
    width: 480px;
    top: 1010px;
    box-shadow: 0 0 10px rgba(0, 0, 0, 0.5)
}

.userMenu {
    display: flex;
    justify-content: space-around;
}

#exitImg {
    width: 30px;
    height: 30px;
    transform: rotate(180deg);
}

.paragraph {
    padding: 15px;
}

#userParagraph {
    width: 50px;
}

.userProfile {
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: center;
}

.userImage {
    display: flex;
    flex-direction: column;
    align-items: flex-end;
}

#userProfileImg {
    width: 148px;
    height: 148px;
    border-radius: 50%;
}

.changePhotoInput {
    display: none;
}

.changePhotoContainer {
    margin-top: 10px;
    color: mediumaquamarine;
}

.changePhotoText {
    cursor: pointer;
}

.userId {
    font-family: 'Play', sans-serif;
    font-style: normal;
    font-weight: lighter;
    font-size: xx-large;
    color: white;
    align-items: center;
    text-align: center;
    margin-bottom: 10px;
}

.userId * {
    margin: 0;
}

#locate {
    color: mediumaquamarine !important;
    text-decoration: none;
}

#locate:visited {
    color: white;
}

.heading-wrapper {
    display: flex;
    justify-content: center;
}

#profileSlogan {
    display: flex;
    justify-content: center !important;
    width: 60%;
    font-size: 20px;
    font-family: 'Play', sans-serif;
    font-style: normal;
    font-weight: normal;
    margin-left: 20%;
    color: white;
    justify-content: flex-end;
}

.exitLink {
    display: flex;
    justify-content: left;
    padding-top: 20px;
    padding-left: 20px;
}

.exitLink a {
    cursor: pointer;
}

#locate,
#XPLevel,
#allTasks,
#nameTask,
#reward {
    font-family: 'Play', sans-serif;
    font-style: normal;
    font-weight: normal;
    font-size: 19px;
    margin: 0;
}

#completedTasks,
#notCompletedTasks {
    font-family: 'Play', sans-serif;
    font-style: normal;
    font-weight: normal;
    font-size: 29px;
    margin: 0;
    width: 80%;
    padding-left: 30px;
}

#locateSlogan,
#XPSlogan,
#tasksSlogan,
.infoTask,
.notInfoTask {
    font-family: 'Play', sans-serif;
    font-style: normal;
    font-weight: normal;
    margin: 0;
    color: #818181;
    padding-top: 5px;
    width: 98%;
}

.notInfoTask {
    color: black !important;
}

.taskXP {
    padding-right: 10px;
    display: flex;
    flex-direction: column;
    align-items: center;
}

.infoTask {
    color: black !important;
}

.userPerscentIcon {
    width: 20px;
}

.taskXP p {
    font-size: large;
    font-weight: bold;
    margin: 0;
    /*margin-right: 4px;*/
    color: white !important;
}

.taskXPNotCompleted p {
    color: black !important
}

.generalInfo {
    text-align: center;
}

.generalInfo,
.completedTask,
.notCompletedTask {
    background: white;
    margin: 30px;
    padding: 15px;
    border-radius: 20px;
}

#completed,
#notCompleted {
    margin-bottom: 10px;
}

#notCompleted {
    margin-top: 15px;
}

#completedTask,
#notCompletedTask {
    margin-top: 10px;
    margin-bottom: 10px;
    background: #7BC3FF;
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 8px;
}

#notCompletedTask {
    background: white;
}

#notComplitedTaskHighlighted {
    background: #98ffdf;
}

#acceptList {
    right: 22%;
    bottom: 11%;
    z-index: 1;
    text-align: center;
    position: fixed;
    border-radius: 20px;
    padding: 15px;
    background: #98ffdf;
}

.aboutTask {
    width: 100%;
}

#infoTask {
    color: white;
}

#logoTask {
    width: 60px;
    object-fit: cover;
}

#clubName {
    color: #0D4E81;
}


#hr {
    background: #818181;
}

#markUp,
.markUpRed {
    margin: 0;
    font-family: 'Play', sans-serif;
    font-style: normal;
    font-weight: normal;
    border: 1px solid #0D4E81;
    padding: 4px;
    border-radius: 10px;
    color: #0D4E81;
}

#markUpRed {
    border: 1px solid #ff0000;
    color: #ff0000;
}

.mark {
    display: flex;
    justify-content: space-between;
}

.taskXP {
    padding-left: 10px;
}

.taskImg {
    padding-right: 10px;
    padding-top: 4px;
}

#markUp {
    height: 20px;
}

.markUpRed {
    height: 20px;
}

footer {
    width: 100%;
    position: fixed;
    bottom: 0;
}

.footerSections {
    display: flex;
    background: white;
    width: 100%;
    justify-content: center;
    -webkit-box-shadow: 0px -5px 15px 4px rgba(0, 0, 0, 0.2);
    -moz-box-shadow: 0px -5px 15px 4px rgba(0, 0, 0, 0.2);
    box-shadow: 0px -5px 15px 4px rgba(0, 0, 0, 0.2);
}

footer img {
    width: 50px;
    margin: 10px 30px 5px 30px;
    object-fit: cover;
}

.secondSection hr {
    border-bottom: 5px solid #0D4E81;
    border-radius: 10px 10px 0px 0px;
    margin: 0;
}

.px {
    width: 100%;
    height: 75px;
}

@media(max-width:430px) {
    #infoTask {
        text-align: left;
    }
}

@media(max-width:600px) {

    .generalInfo,
    .completedTask,
    .notCompletedTask {
        margin-left: 10px;
        margin-right: 10px;
    }

    #notCompletedTasks,
    #completedTasks {
        padding-left: 10px;
    }
}

@media (max-width: 800px) {
    #acceptList {
        right: 2%;
    }
}

@media (min-width:1100px) {
    #infoTask {
        width: 100%;
        text-align: left;
    }

    .notInfoTask {
        width: 100%;
        text-align: left;
    }

    #nameTask {
        text-align: left;
    }

    .mark {
        display: unset;
    }

    .taskXP {
        width: 100px;
        text-align: right;
    }

    .notCompletedTask {
        cursor: pointer;
    }
}
</style>