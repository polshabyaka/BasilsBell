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
- distance-based placement logic
- fog of war
- A* click/tap-to-move
- smooth player movement
- simple shop/forest switching through GameFlowController
- HerbType / HerbInventory foundation
- herb pickup logic through HerbPickupController
- herb pickups with rarity and sprite support through LootItem
- temporary blocker behavior for active herb pickups

Keep these working unless the task explicitly asks to replace them.

The old coin / completion loop from the PCG retake may still exist in LevelGoal, but it is prototype-only. Do not rely on it for new gameplay.

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

Do not build professional-scale architecture for tiny demo features.

## Preserve existing scripts

Do not casually rename these scripts or classes:

- Assets/Scripts/Grid/CellData.cs
- Assets/Scripts/Grid/CellView.cs
- Assets/Scripts/Grid/GridManager.cs
- Assets/Scripts/Grid/Pathfinder.cs
- Assets/Scripts/Player/Player.cs
- Assets/Scripts/Player/CameraFollow.cs
- Assets/Scripts/Player/HerbPickupController.cs
- Assets/Scripts/Generation/LootItem.cs
- Assets/Scripts/Core/HerbType.cs
- Assets/Scripts/Core/HerbInventory.cs
- Assets/Scripts/UI/LevelGoal.cs
- Assets/Scripts/UI/GameFlowController.cs

Unity components can lose references when scripts/classes are renamed.

Prefer extending existing scripts in small safe steps.

`LootItem` is currently still named this way for Unity reference safety, even though it now represents herb pickups. Do not rename it unless explicitly requested.

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

Use placeholders freely when they help test gameplay.

Current / next likely steps:
1. Finish and stabilize herb pickup behavior
2. Add simple forest obstacle visuals
3. Show herb inventory in shop
4. Simple cauldron / brewing
5. One order
6. One complete day
7. Soft time

Do not spend time on final visuals unless the task is specifically about asset testing or visual integration.

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

## Forest map persistence

Returning from Forest to Shop should not regenerate the forest map.

The forest should stay the same when toggling Shop <-> Forest during the same session/day.

Regeneration is allowed only through explicit debug actions, such as the R key, or through a future intentional gameplay action like entering a new forest area.

Do not call Regenerate() from EnterForest(), EnterShop(), or ReturnFromForest().

## Forest structure direction

The forest should be structured as separate logical and visual layers:

1. Base ground — calm visual background, no gameplay logic.
2. Gameplay grid — walkability, blocked cells, pathfinding, fog, distance maps.
3. Obstacles — trees, bushes, rocks, roots, thickets; these visually represent blocked cells.
4. Decor — flowers, small grass, leaves, moss, stones; visual-only.
5. Herbs/resources — collectible gameplay objects.

Do not force the forest to look like individual square tiles.

The gameplay grid may remain cell-based while visuals become softer and less grid-like.

For the next obstacle step, prefer a simple visual layer:
- spawn obstacle prefabs on existing CellType.Forest cells
- do not change validation
- do not rewrite pathfinding
- do not add footprints yet unless explicitly requested

## Herb types and rarity

Herbs are core gameplay objects, not generic coins or temporary loot.

Current herb types:
- BellLeaf
- LavenderFern
- ButtonRoot
- HoneyClover
- WarmNettle
- SleepGrass
- Glowberry

Default rarity mapping:
- Common: BellLeaf, LavenderFern, WarmNettle, SleepGrass
- Uncommon: ButtonRoot, HoneyClover
- Rare: Glowberry

Herbs are placed on the forest grid by rarity and distance from the home / return point.

Do not replace real herb types with generic fallback pickup unless explicitly requested.

Fallback values are okay only for safety, not as normal gameplay behavior.

## Herb pickup rules

Herb cells should behave as temporary blockers until collected.

The player should not stand on top of a herb pickup.

Herbs can only be collected from the 4 cardinal neighboring cells:
- up
- down
- left
- right

Diagonal pickup is not allowed.

Pickup from the player's own cell is not allowed.

If the player taps a herb from far away or diagonally:
- the player should move to the best reachable cardinal-adjacent cell
- when the player arrives next to the herb, the herb can be auto-collected

E pickup should check only the 4 cardinal neighboring cells.

This rule is important for future pickup animations, because Basil should only need 4 pickup directions.

Do not change this back to 8-direction pickup unless explicitly requested.

## Herb visibility with fog of war

Herbs should be hidden on Unseen cells.

Herbs should remain visible on Explored cells after the player has discovered them.

Herbs should remain visible on Visible cells.

Do not make discovered herbs disappear just because the player moved away and the cell is no longer currently Visible.

Do not change CellView fog visuals unless explicitly requested.

## Herb placement rules

Herb placement should stay simple and Inspector-friendly.

Prefer count ranges by rarity instead of fixed counts:
- common herbs: range
- uncommon herbs: range
- rare herbs: range

Herbs should not spawn directly next to the home / return point unless explicitly requested.

Use existing distanceFromHome data when possible.

Do not rewrite the whole placement system if a small extension to GridManager is enough.

## Pathfinding and occupied cells

Pathfinder should still avoid:
- Forest cells
- Unseen cells
- active herb pickup cells

Active herb pickups are temporary blockers.

Do not turn herb cells into CellType.Forest. Herbs are not terrain.

Use GridManager helper methods such as HasActiveLootAt or equivalent when checking active pickup occupancy.

Normal click/tap movement to empty cells should keep working.

Click/tap on a herb cell should be handled as herb interaction, not normal movement onto that cell.

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

Do not silently format or rewrite whole files unless formatting is the explicit task.

## Testing checklist

After gameplay changes, check:
- scene enters Play Mode without console errors
- game starts in shop if GameFlowController is involved
- Go to Forest works
- Return to Shop works
- returning to shop and entering forest again does not regenerate the map
- player movement still works
- A* still avoids blocked cells
- A* still avoids active herb pickup cells
- fog of war still updates
- herb pickup still works if touched
- herb pickup does not rely on old LevelGoal
- old prototype UI is not accidentally relied on

After herb pickup changes, check:
- herbs appear on the map
- herbs have correct sprites if assigned
- common/uncommon/rare placement still works
- player cannot stand on a herb cell
- tapping a herb moves player next to it
- player auto-collects after reaching a cardinal-adjacent cell
- E pickup checks only up/down/left/right
- diagonal pickup does not happen
- discovered herbs stay visible on explored cells
- herbs are hidden only on unseen cells

After UI changes, check:
- Canvas elements are under proper UI RectTransforms
- buttons are clickable
- text is readable in 16:9 landscape
- UI does not depend on Scene View placement
- Game View is the source of truth

After visual forest changes, check:
- obstacle visuals do not change pathfinding unless explicitly intended
- obstacle visuals are parented under the correct generated map root
- regeneration cleans old generated visuals
- player, herbs, obstacles, and fog have readable sorting order
- visuals remain readable at mobile size

## Unity setup style

When adding a new script, explain:
- which GameObject should receive it
- which Inspector fields need to be assigned
- what objects should stay enabled/disabled by default
- how to test it in Play Mode

Prefer manual Unity setup when that is safer than code.

When adding or changing prefabs, explain:
- where the prefab should live
- what child objects it should have
- which component goes on the root
- which component goes on the visual child
- what SpriteRenderer / sorting settings should be checked
- which Inspector fields must be assigned

## Git / branch workflow

Use commits as checkpoints after stable steps.

Codex should not create commits, push branches, merge branches, or run destructive git commands unless explicitly asked.

Codex may suggest a commit message after a stable change.

The user will usually review, test, commit, push, and merge manually through GitHub Desktop.

Before risky changes, remind the user to commit the current working state.

Use feature branches for larger separate chunks, for example:
- feature/herb-types
- feature/forest-obstacles
- feature/cauldron

Do not mix unrelated systems in the same feature branch if it can be avoided.

Good commit examples:
- Add herb pickup prefab
- Add herb types to forest pickups
- Polish herb pickup rules
- Add forest obstacle visuals

Before merging a branch, check:
- Play Mode works
- no console errors
- main loop still works
- the feature has been pushed

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

Focus on a playable demo.