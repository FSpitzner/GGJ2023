using UnityEngine;
using System;
using System.Collections;

//Original version of the ConditionalEnumHideAttribute created by Brecht Lecluyse (www.brechtos.com)
//Modified by: -

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalEnumHideAttribute : PropertyAttribute
{
    //The name of the bool field that will be in control
    public string ConditionalSourceField = "";

    public int EnumValue1 = 0;
    public int EnumValue2 = 0;

    public bool HideInInspector = false;
    public bool Inverse = false;
    public bool InBetween = false;

    public ConditionalEnumHideAttribute(string conditionalSourceField, int enumValue1, bool hideInInspector = false, bool inverse = false)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.EnumValue1 = enumValue1;
        this.EnumValue2 = enumValue1;
        this.HideInInspector = hideInInspector;
        this.Inverse = inverse;
    }

    public ConditionalEnumHideAttribute(string conditionalSourceField, int enumValue1, int enumValue2, bool hideInInspector = false, bool inverse = false, bool inBetween = false)
    {
        this.ConditionalSourceField = conditionalSourceField;
        this.EnumValue1 = enumValue1;
        this.EnumValue2 = enumValue2;
        this.HideInInspector = hideInInspector;
        this.Inverse = inverse;
        this.InBetween = inBetween;
    }
}