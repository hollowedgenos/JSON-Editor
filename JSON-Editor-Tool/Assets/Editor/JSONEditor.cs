using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class JSONEditor : EditorWindow
{
    private int selected = 0; // 'Select JSON' dropdown menu index
    private int prevSelected = -1; // Previously selected JSON dropdown menu index
    private int charSelect = 0; // Selected character index
    private int enemySelect = 0; // Selected enemy index
    private int itemSelect = 0; // Selected item index
    private bool jsonLoaded = false; // Flag that tells whether JSON is loaded or not
    private string fileName = ""; // JSON filename holder
    private string jsonPath = ""; // JSON filepath holder
    private JsonNode rootNode; // Holds the parsed JSON data as a JsonNode
    public static EditorWindow AreYouSure; // Confirmation Window

    // Dropdown Menu Options
    string[] jsonOptions = new string[6] { "", "Character_Data", "Enemy_Data", "Item_Data", "Dialogue", "Progression" };

    [MenuItem("Window/JSON Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow<JSONEditor>();
    }

    void OnGUI()
    {
        EditorGUI.BeginChangeCheck();

        // Dropdown Menu
        this.selected = EditorGUILayout.Popup("Select a JSON", selected, jsonOptions);

        JsonSelected();

        // Draw buttons if any popup is selected besides the blank one
        if (selected != 0)
        {
            DrawButtons();
            DrawsElementButtons();
        }

        if (EditorGUI.EndChangeCheck())
        {
            Debug.Log(jsonOptions[selected]);
        }


    }

    // Draws the Button GUI
    private void DrawButtons()
    {
        // Button GUI
        EditorGUILayout.BeginVertical();
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace(); // To align button to the right
        if (GUILayout.Button("Apply Changes to JSON", GUILayout.MaxWidth(200)))
        {
            ApplyJsonChanges();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Apply Changes to SO", GUILayout.Width(200)))
        {
            ApplySOChanges();
        }
        EditorGUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Apply Changes to Both", GUILayout.Width(200)))
        {
            ApplyJsonChanges();
            ApplySOChanges();
        }
        GUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
    }

    // Draws the add/remove element buttons for the corresponding JSON 
    // Also handle the logic upon pressing a button
    private void DrawsElementButtons()
    {
        string warning = "This will remove this from the index but not the JSON file. You must apply the JSON changes to remove it. Are you sure you want to do this?";

        // If character JSON selected
        if (selected == 1)
        {
            if (GUILayout.Button("Add Character"))
            {
                JsonArray characters = rootNode["characters"].AsArray();

                // Creates new JSON object
                var newCharacter = new JsonObject
                {
                    ["characterID"] = $"char{characters.Count + 1}_id",
                    ["characterName"] = $"character{characters.Count + 1}Name",
                    ["stats"] = new JsonObject
                    {
                        ["hp"] = 100,
                        ["defense"] = 0,
                        ["speed"] = 0,
                        ["gold"] = 0,
                        ["xp"] = 0
                    }
                };

                // Add the character
                characters.Add(newCharacter);
            }
            if (GUILayout.Button("Remove Character"))
            {
                // Create Dialog Box, true or false if Yes was clicked
                bool Yes = EditorUtility.DisplayDialog("Are You Sure?", warning, "Yes", "No");
                // If Yes, delete character
                if (Yes)
                {
                    JsonArray characters = rootNode["characters"].AsArray();

                    // Prevent deletion of character in an invalid index
                    if (charSelect >= 0 && charSelect < characters.Count)
                    {
                        characters.RemoveAt(charSelect);
                        Debug.Log($"Character at index {charSelect} removed.");

                        string charAssetPath = $"Assets/ScriptableObjects/Character/Character{charSelect + 1}.asset";

                        // If there is a scriptable object asset in the filepath, delete it
                        if (!string.IsNullOrEmpty(charAssetPath))
                        {
                            AssetDatabase.DeleteAsset(charAssetPath);
                            Debug.Log("ScriptableObject Asset deleted");
                        }
                        else
                        {
                            Debug.LogWarning($"Failed to delete file at: {charAssetPath}");
                        }

                        // Update dropdown selection
                        charSelect = Mathf.Clamp(charSelect - 1, 0, characters.Count - 1);
                    }
                    else
                    {
                        Debug.LogWarning("No character selected or index out of range happening.");
                    }
                }
            }
        }
        // If Enemy JSON selected
        if (selected == 2)
        {
            if (GUILayout.Button("Add Enemy"))
            {
                JsonArray enemies = rootNode["enemies"].AsArray();

                var newEnemy = new JsonObject
                {
                    ["enemyID"] = $"enemy{enemies.Count + 1}_id",
                    ["enemyName"] = $"enemy{enemies.Count + 1}Name",
                    ["stats"] = new JsonObject
                    {
                        ["hp"] = 100,
                        ["defense"] = 0,
                        ["speed"] = 50,
                        ["gold_amount"] = 10,
                        ["xp_amount"] = 5
                    }
                };

                enemies.Add(newEnemy);
            }
            if (GUILayout.Button("Remove Enemy"))
            {
                bool Yes = EditorUtility.DisplayDialog("Are You Sure?", warning, "Yes", "No");
                if (Yes)
                {
                    JsonArray enemies = rootNode["enemies"].AsArray();

                    if (enemySelect >= 0 && enemySelect < enemies.Count)
                    {
                        enemies.RemoveAt(enemySelect);
                        Debug.Log($"Enemy at index {enemySelect} removed.");

                        string enemyAssetPath = $"Assets/ScriptableObjects/Enemy/Enemy{enemySelect + 1}.asset";

                        if (!string.IsNullOrEmpty(enemyAssetPath))
                        {
                            AssetDatabase.DeleteAsset(enemyAssetPath);
                            Debug.Log("ScriptableObject Asset deleted");
                        }
                        else
                        {
                            Debug.LogWarning($"Failed to delete file at: {enemyAssetPath}");
                        }

                        enemySelect = Mathf.Clamp(enemySelect - 1, 0, enemies.Count - 1);
                    }
                    else
                    {
                        Debug.LogWarning("No enemy selected or index out of range happening.");
                    }
                }
            }
        }
        // If Item JSON selected
        if (selected == 3)
        {
            if (GUILayout.Button("Add Item"))
            {
                JsonArray items = rootNode["items"].AsArray();

                var newItem = new JsonObject
                {
                    ["itemID"] = $"item{items.Count + 1}_id",
                    ["itemName"] = $"item{items.Count + 1}Name",
                    ["effects"] = new JsonObject
                    {
                        ["hp_multiply"] = 0,
                        ["defense_add"] = 0,
                        ["speed_multiply"] = "0.25",
                        ["gold_gain_multiply"] = "0.00",
                        ["xp_gain_multiply"] = "0.00"
                    }
                };

                items.Add(newItem);
            }
            if (GUILayout.Button("Remove Item"))
            {
                bool Yes = EditorUtility.DisplayDialog("Are You Sure?", warning, "Yes", "No");
                if (Yes)
                {
                    JsonArray items = rootNode["items"].AsArray();

                    if (itemSelect >= 0 && itemSelect < items.Count)
                    {
                        items.RemoveAt(itemSelect);
                        Debug.Log($"Item at index {itemSelect} removed.");

                        string itemAssetPath = $"Assets/ScriptableObjects/Items/ItemData/Item{itemSelect + 1}.asset";

                        if (!string.IsNullOrEmpty(itemAssetPath))
                        {
                            AssetDatabase.DeleteAsset(itemAssetPath);
                            Debug.Log("ScriptableObject Asset deleted");
                        }
                        else
                        {
                            Debug.LogWarning($"Failed to delete file at: {itemAssetPath}");
                        }


                        itemSelect = Mathf.Clamp(itemSelect - 1, 0, items.Count - 1);
                    }
                    else
                    {
                        Debug.LogWarning("No item selected or index out of range happening.");
                    }
                }
            }
        }
    }

    // Tells the window to grab the JSON data based on which item was selected in the popup
    private void JsonSelected()
    {
        // Resets jsonLoaded flag when switching cases
        if (selected != prevSelected)
        {
            jsonLoaded = false;
            prevSelected = selected;
        }

        // Check if any option besides 0 in dropdown menu has been selected
        switch (selected)
        {
            // If Character_Data is selected
            case 1:
                if (!jsonLoaded) // Prevents OnGUI from loading JSON data every frame
                {
                    // Filename will be passed over to the GrabJsonData method
                    fileName = "Character_Stats";
                    GrabJsonData(fileName);
                    jsonLoaded = true;
                }
                DrawCharacterStats(rootNode);
                break;
            // If Enemy_Data is selected
            case 2:
                if (!jsonLoaded)
                {
                    fileName = "Enemy_Stats";
                    GrabJsonData(fileName);
                    jsonLoaded = true;
                }
                DrawEnemyStats(rootNode);
                break;
            // If Item_Stats is selected
            case 3:
                if (!jsonLoaded)
                {
                    fileName = "Item_Stats";
                    GrabJsonData(fileName);
                    jsonLoaded = true;
                }
                DrawItemStats(rootNode);
                break;
            // If Dialogue is selected
            // Not Fully Implemented
            case 4:
                if (!jsonLoaded)
                {
                    fileName = "Dialogue";
                    GrabJsonData(fileName);
                    jsonLoaded = true;
                }
                rootNode = DrawJsonData(rootNode, 0);
                break;
            // If Progression is selected
            // Not Fully Implemented
            case 5:
                if (!jsonLoaded)
                {
                    fileName = "Progression";
                    GrabJsonData(fileName);
                    jsonLoaded = true;
                }
                rootNode = DrawJsonData(rootNode, 0);
                break;
        }
    }

    // Grabs the JSON data and stores it as a node
    private void GrabJsonData(JsonNode rootFile)
    {
        // Takes filename to locate JSON and deserializes JSON contents into dictionary
        jsonPath = Application.streamingAssetsPath + $@"/JSONs/{rootFile}.json";

        var json = File.ReadAllText(jsonPath); // Reads the JSON
        // Stores the root node
        // This will be used by the DrawJsonData method called in OnGUI
        rootNode = JsonNode.Parse(json);
    }

    // Draws the UI for the JSON data on the window
    // Indent level is for the neatness of the Editor GUI
    private JsonNode DrawJsonData(JsonNode node, int indentLevel, string currentKey = "")
    {
        // Check if the data node in question is a JSON Object, Array, or Value
        // In this case it's checking if it's an Object
        if (node is JsonObject jsonObj)
        {
            // Iterate through each key and value pair in the JSON
            foreach (var keyValue in jsonObj.ToList())
            {
                var editedNode = DrawJsonData(keyValue.Value, indentLevel + 1, keyValue.Key);

                // Only replace the node if it's a value that changed
                if (editedNode is JsonValue && editedNode != keyValue.Value)
                {
                    jsonObj[keyValue.Key] = editedNode;
                }
            }
            return jsonObj;
        }
        // Specifically for JSON Arrays
        else if (node is JsonArray jsonArray)
        {
            // Iterate through each array in the JSON file
            for (int i = 0; i < jsonArray.Count; i++)
            {
                var editedNode = DrawJsonData(jsonArray[i], indentLevel + 1, $"[{i}]");

                if (editedNode is JsonValue && editedNode != jsonArray[i])
                {
                    jsonArray[i] = editedNode;
                }
            }
            return jsonArray;
        }
        // Specifically for JSON values
        else if (node is JsonValue jsonVal)
        {
            object value = jsonVal.GetValue<object>();
            object newValue = value;

            // Adds an indent for readability
            EditorGUI.indentLevel = indentLevel;
            EditorGUILayout.BeginHorizontal();

            // Draw label of current key
            if (!string.IsNullOrEmpty(currentKey))
            {
                EditorGUILayout.LabelField(currentKey, GUILayout.Width(250));
            }

            // Checks if the values in the JSON are string, int or bool
            if (jsonVal.TryGetValue<string>(out string strVal))
            {
                // Creates a TextField for the string value
                newValue = EditorGUILayout.TextField(strVal);
            }
            else if (jsonVal.TryGetValue<int>(out int intVal))
            {
                // Creates an IntField for the int value
                newValue = EditorGUILayout.IntField(intVal);
            }
            else if (jsonVal.TryGetValue<bool>(out bool boolVal))
            {
                // Creates a Toggle for the bool value
                newValue = EditorGUILayout.Toggle(boolVal);
            }
            else
            {
                // Fallback in case none of these values are a string, int, or bool
                EditorGUILayout.LabelField(value.ToString());
            }

            EditorGUILayout.EndHorizontal();

            if (!newValue.Equals(value)) return JsonValue.Create(newValue);

            return jsonVal;
        }
        // Fallback on this if null
        return node;
    }

    // Applies changes made in the editor to the JSON
    private void ApplyJsonChanges()
    {
        // Standard null check
        if (rootNode == null || string.IsNullOrEmpty(jsonPath)) return;

        // Serialize to JSON unindented (unreadable)
        string updateJson = rootNode.ToJsonString();

        // Parse JSON again
        var unindentedJson = JsonDocument.Parse(updateJson);

        //Serialize JSON to indented (readable)
        string indentedJson = JsonSerializer.Serialize(unindentedJson, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(jsonPath, indentedJson);
        Debug.Log("JSON has been updated");
    }

    private void ApplySOChanges()
    {
        // Standard null check
        if (rootNode == null) return;

        // If Character JSON is selected
        if (selected == 1)
        {
            JsonArray characterArray = rootNode["characters"].AsArray();
            JsonNode selectedChar = rootNode["characters"][charSelect];

            // Element Index of the selected JSON element from the dropdown
            var selectedCharIndex = characterArray[charSelect].GetElementIndex();

            string charAssetPath = $"Assets/ScriptableObjects/Character/Character{selectedCharIndex + 1}.asset";
            CharacterData scriptableObject = AssetDatabase.LoadAssetAtPath<CharacterData>(charAssetPath);

            // If there is no SO file, create it
            if (scriptableObject == null)
            {
                scriptableObject = ScriptableObject.CreateInstance<CharacterData>();
                AssetDatabase.CreateAsset(scriptableObject, charAssetPath);
            }

            // Update fields in the scriptable object
            scriptableObject.characterID = (string)selectedChar["characterID"];
            scriptableObject.characterName = (string)selectedChar["characterName"];

            scriptableObject.stats.hp = (int)selectedChar["stats"]["hp"];
            scriptableObject.stats.defense = (int)selectedChar["stats"]["defense"];
            scriptableObject.stats.speed = (int)selectedChar["stats"]["speed"];
            scriptableObject.stats.gold = (int)selectedChar["stats"]["gold"];
            scriptableObject.stats.xp = (int)selectedChar["stats"]["xp"];

            EditorUtility.SetDirty(scriptableObject);
            AssetDatabase.SaveAssets();
        }
        else if (selected == 2)
        {
            JsonArray enemyArray = rootNode["enemies"].AsArray();
            JsonNode selectedEnemy = rootNode["enemies"][enemySelect];

            var selectedEnemyIndex = enemyArray[enemySelect].GetElementIndex();

            string enemyAssetPath = $"Assets/ScriptableObjects/Enemy/Enemy{selectedEnemyIndex + 1}.asset";
            EnemyData scriptableObject = AssetDatabase.LoadAssetAtPath<EnemyData>(enemyAssetPath);

            if (scriptableObject == null)
            {
                scriptableObject = ScriptableObject.CreateInstance<EnemyData>();
                AssetDatabase.CreateAsset(scriptableObject, enemyAssetPath);
            }

            scriptableObject.enemyID = (string)selectedEnemy["enemyID"];
            scriptableObject.enemyName = (string)selectedEnemy["enemyName"];

            scriptableObject.stats.hp = (int)selectedEnemy["stats"]["hp"];
            scriptableObject.stats.defense = (int)selectedEnemy["stats"]["defense"];
            scriptableObject.stats.speed = (int)selectedEnemy["stats"]["speed"];
            scriptableObject.stats.gold_amount = (int)selectedEnemy["stats"]["gold_amount"];
            scriptableObject.stats.xp_amount = (int)selectedEnemy["stats"]["xp_amount"];

            EditorUtility.SetDirty(scriptableObject);
            AssetDatabase.SaveAssets();

        }
        else if (selected == 3)
        {
            JsonArray itemArray = rootNode["items"].AsArray();
            JsonNode selectedItem = rootNode["items"][itemSelect];

            var selectedItemIndex = itemArray[itemSelect].GetElementIndex();

            string itemAssetPath = $"Assets/ScriptableObjects/Items/ItemData/Item{selectedItemIndex + 1}.asset";
            ItemData scriptableObject = AssetDatabase.LoadAssetAtPath<ItemData>(itemAssetPath);

            if (scriptableObject == null)
            {
                scriptableObject = ScriptableObject.CreateInstance<ItemData>();
                AssetDatabase.CreateAsset(scriptableObject, itemAssetPath);
            }

            scriptableObject.itemID = (string)selectedItem["itemID"];
            scriptableObject.itemName = (string)selectedItem["itemName"];

            // Make new elements if ItemEffect list is empty
            if (!scriptableObject.effects.Any())
            {
                scriptableObject.effects.Add(new ItemEffect("hp_multiply", (int)selectedItem["effects"]["hp_multiply"]));
                scriptableObject.effects.Add(new ItemEffect("defense_add", (int)selectedItem["effects"]["defense_add"]));
                scriptableObject.effects.Add(new ItemEffect("speed_multiply", (string)selectedItem["effects"]["speed_multiply"]));
                scriptableObject.effects.Add(new ItemEffect("gold_gain_multiply", (string)selectedItem["effects"]["gold_gain_multiply"]));
                scriptableObject.effects.Add(new ItemEffect("xp_gain_multiply", (string)selectedItem["effects"]["xp_gain_multiply"]));
            }
            else if (scriptableObject.effects.Any())
            {
                // Copy the element, edit the copied element, and reassign the copied element in the array
                var HpElement = scriptableObject.effects[0];
                HpElement.IntValue = (int)selectedItem["effects"]["hp_multiply"];
                scriptableObject.effects[0] = HpElement;

                var DefenseElement = scriptableObject.effects[1];
                DefenseElement.IntValue = (int)selectedItem["effects"]["defense_add"];
                scriptableObject.effects[1] = DefenseElement;

                var SpeedElement = scriptableObject.effects[2];
                SpeedElement.StringValue = (string)selectedItem["effects"]["speed_multiply"];
                scriptableObject.effects[2] = SpeedElement;

                var GoldGainElement = scriptableObject.effects[3];
                GoldGainElement.StringValue = (string)selectedItem["effects"]["gold_gain_multiply"];
                scriptableObject.effects[3] = GoldGainElement;

                var XpGainElement = scriptableObject.effects[4];
                HpElement.StringValue = (string)selectedItem["effects"]["xp_gain_multiply"];
                scriptableObject.effects[4] = XpGainElement;
            }


            //scriptableObject.effects.hp_multiply = (int)selectedItem["effects"]["hp_multiply"];
                //scriptableObject.effects.defense_add = (int)selectedItem["effects"]["defense_add"];
                //scriptableObject.effects.speed_multiply = (string)selectedItem["effects"]["speed_multiply"];
                //scriptableObject.effects.gold_gain_multiply = (string)selectedItem["effects"]["gold_gain_multiply"];
                //scriptableObject.effects.xp_gain_multiply = (string)selectedItem["effects"]["xp_gain_multiply"];

                EditorUtility.SetDirty(scriptableObject);
            AssetDatabase.SaveAssets();
        }
    }

    // Draw the Character Stats upon selecting a character from the dropdown
    private void DrawCharacterStats(JsonNode _rootNode)
    {
        JsonArray characterArray = _rootNode["characters"].AsArray();
        string[] characterNames = new string[characterArray.Count];

        for (int i = 0; i < characterArray.Count; i++)
        {
            characterNames[i] = $"{characterArray[i]["characterID"]} - {characterArray[i]["characterName"]}";
        }

        // Dropdown for selecting which character to edit
        charSelect = EditorGUILayout.Popup("Select Character", charSelect, characterNames);

        // Get selected character
        var selectedChar = characterArray[charSelect];

        DrawJsonData(selectedChar, 0);
    }

    // Draw the Enemy Stats upon selecting a enemy from the dropdown
    private void DrawEnemyStats(JsonNode _rootNode)
    {
        JsonArray enemyArray = _rootNode["enemies"].AsArray();
        string[] enemyNames = new string[enemyArray.Count];

        for (int i = 0; i < enemyArray.Count; i++)
        {
            enemyNames[i] = $"{enemyArray[i]["enemyID"]} - {enemyArray[i]["enemyName"]}";
        }

        enemySelect = EditorGUILayout.Popup("Select Enemy", enemySelect, enemyNames);

        var selectedEnemy = enemyArray[enemySelect];

        DrawJsonData(selectedEnemy, 0);
    }

    // Draw the Item Stats upon selecting a item from the dropdown
    private void DrawItemStats(JsonNode _rootNode)
    {
        JsonArray itemArray = _rootNode["items"].AsArray();
        string[] itemNames = new string[itemArray.Count];

        for (int i = 0; i < itemArray.Count; i++)
        {
            itemNames[i] = $"{itemArray[i]["itemID"]} - {itemArray[i]["itemName"]}";
        }

        itemSelect = EditorGUILayout.Popup("Select Item", itemSelect, itemNames);

        var selectedItem = itemArray[itemSelect];

        DrawJsonData(selectedItem, 0);
    }

}