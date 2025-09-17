using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

[RequireComponent(typeof(Debug_Spawn_Entity))]
public class Debug_Console : MonoBehaviour
{
    [Header("Console Settings")]
    public KeyCode toggleKey = KeyCode.BackQuote;
    public bool showConsole = false;
    public float consoleWidth = 400f;
    public int maxHistoryLines = 50;

    private string inputText = "";
    private Vector2 scrollPosition = Vector2.zero;
    private List<string> outputHistory = new List<string>();
    private Dictionary<string, Action<string[]>> commandDictionary = new Dictionary<string, Action<string[]>>();
    private bool focusOnTextfield = false;

    public PlayerInput playerInput;
    public PlayerInput consoleInput;

     public InputAction toggleConsoleAction;

    private Debug_Spawn_Entity debug_Spawn_Entity;



    void Start()
    {
        debug_Spawn_Entity = GetComponent<Debug_Spawn_Entity>();
        // Register default commands
        RegisterCommands();

        // Add welcome message
        AddOutputLine("Debug Console Initialized. Press ` to toggle.");
        AddOutputLine("Type 'help' for available commands.");
    }

    void Update()
    {
        // Toggle console with "`" key
        //if (toggleConsoleAction.triggered)
        //{
        //    playerInput.enabled = !playerInput.enabled;
        //    consoleInput.enabled = !consoleInput.enabled;

        //    showConsole = !showConsole;
        //    focusOnTextfield = showConsole;

        //    // Lock/unlock cursor based on console state
        //    Cursor.lockState = showConsole ? CursorLockMode.None : CursorLockMode.Locked;
        //    Cursor.visible = showConsole;
        //}
        // Toggle console with "`" key
        if (Input.GetKeyDown(toggleKey))
        {
            showConsole = !showConsole;
            focusOnTextfield = showConsole;

            // Lock/unlock cursor based on console state
            Cursor.lockState = showConsole ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = showConsole;
        }
    }

    void OnGUI()
    {
        if (!showConsole) return;

        // Draw console background
        GUI.Box(new Rect(0, 0, consoleWidth, Screen.height), "");

        // Draw output history with scroll view
        GUILayout.BeginArea(new Rect(5, 5, consoleWidth - 10, Screen.height - 40));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(consoleWidth - 10), GUILayout.Height(Screen.height - 40));

        // Display all output lines
        for (int i = 0; i < outputHistory.Count; i++)
        {
            GUILayout.Label(outputHistory[i]);
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();

        // Draw input field at the bottom
        GUILayout.BeginArea(new Rect(5, Screen.height - 30, consoleWidth - 10, 25));
        GUILayout.BeginHorizontal();

        GUI.SetNextControlName("ConsoleInput");
        inputText = GUILayout.TextField(inputText, GUILayout.Width(consoleWidth - 80));

        // Focus on textfield if needed
        if (focusOnTextfield)
        {
            GUI.FocusControl("ConsoleInput");
            focusOnTextfield = false;
        }

        if (GUILayout.Button("Submit", GUILayout.Width(70)) || (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return))
        {
            if (!string.IsNullOrEmpty(inputText))
            {
                ProcessCommand(inputText);
                inputText = "";
                focusOnTextfield = true;

                // Scroll to bottom after new input
                scrollPosition.y = Mathf.Infinity;
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    void ProcessCommand(string command)
    {
        // Add command to output
        AddOutputLine("> " + command);

        if (string.IsNullOrWhiteSpace(command))
            return;

        // Split command into parts
        string[] parts = command.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string commandName = parts[0].ToLower();

        // Check if command exists
        if (commandDictionary.ContainsKey(commandName))
        {
            try
            {
                // Execute command
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

    void AddOutputLine(string line)
    {
        outputHistory.Add(line);

        // Limit the number of lines
        while (outputHistory.Count > maxHistoryLines)
        {
            outputHistory.RemoveAt(0);
        }
    }

    void RegisterCommands()
    {
        // Help command
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

        // Clear command
        commandDictionary.Add("clear", (parts) =>
        {
            outputHistory.Clear();
            AddOutputLine("Console cleared.");
        });

        // Time scale command
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


        // Quit command
        commandDictionary.Add("quit", (parts) =>
        {
            AddOutputLine("Quitting application...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });

        // Add money
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

        // God mode command
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
                    //playerStats.ToggleGodMode(true);
                    playerStats.isGodMode = true;

                    AddOutputLine("God mode enabled.");
                }
                else if (boolStr == "false" || boolStr == "0" || boolStr == "off")
                {
                    //playerStats.ToggleGodMode(false);
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

        //commandDictionary.Add("sethealth", (parts) =>

        //{
        //    if (parts.Length < 2)
        //    {
        //        AddOutputLine("Usage: sethealth <value>");
        //        return;
        //    }

        //    if (int.TryParse(parts[1], out int amount))
        //    {
        //        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        //        if (playerStats != null)
        //        {   

        //            playerStats.maxHealth = amount;

        //            playerStats.currentHealth = playerStats.maxHealth;
        //            AddOutputLine($"Updated Health: {playerStats.currentHealth}");
        //        }
        //        else
        //        {
        //            AddOutputLine("PlayerStats component not found.");
        //        }
        //    }
        //    else
        //    {
        //        AddOutputLine("Invalid amount. Usage: sethealth <value>");
        //    }
        //});

        // Stat value changer command
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

        // Spawning Entity Command
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

        // Remove specfic spawned entities command
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

        // Clears all spawned entities
        commandDictionary.Add("clearall", (parts) =>
        {
            AddOutputLine("Clearing spawned Entities)");
            debug_Spawn_Entity.ClearEntities();
            AddOutputLine("Removed " + debug_Spawn_Entity.spawnedPrefabs.Count + "Entities");
        });


        // Control Waves Command
        commandDictionary.Add("setwave", (parts) =>
        {
            WaveManager waveManager = FindObjectOfType<WaveManager>();

            
        });

        commandDictionary.Add("nextwave", (parts) =>
        {
            
        });


        // Summling Party Cheats
        commandDictionary.Add("summling", (parts) =>
        {



        });

        // Upgrade Editor Command
        commandDictionary.Add("upgrade", (parts) =>
        {



        });

    }

    // Adds output to unity log
    public void LogToConsole(string message)
    {
        AddOutputLine(message);
    }

    // Easy creation of new commands
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