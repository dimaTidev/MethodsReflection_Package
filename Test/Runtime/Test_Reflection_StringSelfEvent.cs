using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Reflection_StringSelfEvent : MonoBehaviour
{
    public string publicStringSecond;
    public string publicStringThird;


    public StringSelfField testEvent;

    private void Start()
    {
        testEvent.Invoke(serializString);
    }

    public string publicString;
   
    [SerializeField] string serializString = null;
   // private string privateString;
}
