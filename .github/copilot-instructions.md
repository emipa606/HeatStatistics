# GitHub Copilot Instructions for Heat statistics (Continued) Mod

## Mod Overview and Purpose

The Heat statistics (Continued) mod is an update to the Helldraggers mod, aimed at enhancing the management of heat sources and AC systems in RimWorld. This mod adds dynamic and explained statistics to various heat-producing and cooling buildings. It is safe to add or remove from saves and should not conflict with other mods, although some buildings may not support these dynamic stats due to limitations in the game's API.

## Key Features and Systems

### Heat Pushers
- Displays the amount of heat pushed per second by heat pushers such as campfires, passive coolers, and generators.

### AC Buildings
- In-place AC buildings (e.g., heaters) and room-exchange AC buildings (e.g., coolers) display their max and current temperature control per second.

### Geothermal Power
- Geothermal power generators now show their temperature and power output, helping players manage their base's temperature more effectively.

## Coding Patterns and Conventions

### XML Integration
- XML files are used to define and integrate new stats and categories into the game. Each XML file follows the RimWorld XML schema and contains definitions for stats, stat categories, and patches.

### Harmony Patching
- Harmony patching is used to inject custom behavior into existing RimWorld game code. This includes adding new statistics, modifying existing ones, and handling compatibility with other mods.

## Suggestions for Copilot

### General Tips
- **Follow RimWorld Coding Standards**: Use PascalCase for method names and CamelCase for variable names.
- **Implement XML Definitions**: When adding new stats or categories, ensure that they are defined in the appropriate XML files.
- **Use Harmony Patching Wisely**: Ensure that Harmony patches are well-documented and do not interfere with existing game behavior.

### Common Tasks
- **Adding New Stats**: Define new `StatDef` entries in XML files and implement corresponding `StatWorker` classes in C#.
- **Modifying Existing Stats**: Patch existing stats using Harmony to add additional information or functionality.
- **Handling Compatibility**: Test the mod with other popular mods to ensure compatibility and handle any conflicts gracefully.

### Example Code Snippets

#### Example XML Definition
xml
<StatCategoryDef>
    <label>Heat Statistics</label>
    <description>Dynamic statistics for heat sources and AC buildings.</description>
</StatCategoryDef>

<StatDef>
    <label>Heat Pushed Per Second</label>
    <category>Heat Statistics</category>
    <statType>Float</statType>
</StatDef>


#### Example C# StatWorker
csharp
using RimWorld;
using Verse;

namespace HeatStatistics
{
    public class StatWorker_PowerPlantSteam_PushedPerSecond : StatWorker
    {
        public override bool IsDisabledFor(Thing thing)
        {
            return !(thing is Building_PowerPlantSteam);
        }

        public override float GetValueUnfinalized(Thing thing, StatRequest req, bool applyPostProcess)
        {
            var steamPlant = thing as Building_PowerPlantSteam;
            if (steamPlant != null)
            {
                return steamPlant.GetHeatPushedPerSecond();
            }
            return 0f;
        }

        public override string GetExplanationUnfinalized(Thing thing, StatRequest req, ToStringNumberSense numberSense, StatDef affixStat, ref bool blank)
        {
            var steamPlant = thing as Building_PowerPlantSteam;
            if (steamPlant != null)
            {
                return $"Pushes {steamPlant.GetHeatPushedPerSecond()} heat per second.";
            }
            return "No heat data available.";
        }
    }
}


### Testing and Validation
- **Test with Different Scenarios**: Ensure that the new stats and behaviors work correctly in various in-game scenarios.
- **Check for Compatibility**: Test the mod with other popular mods to ensure it does not conflict with them.
- **Handle Edge Cases**: Consider and handle edge cases, such as geothermal generators in multiple rooms or vents equalizing temperature between rooms.

By following these guidelines, you can contribute effectively to the Heat statistics (Continued) mod and enhance the RimWorld experience for players.

## License and Support
- **License**: MIT License. Feel free to use, modify, and distribute the mod as long as you give proper credit to the original author.
- **Support**: For support, join the Discord channel for error reporting and discussions.



This .github/copilot-instructions.md file provides a comprehensive guide for contributing to the Heat statistics (Continued) mod, covering key features, coding patterns, XML integration, Harmony patching, and suggestions for using GitHub Copilot effectively.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

