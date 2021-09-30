using HarmonyLib;
using JetBrains.Annotations;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.JsonSystem;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SpellMetamagicOptionsFix
{
    public static class Helpers
    {
		internal static void Load()
        {
			Helpers.classes = (from c in ResourcesLibrary.GetRoot().Progression.m_CharacterClasses where c.Guid != "f5b8c63b141b2f44cbb8c2d7579c34f5" select c).ToList<BlueprintCharacterClassReference>();
			Dictionary<BlueprintGuid, BlueprintsCache.BlueprintCacheEntry> blueprints = (Dictionary<BlueprintGuid, BlueprintsCache.BlueprintCacheEntry>)AccessTools.Field(typeof(BlueprintsCache), "m_LoadedBlueprints").GetValue(ResourcesLibrary.BlueprintsCache);
			BlueprintGuid[] keys = blueprints.Keys.ToArray<BlueprintGuid>();
			LocalBlueprintLib = (from k in keys select ResourcesLibrary.TryGetBlueprint(k)).ToArray<SimpleBlueprint>();
		}
		public static T[] GetBlueprints<T>() where T : SimpleBlueprint
		{
			return LocalBlueprintLib.OfType<T>().Distinct<T>().ToArray<T>();
		}

		[CanBeNull]
		public static T GetComponent<T>([CanBeNull] this BlueprintScriptableObject blueprint)
		{
			if (blueprint == null)
			{
				return default;
			}
			for (int i = 0; i < blueprint.ComponentsArray.Length; i++)
			{
				BlueprintComponent blueprintComponent;
				if ((blueprintComponent = blueprint.ComponentsArray[i]) is T)
				{
					return (T)(object)blueprintComponent;
				}
			}
			return default;
		}

		public static IEnumerable<T> GetComponents<T>([CanBeNull] this BlueprintScriptableObject blueprint)
		{
			if (blueprint == null)
			{
				yield break;
			}
			int num;
			for (int i = 0; i < blueprint.ComponentsArray.Length; i = num)
			{
				BlueprintComponent blueprintComponent;
				if ((blueprintComponent = blueprint.ComponentsArray[i]) is T)
				{
					yield return (T)(object)blueprintComponent;
				}
				num = i + 1;
			}
			yield break;
		}

		public static List<BlueprintCharacterClassReference> classes;

		public static SimpleBlueprint[] LocalBlueprintLib;
	}
}
