using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(fileName = "New Boss Weapon", menuName = "Weapons/Boss Weapon")]
    public class BossWeaponData : WeaponData
    {
        public override WeaponBehaviour AttachWeapon(GameObject owner, ActorComponent actor)
        {
            var behaviour = owner.AddComponent<BossWeaponBehaviour>();
            behaviour.Initialize(this, actor);
            return behaviour;
        }
    }
}