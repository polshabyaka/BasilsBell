# Basil's Bell — Claude working instructions

## Project context

Basil's Bell is a mobile-first 2D top-down cozy herbalist fantasy game made in Unity.

The current project started as a procedural generation prototype, but it is now being turned into a small playable demo. Do not treat it as a throwaway school prototype. Treat it as a working base that should be improved carefully.

The game already has:
- runtime grid generation
- CellData / CellView structure
- GridManager as the central level script
- Perlin-based forest blocking
- map validation and regeneration
- distance-based loot placement
- fog of war
- A* click/tap-to-move
- smooth player movement
- collection loop
- completion screen and restart

Main goal:
Keep the working prototype stable while slowly improving it into a small, cozy, mobile-friendly game.

## Main work rule

Before changing code, always start with a short plan in 3-6 points.

Do not make large rewrites unless I explicitly ask for them.

Prefer the smallest safe change that moves the project forward.

If a change is risky, say so before writing code.

If something can be done manually in Unity instead of code, mention that.

## Architecture rule

Do not introduce advanced architecture unless there is a clear reason.

Avoid:
- unnecessary managers
- interfaces
- dependency injection
- event buses
- complex inheritance
- generic systems
- ScriptableObject architecture
- large refactors
- splitting one simple change into many new files

Use simple Unity C#.

It is okay for this project to have practical scripts with public Inspector fields when that makes the scene easier to edit.

Do not rewrite the whole project to make it look more professional.

## Preserve current structure

Keep these existing scripts and class names unless there is a strong reason to change them:

- Assets/Scripts/Grid/CellData.cs
- Assets/Scripts/Grid/CellView.cs
- Assets/Scripts/Grid/GridManager.cs
- Assets/Scripts/Grid/Pathfinder.cs
- Assets/Scripts/Player/Player.cs
- Assets/Scripts/Player/CameraFollow.cs
- Assets/Scripts/Generation/LootItem.cs
- Assets/Scripts/UI/LevelGoal.cs

Do not rename classes casually. Unity components can lose references if scripts are renamed badly.

Prefer extending the existing scripts in small steps.

## Mobile-first rule

All gameplay and UI decisions should work well on a phone.

Prioritize:
- touch input
- large tap targets
- readable silhouettes
- short play sessions
- simple interactions
- low visual clutter
- clear feedback
- portrait-friendly layouts when relevant

Avoid:
- tiny UI buttons
- complex drag-and-drop unless asked
- keyboard-only mechanics
- desktop-first controls
- overloaded screens

Keyboard controls can stay for testing, but they should not be the only way to play.

## Visual direction

The game should feel like cozy herbalist fantasy.

Preferred mood:
- soft green
- lavender
- cream
- warm brown
- gentle forest shadows
- readable top-down 2D
- stylized hand-drawn / painted feeling

The project is moving away from obvious placeholder grid visuals.

Important forest direction:
- base ground can be a large calm background
- gameplay grid stays for logic
- obstacles define blocked cells
- decorative props can be visual-only
- herbs/resources are collectible and placed on valid walkable cells

Do not force everything to look like individual square tiles if it can be avoided visually.

## Current forest transition plan

The safe direction is:

1. Keep the gameplay grid.
2. Keep pathfinding and validation.
3. Hide or soften individual cell visuals.
4. Add a large base ground background.
5. Spawn visual obstacle prefabs on blocked cells.
6. Add decor-only objects later.
7. Add larger obstacle footprints later only after the simple version works.

Do not jump straight to a new procedural world system.

## Code style

Write clean, readable Unity C#.

The code should be understandable for a student project, but it should not be intentionally bad or childish.

Prefer:
- clear method names
- simple control flow
- small methods
- Inspector-friendly fields
- direct references when appropriate
- practical safety checks

Avoid:
- clever tricks
- hidden magic
- unnecessary LINQ in gameplay loops
- abstract solutions for tiny problems
- fake beginner code
- joke comments
- excessive comments

Comments should be short and useful. English comments are preferred.

Comment the reason for a non-obvious decision, not every line.

Good comments:
- `// Keep the grid logic separate from the visual obstacle.`
- `// Small offset helps the forest feel less tile-based.`
- `// Do not allow pathfinding through unseen cells.`

Bad comments:
- `// set x`
- `// loop through list`
- `// idk why this works`
- long AI-style explanations inside code

## When editing code

When I ask for a code change, respond with:

1. Short plan
2. Files you want to touch
3. The code change
4. How to test it in Unity
5. What could break or what to watch for

If you need to edit a long file, prefer showing:
- the exact new fields
- the exact new methods
- the exact place where they should be called

Only provide the full file if that is clearer and safer.

Do not silently change unrelated logic.

## Testing checklist

After every code change, tell me how to test it.

For gameplay changes, check:
- scene enters Play Mode without errors
- map still generates
- player still spawns
- click/tap movement still works
- A* still avoids blocked cells
- fog of war still updates
- loot still appears on valid walkable cells
- collection still works
- completion screen still works
- restart still works

For visual changes, also check:
- objects are not blocking the camera
- sorting order looks correct
- visuals are readable at mobile size
- nothing important is too small to see
- the grid does not become visually noisy

## Asset workflow

Before referencing an asset by name, search the project structure or ask me to confirm the file.

If several assets could match, list the likely candidates and ask me to choose.

Do not assume an asset exists if it was not shown.

When adding prefab fields, make them easy to assign in the Inspector.

If an asset can be temporary, say so clearly.

## Unity Editor friendliness

Prefer changes that are easy to inspect in the Unity Editor.

Use `[Header]` for groups of public Inspector fields when helpful.

Use `[SerializeField]` for private fields that should be edited in Inspector.

Do not require complicated setup unless necessary.

When a new GameObject is needed, explain where to create it in the Hierarchy.

When a prefab field is added, explain what prefab to assign.

## Refactor rule

Do not refactor just because something could be cleaner.

Refactor only when:
- the current code blocks the requested feature
- the current code is likely to break
- the same logic is becoming duplicated in several places
- I explicitly ask for cleanup

When refactoring is needed, do it in small steps and explain why.

## Current priority

The current priority is not adding many new systems.

The current priority is:
1. keep the prototype working
2. improve the forest visuals
3. reduce the square grid feeling
4. keep movement/pathfinding stable
5. make the game feel better on mobile
6. slowly move toward the Basil's Bell demo loop

## Scope guard

Do not suggest these unless I ask:
- full economy system
- large NPC system
- complex dialogue system
- multiple biomes
- advanced inventory
- crafting quality/stats
- save/load system
- big shop upgrade system
- procedural art pipeline
- complete architecture rewrite

For now, focus on small playable demo improvements.