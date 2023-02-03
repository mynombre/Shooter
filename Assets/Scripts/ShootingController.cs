using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    public Inventory inventoryReference;
    List<Weapon> inventory;
    Weapon currentWeapon;
    float time = 0;
    Coroutine reloading;

    void Awake()
    {
        foreach(Weapon weapon in inventoryReference.weapons)
        {
            inventory.Add(weapon);
        }
        if(inventory.Count == 0)
        {
            Debug.LogError("The reference inventory is empty!");
            return;
        }
        currentWeapon = inventory[0];
    }

    // Update is called once per frame
    void Update()
    {
        if(currentWeapon.fireType == FireType.Auto)
        {
            if(Input.GetMouseButton(0))
            {
                Shoot();
            }
        }
        else if (currentWeapon.fireType == FireType.Semi)
        {
            if (Input.GetMouseButton(0))
            {
                Shoot();
            }
        }
        time += Time.deltaTime;
        if(Input.GetKeyDown(KeyCode.R))
        {
            reloading = StartCoroutine(Reload());
        }
    }
    IEnumerator Reload()
    {
        yield return new WaitForSeconds(currentWeapon.timeToReload);
        currentWeapon.currentAmmo = currentWeapon.magazine;
        reloading = null;
    }
    void Shoot()
    {
        if(currentWeapon.timeToShoot < time && currentWeapon.currentAmmo > 0)
        {
            time = 0;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Physics.Raycast(ray.origin, ray.direction, out hit, currentWeapon.range);
            if(hit.collider)
            {
                Debug.Log(hit.collider.gameObject.name);
            }
        }
    }
}
