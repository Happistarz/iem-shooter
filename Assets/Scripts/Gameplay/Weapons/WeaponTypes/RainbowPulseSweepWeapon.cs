using UnityEngine;
using Weapons;

[CreateAssetMenu(menuName = "Weapons/RainbowPulseWeaponEffect")]
public class RainbowPulseSweepWeapon : AWeaponType
{
    public BulletComponent BulletPrefab;

    public Gradient Colors;
    public float GradientDuration = 3; 

    public float BulletSpeed = 30;
    public float RateOfFire = 1;
 
    public float RotationSpeed = 5;
    
    public override IWeaponEffect GetWeaponEffect()
    {
        return new RainbowPulseWeaponEffect(BulletPrefab, BulletSpeed, RateOfFire, RotationSpeed, Colors, GradientDuration);
    }
}