using System;
using System.Collections.Generic;
using System.Text;
using DiskCardGame;
using UnityEngine;

namespace OmniBackport {
	public static class StringExtensions {
		public static string Gemify(this string str) {
			StringBuilder sb = new StringBuilder();
			int colorToUse = 0;
			List<Color> colors = new List<Color>() {
				GameColors.Instance.blue,
				GameColors.Instance.gold,
				GameColors.Instance.limeGreen,
			};
			foreach(char c in str) {
				sb.Append(GameColors.ColorCharacter(c, colors[colorToUse]));
				colorToUse++;
				if(colorToUse >= colors.Count) colorToUse = 0;
			}
			return sb.ToString();
		}
	}
}
