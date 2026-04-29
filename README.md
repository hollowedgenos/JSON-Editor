## What is this?
---

This is an Editor script tool design for the Unity Engine, specifically made to aid development in the Soulsync project. 
<p>The function of this tool is to allow for the ease of editing JSON file data. It is also capable of editing scriptable object data which corresponds to an object in the JSON file.
The editor is also able to allow for the addition of new JSON objects to a file and removal, as well as scriptable objects.</p>

---

<b>System Name:</b> JSON Editor

<b>Summary:</b> This is an editor tool created in Unity made specifically to aid developers. The editor can access JSON files which hold information pertaining to stats of characters, enemies, and items in the game. Developers can edit these stats in the JSON direction, apply them to their corresponding scriptable objects, or add entirely new characters, enemies, and items.

<b>Technical Constraints/Goals:</b> There was only one technical constraint during the development of this tool. Which was Unity’s limited JSON parsing capabilities. Unity did not have the tools needed to parse JSON files in the way we needed it to. The solution to this was adding an external library, System.Text.Json. The goal for this tool was to allow editing capabilities for all JSON files in the project. This included the JSON files for the game dialogue and player meta-progression. However, these files cannot be edited by the tool as the JSON structure of both files are different from the characters, enemies, and items JSON files. While the data can be parsed, editing the data is a completely different endeavor which the dev team determined not worth the extra development time.

<b>Design Pattern Rationale:</b> There was no rationale in context to a design pattern for the development of this tool. There is one which materialized out of necessity. It is the composite pattern, specifically for the DrawJsonData method. The JSON files are intrinsically hierarchical, which needed a method to recursively go through this hierarchy.

<b>Software & Language:</b> Unity/C#

<b>Contribution Note:</b> My contributions to this project consisted of the script for the editor itself, JSONEditor.cs. I also created the structures for the Character_Stats, Enemy_Stats, Item_Stats, and Progression JSONs. While I did create the initial structure for the Dialogue JSON, the current version shown in the repository is a complete rewrite by developer [Haiying Zeng](https://github.com/hertz30). Other scripts provided to me were CharacterData.cs, EnemyData.cs, ItemData.cs and Ability.cs. The first three scripts were created by developer [Jalen Carney](https://github.com/JalenCarney). Ability.cs was created by developer [Johnathan Martin](https://github.com/Ingig0). It should be noted that while the initial ItemData.cs script was provided, I eventually made a complete structural rewrite of the script. CharacterData.cs and EnemyData.cs contain structural edits but were not completely rewritten. Ability.cs was not rewritten or edited in any shape or form. 

<b>External Assets & Libraries:</b> NuGetForUnity, System.Text.Json

<b>Optimization Note:</b> Much of the original JSON Editor script was scrapped as duplicate methods started to be created for drawing the stats for each separate JSON. This function is now relegated to the DrawJsonData method.

---

### How does this work?
Watch the video here for a demo:
https://www.youtube.com/watch?v=J_7WibZy7Xw
 
