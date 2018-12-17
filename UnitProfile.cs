using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Forest/Unit Profile")]
public class UnitProfile : ScriptableObject {

    public UnitType unitType; //plant or animal?
    public int nutritionalValue;

    [Header("Multiplier")]
    public bool multiplier;
    public int multiplyFrequency;
    public int parentCountRequired;
    public int maxHungerAllowed;
    public int childCountProduced;

    [Header("Consumer")]
    public bool consumer;
    public int maxHungerBeforeDeath;
    public int hungerIncreasePerTick;
    public int eatThreshold;
    public UnitType eatPreference;
}

public enum UnitType
{
    Plant,
    Animal
}
