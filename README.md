# UnityGameObjectTracker
An in-scene method of viewing active gameobject/component/property/value combinations in real time. May be particularly useful for debugging in certain cases.

Import prefab and scripts into a Unity project, place Debug prefab as a child of a Canvas UI component (make sure an EventSystem gameobject is present), and rearrange menu position and size in scene as necessary.

- CTRL+D opens up the debug menu
- CTRL+Q opens up the input menu
- CTRL+Arrow keys moves any of the active menus around the screen 10 pixels at a time (100 pixels if also holding SHIFT)

Input an active gameobject's name in the gameobject input field, choose valid component/property combo to track, click add tracked object to use.

Can also add tracked objects via script with AddCustomDebug(string gameobjectname, string componentname, string propertyname) called from the DebugMenuBase component of the prefab. This is very case and space sensitive.

For example, GameObject.Find("Debug").GetComponent<DebugMenuBase>().AddCustomDebug("Main Camera", "Transform", "position")
