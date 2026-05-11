# Basil's Bell — Codex working instructions

## Project context

Basil's Bell is a mobile-first 2D top-down cozy herbalist fantasy game made in Unity.

This project started as a procedural generation prototype, but it is now becoming a small playable demo. Do not treat it as a throwaway school prototype. Treat it as a working base that should be improved carefully.

The current demo direction is:
1. Forest return point
2. Simple shop hub
3. Inventory for herbs
4. Simple cauldron / brewing
5. One order
6. One complete day
7. Soft time
8. Day progression
9. Book
10. Motivation scenes

The main goal is to build the playable loop first, using placeholders when needed.

## Current working systems

The project already has:
- runtime grid generation
- CellData / CellView grid structure
- GridManager as the central forest level script
- Perlin-based blocked forest generation
- map validation and regeneration
- distance-based loot placement
- fog of war
- A* click/tap-to-move
- smooth player movement
- collectible loop from the old prototype
- simple shop/forest switching through GameFlowController

Keep these working unless the task explicitly asks to replace them.

## Main rule

Before changing code, explain the plan in 3-6 short points.

Do not make code changes until the task is clear.

Prefer the smallest safe change that moves the project forward.

Do not rewrite the project.

Do not refactor just because something could be cleaner.

If a change is risky, say so before editing.

## Architecture rule

Keep the architecture simple and practical.

Avoid:
- unnecessary managers
- event buses
- dependency injection
- generic systems
- complex inheritance
- ScriptableObject architecture unless explicitly requested
- large rewrites
- scene loading systems unless explicitly requested
- save/load systems unless explicitly requested

Use simple Unity C# with Inspector-friendly fields.

It is okay for this project to have direct references in the Inspector.

## Preserve existing scripts

Do not casually rename these scripts or classes:

- Assets/Scripts/Grid/CellData.cs
- Assets/Scripts/Grid/CellView.cs
- Assets/Scripts/Grid/GridManager.cs
- Assets/Scripts/Grid/Pathfinder.cs
- Assets/Scripts/Player/Player.cs
- Assets/Scripts/Player/CameraFollow.cs
- Assets/Scripts/Generation/LootItem.cs
- Assets/Scripts/UI/LevelGoal.cs
- Assets/Scripts/UI/GameFlowController.cs

Unity components can lose references when scripts/classes are renamed.

Prefer extending existing scripts in small safe steps.

## Mobile-first rule

All gameplay and UI should work well on a phone.

Prioritize:
- touch input
- large buttons
- readable text
- short sessions
- clear feedback
- simple interactions
- low visual clutter
- landscape mobile layout for now

Avoid:
- keyboard-only gameplay
- tiny UI
- complex drag-and-drop
- desktop-first controls
- overloaded screens

Keyboard shortcuts are okay for testing only.

## Current implementation priority

Current priority is the playable loop, not final art.

Use placeholders freely.

Next likely steps:
1. HerbInventory
2. connect forest pickups to herbs
3. show herb inventory in shop
4. simple cauldron
5. one order
6. one complete day

Do not spend time on final visuals unless the task is specifically about asset testing.

## Forest / shop flow

The project currently uses one Unity scene.

Use:
- GameFlowController
- ShopRoot / ShopUI
- ForestRoot
- GameState for persistent demo data

Do not move to multiple scenes yet.

The current one-scene setup should stay simple, but it should not become a dead-end hack.

Keep data separate from UI. For example, herb counts should live in HerbInventory or GameState, not only in TMP text fields.

## Code style

Write clean, readable Unity C#.

Prefer:
- clear method names
- small methods
- simple control flow
- useful null checks
- Inspector-friendly serialized fields
- direct obvious logic

Avoid:
- clever tricks
- unnecessary LINQ in gameplay loops
- joke comments
- fake beginner code
- excessive comments
- long AI-style comments inside code

Comments should be short and useful, only where they explain intent.

## Editing rules

When editing code:
1. State the short plan.
2. State which file(s) will be touched.
3. Make only the requested change.
4. Do not silently change unrelated logic.
5. Include a short Unity test checklist.

If a file is long, prefer showing or changing only the needed sections.

## Testing checklist

After gameplay changes, check:
- scene enters Play Mode without console errors
- game starts in shop if GameFlowController is involved
- Go to Forest works
- Return to Shop works
- player movement still works
- A* still avoids blocked cells
- fog of war still updates
- loot/herb pickup still works if touched
- old prototype UI is not accidentally relied on

After UI changes, check:
- Canvas elements are under proper UI RectTransforms
- buttons are clickable
- text is readable in 16:9 landscape
- UI does not depend on Scene View placement
- Game View is the source of truth

## Unity setup style

When adding a new script, explain:
- which GameObject should receive it
- which Inspector fields need to be assigned
- what objects should stay enabled/disabled by default
- how to test it in Play Mode

Prefer manual Unity setup when that is safer than code.

## Scope guard

Do not suggest or implement these unless asked:
- full economy
- big NPC system
- complex dialogue system
- multiple biomes
- advanced inventory UI
- recipe quality/stats
- save/load
- shop upgrades
- full art pipeline
- complete rewrite

Focus on a small playable demo.

