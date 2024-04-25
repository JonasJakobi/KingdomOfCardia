using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Tile of the grid. 
/// Instantiated by the GridManager from the Tile prefab.
/// </summary>
public class Tile : MonoBehaviour
{
    [SerializeField] private Color baseColor, hoverColor;
    [SerializeField] private SpriteRenderer spriteRenderer;


    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = baseColor;

    }
    private void OnMouseEnter()
    {
        Debug.Log("Mouse entered");
        // Change the color of the tile when the mouse hovers over it
        spriteRenderer.color = hoverColor;
    }

    private void OnMouseExit()
    {
        // Change the color of the tile back to the base color when the mouse exits
        spriteRenderer.color = baseColor;
    }
}
