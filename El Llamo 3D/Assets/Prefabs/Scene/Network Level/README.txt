To create a new Network Scene Level:

1. Create a new Scene
2. Remove the MainCamera and from Prefabs/Scene, drag and drop the "MainCamera" prefab
3. Drag and Drop the "Game Manager" to the scene
4. Choose a level theme sound in the "LevelThemeSound" prefab
5. Change the "Spawner Id" value on the "SceneSpawner" prefab (it has to be different from other scenes)
6. Add the players Spawn Points under "Spawn Points" in the "SceneSpawner" prefab
7. Change the level settings in the "Scene Manager" prefab, such as Spawn Points
8. Create or drag and drop a "Level" prefab containing all the level data such as ground, trees, water, etc
9. Mark this new "Level" game object as a static -> Navigation static
10. Change the "Level" game object LayerMask to either "Level" or "Ground"
11. Under Window -> AI -> Navigation, open that window and adjust the settings. Click on "Bake" button
12. Under Window -> Rendering -> Lightning Settings, open the window
13. In there, change the Skybox and Directional Light in the Environment tab
14. Click the "Generate Lightning" button on the bottom of that window

Enjoy!