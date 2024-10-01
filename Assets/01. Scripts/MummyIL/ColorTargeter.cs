using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTargeter : MonoBehaviour
{
    public enum TARGET_COLOR
    {
        BLACK,
        RED,
        GREEN,
        BLUE,
    }

    public TARGET_COLOR targetColor = TARGET_COLOR.BLACK;
    public Material[] targetColorMaterials;

    private Renderer hintRender;
    private int prevColorIndex = -1;


    private void Start()
    {
        hintRender = transform.Find("Hint").GetComponent<Renderer>();
    }

    public void TargetingColor()
    {
        int currentColorIndex;
        do
        {
            currentColorIndex = Random.Range(0, targetColorMaterials.Length);
        }
        while (prevColorIndex == currentColorIndex);

        prevColorIndex = currentColorIndex;

        targetColor = (TARGET_COLOR)currentColorIndex;
        hintRender.material = targetColorMaterials[currentColorIndex];

    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TargetingColor();
        }
    }


}
