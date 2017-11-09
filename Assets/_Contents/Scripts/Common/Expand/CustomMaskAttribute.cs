using UnityEngine;

public class CustomMaskAttribute : PropertyAttribute
{
    public string enumName;

    public CustomMaskAttribute() { }

    public CustomMaskAttribute(string name)
    {
        enumName = name;
    }
}