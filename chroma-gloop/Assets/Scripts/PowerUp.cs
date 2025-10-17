using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    [SerializeField] public int colourNum;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (colourNum == 0)
        {
            spriteRenderer.color = Color.yellow;
        }
        else if (colourNum == 1)
        {
            spriteRenderer.color = Color.blue;
        }
        else if(colourNum == 2)
        {
            spriteRenderer.color = Color.red;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
