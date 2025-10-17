using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashOrb : MonoBehaviour
{
    public bool collected;
    [SerializeField] private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Collected()
    {
        collected = true;
        spriteRenderer.color = spriteRenderer.color + new Color(0, 0, 0, -255);
        yield return new WaitForSeconds(3.5f);
        collected = false;
        spriteRenderer.color = spriteRenderer.color + new Color(0, 0, 0, 255);
    }
}
