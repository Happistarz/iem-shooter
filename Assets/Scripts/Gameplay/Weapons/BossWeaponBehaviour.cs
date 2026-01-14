using UnityEngine;

namespace Weapons
{
    public class BossWeaponBehaviour : WeaponBehaviour
    {
        protected override void Shoot(Vector3 origin, Vector3 direction)
        {
            // dual shot
            var offset = Vector3.Cross(direction, Vector3.up) * 0.5f;
            SpawnBullet(origin + offset, direction * data.speed, Quaternion.LookRotation(direction));
            SpawnBullet(origin - offset, direction * data.speed, Quaternion.LookRotation(direction));
        }
    }
}