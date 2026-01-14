using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(fileName = "New Simple Weapon", menuName = "Weapons/Simple Weapon")]
    public class SimpleWeaponData : WeaponData
    {
        public override WeaponBehaviour AttachWeapon(GameObject owner, ActorComponent actor)
        {
            var behaviour = owner.AddComponent<SimpleWeaponBehaviour>();
            behaviour.Initialize(this, actor);
            return behaviour;
        }
    }
}