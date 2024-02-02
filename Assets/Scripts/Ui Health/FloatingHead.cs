using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingHead : MonoBehaviour
{
    public float rotationSpeed;
    private bool isRotating = true;

    void Update()
    {
        if (isRotating)
        {
            float rotationAmount = rotationSpeed * Time.deltaTime;
            transform.Rotate(Vector3.up, rotationAmount);
        }

    }

    public void RotatePlatform(bool _state)
    {
        isRotating = _state;
    }
}
