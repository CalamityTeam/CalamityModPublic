using CalamityMod.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    public class AcidRainUI : InvasionProgressUI
    {
        public override bool IsActive => AcidRainEvent.AcidRainEventIsOngoing && Main.LocalPlayer.Calamity().ZoneSulphur;
        public override float CompletionRatio => 1f - AcidRainEvent.AcidRainCompletionRatio;
        public override string InvasionName => "Acid Rain";
        public override Color InvasionBarColor => AcidRainEvent.TextColor;
        public override Texture2D IconTexture => ModContent.Request<Texture2D>("CalamityMod/UI/MiscTextures/AcidRainIcon").Value;
    }
}
