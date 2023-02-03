using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FireType
{
    Auto = 0,
    Semi
}


[CreateAssetMenu(menuName = "Objects/Weapon")]

public class Weapon : ScriptableObject
{
    public new string name;
    public float damage;
    public int magazine;
    public int currentAmmo;
    public float timeToShoot;
    public float range;
    public float timeToReload;
    public float accuracy;
    public FireType fireType;
}
