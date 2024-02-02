using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverTimeDamage : MonoBehaviour
{
 
    public int timeBeforeNextDmage;


   

    private void Start()
    {
       InvokeRepeating("takeDamageOverTime",1, timeBeforeNextDmage);
    }

    void takeDamageOverTime()
    {     
        GetComponent<UnitBase>().TakeDamage(1);
    }
}
