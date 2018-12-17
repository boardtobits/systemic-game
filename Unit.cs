using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public UnitProfile profile;
    public ForestSystem forest;
    public int currentHunger;
    public string baseName;

    public bool IsHealthy
    {
        get {
            return (!profile.consumer || currentHunger < profile.maxHungerAllowed);
        }
    }

    public void Initialize(UnitProfile profile, ForestSystem forest, int i)
    {
        this.profile = profile;
        this.forest = forest;
        baseName = profile.name + " " + forest.unitsCreated[i];
        transform.SetParent(forest.unitHolders[i]);
        forest.unitsCreated[i]++;
        forest.unitCount[i]++;
        if (profile.consumer)
        {
            SetName();
        }
        else
        {
            name = baseName;
        }
    }

    void SetName()
    {
        name = baseName + "(" + currentHunger + "/" + profile.maxHungerBeforeDeath + ")";
    }

    public void OnTick()
    {

        //checking hunger level
        currentHunger += profile.hungerIncreasePerTick;
        if (currentHunger >= profile.maxHungerBeforeDeath)
        {
            Destroy(gameObject);
        }
        else if (currentHunger >= profile.eatThreshold)
        {
            Eat();
        }
        SetName();

    }

    void Eat() {
        Unit food = forest.GetUnit(profile.eatPreference);
        if (food != null)
        {
            //Debug.Log(name + " eats " + food.name);
            currentHunger -= food.profile.nutritionalValue;
            food.transform.parent = null;
            Destroy(food.gameObject);
        }
    }

    private void OnDestroy()
    {
        forest.OnUnitDeath(profile);
    }
}
