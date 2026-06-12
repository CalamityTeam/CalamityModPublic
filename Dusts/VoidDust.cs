using System;
using CalamityMod.Enums;
using CalamityMod.Systems.Graphic.PixelationSystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Dusts
{
    public class VoidDust : ModDust
    {
        public static Asset<Texture2D> SolidCircle { get; private set; }

        public static Asset<Texture2D> BloomCircle { get; private set; }

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
            float fade = (1 + dust.fadeIn);
            dust.velocity *= 0.96f;
            if (dust.noGravity)
                dust.scale -= 0.05f * fade;
            else
                dust.scale -= 0.065f * fade;

            float light = MathHelper.Clamp(dust.scale * 0.8f, 0f, 1f);
            if (!dust.noLightEmittence)
                Lighting.AddLight(dust.position, dust.color.ToVector3() * light);
            
            if (dust.scale <= 0)
                dust.active = false;

            dust.position += dust.velocity;
            return false;
        }
        public override bool MidUpdate(Dust dust)
        {
            dust.rotation = dust.velocity.ToRotation() + MathHelper.PiOver2;
            return false;
        }

        public override bool PreDraw(Dust dust)
        {
            dust.rotation = dust.velocity.ToRotation() + MathHelper.PiOver2;
            Vector2 baseSize = Vector2.One;
            bool useSquash = false;
            float squashMaxSpeed = 0;
            if (dust.customData != null && dust.customData is int)
            {
                useSquash = true;
                squashMaxSpeed = (float)dust.customData;
            }

            Vector2 squash = useSquash ? new Vector2(Utils.Remap(dust.velocity.Length(), squashMaxSpeed / 4, squashMaxSpeed, 1 * baseSize.X, 0.5f * baseSize.X), Utils.Remap(dust.velocity.Length(), squashMaxSpeed / 4, squashMaxSpeed, 1 * baseSize.Y, 2.5f * baseSize.Y)) : baseSize;

            if (!dust.noLight)
            {
                // Glow Orb
                Main.spriteBatch.Draw(BloomCircle.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.rotation, BloomCircle.Size() * 0.5f, squash * dust.scale * 0.1f, SpriteEffects.None, 0);
                if (dust.alpha < 1)
                    Main.spriteBatch.Draw(BloomCircle.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * 0.85f * Utils.GetLerpValue(255, 0, dust.alpha), dust.rotation, BloomCircle.Size() * 0.5f, squash * dust.scale * 0.04f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Draw(SolidCircle.Value, dust.position - Main.screenPosition, null, Color.Black * Utils.GetLerpValue(255, 0, dust.alpha), dust.rotation, SolidCircle.Size() * 0.5f, squash * dust.scale * 0.075f, SpriteEffects.None, 0);
            return false;
        }
    }

    public class VoidDustPixelated : VoidDust
    {
        public override string Texture => "CalamityMod/Dusts/VoidDust";

        public override bool PreDraw(Dust dust)
        {
            dust.rotation = dust.velocity.ToRotation() + MathHelper.PiOver2;
            PixelationManager.AddPixelatedDrawer((_) => DrawPixelated(dust), GeneralDrawLayer.AfterDusts);
            return false;
        }

        private static void DrawPixelated(Dust dust)
        {
            Vector2 baseSize = Vector2.One;
            bool useSquash = false;
            float squashMaxSpeed = 0;
            if (dust.customData != null && dust.customData is int)
            {
                useSquash = true;
                squashMaxSpeed = (int)dust.customData;
            }

            Vector2 squash = useSquash ? new Vector2(Utils.Remap(dust.velocity.Length(), squashMaxSpeed / 4, squashMaxSpeed, 1 * baseSize.X, 0.5f * baseSize.X), Utils.Remap(dust.velocity.Length(), squashMaxSpeed / 4, squashMaxSpeed, 1 * baseSize.Y, 2.5f * baseSize.Y)) : baseSize;

            if (!dust.noLight)
            {
                // Glow Orb
                Main.spriteBatch.Draw(BloomCircle.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * Utils.GetLerpValue(255, 0, dust.alpha), dust.rotation, BloomCircle.Size() * 0.5f, squash * dust.scale * 0.1f, SpriteEffects.None, 0);
                if (dust.alpha < 1)
                    Main.spriteBatch.Draw(BloomCircle.Value, dust.position - Main.screenPosition, null, dust.color with { A = 0 } * 0.85f * Utils.GetLerpValue(255, 0, dust.alpha), dust.rotation, BloomCircle.Size() * 0.5f, squash * dust.scale * 0.04f, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Draw(SolidCircle.Value, dust.position - Main.screenPosition, null, Color.Black * Utils.GetLerpValue(255, 0, dust.alpha), dust.rotation, SolidCircle.Size() * 0.5f, squash * dust.scale * 0.075f, SpriteEffects.None, 0);
        }
    }
}
