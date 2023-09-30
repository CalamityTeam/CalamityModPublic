using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class NorfleetComet : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Melee/GalacticaComet";

        private int noTileHitCounter = 120;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            int randomToSubtract = Main.rand.Next(1, 4);
            noTileHitCounter -= randomToSubtract;
            if (noTileHitCounter == 0)
            {
                Projectile.tileCollide = true;
            }
            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                {
                    SoundEngine.PlaySound(SoundID.Item9, Projectile.position);
                }
            }
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] == 18f)
            {
                Projectile.localAI[0] = 0f;
                for (int l = 0; l < 12; l++)
                {
                    Vector2 dustOffset = Vector2.UnitX * (float)-(float)Projectile.width / 2f;
                    dustOffset += -Vector2.UnitY.RotatedBy((double)((float)l * MathHelper.Pi / 6f), default) * new Vector2(8f, 16f);
                    dustOffset = dustOffset.RotatedBy((double)(Projectile.rotation - MathHelper.PiOver2), default);
                    int idx = Dust.NewDust(Projectile.Center, 0, 0, Main.rand.NextBool() ? 221 : 244, 0f, 0f, 160, default, 1f);
                    Main.dust[idx].scale = 1.1f;
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].position = Projectile.Center + dustOffset;
                    Main.dust[idx].velocity = Projectile.velocity * 0.1f;
                    Main.dust[idx].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[idx].position) * 1.25f;
                }
            }
            Projectile.alpha -= 15;
            int alphaLimit = 150;
            if (Projectile.alpha < alphaLimit)
            {
                Projectile.alpha = alphaLimit;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (Main.rand.NextBool(16))
            {
                Vector2 dustOffset = Vector2.UnitX.RotatedByRandom(Math.PI * 0.5).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? 221 : 244, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default, 1.2f);
                Main.dust[idx].velocity = dustOffset * 0.66f;
                Main.dust[idx].position = Projectile.Center + dustOffset * 12f;
            }
            if (Projectile.ai[1] == 1f)
            {
                Projectile.light = 0.5f;
                if (Main.rand.NextBool(10))
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? 221 : 244, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default, 1.2f);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, 100, 255, Projectile.alpha);
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<NorfleetExplosion>(), (int)(Projectile.damage * 0.3), Projectile.knockBack * 0.1f, Projectile.owner);
            }
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
            Projectile.ExpandHitboxBy(144);
            for (int d = 0; d < 4; d++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? 221 : 244, 0f, 0f, 50, default, 1.5f);
            }
            for (int d = 0; d < 20; d++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? 221 : 244, 0f, 0f, 0, default, 2.5f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 3f;
                idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, Main.rand.NextBool() ? 221 : 244, 0f, 0f, 50, default, 1.5f);
                Main.dust[idx].velocity *= 2f;
                Main.dust[idx].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft >= 600)
                return false;
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }
    }
}
