# Virtual Reality Project (Unity)
Course project for TU Dresden using Unity. This README tracks setup, workflows, and a running log of completed task blocks with linked media.

## Table of Contents
- [Overview](#overview)
- [Requirements](#requirements)
- [Project Setup](#project-setup)
- [Project Structure](#project-structure)
- [Development Workflow](#development-workflow)
- [Task Log](#task-log-section-per-completed-task)
  - [Task 3 - Stereo Rendering & Projection](#task-3---stereo-rendering--projection)
    - [Tasks 3.1 & 3.2](#tasks-31--32)
    - [Task 3.3: Anaglyph 3D Rendering](#task-33-anaglyph-3d-rendering)
    - [Task 3.4: Toe-In Projection](#task-34-toe-in-projection)
    - [Task 3.5: Off-Axis Projection](#task-35-off-axis-projection)
- [Troubleshooting](#troubleshooting)

## Overview
- Platform: Unity (C#)
- Goal: Build and document VR project deliverables in iterative task blocks
- Media: Screenshots/videos per task stored in `docs/task-media/`

## Requirements
- Unity Editor (matching the version defined in `ProjectSettings/ProjectVersion.txt`)
- Git
- Optional: Visual Studio / Rider, Git LFS (if storing large media)

## Project Setup
1) Clone the repository.
2) Open the project in Unity via the `Course Project.sln` or directly in the Unity Hub.
3) Let Unity generate local `Library/` and other cached folders (ignored via `.gitignore`).

## Project Structure
- `Assets/` - Game assets, scripts, scenes.
- `Packages/` - Unity package manifest and dependencies.
- `ProjectSettings/` - Unity project configuration.
- `UserSettings/` - Local user settings (ignored).
- `docs/task-media/` - Place screenshots and videos for task logs (`.gitkeep` keeps the folder in Git).

## Development Workflow
- Use feature branches per task block.
- Commit only source assets and settings; avoid committing generated caches/builds.
- For large binaries (videos), prefer compressed formats or Git LFS.
- Run play mode tests or manual checks before committing scenes or scripts.

## Task Log (Section per completed Task)

## Task 3 - Stereo Rendering & Projection
### Tasks 3.1 & 3.2
- Theory and anaglyph 3D test.

### Task 3.3: Anaglyph 3D Rendering
- Summary:
  - Added basic 3D scene content (white cubes) for depth perception.
  - Implemented stereo rendering using two eye cameras (left/right) as children of the Main Camera.
  - Used Inter-Pupillary Distance (IPD) to offset eye cameras correctly.
  - Rendered each eye's view to a dedicated Render Texture.
  - Integrated a custom render pipeline and fullscreen shader to combine color channels into an anaglyph image.

---

### Task 3.4: Toe-In Projection
- Summary:
  - Extended the stereo camera controller to support toe-in projection.
  - Added a configurable convergence point that both eye cameras can rotate towards.
  - Implemented toggling between parallel view and toe-in mode.
  - Observed and documented keystone distortion caused by toe-in projection.
  - As shown in the images: red on the right when toe-in is false; red on the left when toe-in is true.
- Media:
  - Screenshot 1: `docs/task-media/task3.4-toe-in-false.jpg`
  - Screenshot 2: `docs/task-media/task3.4-toe-in-true.jpg`

---

### Task 3.5: Off-Axis Projection
- Summary:
  - Implemented off-axis stereo projection to eliminate toe-in distortion.
  - Added a ProjectionPlane object defining the physical screen geometry.
  - Calculated asymmetric frustum parameters per eye using plane corners and eye position.
  - Replaced the default camera projection matrix using `Matrix4x4.Frustum`.
  - Achieved correct stereo depth with parallel cameras and no keystone distortion.
- Media:
  - Video 1: `docs/task-media/task3.5-part-1.mp4`
  - Video 2: `docs/task-media/task3.5-part-2.mp4`

## Troubleshooting
- If Unity prompts to upgrade the project version, confirm it matches teammates' versions before proceeding.
- Delete `Library/` locally if you encounter persistent editor/cache corruption (Unity will regenerate).
