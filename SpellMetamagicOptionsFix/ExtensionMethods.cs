using System.Reflection;
using HarmonyLib;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using static SpellMetamagicOptionsFix.Helpers;

namespace SpellMetamagicOptionsFix
{
	public static class ExtensionMethods
	{
		public static Helpers.FlatAbility FlattenAbility(this BlueprintAbility ability)
        {
            flatAbility = new();
			flatAbility.Add(ability);

			foreach (FieldInfo field in ability.GetType().GetFields(AccessTools.all))
			{
				ParseObject(field.GetValue(ability));
				if (flatAbility.AllMetamagicSet) { break; }
			}

			foreach (PropertyInfo property in ability.GetType().GetProperties(AccessTools.all))
			{
				ParseObject(property.GetValue(ability));
				if (flatAbility.AllMetamagicSet) { break; }
			}

			return flatAbility;
        }
		public static void Flatten<T>(this T o)
        {
			if (o is null) { return; }
			if (!flatAbility.Add(o))
            {
				return;
			}

			foreach (FieldInfo field in o.GetType().GetFields(AccessTools.all))
			{
				ParseObject(field.GetValue(o));
				if (flatAbility.AllMetamagicSet) { return; }
			}

			foreach (PropertyInfo property in o.GetType().GetProperties(AccessTools.all))
			{
				try
				{
					ParseObject(property.GetValue(o));
				}
				catch (TargetInvocationException) { }
				if (flatAbility.AllMetamagicSet) { return; }
			}
		}
	}
}
