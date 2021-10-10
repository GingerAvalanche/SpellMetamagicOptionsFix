using Kingmaker.UnitLogic.Abilities;
using Kingmaker.Utility;
using System.Linq;

namespace SpellMetamagicOptionsFix.Fixes
{
    static class MetamagicFixes
    {
        public static void FixMetamagic()
        {
            Util.SpellLists.AllSpellLists
                .SelectMany(list => list.SpellsByLevel)
                .Where(spellList => spellList != null)
                .SelectMany(level => level.Spells)
                .Distinct()
                .ForEach(spell => {
                    if (spell == null)
                    {
                        return;
                    }
                    Helpers.FlatAbility ability = spell.FlattenAbility();
                    Metamagic handled = (Metamagic)559; // Empower, Maximize, Quicken, Extend, Reach, Bolstered
                    spell.AvailableMetamagic &= ~handled;
                    spell.AvailableMetamagic |= ability.metamagic;
                });
        }
    }
}
