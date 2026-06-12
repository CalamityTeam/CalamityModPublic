using System;
using System.Collections.Generic;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class WarbannerLight : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Typeless";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer moddedOwner => Owner.Calamity();
        public Color bColor = Color.White;
        public int time = 0;
        public float rotMult = 0.05f;
        public int direction = 1;
        public float rot2 = 0;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 300;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            float rate = Main.GlobalTimeWrappedHourly * 5;
            List<Color> eColors = new List<Color>()
            {
                Color.Goldenrod,
                Color.Orange
            };

            int colorIndex = (int)(rate / 2 % eColors.Count);
            Color currentColor = eColors[colorIndex];
            Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
            bColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);

            Projectile.Center = Owner.MountedCenter;

            if (moddedOwner.warbannerGlow && moddedOwner.WarbanneroftheRighteous)
                Projectile.timeLeft++;
            else
                Projectile.Kill();
            if (Owner.dead)
                Projectile.Kill();

            // Emit some light
            Lighting.AddLight(Projectile.Center, Color.Goldenrod.ToVector3() * 1.5f);

            rot2 = Math.Abs((float)Math.Sin(time * 0.15f / MathHelper.Pi) * 0.2f) + 0.8f;

            if (time % 2 == 0)
            {
                Vector2 dustVel = new Vector2(10, 10).RotatedByRandom(100);
                Dust dust = Dust.NewDustPerfect(Owner.Center + dustVel, ModContent.DustType<LightDust>(), dustVel * Main.rand.NextFloat(0.1f, 0.4f));
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.5f, 0.7f);
                dust.color = bColor;
                dust.noLightEmittence = true;
            }
            time++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D rTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/ShatteredExplosion").Value;
            Texture2D bTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            Color drawColor = bColor;
            Vector2 baseDrawPos = Owner.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY);

            for (int i = 0; i < 5; i++)
            {
                float bScale2 = 0.75f;
                Main.EntitySpriteDraw(bTexture, baseDrawPos, null, Color.Lerp(drawColor, Color.White, i * 0.15f) with { A = 0 }, 0, bTexture.Size() * 0.5f, (bScale2 - i * 0.15f) * rot2 * Projectile.scale, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(rTexture, baseDrawPos, null, drawColor with { A = 0 }, Main.rand.NextFloat(-2, 2), rTexture.Size() * 0.5f, 0.03f * rot2 * Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(rTexture, baseDrawPos, null, drawColor with { A = 0 }, Main.rand.NextFloat(-2, 2), rTexture.Size() * 0.5f, 0.04f * rot2 * Projectile.scale, SpriteEffects.None, 0);
            return false;
        }
        public override bool? CanDamage() => false;
    }
}
