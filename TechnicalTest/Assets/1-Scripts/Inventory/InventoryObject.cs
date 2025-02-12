using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Scriptable inventory Object")]
public class InventoryObject : ScriptableObject 
{
    [Header("In-Game")]
    public TileBase tile;
    public Sprite image;
    public ItemType type;
    //public ActionType action;
    public Vector2Int range = new Vector2Int(5, 4);

    [Header("UI")]
    public bool stackable = true;

}

public enum ItemType
{
    Prop,
    Weapon
}

