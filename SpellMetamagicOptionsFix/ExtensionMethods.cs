using System.Collections.Generic;
using System.Linq;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.ElementsSystem;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Mechanics.Actions;

namespace SpellMetamagicOptionsFix
{
	public static class ExtensionMethods
	{

		public static IEnumerable<GameAction> FlattenAllActions(this BlueprintAbility Ability)
		{
			return Ability.GetComponents<AbilityExecuteActionOnCast>().SelectMany((AbilityExecuteActionOnCast a) => a.FlattenAllActions()).Concat(Ability.GetComponents<AbilityEffectRunAction>().SelectMany((AbilityEffectRunAction a) => a.FlattenAllActions()));
		}

		public static IEnumerable<GameAction> FlattenAllActions(this AbilityExecuteActionOnCast Action)
		{
			return Action.Actions.Actions.FlattenAllActions();
		}

		public static IEnumerable<GameAction> FlattenAllActions(this AbilityEffectRunAction Action)
		{
			return Action.Actions.Actions.FlattenAllActions();
		}

		public static IEnumerable<GameAction> FlattenAllActions(this IEnumerable<GameAction> Actions)
		{
			List<GameAction> NewActions = new List<GameAction>();
			NewActions.AddRange(Actions.OfType<ContextActionOnRandomTargetsAround>().SelectMany((ContextActionOnRandomTargetsAround a) => a.Actions.Actions));
			NewActions.AddRange(Actions.OfType<ContextActionConditionalSaved>().SelectMany((ContextActionConditionalSaved a) => a.Failed.Actions));
			NewActions.AddRange(Actions.OfType<ContextActionConditionalSaved>().SelectMany((ContextActionConditionalSaved a) => a.Succeed.Actions));
			NewActions.AddRange(Actions.OfType<Conditional>().SelectMany((Conditional a) => a.IfFalse.Actions));
			NewActions.AddRange(Actions.OfType<Conditional>().SelectMany((Conditional a) => a.IfTrue.Actions));
			bool flag = NewActions.Count > 0;
			IEnumerable<GameAction> result;
			if (flag)
			{
				result = Actions.Concat(NewActions.FlattenAllActions());
			}
			else
			{
				result = Actions;
			}
			return result;
		}
	}
}
