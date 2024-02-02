using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StateHandler : MonoBehaviour
{
  
    public Sprite[] spriteStates; 

 
    public Sprite currentstate;
    public static StateHandler instance { get;  set; }

    private void Awake()
    {
        instance = this;
       
    }



    private void Start()
    {
        gameObject.GetComponent<Image>().sprite = spriteStates[2];
      
    }
}
