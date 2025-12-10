using System;
using UnityEngine;

namespace Weapons
{
    public interface IWeaponEffect
    {
        ActorComponent Owner { get; set; }
        event Action OnShoot;
        public void Update(Vector3 origin, Vector3 direction);
    }
}