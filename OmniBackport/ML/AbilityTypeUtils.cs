using System;
using System.Collections.Generic;
using System.Text;
using DiskCardGame;

namespace OmniBackport.ML {
	public static class AbilityTypeUtils {
		private static List<Ability> GimmickAbilities = new List<Ability>() { 
			Ability.Strafe,
			Ability.StrafePush,
			Ability.StrafeSwap,
			Ability.SubmergeSquid,
			Ability.MoveBeside,
			Ability.RandomAbility,
			Ability.SwapStats,
			Ability.Transformer,
			Ability.SquirrelOrbit,
			Ability.DrawNewHand,
			Ability.CreateBells
		};
		private static List<Ability> OffensiveAbilities = new List<Ability>() { 
			Ability.BuffGems,
			Ability.CellBuffSelf,
			Ability.BuffNeighbours,
			Ability.GainAttackOnKill,
			Ability.SteelTrap,
			Ability.AllStrike,
			Ability.CellTriStrike,
			Ability.TriStrike,
			Ability.DoubleStrike,
			Ability.SplitStrike,
			Ability.Flying,
			Ability.Sentry,
			Ability.Deathtouch,
			Ability.ActivatedDealDamage,
			Ability.Sniper,
			Ability.ExplodeOnDeath,
			Ability.LatchBrittle,
			Ability.ExplodeGems,
			Ability.GuardDog,
			Ability.ConduitBuffAttack,
			Ability.FileSizeDamage,
		};
		public static List<Ability> DefensiveAbilities = new List<Ability>() { 
			Ability.TailOnHit,
			Ability.Submerge,
			Ability.Reach,
			Ability.PreventAttack,
			Ability.ActivatedHeal,
			Ability.DeathShield,
			Ability.ShieldGems,
			Ability.WhackAMole,
			Ability.ConduitHeal,
			Ability.ConduitFactory,
			Ability.CreateDams,
		};
		public static List<Ability> UtilityAbilities = new List<Ability>() {
			Ability.GainBattery,
			Ability.GainGemBlue,
			Ability.GainGemGreen,
			Ability.GainGemOrange,
			Ability.GainGemTriple,
			Ability.LatchDeathShield,
			Ability.LatchExplodeOnDeath,
			Ability.ConduitNull,
			Ability.ConduitEnergy,
			Ability.ConduitSpawnGems,
			Ability.DrawCopyOnDeath,
			Ability.DrawRandomCardOnDeath,
			Ability.CellDrawRandomCardOnDeath,
			Ability.DrawCopy,
			Ability.DrawAnt,
			Ability.DrawRabbits,
			Ability.BeesOnHit,
			Ability.DrawVesselOnHit,
		};
		public static List<Ability> NegativeAbilities = new List<Ability>() { 
			Ability.BuffEnemy,
			Ability.DeleteFile,
		};
		public static AbilityType GetType(Ability ability) {
			if(GimmickAbilities.Contains(ability)) return AbilityType.Gimmick;
			if(OffensiveAbilities.Contains(ability)) return AbilityType.Offenisve;
			if(DefensiveAbilities.Contains(ability)) return AbilityType.Defensive;
			if(UtilityAbilities.Contains(ability)) return AbilityType.Utility;
			MainPlugin.logger.LogWarning($"Could not get ability type for {ability}");
			return AbilityType.Gimmick;
		}
	}
}
