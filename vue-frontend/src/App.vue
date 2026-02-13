<script setup>
import HomePage from './components/HomePage.vue'
</script>

<template>
  <div id="app">
    <HomePage />
  </div>
</template>

<style>
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial,
    sans-serif;
  background-color: #f0f2f5;
}

#app {
  min-height: 100vh;
}
</style>
