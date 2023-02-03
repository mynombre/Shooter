using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Objects/Inventory")]

public class Inventory : ScriptableObject
{
    public List<Weapon> weapons;
}
