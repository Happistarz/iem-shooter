using UnityEngine;

namespace Weapons
{
    public interface IWeaponEffect
    {
        ActorComponent Owner { get; set; }
        public void Update(Vector3 origin, Vector3 direction);
    }
}