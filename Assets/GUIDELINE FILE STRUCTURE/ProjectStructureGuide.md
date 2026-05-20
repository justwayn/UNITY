# Project Structure Guide

This document provides a comprehensive overview of the project structure for the **GamersProg** repository. Use this as a reference for locating scripts, assets, and scene files.

---

## Core Architecture

The logic is primarily organized within the `Assets/Scripts` directory, following a component-based approach.

### Key Management Systems
- **GameManager.cs**: Oversees global game state and persistent data.
- **GameFlowManager.cs**: Manages transitions between gameplay states (Start, Play, Pause, GameOver).
- **LevelManager.cs**: Handles level loading, checkpoints, and scene transitions.

---

##  Player Systems
All player-related logic can be found in `Assets/Scripts`:

- **PlayerController.cs**: Core movement, physics, and input handling.
- **PlayerCombat.cs** & **PlayerAttack.cs**: Melee and ranged combat logic.
- **PlayerHealth.cs**: Manages HP, invulnerability frames, and death events.
- **PlayerAnimation.cs**: Bridges the character state with the Animator Controller.
- **MutationSystem.cs**: Handles special abilities or character variations.

---

## Enemy AI & Combat
Enemies are specialized by type and can be found in `Assets/Scripts` and `Assets/Prefabs/Enemies`:

- **Bosses**: 
    - `TentacleBossAI.cs`: Complex attack patterns for the tentacle boss.
- **Standard Enemies**:
    - `SoldierAI.cs`, `BearEnemy.cs`, `OwlAI.cs`, `CrystalEnemyAI.cs`.
- **General Logic**:
    - `Enemy.cs` / `EnemyHealth.cs`: Base classes for common enemy behaviors.
    - `EnemyPatrol.cs`: Logic for patrolling between waypoints.

---

## World & Environment
- **ParallaxSystem.cs**: Handles multi-layered background scrolling.
- **ForestAtmosphere.cs**: Manages environmental effects (fog, lighting, etc.).
- **Camera Management**:
    - `CameraTargetFollow.cs`: Standard follow logic.
    - `CameraShake.cs`: Feedback for impacts and explosions.
    - `BossCameraController.cs`: Specialized framing for boss encounters.

---

## Asset Organization

### Assets/Sprites
- **UI**: Button frames, backgrounds, and icons.
- **Background**: Divided by theme (`Forest`, `Laboratory`, `City Ruins`).
- **Sprites (animation)**: Character-specific animation frames (Attack, Run, Idle, etc.).
- **Sprites (Enemy)**: Visual assets for all enemy types.

### Assets/Animations & Animators
- Contains `.anim` files and `.controller` assets.
- `PlayerMovements.controller`: The main state machine for the player.
- Specialized controllers for each enemy type (e.g., `SoldierController`, `BearEnemy`).

### Assets/Prefabs
- **Enemies**: Ready-to-use enemy variants.
- **Environment**: Pre-configured tilesets and background elements.

---

## Scenes
Found in `Assets/` and `Assets/Scenes/`:

1. **MainMenu.unity**: The entry point with the wooden-themed UI.
2. **Cutscene.unity**: Dedicated scene for narrative segments.
3. **Laboratory.unity**: Key level/environment.
4. **Scenes/City.unity**: Urban-themed environment.
5. **Scenes/SampleScene.unity**: Testing and prototyping area.

---

## Tools & Settings
- **InputSystem_Actions**: The Input Action Asset for modern input handling.
- **TextMesh Pro**: Used for all UI text for high-fidelity rendering.
- **Settings/UniversalRP**: Universal Render Pipeline configuration files.

---

## Developer Notes
- **Namespaces**: Most scripts are in the default namespace; ensure new scripts follow the existing component pattern.
- **UI Interaction**: The Main Menu uses a `Canvas` with `Image` components. The background is centered and stretched to fill the screen.
- **Physics**: The player uses a 2D physics-based controller. Adjust values in the Inspector on the Player prefab.
