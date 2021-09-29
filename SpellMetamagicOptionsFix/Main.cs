using UnityModManagerNet;
using HarmonyLib;
using System;
using System.Reflection;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using System.Collections.Generic;

namespace SpellMetamagicOptionsFix
{
    static class Main
    {
        public static bool Enabled;

        static bool Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnToggle = OnToggle;
            var harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            return true;
        }

        static bool OnToggle(UnityModManager.ModEntry modEntry, bool value)
        {
            Enabled = value;
            return true;
        }

        /// <summary>
        /// We cannot modify blueprints until after the game has loaded them, we patch BlueprintsCache.Init
        /// to be able to make our modifications as soon as the blueprints have loaded.
        /// </summary>
        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch
        {
            static bool loaded = false;
            static void Postfix()
            {
                if (loaded) return;
                loaded = true;

                var patchDict = new Dictionary<string, Metamagic>(12);
                Metamagic metamagic;

                // Cure wounds, 12 entries
                metamagic = ResourcesLibrary.TryGetBlueprint<BlueprintAbility>("47808d23c67033d4bbab86a1070fd62f").AvailableMetamagic;
                patchDict["e84cb97373ca6174397bfe778a039eab"] = metamagic;
                patchDict["83d6d8f4c4d296941838086f60485fb7"] = metamagic;
                patchDict["44cf8a9f080a23f4689b4bb51e3bdb64"] = metamagic;
                patchDict["1203e2dab8a593a459c0cc688f568052"] = metamagic;
                patchDict["5590652e1c2225c4ca30c4a699ab3649"] = metamagic;
                patchDict["e5af3674bb241f14b9a9f6b0c7dc3d27"] = metamagic;
                patchDict["6b90c773a6543dc49b2505858ce33db5"] = metamagic;
                patchDict["65f0b63c45ea82a4f8b8325768a3832d"] = metamagic;
                patchDict["3361c5df793b4c8448756146a88026ad"] = metamagic;
                patchDict["bd5da98859cf2b3418f6d68ea66cabbe"] = metamagic;
                patchDict["41c9016596fe1de4faf67425ed691203"] = metamagic;
                patchDict["651110ed4f117a948b41c05c5c7624c0"] = metamagic;

                // Shocking grasp, 1 entry
                metamagic = ResourcesLibrary.TryGetBlueprint<BlueprintAbility>("17451c1327c571641a1345bd31155209").AvailableMetamagic;
                patchDict["ab395d2335d3f384e99dddee8562978f"] = metamagic;

                // Corrosive touch, 1 entry
                metamagic = ResourcesLibrary.TryGetBlueprint<BlueprintAbility>("1a40fc88aeac9da4aa2fbdbb88335f5d").AvailableMetamagic;
                patchDict["95810d2829895724f950c8c4086056e7"] = metamagic;

                foreach (KeyValuePair<string, Metamagic> patch in patchDict)
                {
                    var spell = ResourcesLibrary.TryGetBlueprint<BlueprintAbility>(patch.Key);
                    spell.AvailableMetamagic |= patch.Value;
                }
            }
        }
    }
}
