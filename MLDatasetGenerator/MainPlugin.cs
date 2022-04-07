using System;
using BepInEx;
using BepInEx.Logging;

namespace MLDatasetGenerator {
	[BepInPlugin(GUID, Name, Version)]
	[BepInDependency("cyantist.inscryption.api")]
	[BepInDependency("io.github.TeamDoodz.TDLib")]
	[BepInDependency("io.github.TeamDoodz.OmniBackport")]
	public class MainPlugin : BaseUnityPlugin {

		internal const string GUID = "io.github.TeamDoodz." + Name;
		internal const string Name = "MLDatasetGenerator";
		internal const string Version = "1.0.0";

		internal static ManualLogSource logger;

		private void Awake() {
			logger = Logger;
			logger.LogMessage($"{Name} v{Version} Loaded!");
		}

	}
}
