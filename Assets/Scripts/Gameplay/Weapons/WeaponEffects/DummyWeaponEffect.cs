using UnityEngine;

namespace Weapons
{
    public class DummyWeaponEffect : IWeaponEffect
    {
        public ActorComponent Owner { get; set; }

        public void Update(Vector3 origin, Vector3 direction)
        {
        }
    }
}