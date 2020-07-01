To use PosessObject:

1. You need to add a 3D model as a child of the Model object inside.
2. Insid Cameras -> FrontCamera assign the “Head” of given model to the “Follow” field of the Virtual Camera
3. Adjust the final 3rd person view on the root’s LocalMouseLook script
4. Adjust the final 1st person view on the root’s LocalMouseLook script (the same as when you move the Front Camera in the Editor)
5. Adjust the Box Collider on the root game object so it fits the model. This one is used to know what is “Posessable”

Enjoy!