using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndorphinEmissary : TowerBase
{
    public ProjectileBehaviour projectileBehaviour;
    public bool useHitscan;

    protected override void FireProjectile()
    {
        soundManager.PlaySound(new AudioSettings(audioFields[0].clip, audioFields[0].volume, AudioType.TOWER), shootAnchors[0].position);

        if (useHitscan)
        {
            OnProjectileFired();
            OnProjectileHit(currentTarget.GetComponent<UnitBase>(), null);
        }
        else
        {
            base.FireProjectile();
            OnProjectileFired();
        }
    }
}
