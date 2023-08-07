using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.Localization;
using static Terraria.ModLoader.ModContent;
using CalamityMod.Items.Armor.DesertProwler;
using CalamityMod.Particles;
using CalamityMod.UI;

namespace CalamityMod.Cooldowns
{
    public class SandsmokeBomb : CooldownHandler
    {
        public bool PowerActive => instance.timeLeft > DesertProwlerHat.SmokeCooldown;
        public float PowerPercent => (instance.timeLeft - DesertProwlerHat.SmokeCooldown) / (float)DesertProwlerHat.SmokeDuration;


        public static new string ID => "SandsmokeBomb";
        public override bool ShouldDisplay => true;
        public override LocalizedText DisplayName => CalamityUtils.GetText("UI.Cooldowns.SandsmokeBomb" + (PowerActive ? "Active" : "Cooldown"));
        public override string Texture => "CalamityMod/Cooldowns/SandsmokeBomb";
        public override string OutlineTexture => "CalamityMod/Cooldowns/SandsmokeBombOutline";
        public override string OverlayTexture => "CalamityMod/Cooldowns/SandsmokeBombOverlay";
        public override Color OutlineColor => new Color(255, 299, 156);
        public override Color CooldownStartColor => PowerActive ? Color.Lerp(new Color(204, 181, 72), new Color(169, 142, 16), PowerPercent) : new Color(204, 181, 72);
        public override Color CooldownEndColor => PowerActive ? Color.Lerp(new Color(204, 181, 72), new Color(169, 142, 16), PowerPercent) : new Color(169, 142, 16);
        public override SoundStyle? EndSound => new("CalamityMod/Sounds/Custom/AbilitySounds/DesertProwlerSmokeBombReload");

        public override void OnCompleted()
        {
            Vector2 playerVelocity = instance.player.velocity / 8f;
            Vector2 particleGravity = Vector2.UnitY * 0.03f;
            for (int i = 0; i < 16; i++)
            {
                Vector2 dustDisplace = Main.rand.NextVector2Circular(80f, 50f);
                Vector2 dustPosition = instance.player.MountedCenter + dustDisplace;
                Vector2 dustSpeed = Main.rand.NextVector2Circular(0.5f, 0.5f) + playerVelocity - Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4) * 0.06f;
                dustSpeed.X += 1.4f * (float)Math.Sin(((dustDisplace.X + 80f) / 160f) * MathHelper.Pi) * (Main.rand.NextBool() ? -1 : 1);
                Particle dust = new SandyDustParticle(dustPosition, dustSpeed, Color.White, Main.rand.NextFloat(0.7f, 1.2f), Main.rand.Next(20, 50), 0.03f, particleGravity);
                GeneralParticleHandler.SpawnParticle(dust);
            }
        }

        //Charge down at first, and then charge back up
        private float AdjustedCompletion => CooldownRackUI.DebugFullDisplay ? CooldownRackUI.DebugForceCompletion : PowerActive ? PowerPercent : 1 - (instance.timeLeft / (float)DesertProwlerHat.SmokeCooldown);

        public override void ApplyBarShaders(float opacity)
        {
            //Use the adjusted completion
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseOpacity(opacity);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseSaturation(AdjustedCompletion);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseColor(CooldownStartColor);
            GameShaders.Misc["CalamityMod:CircularBarShader"].UseSecondaryColor(CooldownEndColor);
            GameShaders.Misc["CalamityMod:CircularBarShader"].Apply();
        }

        public override void DrawCompact(SpriteBatch spriteBatch, Vector2 position, float opacity, float scale)
        {
            Texture2D sprite = Request<Texture2D>(Texture).Value;
            Texture2D outline = Request<Texture2D>(OutlineTexture).Value;
            Texture2D overlay = Request<Texture2D>(OverlayTexture).Value;

            //Draw the outline
            spriteBatch.Draw(outline, position, null, OutlineColor * opacity, 0, outline.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            //Draw the icon
            spriteBatch.Draw(sprite, position, null, Color.White * opacity, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);

            //Draw the small overlay
            int lostHeight = (int)Math.Ceiling(overlay.Height * AdjustedCompletion);
            Rectangle crop = new Rectangle(0, lostHeight, overlay.Width, overlay.Height - lostHeight);
            spriteBatch.Draw(overlay, position + Vector2.UnitY * lostHeight * scale, crop, OutlineColor * opacity * 0.9f, 0, sprite.Size() * 0.5f, scale, SpriteEffects.None, 0f);
        }
    }
}
