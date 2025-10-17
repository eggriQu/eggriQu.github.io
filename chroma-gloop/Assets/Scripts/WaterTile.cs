using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTile : MonoBehaviour
{
    public BoxCollider2D boxCollider;
    public bool frozen;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Sprite iceTile;
    [SerializeField] private Sprite waterTile;

    // Start is called before the first frame update
    void Awake()
    {
        if (frozen)
        {
            spriteRenderer.sprite = iceTile;
            boxCollider.isTrigger = false;
        }
        else
        {
            spriteRenderer.sprite = waterTile;
            boxCollider.isTrigger = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Freeze()
    {
        spriteRenderer.sprite = iceTile;
        boxCollider.isTrigger = false;
        frozen = true;
    }

    public IEnumerator Melting()
    {
        yield return new WaitForSeconds(2.2f);
        spriteRenderer.sprite = waterTile;
        boxCollider.isTrigger = true;
        frozen = false;
    }


}
