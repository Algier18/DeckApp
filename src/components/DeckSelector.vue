<template>
  <div class="deck-panel">
    <h2>🎴 Wybór talii</h2>

    <div class="deck-actions-top">
      <label class="file-input">
        <input type="file" @change="importDeck" />
        📤 Wczytaj talię (PDF/XLS)
      </label>
      <button class="btn-outline" @click="downloadTemplate">📥 Pobierz szablon kart</button>
    </div>

    <div class="deck-select">
      <label for="deckDropdown">Wybierz talię:</label>
      <select id="deckDropdown" v-model="selectedDeckId" @change="onSelectChange">
        <option disabled value="">-- wybierz --</option>
        <option v-for="deck in decks" :key="deck.id" :value="deck.id">
          {{ deck.name }}
        </option>
      </select>
    </div>

    <div class="deck-actions">
      <button class="btn-warning" @click="renameDeck" :disabled="!selectedDeckId">✏️ Zmień nazwę</button>
      <button class="btn-danger" @click="deleteDeck" :disabled="!selectedDeckId">🗑️ Usuń</button>
    </div>

    <div v-if="selectedDeck" class="deck-selected">
      <h3>✅ Wybrana talia: {{ selectedDeck.name }}</h3>
      <p class="muted">ID: {{ selectedDeck.id }}</p>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'

const API_BASE = "https://localhost:7144/api/decks"

const decks = ref([])
const selectedDeckId = ref("")
const selectedDeck = ref(null)

async function refreshDecks() {
  const res = await fetch(API_BASE)
  decks.value = await res.json()
}

function onSelectChange() {
  selectDeck(selectedDeckId.value)
}

async function selectDeck(id) {
  if (!id) { selectedDeck.value = null; return }
  const res = await fetch(`${API_BASE}/${id}`)
  selectedDeck.value = await res.json()
}

async function renameDeck() {
  if (!selectedDeckId.value) return
  const current = decks.value.find(d => d.id === parseInt(selectedDeckId.value))
  const newName = prompt("Nowa nazwa talii:", current?.name ?? "")
  if (!newName) return
  const res = await fetch(`${API_BASE}/${selectedDeckId.value}`, {
    method: "PUT",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ name: newName })
  })
  const updated = await res.json()
  const idx = decks.value.findIndex(d => d.id === updated.id)
  if (idx >= 0) decks.value[idx] = updated
  selectedDeck.value = updated
}

async function deleteDeck() {
  if (!selectedDeckId.value) return
  await fetch(`${API_BASE}/${selectedDeckId.value}`, { method: "DELETE" })
  decks.value = decks.value.filter(d => d.id !== parseInt(selectedDeckId.value))
  selectedDeckId.value = ""
  selectedDeck.value = null
}

async function importDeck(event) {
  const file = event.target.files[0]
  if (!file) return
  const formData = new FormData()
  formData.append("file", file)
  const res = await fetch(`${API_BASE}/import`, { method: "POST", body: formData })
  const deck = await res.json()
  decks.value.push(deck)
}

function downloadTemplate() {
  alert("Podmień na link do szablonu XLS/PDF.")
}

onMounted(async () => {
  await refreshDecks()
})
</script>

<style scoped>
.deck-panel { max-width: 720px; margin: 40px auto; padding: 24px; background: #f8f9fa; border-radius: 12px; box-shadow: 0 0 10px rgba(0,0,0,0.08); font-family: 'Segoe UI', system-ui, -apple-system, sans-serif; }
h2 { text-align: center; margin-bottom: 20px; color: #2b2f33; }
.deck-actions-top { display: flex; gap: 12px; align-items: center; margin-bottom: 16px; }
.file-input { display: inline-block; background: #ffffff; border: 1px dashed #b6c0cc; color: #2b2f33; padding: 8px 12px; border-radius: 8px; cursor: pointer; }
.file-input input[type="file"] { display: none; }
.btn-outline { background: transparent; border: 1px solid #b6c0cc; color: #2b2f33; padding: 8px 12px; border-radius: 8px; cursor: pointer; }
.btn-outline:hover { background: #e9f5ff; }
.deck-select { margin-bottom: 20px; display: flex; flex-direction: column; }
.deck-select label { margin-bottom: 6px; font-weight: 600; color: #2b2f33; }
.deck-select select { padding: 10px; border-radius: 8px; border: 1px solid #b6c0cc; font-size: 15px; background: #fff; }
.deck-actions { display: flex; gap: 12px; margin-bottom: 20px; }
button { padding: 10px 14px; border-radius: 8px; border: none; cursor: pointer; font-size: 15px; transition: 0.2s ease; }
.btn-warning { background: #ffc107; color: #212529; }
.btn-warning:hover { background: #e0a800; }
.btn-danger { background: #dc3545; color: white; }
.btn-danger:hover { background: #c82333; }
.deck-selected { margin-top: 24px; background: #fff3cd; padding: 16px; border-radius: 8px; border: 1px solid #ffeeba; }
.muted { color: #6c757d; font-size: 13px; }
</style>
