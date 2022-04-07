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
			DatasetEntry entry = new DatasetEntry();
			entry.Health = card.Health;
			entry.Attack = card.Attack;
			int count = card.Abilities.Count;
			entry.PercentGimmick = count > 0 ? card.Abilities.FindAll((x) => AbilityTypeUtils.GetType(x) == AbilityType.Gimmick).Count / count : 0f;
			entry.PercentOffensive = count > 0 ? card.Abilities.FindAll((x) => AbilityTypeUtils.GetType(x) == AbilityType.Offensive).Count / count : 0f;
			entry.PercentDefensive = count > 0 ? card.Abilities.FindAll((x) => AbilityTypeUtils.GetType(x) == AbilityType.Defensive).Count / count : 0f;
			entry.PercentUtility = count > 0 ? card.Abilities.FindAll((x) => AbilityTypeUtils.GetType(x) == AbilityType.Utility).Count / count : 0f;
			entry.PowerLevel = card.PowerLevel;
			entry.CostTier = card.CostTier;
			entry.IsPart3Random = card.metaCategories.Contains(CardMetaCategory.Part3Random);
			if(entry.IsPart3Random) MainPlugin.logger.LogDebug("Card is part3random");
			return entry;
		}
	}
}
