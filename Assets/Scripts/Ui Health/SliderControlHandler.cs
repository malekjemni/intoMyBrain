using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderControlHandler : MonoBehaviour
{
    public Slider slider;
    public static SliderControlHandler instance { get; private set; }

    private void Awake()
    {
        instance = this;
        slider = GetComponent<Slider>();
        slider.value = 0.5f;
      
    }

   


 


}
