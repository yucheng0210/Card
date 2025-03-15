using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectIDCard : MonoBehaviour
{
    public enum AttackType
    {
        Melee,
        LongDistance
    }
    [SerializeField]
    private AttackType myAttackType;
    [SerializeField]
    private int soundIndex;
    public AttackType MyAttackType
    {
        get { return myAttackType; }
        set { myAttackType = value; }
    }
    public int SoundIndex
    {
        get { return soundIndex; }
        set { soundIndex = value; }
    }

}
