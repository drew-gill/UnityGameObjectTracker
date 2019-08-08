using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;


/// <summary>
/// A class used for the DebugMenuFunction in order to store references to gameobjects and their properties
/// ToString method formats info in readable way
/// </summary>
public class TrackedObject
{
    public TrackedObject(GameObject g, Component c, string p)
    {
        trackedGameObject = g;
        component = c;
        property = p;
    }


    private GameObject trackedGameObject;
    private Component component;
    private string property;

    public GameObject GameObject
    {
        get
        {
            return trackedGameObject;
        }
        set
        {
            trackedGameObject = value;
        }
    }
    public Component Component
    {
        get
        {
            return component;
        }
        set
        {
            component = value;
        }
    }
    public string Property
    {
        get
        {
            return property;
        }
        set
        {
            property = value;
        }
    }


    public override string ToString()
    {
        string output = "";

        Type componentType = component.GetType();

        var propertyValue = componentType.GetProperty(property);

        output = output + ("<b>GameObject: </b>" + trackedGameObject.name);
        output = output + ("\n\t<b>Component: </b>" + componentType.Name);
        output = output + ("\n\t<b>Var Name: </b>" + propertyValue.Name);
        output = output + ("\n\t<b>Type: </b>" + propertyValue.PropertyType);
        output = output + ("\n\t<b>Value: </b>" + propertyValue.GetValue(component, null));


        



        return output;
    }
}
