using UnityEngine;

namespace Weapons
{
    public class SimpleWeaponBehaviour : WeaponBehaviour
    {
        protected override void Shoot(Vector3 origin, Vector3 direction)
        {
            SpawnBullet(origin, direction * data.speed, Quaternion.identity);
        }
    }
}