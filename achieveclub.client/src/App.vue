<template>
    <div v-if="achievements">
        <template v-for="(item, key) in achievements" :key="key">
            <h3>{{ item["title"] }}</h3>
            <p>{{ item["description"] }}</p>
        </template>
    </div>
    <div v-else>
        <h2>Loading...</h2>
    </div>
</template>


<script setup lang="ts">
import { onMounted, ref } from "vue";

const achievements = ref<Array<any>>()

onMounted(async () => {
    var f = await fetch("/api/achievements");
    achievements.value = await f.json();
    console.log(achievements);
})
</script>