using cherrydev;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GuardsType
{
    Fat,
    Veteran,
    WithDog
}

public class Guard
{
    public string Name { get; set; }
    public GuardsType Type { get; set; }

    public Guard(string name, GuardsType type)
    {
        Name = name;
        Type = type;
    }
}

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    [SerializeField] private string[] carSpots;
    private List<string> willBeChekcedSpots;
    private List<string> hidenSpots;

    public List<string> CarSpots => carSpots.ToList();
    public List<string> WillBeCheckedSpots => willBeChekcedSpots;
    public List<string> HiddenSpots => hidenSpots;

    public static bool IsDay { private set; get; } = true;

    private List<Guard> guards;

    // Event for car searching
    public static event Action OnCarSearchRequested;

    public GuardsType GetCurrentGuardType()
    {
        // Return the main guard's type (assuming the second guard is the main one)
        if (guards != null && guards.Count > 1)
        {
            return guards[1].Type;
        }

        // Fallback to first guard if available
        if (guards != null && guards.Count > 0)
        {
            return guards[0].Type;
        }

        // Default fallback
        return GuardsType.Fat;
    }

    private void Start()
    {
        InitializeGuards();
        RandomlyChooseCarSpotsToCheck();
    }

    private void InitializeGuards()
    {
        guards = new List<Guard>();

        // Generate the first guard randomly
        GuardsType firstGuardType = (GuardsType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(GuardsType)).Length);
        guards.Add(new Guard("Guard1", firstGuardType));

        // Generate the second guard with 50% chance to be the same as the first guard or a different type
        GuardsType secondGuardType = UnityEngine.Random.value < 0.5f
            ? firstGuardType
            : (GuardsType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(GuardsType)).Length);

        guards.Add(new Guard("MainGuard", secondGuardType));

        Debug.Log($"Generated Guards: {guards[0].Name} ({guards[0].Type}), {guards[1].Name} ({guards[1].Type})");
    }

    private void RandomlyChooseCarSpotsToCheck()
    {
        if (carSpots == null || carSpots.Length < 2)
        {
            Debug.LogWarning("Not enough car spots to ensure at least two are free of checks.");
            return;
        }

        foreach (var guard in guards)
        {
            switch (guard.Type)
            {
                case GuardsType.Fat:
                    willBeChekcedSpots = carSpots.OrderBy(_ => UnityEngine.Random.value).Take(1).ToList();
                    break;

                case GuardsType.Veteran:
                    willBeChekcedSpots = carSpots.OrderBy(_ => UnityEngine.Random.value).Take(2).ToList();
                    break;

                case GuardsType.WithDog:
                    willBeChekcedSpots = carSpots.ToList();
                    break;
            }

            hidenSpots = carSpots.Except(willBeChekcedSpots).ToList();

            Debug.Log($"Guard {guard.Name} ({guard.Type}) checked spots: {string.Join(", ", willBeChekcedSpots)}");
            Debug.Log($"Hidden spots: {string.Join(", ", hidenSpots)}");
        }
    }

    public void StartSearchingCars()
    {
        Debug.Log("Car search requested by GameManager.");
        OnCarSearchRequested?.Invoke();
    }

    public void PrepareCharactersInfo(DialogBehaviour dialogBehaviour)
    {
        // Set the current guard type name as a dialog variable
        GuardsType currentGuardType = GetCurrentGuardType();
        string guardTypeName = currentGuardType.ToString();
        dialogBehaviour.SetVariableValue("currentGuardType", guardTypeName);

        // Set the spots that will be checked as a comma-separated string
        if (willBeChekcedSpots != null && willBeChekcedSpots.Count > 0)
        {
            string spotsToCheck = string.Join(", ", willBeChekcedSpots);
            dialogBehaviour.SetVariableValue("spotsToCheck", spotsToCheck);
        }
        else
        {
            dialogBehaviour.SetVariableValue("spotsToCheck", "None");
        }

        Debug.Log($"PrepareCharactersInfo: Guard Type = {guardTypeName}, Spots to Check = {string.Join(", ", willBeChekcedSpots ?? new List<string>())}");
    }

    public void SkipTime()
    {
        IsDay = !IsDay;
    }
}
