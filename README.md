# 🎮 GameLobby – Distributed Multiplayer Lobby System (C# + Redis)

A lightweight **distributed lobby service** built in **.NET 8** with **Redis** as the in-memory data store.  
This service is designed to handle **high-concurrency multiplayer lobbies**, using Redis primitives for **fast coordination**, **state tracking**, and **atomic locking**.

---

## 🚀 Overview

GameLobby allows multiple application instances (pods/containers) to manage player lobbies together — all synchronized through a shared Redis instance.

Each **lobby** is a logical room that can be created, joined, or locked.  
Redis stores all lobby data (metadata, members, and indexes) and ensures that only one process modifies a lobby at a time using distributed locks.

---

## 🧩 Core Features

| Feature | Description |
|----------|-------------|
| 🎯 **Lobby Creation** | Creates unique lobby IDs using `INCR`, stores metadata in a Redis `HASH`, and registers it in a `SET` index by status. |
| 👥 **Member Management** | Each lobby maintains a Redis `SET` of player IDs (`lobby:{id}:members`) for atomic joins/leaves. |
| 🔒 **Distributed Locking** | Uses Redis-based tokenized locks (`SET NX PX`) and a Lua unlock script to ensure safe unlocks. |
| 🧮 **Efficient Indexing** | Lobbies are grouped by status (e.g. open/locked/full) using indexed Sets (`lobby:by-status:{status}`). |
| ⚡ **High Performance** | In-memory operations only (no SQL), ideal for real-time or high-throughput multiplayer systems. |


---



---
## 🏗️ Architecture

```text
    ┌───────────────────────┐
    │      Game Clients     │
    │   (WebSocket/HTTP)    │
    └──────────┬────────────┘
               │
               ▼
  ┌──────────────────────────────┐
  │     GameLobby API (.NET 8)   │
  │ • Create/Join                │
  │ • Manage Members/Status      │
  │ • Uses Redis for Sync/Lock   │
  └──────────────┬───────────────┘
                 │
                 ▼
  ┌──────────────────────────────┐
  │          Redis 7             │
  │ • INCR (IDs)                 │
  │ • HASH (Lobby Meta)          │
  │ • SET (Members/Indexes)      │
  │ • Lua (Safe Unlock)          │
  └──────────────────────────────┘
---

## 🐳 How to Run the App (Docker Compose)

You can easily run **GameLobby** locally using **Docker Compose**.  
No manual setup is required — everything is containerized and auto-configured.

---

### 🕹️ Step 1 – Open CMD or Terminal

Go to the root folder of the project (where the `docker-compose.yml` file is located):



