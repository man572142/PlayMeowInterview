# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**PlayMeowInterview** — A Unity 2021.3.45f2 (LTS) project targeting **Android**, containing a single login menu scene. UI is built with **UGUI** and authentication is handled via **GraphQL**.

## Unity Version & Platform

- **Unity:** 2021.3.45f2 LTS
- **Primary Target:** Android
- **Render Pipeline:** Universal Render Pipeline (URP) 12.1.15, configured for 2D

## Build & Test Commands

Unity operations are run via the Unity Editor or CLI. There is no standalone build script yet.

**Run tests via Unity Test Runner:**
- Open Unity → Window → General → Test Runner
- Or via CLI: `"<UnityPath>/Unity.exe" -runTests -projectPath . -testResults results.xml -testPlatform EditMode`

## MCP for Unity

The project includes `com.coplaydev.unity-mcp: 9.6.6`, which allows Claude Code to directly control the Unity Editor (create GameObjects, modify scenes, run tests, capture screenshots). Use the `unity-mcp-skill` when automating Unity Editor tasks.

## Architecture

### Scene Structure
Single scene: `Assets/Scenes/SampleScene.unity` (to be repurposed as the login scene). Contains only a Main Camera and Global Light 2D at this point.

### UI Layer (UGUI)
- All UI must use **UGUI** (Canvas-based). TextMeshPro (3.0.6) is installed for text components — prefer `TextMeshProUGUI` over legacy `Text`.
- UI prefabs go in `Assets/Prefabs/`.
- UI scripts go in `Assets/Scripts/UI/`.

### Login / Authentication (GraphQL)
- No GraphQL client library is installed yet — one must be added to `Packages/manifest.json`.
- Login API communication belongs in `Assets/Scripts/Network/` or `Assets/Scripts/Auth/`.
- Use a service/manager pattern (e.g., `AuthService.cs`) to separate API logic from UI logic.

### Asset Organization
```
Assets/
  Arts/
    Audio/      — audio clips
    Images/     — sprites and textures
  Prefabs/      — reusable UI and game prefabs
  Scenes/       — Unity scenes
  Scripts/
    UI/         — UGUI view/controller scripts
    Auth/       — login/authentication logic
    Network/    — GraphQL client, HTTP utilities
    Editor/     — editor-only tools and build scripts
  Settings/     — URP render pipeline assets
```

### Key Packages
| Package | Version | Purpose |
|---|---|---|
| `com.unity.feature.2d` | 2.0.1 | Full 2D development suite |
| `com.unity.render-pipelines.universal` | 12.1.15 | URP rendering |
| `com.unity.textmeshpro` | 3.0.6 | Text rendering |
| `com.unity.test-framework` | 1.1.33 | NUnit-based unit/integration tests |
| `com.coplaydev.unity-mcp` | 9.6.6 | MCP bridge for AI-assisted development |

## Design Reference

`Interview_man572142@gmail.com.fig` in the project root is a Figma design file containing UI specifications for the login screen. Use the Figma MCP tools to inspect this design when implementing the login UI.
