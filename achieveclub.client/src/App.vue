<template>
    <RouterView v-if="loaded" />
</template>
<script setup lang="ts">
import { onMounted, ref } from "vue";
import { useRouter } from "vue-router";

const router = useRouter()
const loaded = ref(false)

onMounted(LoadCurrentUser);

async function LoadCurrentUser() {
    let result = await TryRefresh()
    if (result == false)
        router.push("/login");
    else
        router.push("/");

    loaded.value = true;
}

async function TryRefresh(): Promise<boolean> {
    let f = await fetch("/api/auth/refresh");
    return f.status == 200;
}

</script>