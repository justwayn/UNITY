using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ObjectiveHUD : MonoBehaviour
{
    public TextMeshProUGUI objectiveText;

    private Dictionary<string, int> initialCounts = new Dictionary<string, int>();
    private bool allObjectivesMet = false;

    void Start()
    {
        InitializeObjectives();
        UpdateUI();
    }

    void InitializeObjectives()
    {
        initialCounts.Clear();
        
        int soldiers = CountActive<SoldierAI>();
        if (soldiers > 0) initialCounts["Soldiers"] = soldiers;

        int crystals = CountActive<CrystalEnemyAI>();
        if (crystals > 0) initialCounts["Crystals"] = crystals;

        int tentacles = CountActive<TentacleBossAI>();
        if (tentacles > 0) initialCounts["Tentacles"] = tentacles;

        int owls = CountActive<OwlAI>();
        if (owls > 0) initialCounts["Owls"] = owls;

        int bears = 0;
        foreach (var enemy in Object.FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None))
        {
            if (enemy.enabled && enemy.name.ToLower().Contains("bear")) bears++;
        }
        if (bears > 0) initialCounts["Bears"] = bears;
    }

    int CountActive<T>() where T : MonoBehaviour
    {
        int count = 0;
        foreach (var obj in Object.FindObjectsByType<T>(FindObjectsSortMode.None))
        {
            if (obj.enabled) count++;
        }
        return count;
    }

    void UpdateUI()
    {
        if (objectiveText == null) return;

        string display = "";

        if (initialCounts.ContainsKey("Soldiers"))
        {
            int current = CountActive<SoldierAI>();
            int killed = initialCounts["Soldiers"] - current;
            display += $"Eliminate the soldier ({killed}/{initialCounts["Soldiers"]}){(current <= 0 ? " <color=green>✓</color>" : "")}\n";
        }

        if (initialCounts.ContainsKey("Crystals"))
        {
            int current = CountActive<CrystalEnemyAI>();
            int destroyed = initialCounts["Crystals"] - current;
            display += $"Destroy the tentacle's crystal ({destroyed}/{initialCounts["Crystals"]}){(current <= 0 ? " <color=green>✓</color>" : "")}\n";
        }

        if (initialCounts.ContainsKey("Tentacles"))
        {
            int current = CountActive<TentacleBossAI>();
            int killed = initialCounts["Tentacles"] - current;
            display += $"Eliminate the tentacle monster ({killed}/{initialCounts["Tentacles"]}){(current <= 0 ? " <color=green>✓</color>" : "")}\n";
        }

        if (initialCounts.ContainsKey("Owls"))
        {
            int current = CountActive<OwlAI>();
            int killed = initialCounts["Owls"] - current;
            display += $"Eliminate the owl ({killed}/{initialCounts["Owls"]}){(current <= 0 ? " <color=green>✓</color>" : "")}\n";
        }

        if (initialCounts.ContainsKey("Bears"))
        {
            int current = 0;
            foreach (var enemy in Object.FindObjectsByType<EnemyHealth>(FindObjectsSortMode.None))
            {
                if (enemy.enabled && enemy.name.ToLower().Contains("bear")) current++;
            }
            int killed = initialCounts["Bears"] - current;
            display += $"Eliminate the bear ({killed}/{initialCounts["Bears"]}){(current <= 0 ? " <color=green>✓</color>" : "")}\n";
        }

        objectiveText.text = display;
    }
}
