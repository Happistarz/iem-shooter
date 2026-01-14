using UnityEngine;

namespace Weapons
{
    public class SpikeWeaponBehaviour : WeaponBehaviour
    {
        private SpikeWeaponData SpikeData => data as SpikeWeaponData;
        
        protected override void Shoot(Vector3 origin, Vector3 direction)
        {
            if (!SpikeData) return;

            for (var i = 0; i < SpikeData.spikeCount; i++)
            {
                var rotation = GetSpikeRotation(i);
                var velocity = rotation * Vector3.forward * SpikeData.speed;
                SpawnBullet(origin, velocity, rotation);
            }
        }

        private Quaternion GetSpikeRotation(int index)
        {
            var angleStep = 360f / SpikeData.spikeCount * index;
            var rotation  = Quaternion.AngleAxis(angleStep, Vector3.up);
            return rotation;
        }
    }
}