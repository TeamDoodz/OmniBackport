using System;
using System.Collections.Generic;
using System.Text;
using DiskCardGame;

namespace OmniBackport.ML {
	public struct DatasetEntry {
		public int Health;
		public int Attack;
		public float PercentGimmick;
		public float PercentOffensive;
		public float PercentDefensive;
		public float PercentUtility;
		public float PercentNegative;
		public int PowerLevel;
		public int CostTier;
		public bool IsPart3Random;

		public override string ToString() {
			return $"{Health}	{Attack}	{PercentGimmick}	{PercentDefensive}	{PercentOffensive}	{PercentUtility}	{PercentNegative}	{PowerLevel}	{CostTier}	{(IsPart3Random? "1" : "0")}";
		}
		public static DatasetEntry FromCard(CardInfo card) {
			DatasetEntry entry = new DatasetEntry {
				Health = card.Health,
				Attack = card.Attack,
				PercentGimmick = card.Abilities.FindAll((x) => AbilityTypeUtils.GetType(x) == AbilityType.Gimmick).Count / card.Abilities.Count,
				PercentOffensive = card.Abilities.FindAll((x) => AbilityTypeUtils.GetType(x) == AbilityType.Offensive).Count / card.Abilities.Count,
				PercentDefensive = card.Abilities.FindAll((x) => AbilityTypeUtils.GetType(x) == AbilityType.Defensive).Count / card.Abilities.Count,
				PercentUtility = card.Abilities.FindAll((x) => AbilityTypeUtils.GetType(x) == AbilityType.Utility).Count / card.Abilities.Count,
				PowerLevel = card.PowerLevel,
				CostTier = card.CostTier,
				IsPart3Random = card.metaCategories.Contains(CardMetaCategory.Part3Random)
			};
			return entry;
		}
	}
}
