using UnityModManagerNet;
using HarmonyLib;
using System.Reflection;
using Kingmaker.Blueprints.JsonSystem;

namespace SpellMetamagicOptionsFix
{
    static class Main
    {
        public static bool Enabled;
        private static Settings modSettings;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            modSettings = UnityModManager.ModSettings.Load<Settings>(modEntry);
            Settings.ModEntry = modEntry;
            modEntry.OnToggle = OnToggle;
            new Harmony(modEntry.Info.Id).PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Enabled = value;
            return true;
        }

        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch
        {
            static bool loaded = false;

            [HarmonyAfter(new string[] { "WorldCrawl" })]
            static void Postfix()
            {
                if (loaded) return;
                loaded = true;

                Fixes.SpellFixes.FixBestowGraceOfTheChampion();

                Fixes.MetamagicFixes.FixMetamagic();
            }
        }
    }
}
