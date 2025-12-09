using System;
using UnityEngine;

namespace Weapons
{
    //Shoot several projectiles in a configurable pattern
    [CreateAssetMenu(menuName = "Weapons/Multishot")]
    public class MultishotProjectileWeapon : AWeaponType
    {
        public enum MultiShotType
        {
            Parallel,
            Arc,
        }

        public BulletComponent BulletPrefab;
        
        public MultiShotType MultiShot = MultiShotType.Parallel;
        public float Speed = 30;
        public float RateOfFire = 1;
        public int MultiShotCount = 1;

        public override IWeaponEffect GetWeaponEffect()
        {
            switch (MultiShot)
            {
                case MultiShotType.Parallel:
                    return new ParallelProjectileWeaponEffect(BulletPrefab, MultiShotCount, Speed, RateOfFire);
                case MultiShotType.Arc:
                    return new ArcProjectileWeaponEffect(BulletPrefab, MultiShotCount, Speed, RateOfFire);
            }

            throw new NotImplementedException();
        }
    }
}