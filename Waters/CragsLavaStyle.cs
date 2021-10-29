using Terraria;

namespace CalamityMod.Waters
{
	public class CragsLavaStyle : CustomLavaStyle
	{
		public override string LavaTexturePath => "CalamityMod/Waters/CragsLava";

		public override string BlockTexturePath => LavaTexturePath + "_Block";

		public override bool ChooseLavaStyle() => Main.LocalPlayer.Calamity().ZoneCalamity;

		public override int ChooseWaterfallStyle() => 0;

		public override int GetSplashDust() => 0;

		public override int GetDropletGore() => 0;
	}
}
