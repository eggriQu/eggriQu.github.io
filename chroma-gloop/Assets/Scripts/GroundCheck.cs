using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;

public class GroundCheck : MonoBehaviour
{
    public PlayerController player;
    public bool isGrounded;
    public bool isOnWater;
    private bool isWaterFrozen;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask waterLayer;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private CapsuleCollider2D capsuleCollider;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = (Physics2D.IsTouchingLayers(capsuleCollider, groundLayer) || (isOnWater && isWaterFrozen));
        isOnWater = Physics2D.IsTouchingLayers(capsuleCollider, waterLayer);
    }

    private void FixedUpdate()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 6)
        {
            player.canDash = true;
        }
        else if (collision.gameObject.layer == 4)
        {
            WaterTile waterTile = collision.gameObject.GetComponent<WaterTile>();
            if (waterTile.frozen || !waterTile.frozen && player.colorNum == 1)
            {
                player.canDash = true;
                isWaterFrozen = true;
            }
            else
            {
                isWaterFrozen = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.gameObject.layer == 6)
        //{
        //    isGrounded = false;
        //}
    }
}
