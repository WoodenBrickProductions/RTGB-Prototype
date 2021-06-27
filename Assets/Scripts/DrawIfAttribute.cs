﻿using UnityEngine;
using UnityEditor;

// [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)]
public class DrawIfAttribute : PropertyAttribute
{
    public string comparedPropertyName { get; private set; }
    public object comparedValue { get; private set; }
 
    /// <summary>
    /// Only draws the field only if a condition is met.
    /// </summary>
    /// <param name="comparedPropertyName">The name of the property that is being compared (case sensitive).</param>
    /// <param name="comparedValue">The value the property is being compared to.</param>
    /// <param name="comparisonType">The type of comperison the values will be compared by.</param>
    /// <param name="disablingType">The type of disabling that should happen if the condition is NOT met. Defaulted to DisablingType.DontDraw.</param>
    public DrawIfAttribute(string comparedPropertyName, object comparedValue)
    {
        this.comparedPropertyName = comparedPropertyName;
        this.comparedValue = comparedValue;
    }
}
