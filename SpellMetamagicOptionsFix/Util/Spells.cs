using Kingmaker.Blueprints;
using Kingmaker.UnitLogic.Abilities.Blueprints;

namespace SpellMetamagicOptionsFix.Util
{
    class Spells
    {
        public static BlueprintAbility BestowGraceOfTheChampion => ResourcesLibrary.TryGetBlueprint<BlueprintAbility>("3cf5b2bd093a36a468b4ece38ad4d5fa");
    }
}
