# Adaptive Construction

A small, original Unity 6 starter for a third-person online battle-royale prototype. All game names, code, and future assets are intended to be original.

## Requirements

- Unity **6000.0 LTS** (or a compatible Unity 6 release)
- Windows 10/11 for the initial desktop build
- Unity Hub and Git

## Open the project

1. Clone this repository.
2. In Unity Hub, choose **Add > Add project from disk** and select the repository root.
3. Let Package Manager resolve the packages in `Packages/manifest.json`.
4. Create a scene and add a `NetworkManager` with `UnityTransport`.
5. Create a player prefab with `NetworkObject`, `NetworkTransform`, `CharacterController`, `NetworkPlayerController`, and `NetworkBuildSystem`. Assign it as the Player Prefab in `NetworkManager` and register it in Network Prefabs.
6. Add a camera under the player prefab and assign it to the player controller and build system. Configure a ground layer and build-piece prefabs (each build prefab needs a `NetworkObject`).
7. Press Play and use the NetworkManager's Start Host/Client controls, or add your own UI calling `NetworkManager.Singleton.StartHost()` / `StartClient()`.

## Controls (prototype)

- WASD: move
- Mouse: rotate player/camera
- Space: jump
- Left mouse button: place the selected build piece (after assigning build prefabs)

## Included foundation

- Server-simulated third-person movement with owner input
- Server-validated, grid-snapped network building
- Networked health component
- Server-owned safe-zone radius that shrinks over time
- Basic server-side NavMesh bot target-following behavior

This is a **starter scaffold**, not a finished production game. It does not yet include matchmaking, dedicated-server deployment, polished UI, content, anti-cheat, a full inventory/loot loop, or production-grade prediction and reconciliation. Configure a NavMesh and assign references in the Inspector before using the bot. Build and test multiplayer in a development environment before release.

## Build for Windows

In Unity, open **File > Build Profiles**, add a Windows profile, add your gameplay scene, then build. The repository currently contains scripts and package configuration; create and save the scene/prefabs in the Unity Editor.

## License

MIT. See `LICENSE`.
