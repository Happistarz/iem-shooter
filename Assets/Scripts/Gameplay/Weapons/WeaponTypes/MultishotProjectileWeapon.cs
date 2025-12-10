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
            Spike
        }

        public BulletComponent BulletPrefab;
        
        public MultiShotType MultiShot = MultiShotType.Parallel;
        public float Speed = 30;
        public float RateOfFire = 1;
        public int MultiShotCount = 1;

        public override IWeaponEffect GetWeaponEffect()
        {
            return MultiShot switch
            {
                MultiShotType.Parallel => new ParallelProjectileWeaponEffect(
                    BulletPrefab, MultiShotCount, Speed, RateOfFire),
                MultiShotType.Arc   => new ArcProjectileWeaponEffect(BulletPrefab, MultiShotCount, Speed, RateOfFire),
                MultiShotType.Spike => new SpikeWeaponEffect(BulletPrefab, MultiShotCount, Speed, RateOfFire),
                _                   => throw new NotImplementedException()
            };
        }
    }
}