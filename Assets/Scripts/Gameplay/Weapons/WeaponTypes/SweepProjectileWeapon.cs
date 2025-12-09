using UnityEngine;

namespace Weapons
{
    //Shoot projectiles in a sweeping motion around the player
    [CreateAssetMenu(menuName = "Weapons/Sweep")]
    public class SweepProjectileWeapon : AWeaponType
    {    
        public BulletComponent BulletPrefab;

        public float BulletSpeed = 30;
        public float RateOfFire = 1;
 
        public float RotationSpeed = 5;
    
        public override IWeaponEffect GetWeaponEffect()
        {
            return new SweepWeaponEffect(BulletPrefab, BulletSpeed, RateOfFire, RotationSpeed);
        }
    }
}