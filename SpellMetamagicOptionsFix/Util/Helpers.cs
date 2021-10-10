using HarmonyLib;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes.Spells;
using Kingmaker.ElementsSystem;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.FactLogic;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker.UnitLogic.Mechanics.Actions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpellMetamagicOptionsFix
{
    public static class Helpers
    {
        public static FlatAbility flatAbility;
        private static readonly Type[] ignoreTypes = new Type[] { typeof(SpellListComponent), typeof(SpellComponent), typeof(AddSpellImmunity) };
        public static void ParseObject(object o)
        {
            if (flatAbility.AllMetamagicSet) { return; }
            if (o is null || ignoreTypes.Contains(o.GetType())) { return; }
            if (o as BlueprintComponent is not null)
            {
                (o as BlueprintComponent).Flatten();
            }
            else if (o as BlueprintScriptableObject is not null)
            {
                (o as BlueprintScriptableObject).Flatten();
            }
            else if (o as GameAction is not null)
            {
                (o as GameAction).Flatten();
            }
            else if (o as ActionList is not null)
            {
                foreach (GameAction a in (o as ActionList).Actions)
                {
                    a.Flatten();
                }
            }
            else if (o as Array is not null)
            {
                Array a = o as Array;
                for (var i = 0; i < a.Length; i++)
                {
                    ParseObject(a.GetValue(i));
                }
            }
            else if (o is ReferenceArrayProxy<BlueprintAbility, BlueprintAbilityReference> rap)
            {
                foreach (BlueprintAbility r in rap)
                {
                    r.Flatten();
                }
            }
        }
        public class FlatAbility
        {
            private readonly List<string> guids = new();
            public Metamagic metamagic { get; private set; }
            private bool EmpowerMaximize;
            private bool Quicken;
            private bool Extend;
            private bool Reach;
            private bool Bolstered;
            public bool AllMetamagicSet { get; private set; }

            public bool Add<T>(T o)
            {
                string hash = o.GetHashCode().ToString();
                if (guids.Contains(hash))
                {
                    return false;
                }

                if (!EmpowerMaximize) { CheckEmpowerMaximizeBolstered(o); }
                if (!Extend) { CheckExtend(o); }

                if (o is BlueprintAbility spell)
                {
                    if (!Quicken) { CheckQuicken(spell); }
                    if (!Reach) { CheckReach(spell); }
                }

                guids.Add(hash);
                return true;
            }

            private void CheckEmpowerMaximizeBolstered(object o)
            {
                bool UsesDice = o.GetType().GetFields(AccessTools.all)
                    .Where(f => f.FieldType == typeof(ContextDiceValue) && (ContextDiceValue)f.GetValue(o) is not null)
                    .Select(f => (ContextDiceValue)f.GetValue(o))
                    .Any(v => v.DiceType != DiceType.Zero);

                if (UsesDice)
                {
                    if (!EmpowerMaximize)
                    {
                        EmpowerMaximize = true;
                        metamagic |= Metamagic.Empower | Metamagic.Maximize;
                        AllMetamagicSet = EmpowerMaximize && Quicken && Extend && Reach && Bolstered;
                    }
                    if (!Bolstered && o.GetType() == typeof(ContextActionDealDamage))
                    {
                        Bolstered = ((ContextActionDealDamage)o).IsSimpleDamage;
                        if (Bolstered)
                        {
                            metamagic |= Metamagic.Bolstered;
                            AllMetamagicSet = EmpowerMaximize && Quicken && Extend && Reach && Bolstered;
                        }
                    }
                }
            }
            private void CheckQuicken(BlueprintAbility spell)
            {
                Quicken = spell.IsStandardAction || spell.IsFullRoundAction;
                if (Quicken) { metamagic |= Metamagic.Quicken; }
            }
            private void CheckExtend(object o)
            {
                Extend = o.GetType().GetFields(AccessTools.all)
                    .Where(f => f.FieldType == typeof(ContextDurationValue) && (ContextDurationValue)f.GetValue(o) is not null)
                    .Select(f => (ContextDurationValue)f.GetValue(o))
                    .Any(v => v.IsExtendable);
                if (Extend)
                {
                    metamagic |= Metamagic.Extend;
                    AllMetamagicSet = EmpowerMaximize && Quicken && Extend && Reach && Bolstered;
                }
            }
            private void CheckReach(BlueprintAbility spell)
            {
                Reach = (int)spell.Range < 3;
                if (Reach)
                {
                    metamagic |= Metamagic.Reach;
                    AllMetamagicSet = EmpowerMaximize && Quicken && Extend && Reach && Bolstered;
                }
            }
        }
    }
}
