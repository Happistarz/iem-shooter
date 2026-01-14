using UnityEngine;

namespace Weapons
{
    [CreateAssetMenu(fileName = "New Spike Weapon", menuName = "Weapons/Spike Weapon")]
    public class SpikeWeaponData : WeaponData
    {
        public int spikeCount = 5;
        
        public override WeaponBehaviour AttachWeapon(GameObject owner, ActorComponent actor)
        {
            var behaviour = owner.AddComponent<SpikeWeaponBehaviour>();
            behaviour.Initialize(this, actor);
            return behaviour;
        }
    }
}