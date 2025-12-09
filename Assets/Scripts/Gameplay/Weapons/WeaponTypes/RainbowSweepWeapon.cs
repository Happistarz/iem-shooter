using System;
using System.Collections.Generic;
using UnityEngine;
using Weapons;

[CreateAssetMenu(menuName = "Weapons/RainbowSweep")]
public class RainbowSweepWeapon : AWeaponType
{
    public BulletComponent BulletPrefab;

    public Gradient Colors;

    public float BulletSpeed = 30;
    public float RateOfFire = 1;
 
    public float RotationSpeed = 5;
    
    public override IWeaponEffect GetWeaponEffect()
    {
        return new RainbowWeaponEffect(BulletPrefab, BulletSpeed, RateOfFire, RotationSpeed, Colors);
    }
}