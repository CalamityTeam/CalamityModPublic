using CalamityMod.Systems.Graphic.PixelationSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Dusts
{
    public class SquashDust : ModDust
    {
        public static Asset<Texture2D> SolidCircle { get; private set; }

        public static Asset<Texture2D> BloomCircle { get; private set; }

        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void Load()
        {
            if (Main.dedServ)
                return;

            SolidCircle = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/BasicCircle");
            BloomCircle = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle");
        }

        public override void OnSpawn(Dust dust)
        {
            dust.scale *= Main.rand.NextFloat(0.8f, 1f);
        }

        public override bool Update(Dust dust)
        {
            float fadeSpeed = (dust.fadeIn + 1);
            dust.rotation = dust.velocity.ToRotation() + MathHelper.PiOver2;
            dust.velocity *= 0.96f;
            if (dust.noGravity)
                dust.scale -= 0.045f * fadeSpeed;
            else
            {
                dust.scale -= 0.03f * fadeSpeed;
                dust.velocity.Y += Main.rand.NextFloat(0.1f, 0.35f) * fadeSpeed;
            }

            float light = MathHelper.Clamp(dust.scale * 0.8f, 0f, 1f);
            if (!dust.noLightEmittence)
                Lighting.AddLight(dust.position, dust.color.ToVector3() * light);

            if (dust.scale <= 0)
                dust.active = false;

            dust.position += dust.velocity;

            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            Vector2 baseSize = Vector2.One;
            if (dust.customData != null && dust.customData is Vector2)
                baseSize = (Vector2)dust.customData;

            Vector2 squash = new Vector2(Utils.Remap(dust.velocity.Length(), 2, 7, 1 * baseSize.X, 0.5f * baseSize.X), Utils.Remap(dust.velocity.Length(), 2, 7, 1 * baseSize.Y, 2.5f * baseSize.Y));

            // Glow Orb
            Main.spriteBatch.Draw(BloomCircle.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.rotation, BloomCircle.Size() * 0.5f, squash * dust.scale * 0.1f, SpriteEffects.None, 0);
            if (dust.alpha < 1)
                Main.spriteBatch.Draw(BloomCircle.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * 0.85f * Utils.GetLerpValue(255, 0, dust.alpha), dust.rotation, BloomCircle.Size() * 0.5f, squash * dust.scale * 0.04f, SpriteEffects.None, 0);
            if (!dust.noLight)
            {
                Main.spriteBatch.Draw(SolidCircle.Value, dust.position - Main.screenPosition, null, Color.Lerp(dust.color, Color.White, 0.3f) with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.rotation, SolidCircle.Size() * 0.5f, squash * dust.scale * 0.075f, SpriteEffects.None, 0);
            }
            return false;
        }
    }

    public class SquashDustPixelated : SquashDust
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override bool PreDraw(Dust dust)
        {
            PixelationManager.AddPixelatedDrawer((_) => DrawPixelated(dust), Enums.GeneralDrawLayer.AfterDusts);
            return false;
        }

        private static void DrawPixelated(Dust dust)
        {
            Vector2 baseSize = Vector2.One;
            if (dust.customData != null && dust.customData is Vector2)
                baseSize = (Vector2)dust.customData;

            Vector2 squash = new Vector2(Utils.Remap(dust.velocity.Length(), 2, 7, 1 * baseSize.X, 0.5f * baseSize.X), Utils.Remap(dust.velocity.Length(), 2, 7, 1 * baseSize.Y, 2.5f * baseSize.Y));

            // Glow Orb
            Main.spriteBatch.Draw(BloomCircle.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.rotation, BloomCircle.Size() * 0.5f, squash * dust.scale * 0.1f, SpriteEffects.None, 0);
            if (dust.alpha < 1)
                Main.spriteBatch.Draw(BloomCircle.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * 0.85f * Utils.GetLerpValue(255, 0, dust.alpha), dust.rotation, BloomCircle.Size() * 0.5f, squash * dust.scale * 0.04f, SpriteEffects.None, 0);
            if (!dust.noLight)
            {
                Main.spriteBatch.Draw(SolidCircle.Value, dust.position - Main.screenPosition, null, Color.Lerp(dust.color, Color.White, 0.3f) with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.rotation, SolidCircle.Size() * 0.5f, squash * dust.scale * 0.075f, SpriteEffects.None, 0);
            }
        }
    }
}
