using System;
using System.Collections.Generic;
using System.Text;

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
	}
}
