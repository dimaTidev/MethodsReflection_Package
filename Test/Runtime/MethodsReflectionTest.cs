using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MethodsReflectionTest : MonoBehaviour
{
 //   public Text text;

    public ActionEvent_Subscribe callbackEvent;
    public ActionEvent_Subscribe[] callbackEvents;

      
  /*  [SerializeField] ActionEventTest_int actionEventTest_int;

    [System.Serializable]
    public class ActionEventTest_int : ActionEventTest<int>
    {
    }*/


    void Start()
    {
        callbackEvent.Invoke();
    }

    public void Callback(int arg)
    {
        // GetComponent<Text>().text = "arg:" + arg;
        Debug.Log("Callback = " + arg);
     //   text.text = arg.ToString();
    }

    public void SimpleCallback()
    {

    }

    public void Subscribe(Action<int> callb)
    {
        Debug.Log("Subscribe");
        callb.Invoke(199);
    }
}
