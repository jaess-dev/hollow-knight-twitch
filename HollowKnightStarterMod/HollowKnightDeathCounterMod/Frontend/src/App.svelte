<script lang="ts">
  import svelteLogo from "./assets/svelte.svg";
  import viteLogo from "/vite.svg";
  import Counter from "./lib/Counter.svelte";
  import { onMount, onDestroy } from "svelte";

  let socket: WebSocket;

  onMount(() => {
    console.log("mounting");

    socket = new WebSocket("ws://localhost:5000/ws/", );

    socket.addEventListener("open", () => {
      console.log("WebSocket connected");
    });

    socket.addEventListener("message", (event) => {
      console.log("WebSocket message received:", event.data);
    });

    socket.addEventListener("error", (error) => {
      console.error("WebSocket error:", error);
    });

    socket.addEventListener("close", (event) => {
      console.log("WebSocket closed:", event.code, event.reason);
    });
  });

  onDestroy(() => {
    if (socket && socket.readyState === WebSocket.OPEN) {
      socket.close();
    }
  });
</script>

<main>
  <div>
    <a href="https://vite.dev" target="_blank" rel="noreferrer">
      <img src={viteLogo} class="logo" alt="Vite Logo" />
    </a>
    <a href="https://svelte.dev" target="_blank" rel="noreferrer">
      <img src={svelteLogo} class="logo svelte" alt="Svelte Logo" />
    </a>
  </div>
  <h1>Vite + Svelte</h1>

  <div class="card">
    <Counter />
  </div>

  <p>
    Check out <a
      href="https://github.com/sveltejs/kit#readme"
      target="_blank"
      rel="noreferrer">SvelteKit</a
    >, the official Svelte app framework powered by Vite!
  </p>

  <p class="read-the-docs">Click on the Vite and Svelte logos to learn more</p>
</main>

<style>
  .logo {
    height: 6em;
    padding: 1.5em;
    will-change: filter;
    transition: filter 300ms;
  }
  .logo:hover {
    filter: drop-shadow(0 0 2em #646cffaa);
  }
  .logo.svelte:hover {
    filter: drop-shadow(0 0 2em #ff3e00aa);
  }
  .read-the-docs {
    color: #888;
  }
</style>
