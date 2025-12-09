using UnityEngine;

namespace Weapons
{
    //Shoot a single projectile
    [CreateAssetMenu(menuName = "Weapons/BasicWeapon")]
    public class BasicWeapon : AWeaponType
    {
        public BulletComponent BulletPrefab;

        public float Speed = 30;
        public float RateOfFire = 1;

        public override IWeaponEffect GetWeaponEffect()
        {
            return new BasicProjectileWeaponEffect(BulletPrefab, Speed, RateOfFire);
        }
    }
}