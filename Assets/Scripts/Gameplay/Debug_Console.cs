using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

// Ensures this component is always attached with Debug_Spawn_Entity
[RequireComponent(typeof(Debug_Spawn_Entity))]
public class Debug_Console : MonoBehaviour
{
    [Header("Console Settings")]
    public KeyCode toggleKey = KeyCode.BackQuote; // Key to toggle the console
    public bool showConsole = false;              // Is the console currently visible?
    public float consoleWidth = 400f;             // Width of the console window
    public int maxHistoryLines = 50;              // Max lines to keep in output history

    private string inputText = "";                // Current input in the text field
    private Vector2 scrollPosition = Vector2.zero;// Scroll position for output history
    private List<string> outputHistory = new List<string>(); // Stores console output lines
    private Dictionary<string, Action<string[]>> commandDictionary = new Dictionary<string, Action<string[]>>(); // Maps command names to actions
    private bool focusOnTextfield = false;        // Should the input field be focused?

    public PlayerInput playerInput;               // Reference to player input (for toggling)
    public PlayerInput consoleInput;              // Reference to console input (for toggling)
    public InputAction toggleConsoleAction;       // (Unused) InputAction for toggling

    private Debug_Spawn_Entity debug_Spawn_Entity; // Reference to entity spawner
    private Debug_Upgrades debug_Upgrades;         // Reference to upgrades debug helper

    void Start()
    {
        // Get required components
        debug_Spawn_Entity = GetComponent<Debug_Spawn_Entity>();
        debug_Upgrades = GetComponent<Debug_Upgrades>();
        // Register all available commands
        RegisterCommands();

        // Show welcome messages
        AddOutputLine("Debug Console Initialized. Press ` to toggle.");
        AddOutputLine("Type 'help' for available commands.");
    }

    void Update()
    {
        // Toggle the console when the toggle key is pressed
        if (Input.GetKeyDown(toggleKey))
        {
            showConsole = !showConsole;
            focusOnTextfield = showConsole;

            // Lock or unlock the cursor based on console state
            Cursor.lockState = showConsole ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = showConsole;
        }
    }

    void OnGUI()
    {
        if (!showConsole) return; // Only draw if console is visible

        // Draw the console background
        GUI.Box(new Rect(0, 0, consoleWidth, Screen.height), "");

        // Draw the output history in a scrollable area
        GUILayout.BeginArea(new Rect(5, 5, consoleWidth - 10, Screen.height - 40));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(consoleWidth - 10), GUILayout.Height(Screen.height - 40));

        // Display each line in the output history
        for (int i = 0; i < outputHistory.Count; i++)
        {
            GUILayout.Label(outputHistory[i]);
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();

        // Draw the input field and submit button at the bottom
        GUILayout.BeginArea(new Rect(5, Screen.height - 30, consoleWidth - 10, 25));
        GUILayout.BeginHorizontal();

        GUI.SetNextControlName("ConsoleInput");
        inputText = GUILayout.TextField(inputText, GUILayout.Width(consoleWidth - 80));

        // Focus the input field if needed
        if (focusOnTextfield)
        {
            GUI.FocusControl("ConsoleInput");
            focusOnTextfield = false;
        }

        // Handle submit button or Enter key
        if (GUILayout.Button("Submit", GUILayout.Width(70)) || (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return))
        {
            if (!string.IsNullOrEmpty(inputText))
            {
                ProcessCommand(inputText); // Process the entered command
                inputText = "";
                focusOnTextfield = true;

                // Scroll to the bottom after new input
                scrollPosition.y = Mathf.Infinity;
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    // Processes a command string entered by the user
    void ProcessCommand(string command)
    {
        AddOutputLine("> " + command); // Echo the command

        if (string.IsNullOrWhiteSpace(command))
            return;

        // Split the command into parts (by spaces)
        string[] parts = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string commandName = parts[0].ToLower(); // First word is the command

        // Check if the command exists and execute it
        if (commandDictionary.ContainsKey(commandName))
        {
            try
            {
                commandDictionary[commandName].Invoke(parts);
            }
            catch (Exception e)
            {
                AddOutputLine($"Error executing command: {e.Message}");
            }
        }
        else
        {
            AddOutputLine($"Command not found: {commandName}");
        }
    }

    // Adds a line to the output history and trims if necessary
    void AddOutputLine(string line)
    {
        outputHistory.Add(line);

        // Remove oldest lines if over the max
        while (outputHistory.Count > maxHistoryLines)
        {
            outputHistory.RemoveAt(0);
        }
    }

    // Registers all available console commands
    void RegisterCommands()
    {
        // Each command is registered with a lambda that implements its logic

        // Help command: lists all available commands
        commandDictionary.Add("help", (parts) =>
        {
            AddOutputLine("Available commands:");
            AddOutputLine("  help - Show this help message");
            AddOutputLine("  clear - Clear console output");
            AddOutputLine("  timescale <value> - Set time scale (0.1-10)");
            AddOutputLine("  godmode - Toggle invincibility");
            AddOutputLine("  quit - Quit the game");
            AddOutputLine("  givemoney <amount> - Adds soul essence");
            AddOutputLine("  set <stat> <value> - Set player stat (health, damage, critchance, critmultiplier, jumpheight, dashstrength, dashcooldown, luck, affinity, pickupradius, firerate, focusduration)");
            AddOutputLine("  get currentstats - Display current player stats");
            AddOutputLine("  spawn <entity name> <count> - Spawn entities at player location");
            AddOutputLine("  despawn <entity name> <count> - Despawn specific number of entities");
            AddOutputLine("  clearall - Clear all spawned entities");
        });

        // Clear command: clears the output history
        commandDictionary.Add("clear", (parts) =>
        {
            outputHistory.Clear();
            AddOutputLine("Console cleared.");
        });

        // Timescale command: sets or displays the current time scale
        commandDictionary.Add("timescale", (parts) =>
        {
            if (parts.Length < 2)
            {
                AddOutputLine($"Current time scale: {Time.timeScale}");
                return;
            }

            if (float.TryParse(parts[1], out float scale))
            {
                scale = Mathf.Clamp(scale, 0.1f, 10f);
                Time.timeScale = scale;
                AddOutputLine($"Time scale set to: {scale}");
            }
            else
            {
                AddOutputLine("Invalid value. Usage: timescale <value>");
            }
        });

        // Quit command: exits the game or stops play mode in editor
        commandDictionary.Add("quit", (parts) =>
        {
            AddOutputLine("Quitting application...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });

        // Givesoul command: adds soul essence to the player
        commandDictionary.Add("givesoul", (parts) =>
        {
            if (parts.Length < 2)
            {
                AddOutputLine("Usage: setmoney <amount>");
                return;
            }

            if (int.TryParse(parts[1], out int amount))
            {
                PlayerStats playerStats = FindObjectOfType<PlayerStats>();
                if (playerStats != null)
                {
                    playerStats.AddSoulEssence(amount);
                    AddOutputLine($"Soul Essence: {playerStats.soulEssence}");
                }
                else
                {
                    AddOutputLine("PlayerStats component not found.");
                }
            }
            else
            {
                AddOutputLine("Invalid amount. Usage: setmoney <amount>");
            }
        });

        // Godmode command: toggles player invincibility
        commandDictionary.Add("godmode", (parts) =>
        {
            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            if (playerStats == null)
            {
                AddOutputLine("PlayerStats not found.");
                return;
            }

            if (parts.Length == 2)
            {
                // Parse the boolean parameter
                string boolStr = parts[1].ToLower();
                if (boolStr == "true" || boolStr == "1" || boolStr == "on")
                {
                    playerStats.isGodMode = true;
                    AddOutputLine("God mode enabled.");
                }
                else if (boolStr == "false" || boolStr == "0" || boolStr == "off")
                {
                    playerStats.isGodMode = false;
                    AddOutputLine("God mode disabled.");
                }
            }
            else
            {
                AddOutputLine("Invalid parameter. Usage: godmode [true|false]");
                return;
            }
        });

        // Get currentstats command: displays current player stats
        commandDictionary.Add("get currentstats", (parts) =>
        {
            PlayerStats playerStats = FindAnyObjectByType<PlayerStats>();

            AddOutputLine("Health: " + playerStats.currentHealth + "/" + playerStats.maxHealth);
            AddOutputLine("Damage: " + playerStats.damage);
            AddOutputLine("Critical Chance: " + playerStats.critChance);
            AddOutputLine("Critical Multiplier:" + playerStats.critChance);
            AddOutputLine("Fire Rate: " + playerStats.fireRate);
            AddOutputLine("Movement Speed: " + playerStats.movementSpeed);
            AddOutputLine("Jump Height: " + playerStats.jumpHeight);
            AddOutputLine("Dash Strength: " + playerStats.dashStrength);
            AddOutputLine("Dash Cooldown: " + playerStats.dashCooldown);
            AddOutputLine("Luck: " + playerStats.luck);
            AddOutputLine("Affinity: " + playerStats.affinity);
            AddOutputLine("Focus Duration: " + playerStats.focusDuration);
        });

        // Set command: sets a player stat to a value
        commandDictionary.Add("set", (parts) =>
        {
            if (parts.Length < 2)
            {
                AddOutputLine("Usage: set <stat> <value>");
                return;
            }

            string stat = parts[1].ToLower().ToString();
            float statValue = Convert.ToSingle(parts[2]);

            PlayerStats playerstats = FindObjectOfType<PlayerStats>();
            AddOutputLine("Stat to set: " + stat);

            playerstats.SetStat(stat, statValue);
        });

        // Spawn command: spawns entities at the player location
        commandDictionary.Add("spawn", (parts) =>
        {
            if (parts.Length < 2)
            {
                AddOutputLine("Usage: spawn <entity name> <count>");
                return;
            }

            GameObject player = GameObject.Find("Player");
            Transform playerLocation = player.transform;

            if (int.TryParse(parts[2], out int amount))
            {
                AddOutputLine("Spawning " + amount);

                for (int i = 0; i < amount; i++)
                {
                    debug_Spawn_Entity.SpawnEntity(parts[1], playerLocation);
                }
            }
            else
            {
                AddOutputLine("Invalid count parameter");
            }
        });

        // Despawn command: removes a specific number of entities by name
        commandDictionary.Add("despawn", (parts) =>
        {
            if (parts.Length < 3)
            {
                AddOutputLine("Usage: despawn <entity name> <count>");
                return;
            }

            if (int.TryParse(parts[2], out int amount))
            {
                AddOutputLine($"Despawning {amount} {parts[1]}");
                debug_Spawn_Entity.DespawnEntity(parts[1], amount);
            }
            else
            {
                AddOutputLine("Invalid count parameter");
            }
        });

        // Clearall command: removes all spawned entities
        commandDictionary.Add("clearall", (parts) =>
        {
            AddOutputLine("Clearing spawned Entities)");
            debug_Spawn_Entity.ClearEntities();
            AddOutputLine("Removed " + debug_Spawn_Entity.spawnedPrefabs.Count + "Entities");
        });

        // setwave command: destroys all enemies and starts a new wave at the specified number
        commandDictionary.Add("setwave", (parts) =>
        {
            if (parts.Length < 2)
            {
                AddOutputLine("Usage: setwave <waveNumber>");
                return;
            }

            if (!int.TryParse(parts[1], out int waveNumber) || waveNumber < 1)
            {
                AddOutputLine("Invalid wave number. Usage: setwave <waveNumber>");
                return;
            }

            // Destroy all enemies with the "Enemy" tag
            string enemyTag = "Enemy";
            var enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            int destroyedCount = 0;
            foreach (var enemy in enemies)
            {
                Destroy(enemy);
                destroyedCount++;
            }
            AddOutputLine($"Destroyed {destroyedCount} enemies with tag '{enemyTag}'.");

            // Find and reset the WaveManager and WaveSpawner
            WaveManager waveManager = FindObjectOfType<WaveManager>();
            if (waveManager == null)
            {
                AddOutputLine("WaveManager not found.");
                return;
            }

            var waveSpawner = waveManager.GetComponent<WaveSpawner>();
            if (waveSpawner != null)
            {
                waveSpawner.StopAllCoroutines();
                waveSpawner.isSpawning = false;
                waveManager.enemiesAlive = 0;
                waveManager.enemiesSpawned = 0;
            }

            waveManager.currentWave = waveNumber - 1; // -1 because StartNextWave() increments it
            waveManager.StartNextWave();
            AddOutputLine($"Set wave to {waveNumber} and started the wave.");
        });

        // nextwave command: destroys all enemies and starts the next wave
        commandDictionary.Add("nextwave", (parts) =>
        {
            string enemyTag = "Enemy";
            var enemies = GameObject.FindGameObjectsWithTag(enemyTag);
            int destroyedCount = 0;
            foreach (var enemy in enemies)
            {
                Destroy(enemy);
                destroyedCount++;
            }
            AddOutputLine($"Destroyed {destroyedCount} enemies with tag '{enemyTag}'.");

            WaveManager waveManager = FindObjectOfType<WaveManager>();
            if (waveManager == null)
            {
                AddOutputLine("WaveManager not found.");
                return;
            }

            var waveSpawner = waveManager.GetComponent<WaveSpawner>();
            if (waveSpawner != null)
            {
                waveSpawner.StopAllCoroutines();
                waveSpawner.isSpawning = false;
                waveManager.enemiesAlive = 0;
                waveManager.enemiesSpawned = 0;
            }

            waveManager.StartNextWave();
            AddOutputLine($"Starting Wave {waveManager.currentWave + 1}");
        });

        // summling command: (placeholder for summling party cheats)
        commandDictionary.Add("summling", (parts) =>
        {
            // No implementation yet
        });

        // addupgrade command: adds upgrades by name, supports names with spaces
        commandDictionary.Add("addupgrade", (parts) =>
        {
            if (parts.Length < 3)
            {
                AddOutputLine("Usage: addupgrade <upgrade name> <count>");
                return;
            }

            // Combine all parts except the last one for the upgrade name
            string upgradeName = string.Join(" ", parts, 1, parts.Length - 2);
            string countStr = parts[parts.Length - 1];

            if (!int.TryParse(countStr, out int count))
            {
                AddOutputLine("Invalid count. Usage: addupgrade <upgrade name> <count>");
                return;
            }

            debug_Upgrades.AddUpgrade(upgradeName, count);
            AddOutputLine($"Added Upgrade: {upgradeName} Amount: {count}");
        });

        // removeupgrade command: removes upgrades by name, supports names with spaces
        commandDictionary.Add("removeupgrade", (parts) =>
        {
            if (parts.Length < 3)
            {
                AddOutputLine("Usage: removeupgrade <upgrade name> <count>");
                return;
            }

            // Combine all parts except the last one for the upgrade name
            string upgradeName = string.Join(" ", parts, 1, parts.Length - 2);
            string countStr = parts[parts.Length - 1];

            if (!int.TryParse(countStr, out int count))
            {
                AddOutputLine("Invalid count. Usage: removeupgrade <upgrade name> <count>");
                return;
            }

            debug_Upgrades.RemoveUpgrade(upgradeName, count);
            AddOutputLine($"Removed Upgrade: {upgradeName} Amount: {count}");
        });
    }

    // Utility: log a message to the console output
    public void LogToConsole(string message)
    {
        AddOutputLine(message);
    }

    // Utility: register a new command at runtime
    public void RegisterCommand(string commandName, Action<string[]> action)
    {
        string key = commandName.ToLower();
        if (commandDictionary.ContainsKey(key))
        {
            Debug.LogWarning($"Command '{commandName}' is already registered.");
            return;
        }

        commandDictionary.Add(key, action);
    }
}