using Fusion;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("Identification")]
    public string characterID;
    public string characterName;
    public Stats stats;

    [Header("Char Select Prefab Reference")]
    public NetworkPrefabRef characterSelectPrefab;

    [Header("Gameplay Prefab Reference")]
    public NetworkPrefabRef gameCharacterPrefab;

    [Header("MoveList")]
    public Ability[] PrimaryAbilities;
    public Ability[] SecondaryAbilities;
    public Ability[] MovementAbilities;
    public Ability[] SpecialAbilities;

    [System.Serializable]
    public class Stats
    {
        [Header("Core Stats")]
        public int hp;
        public int defense;
        public int speed;
        public int gold;
        public int xp;
    }

}
