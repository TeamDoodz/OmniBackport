using System;
using System.Collections.Generic;
using System.Text;
using DiskCardGame;
using InscryptionAPI.Encounters;
using TDLib.Attributes;
using UnityEngine;

namespace OmniBackport.Nodes.Overclock {
	[AutoInit]
	public class OverclockNodeData : CustomNodeData {
		public override void Initialize() {
			AddGenerationPrerequisite(() => {
				// Only spawn the node at region 2 or higher, to prevent the deck from getting too thin
				return RunState.Run.regionTier >= 1;
			});
		}
		private static void Init() {
			NodeManager.Add<OverclockSequencer>(Animation.ToArray(), NodeManager.NodePosition.SpecialEventRandom);
		}
		public static List<Texture2D> Animation => new List<Texture2D>() { 
			MainPlugin.assets.LoadPNG("animated_overclocknode_1"),
			MainPlugin.assets.LoadPNG("animated_overclocknode_2"),
			MainPlugin.assets.LoadPNG("animated_overclocknode_3"),
			MainPlugin.assets.LoadPNG("animated_overclocknode_2"),
		};
	}
}
