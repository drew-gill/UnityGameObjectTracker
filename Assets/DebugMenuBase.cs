using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Used to track hardware components rendered in-scene by tracking with ATC and Microcontroller
/// 
/// Also has a "property tracking" function which allows for user to track individual properties of active gameobjects in the scene.
/// 
/// To use, place "Debug" prefab into canvas of a scene. 
/// 
/// CTRL+D toggles component tracking
/// CTRL+Q toggles property tracking
/// 
/// CTRL+Arrow Keys moves menus around screen in case better location is needed, SHIFT modifier allows for faster movement
/// </summary>
public class DebugMenuBase : MonoBehaviour
{
    [SerializeField]
    private GameObject CustomDebugMenu;

    [SerializeField]
    private GameObject PropertyTrackMenu;

    [SerializeField]
    private Dropdown propertyDropdown;

    [SerializeField]
    private Dropdown componentDropdown;

    private float fps;

    private GameObject GameobjectToTrack;
    private Component ComponentToTrack;
    private string PropertyToTrack;

    private TrackedObject trackedObject;
    private List<TrackedObject> customTrackedObjects;

    //Called when script instance is loaded
    private void Awake()
    {
        customTrackedObjects = new List<TrackedObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //  Calculates framerate of previous frame
        fps = 1.0f / Time.deltaTime;

        //  CTRL+D toggles custom debug menu
        if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.D))
        {
            ToggleMenu(CustomDebugMenu);
        }
        //  CTRL+P toggles property tracker menu
        if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.Q))
        {
            ToggleMenu(PropertyTrackMenu);
        }
        if (PropertyTrackMenu.activeInHierarchy)
        {
            //If property tracking menu is active, ability to move it around screen is enabled
            MoveMenu(PropertyTrackMenu);
        }
        if (CustomDebugMenu.activeInHierarchy)
        {
            //If debug menu is active, ability to move it around screen is enabled
            MoveMenu(CustomDebugMenu);
        }

    }


    //  Used to add items from the property tracker to the custom debug menu
    //  Adds one tracked object at a time
    public void AddCustomDebug()
    {
        customTrackedObjects.Add(trackedObject);
    }


    //  Add any gameobject/component/property combo you would like to the trackedGameObjects List...
    //  Very case sensitive (and these actual case sensitivity of components and properties is hard to predict w/o prior knowledge)
    //  Remove spaces from component/property names even if they show up in inspector (unity adds these automatically for readability)
    //  Gameobject must be ACTIVE in scene
    //  For example, to track the GameObject IndexPlate's position in the scene, use AddCustomDebug("IndexPlate", "Transform", "position") 
    public void AddCustomDebug(string gameObject, string component, string property)
    {
        GameObject tempTrackedGameobject = GameObject.Find(gameObject);

        if (tempTrackedGameobject == null)
            return;

        Component tempTrackedComponent = tempTrackedGameobject.GetComponent(component);

        if (tempTrackedComponent == null)
            return;

        if (property != "" && property != null)
        {
            TrackedObject tempTrackedObject = new TrackedObject(tempTrackedGameobject, tempTrackedComponent, property);
            customTrackedObjects.Add(tempTrackedObject);
        }



    }


    //  Prints all TrackedObject class objects to the customdebugmenu text component
    private void PrintTrackedObjects()
    {
        string output = "";

        if (customTrackedObjects.Count > 0)
        {
            foreach (TrackedObject tracked in customTrackedObjects)
            {
                output = output + tracked.ToString() + "\n\n";
            }
        }

        CustomDebugMenu.GetComponent<Text>().text = output;
    }

    //  Called by "Clear" button on UI.
    //  Clears text of the CustomDebugMenu element
    public void ClearCustomDebug()
    {
        customTrackedObjects.Clear();
    }


    //Used in PropertyTracker()
    //Called when input field is changed, attempts to find object in the scene by NAME
    //Because of this, best if tracked object has unique name and must BE ACTIVE IN SCENE
    public void SetGameObject(string input)
    {
        GameobjectToTrack = GameObject.Find(input);

        if (GameobjectToTrack != null)
        {
            //If a gameobject is found, sets the component options of the dropdown menu based 
            // on selected gameobject
            SetDropdownOptions(GameobjectToTrack);
        }
    }

    //Used in PropertyTracker()
    //Called when component dropdown is changed, attempts to find component by name in the object
    public void SetComponent()
    {
        string componentName = componentDropdown.options[componentDropdown.value].text;

        if (componentName != null && componentName != "")
        {
            ComponentToTrack = GameobjectToTrack.GetComponent(componentName);

            //If component is available, the property dropdown options are set to the available properties of the component
            //Keep in mind that there are a lot more properties to be found than can be easily seen in the inspector (isActive, for example)
            SetDropdownOptions(ComponentToTrack.GetType());
        }
    }

    //Used in PropertyTracker()
    // Called when property dropdown is changed
    // Sets string of property to value selected in dropdown
    public void SetProperties()
    {
        PropertyToTrack = propertyDropdown.options[propertyDropdown.value].text;
    }


    //  Sets all PROPERTY options of a tracked COMPONENT
    private void SetDropdownOptions(Type trackedObjectType)
    {
        List<string> options = new List<string>();
        foreach (var property in trackedObjectType.GetProperties())
        {
            options.Add(property.Name); // Or whatever you want for a label
        }
        propertyDropdown.ClearOptions();
        propertyDropdown.AddOptions(options);

        //This line allows for OnValueChanged() to be called and for first property to be selected in dropdown
        propertyDropdown.value = -1;
    }

    //  Sets all COMPONENT options of a tracked GAMEOBJECT
    private void SetDropdownOptions(GameObject gameObject)
    {
        List<string> components = new List<string>();
        foreach (var component in gameObject.GetComponents<Component>())
        {
            components.Add(component.GetType().Name);
        }
        componentDropdown.ClearOptions();
        componentDropdown.AddOptions(components);

        //This line allows for OnValueChanged() to be called and for first component to be selected in dropdown
        componentDropdown.value = -1;
    }


    //Tracks a selected gameobject/component/property combo, or shows status of why property cannot be tracked
    private void PropertyTracker()
    {
        string output = "";


        if (GameobjectToTrack == null)
        {
            output = "GameObject is not set, inactive, or nonexistent in the scene";
            PropertyTrackMenu.GetComponent<Text>().text = output;
            ToggleAddButton(false);

            return;
        }

        if (ComponentToTrack == null)
        {
            output = "Component is not set or nonexistent on the GameObject";
            PropertyTrackMenu.GetComponent<Text>().text = output;
            ToggleAddButton(false);
            return;
        }




        if (PropertyToTrack != null && PropertyToTrack != "")
        {
            trackedObject = new TrackedObject(GameobjectToTrack, ComponentToTrack, PropertyToTrack);

            try
            {
                output = trackedObject.ToString();
                ToggleAddButton(true);
            }
            catch (Exception e)
            {
                output = output + "Property not valid for this component";
                ToggleAddButton(false);
            }
        }


        //Sets tracking text in UI
        PropertyTrackMenu.GetComponent<Text>().text = output;
    }


    private void ToggleAddButton(bool on)
    {
        PropertyTrackMenu.transform.Find("AddCustom").gameObject.SetActive(on);
    }

    //Toggles menu and property tracker (if menu is the property tracking menu)
    //Invokes repeating propertytracker function as a slightly less computationally taxing version of Update() function, cancels on disable
    private void ToggleMenu(GameObject menu)
    {
        bool previouslyActive = menu.activeInHierarchy;

        menu.SetActive(!previouslyActive);

        if (menu.Equals(PropertyTrackMenu))
        {
            if (!previouslyActive)
            {
                InvokeRepeating("PropertyTracker", 0.1f, 0.1f);
            }
            else
            {
                CancelInvoke();
            }
        }

        if (menu.Equals(CustomDebugMenu))
        {
            if (!previouslyActive)
            {
                InvokeRepeating("PrintTrackedObjects", 0.1f, 0.1f);
            }
            else
            {
                CancelInvoke();
            }
        }
    }

    //  CTRL+Arrows move the selected menu around the screen
    //  Shift modifier allows faster movement
    private void MoveMenu(GameObject menu)
    {
        Text text = menu.GetComponent<Text>();
        RectTransform rect = text.rectTransform;

        int shiftModify = 1;
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            shiftModify = 10;
        }

        if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.UpArrow))
        {

            rect.localPosition += (Vector3.up * 10 * shiftModify);
        }
        else if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.RightArrow))
        {
            rect.localPosition += (Vector3.right * 10 * shiftModify);
        }
        else if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.DownArrow))
        {
            rect.localPosition += (Vector3.down * 10 * shiftModify);
        }
        else if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            rect.localPosition += (Vector3.left * 10 * shiftModify);
        }
    }

}
