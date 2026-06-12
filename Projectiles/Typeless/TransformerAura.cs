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
    public class TransformerAura : ModProjectile, ILocalizedModType
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
        public float fullSine = 1;
        public Vector2 squareRandomPos1;
        public Vector2 squareRandomPos2;
        public float fullChargeMult = 1;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 10;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            float rate = Main.GlobalTimeWrappedHourly * 5;
            List<Color> eColors = new List<Color>()
            {
                Color.LightSkyBlue,
                Color.DodgerBlue
            };

            int colorIndex = (int)(rate / 2 % eColors.Count);
            Color currentColor = eColors[colorIndex];
            Color nextColor = eColors[(colorIndex + 1) % eColors.Count];
            bColor = Color.Lerp(currentColor, nextColor, rate % 2f > 1f ? 1f : rate % 1f);

            Projectile.Center = Owner.MountedCenter;
            fullChargeMult = MathHelper.Lerp(fullChargeMult, (Owner.ownedProjectileCounts[ModContent.ProjectileType<TransformerBlob>()] >= 30 ? 0 : 1), 0.033f);

            if (moddedOwner.transformerVisual && moddedOwner.transformer)
            {
                if (moddedOwner.transformerCooldown == 0)
                {
                    Projectile.timeLeft = (int)MathHelper.Clamp(Projectile.timeLeft + 5, 0, 300);
                }
                else
                    Projectile.timeLeft = (int)(Projectile.timeLeft * 0.98f);
            }
            else
                Projectile.Kill();
            if (Owner.dead)
                Projectile.Kill();

            // Emit some light
            Lighting.AddLight(Projectile.Center, bColor.ToVector3() * 0.9f);

            rot2 = Math.Abs((float)Math.Sin(time * 0.15f / MathHelper.Pi) * 0.2f) + 0.8f;
            fullSine = (float)Math.Sin(time * 0.15f / MathHelper.Pi);

            if (time % 2 == 0)
            {
                Vector2 dustVel = new Vector2(10, 10).RotatedByRandom(100);
                Dust dust = Dust.NewDustPerfect(Owner.Center + dustVel, ModContent.DustType<VoidDustInverted>(), dustVel * Main.rand.NextFloat(0.1f, 0.4f));
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.5f, 0.7f);
                dust.color = new Color(30, 30, 30);
                dust.noLightEmittence = true;
            }

            time++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D vTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomRingThinLarge").Value;
            Texture2D bTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            Texture2D b2Texture = ModContent.Request<Texture2D>("CalamityMod/Particles/Light").Value;
            Color drawColor = bColor;
            float deathLerp = (float)Math.Pow(Utils.GetLerpValue(10, 300, Projectile.timeLeft), 2);
            Vector2 baseDrawPos = Owner.Center - Main.screenPosition + new Vector2(0, Owner.gfxOffY);

            float bScale2 = 0.95f;
            Main.EntitySpriteDraw(bTexture, baseDrawPos, null, drawColor with { A = 0 } * deathLerp, 0, bTexture.Size() * 0.5f, (bScale2 * rot2 * Projectile.scale * deathLerp) + 2.5f * (fullChargeMult - 1), SpriteEffects.None, 0);

            Main.EntitySpriteDraw(b2Texture, baseDrawPos, null, new Color(30, 30, 30) * deathLerp, Main.rand.NextFloat(-2, 2), b2Texture.Size() * 0.5f, bScale2 * Projectile.scale * deathLerp * 1.2f * rot2, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(b2Texture, baseDrawPos, null, Color.Black * deathLerp, Main.rand.NextFloat(-2, 2), b2Texture.Size() * 0.5f, bScale2 * Projectile.scale * deathLerp * 0.8f * rot2, SpriteEffects.None, 0);

            for (int i = 0; i < 2; i++)
            {
                float subsine = rot2;
                float rot = (MathHelper.TwoPi * i / 3f) + Main.GlobalTimeWrappedHourly;
                Main.EntitySpriteDraw(vTexture, baseDrawPos, null, drawColor with { A = 0 } * 0.08f * deathLerp, rot + MathHelper.ToRadians(-105), vTexture.Size() * 0.5f, new Vector2(1f, 0.96f) * 0.16f * deathLerp * fullChargeMult, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(vTexture, baseDrawPos, null, drawColor with { A = 0 } * 0.08f * deathLerp, rot * 2 + MathHelper.ToRadians(-105), vTexture.Size() * 0.5f, new Vector2(1f, 0.96f) * 0.157f * deathLerp * fullChargeMult, SpriteEffects.None, 0);
            }
            return false;
        }
        public override bool? CanDamage() => false;
    }
}
