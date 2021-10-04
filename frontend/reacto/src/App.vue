<template>
  <header class="mb-2">
    <nav class="navbar navbar-dark">
      <div class="container-fluid">
        <a class="navbar-brand" href="/">Reacto</a>
      </div>
    </nav>
  </header>
  <main class="container">
    <router-view />
  </main>
</template>

<style scoped lang="scss">
nav {
  background-color: #29335c;
}
</style>
