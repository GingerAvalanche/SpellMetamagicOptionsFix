using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Mechanics.Actions;
using Kingmaker.Utility;
using System.Linq;

namespace SpellMetamagicOptionsFix.Fixes
{
    static class SpellFixes
    {
        public static void FixBestowGraceOfTheChampion()
        {
            Util.Spells.BestowGraceOfTheChampion.ComponentsArray
                .Where(component => component.GetType() == typeof(AbilityEffectRunAction))
                .SelectMany(component => ((AbilityEffectRunAction)component).Actions.Actions)
                .Where(action => action.GetType() == typeof(ContextActionApplyBuff))
                .Select(action => ((ContextActionApplyBuff)action).DurationValue)
                .ForEach(duration => { duration.m_IsExtendable = true; });
        }
    }
}
