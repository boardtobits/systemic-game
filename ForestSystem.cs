using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestSystem : MonoBehaviour {

    [Header("System Settings")]
    public float tickDurationInSeconds = 1.0f;
    public UnitProfile[] unitProfiles;
    [HideInInspector]
    public Transform[] unitHolders;
    public int ticksPerJump = 5;

    [Header("Session Settings")]
    public int[] startingUnits;

    [Header("Session Data")]
    public int numberOfTicksInSession = 0;
    public int[] unitCount;
    [HideInInspector]
    public int[] unitsCreated;
    public int unitTypeCount;

    // Use this for initialization
    void Start() {
        unitTypeCount = unitProfiles.Length;
        if (startingUnits.Length < unitTypeCount)
        {
            Debug.LogError("Not enough starting unit data.");
            return;
        }
        else if (startingUnits.Length > unitTypeCount)
        {
            Debug.LogWarning("More starting unit data than expected.");
        }
        SetUpForest();
        //StartCoroutine(RunGameLoop());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            for (int i = 0; i < ticksPerJump; i++)
            {
                IncrementTick();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            IncrementTick();
        }
    }  

    void SetUpForest() {
        unitHolders = new Transform[unitTypeCount];
        unitCount = new int[unitTypeCount];
        unitsCreated = new int[unitTypeCount];

        for (int i = 0; i < unitTypeCount; i++)
        {
            unitHolders[i] = new GameObject(unitProfiles[i].name).transform;
            unitHolders[i].SetParent(transform);
            for (int j = 0; j < startingUnits[i]; j++)
            {
                CreateUnit(unitProfiles[i], i);
            }
        }
    }

    void CreateUnit(UnitProfile profile, int i)
    {
        Unit newUnit = new GameObject().AddComponent<Unit>();
        newUnit.Initialize(profile, this, i);
    }

    IEnumerator RunGameLoop() {
        while (true)
        {
            IncrementTick();
            yield return new WaitForSeconds(tickDurationInSeconds);
        }
    }

    void IncrementTick()
    {

        for (int i = 0; i < unitTypeCount; i++)
        {
            CheckForMultiply(unitProfiles[i], unitHolders[i], i);
            if (unitProfiles[i].consumer)
            {
                foreach (Transform u in unitHolders[i])
                {
                    u.GetComponent<Unit>().OnTick();
                }
            }
        }
        numberOfTicksInSession++;
    }

    void CheckForMultiply(UnitProfile profile, Transform holder, int iter)
    {
        if ((numberOfTicksInSession + 1) % profile.multiplyFrequency != 0)
            return;

        int healthyUnits = 0;
        if (profile.consumer)
        {
            foreach (Transform u in holder)
            {
                if (u.GetComponent<Unit>().IsHealthy)
                    healthyUnits++;
            }
        }
        else
        {
            healthyUnits = holder.childCount;
        }
        int timesToMultiply = healthyUnits / profile.parentCountRequired;
        for (int i = 0; i < timesToMultiply; i++)
        {
            for (int j = 0; j < profile.childCountProduced; j++)
            {
                CreateUnit(profile, iter);
            }
        }
    }

    public void OnUnitDeath(UnitProfile profile)
    {
        for (int i = 0; i < unitTypeCount; i++)
        {
            if (profile == unitProfiles[i])
            {
                unitCount[i]--;
            }
        }
    }

    public Unit GetUnit(UnitType unitType)
    {
        for (int i = 0; i < unitTypeCount; i++)
        {
            if (unitType == unitProfiles[i].unitType)
            {
                if (unitHolders[i].childCount > 0)
                {
                    return unitHolders[i].GetChild(0).GetComponent<Unit>();
                }
            }
        }
        return null;
    }
}
