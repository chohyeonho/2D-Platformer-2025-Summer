using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Data/Weapon Data", order = 0)]
public class WeaponData : ScriptableObject
{
    [Header("Info")]
    public string weaponName;

    [Header("Stats")]
    public float damage;
    public float range;
    public float attackDelay;
    public int numberOfAttacks;
    public float comboResetTime;
    public float knockbackForce;

    [Header("FX")]
    public AudioClip attackSound;
    public AudioClip hitSound;
    public GameObject hitEffect;
}