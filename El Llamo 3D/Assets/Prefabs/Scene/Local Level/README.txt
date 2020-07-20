To create a new Local Scene Level:

1. Create a new Scene
2. Remove the MainCamera and from Prefabs/Scene, drag and drop the "MainCamera" prefab
3. Drag and Drop the "Local Game Manager" to the scene
4. Choose a level theme sound in the "LevelThemeSound" prefab
5. Change the level settings in the "Local Scene Manager" prefab, such as Spawn Points
6. Create or drag and drop a "Level" prefab containing all the level data such as ground, trees, water, etc
7. Mark this new "Level" game object as a static -> Navigation static
8. Change the "Level" game object LayerMask to either "Level" or "Ground"
9. Under Window -> AI -> Navigation, open that window and adjust the settings. Click on "Bake" button
10. Under Window -> Rendering -> Lightning Settings, open the window
11. In there, change the Skybox and Directional Light in the Environment tab
12. Click the "Generate Lightning" button on the bottom of that window

Enjoy!