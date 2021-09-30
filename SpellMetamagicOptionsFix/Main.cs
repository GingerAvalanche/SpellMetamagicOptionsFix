using UnityModManagerNet;
using HarmonyLib;
using System.Reflection;
using Kingmaker.Blueprints.JsonSystem;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using System.Linq;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.RuleSystem;
using System.Collections.Generic;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.ElementsSystem;
using System;

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

        [HarmonyPatch(typeof(BlueprintsCache), "Init")]
        static class BlueprintsCache_Init_Patch
        {
            static bool loaded = false;

            static void Postfix()
            {
                if (loaded) return;
                loaded = true;

                Helpers.Load();

                BlueprintAbility[] spells = (from b in Helpers.GetBlueprints<BlueprintAbility>() where b.IsSpell select b).ToArray<BlueprintAbility>();

                foreach (BlueprintAbility spell in spells)
                {
                    if (spell == null)
                    {
                        continue;
                    }
                    BlueprintAbility spell2;
                    AbilityEffectStickyTouch component = spell.GetComponent<AbilityEffectStickyTouch>();
                    if (component != null)
                    {
                        spell2 = component.TouchDeliveryAbility;
                    }
                    else
                    {
                        spell2 = spell;
                    }
                    if ((int)spell2.Range < 3)
                    {
                        spell.AvailableMetamagic |= Metamagic.Reach;
                    }
                    if (spell2.IsFullRoundAction || spell2.IsStandardAction)
                    {
                        spell.AvailableMetamagic |= Metamagic.Quicken;
                    }
                    if (!string.IsNullOrEmpty(spell2.LocalizedDuration.ToString()))
                    {
                        spell.AvailableMetamagic |= Metamagic.Extend;
                    }
                    IEnumerable<GameAction> spellActions = spell2.FlattenAllActions();
                    bool UsesDice = false;
                    foreach (var action in spellActions)
                    {
                        if ((action is ContextActionBreathOfLife life && life.Value.DiceType != DiceType.Zero) ||
                            (action is ContextActionDealDamage damage && damage.Value.DiceType != DiceType.Zero) ||
                            (action is ContextActionHealStatDamage statheal && statheal.Value.DiceType != DiceType.Zero) ||
                            (action is ContextActionHealTarget heal && heal.Value.DiceType != DiceType.Zero) ||
                            (action is ContextActionRepeatedActions repeat && repeat.Value.DiceType != DiceType.Zero) ||
                            (action is ContextActionSpawnMonster spawn && spawn.CountValue.DiceType != DiceType.Zero))
                        {
                            UsesDice = true;
                            break;
                        }
                    }
                    if (UsesDice)
                    {
                        spell.AvailableMetamagic |= Metamagic.Empower | Metamagic.Maximize;
                    }
                }
            }
        }
    }
}
