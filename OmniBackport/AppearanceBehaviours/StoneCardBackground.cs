using DiskCardGame;
using InscryptionAPI.Card;
using TDLib.Attributes;
using UnityEngine;

namespace OmniBackport.AppearanceBehaviours {
	[AutoInit]
	public class StoneCardBackground : CardAppearanceBehaviour {
		private static void Init() {
			Id = CardAppearanceBehaviourManager.Add(MainPlugin.GUID, nameof(StoneCardBackground), typeof(StoneCardBackground)).Id;
		}
		public static Appearance Id { get; private set; }
		public static Texture2D BGTex { get; private set; } = null;
		public static Texture2D BackTex { get; private set; } = null;
		public override void ApplyAppearance() {
			if(BGTex == null) BGTex = MainPlugin.assets.LoadPNG("card_empty_stone");
			if(BackTex == null) BackTex = MainPlugin.assets.LoadPNG("card_back_stone");
			Card.SetCardback(BackTex);
			Card.RenderInfo.baseTextureOverride = BGTex;
			Card.RenderInfo.attackTextColor = GameColors.Instance.nearWhite;
			Card.RenderInfo.healthTextColor = GameColors.Instance.nearWhite;
			Card.RenderInfo.defaultAbilityColor = GameColors.Instance.nearWhite;
			Card.RenderInfo.nameTextColor = GameColors.Instance.nearWhite;
			Card.RenderInfo.portraitColor = GameColors.Instance.nearWhite;
		}
	}
}
