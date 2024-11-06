using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoCollector : MonoBehaviour
{
    public WeaponController[] weapons; // Lista de todas las armas del jugador
    private Dictionary<string, WeaponController> weaponDict;

    void Start()
    {
        // Inicializar el diccionario con las armas y sus tags
        weaponDict = new Dictionary<string, WeaponController>();
        foreach (WeaponController weapon in weapons)
        {
            weaponDict.Add(weapon.tag, weapon);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ammo"))
        {
            AmmoPickup ammoPickup = other.GetComponent<AmmoPickup>();
            if (ammoPickup != null && weaponDict.ContainsKey(ammoPickup.weaponTag))
            {
                WeaponController weapon = weaponDict[ammoPickup.weaponTag];
                weapon.AddAmmo(ammoPickup.amount);
                Destroy(other.gameObject); // Destruir la caja de munición después de recogerla
            }
        }
    }
}
