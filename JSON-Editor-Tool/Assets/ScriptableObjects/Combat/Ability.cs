using Fusion;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public Sprite abilityIcon;
    public LayerMask TargetLayer => targetLayer;
    public float Cooldown => cooldown;
    public bool IsHoldAction => isHoldAction;
    public bool IsRecurring => isRecurring;
    public float RecurTime => recurTime;
    public int RecurTickDelay => recurTickDelay;
    public AnimationClip AbilityAnimation => abilityAnimation;
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] private float cooldown;
    [SerializeField] private bool isHoldAction;
    [SerializeField] private bool isRecurring;
    [SerializeField] private float recurTime;
    [SerializeField] private int recurTickDelay;
    [SerializeField] private AnimationClip abilityAnimation;
    public TickTimer recurTimer;
    
    public abstract void Execute(NetworkRunner _runner, Transform _user, LayerMask targetLayer, Vector3 _userPosition, float _multiplier);

    [SerializeField] public AK.Wwise.Event abilitySFX;
}