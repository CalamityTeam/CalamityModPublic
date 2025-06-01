using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class AstralStarMagic : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/Typeless/AstralStar";

        private const int NoTileCollideTime = 30;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ProjectileID.Sets.CultistIsResistantTo[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Projectile.ai[1] += 1f;
            if (Projectile.ai[1] >= NoTileCollideTime)
                Projectile.tileCollide = true;

            if (Projectile.soundDelay == 0)
            {
                Projectile.soundDelay = 20 + Main.rand.Next(40);
                if (Main.rand.NextBool(5))
                    SoundEngine.PlaySound(SoundID.Item9, Projectile.Center);
            }

            Projectile.rotation += (Math.Abs(Projectile.velocity.X) + Math.Abs(Projectile.velocity.Y)) * 0.01f * (float)Projectile.direction;

            if (Main.rand.NextBool(8))
            {
                int spawnDustAmount = 2;
                for (int i = 0; i < spawnDustAmount; i++)
                {
                    Color newColor = Main.hslToRgb(0.5f, 1f, 0.5f);
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2, 0f, 0f, 0, newColor);
                    Main.dust[dust].position = Projectile.Center + Main.rand.NextVector2Circular((float)Projectile.width, (float)Projectile.height) * 0.5f;
                    Main.dust[dust].velocity *= Main.rand.NextFloat() * 0.8f;
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].fadeIn = 0.6f + Main.rand.NextFloat();
                    Main.dust[dust].velocity += Projectile.velocity.SafeNormalize(Vector2.UnitY) * 3f;
                    Main.dust[dust].scale = 0.7f;
                    if (dust != Main.maxDust)
                    {
                        Dust dust2 = Dust.CloneDust(dust);
                        dust2.scale /= 2f;
                        dust2.fadeIn *= 0.85f;
                        dust2.color = new Color(255, 255, 255, 255);
                    }
                }

                Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.PiOver2).RotatedBy((double)Projectile.velocity.ToRotation(), default);
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150);
                Main.dust[idx].velocity = velocity * 0.33f;
                Main.dust[idx].position = Projectile.Center + velocity * 6f;
            }

            if (Main.rand.NextBool(24) && Main.netMode != NetmodeID.Server)
            {
                int idx = Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity * 0.1f, 16, 1f);
                Main.gore[idx].velocity *= 0.66f;
                Main.gore[idx].velocity += Projectile.velocity * 0.15f;
            }

            Projectile.light = 0.9f;

            if (Main.rand.NextBool(5))
            {
                Color newColor = Main.hslToRgb(1f, 1f, 0.5f);
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2, 0f, 0f, 0, newColor);
                Main.dust[dust].position = Projectile.Center + Main.rand.NextVector2Circular((float)Projectile.width, (float)Projectile.height) * 0.5f;
                Main.dust[dust].velocity *= Main.rand.NextFloat() * 0.8f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].fadeIn = 0.6f + Main.rand.NextFloat();
                Main.dust[dust].velocity += Projectile.velocity * 0.25f;
                Main.dust[dust].scale = 0.7f;
                if (dust != Main.maxDust)
                {
                    Dust dust2 = Dust.CloneDust(dust);
                    dust2.scale /= 2f;
                    dust2.fadeIn *= 0.85f;
                    dust2.color = new Color(255, 255, 255, 255);
                }

                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150);
            }

            if (Main.rand.NextBool(10) && Main.netMode != NetmodeID.Server)
                Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.position, Projectile.velocity * 0.1f, Main.rand.Next(16, 18), 1f);

            if (Projectile.ai[0] == 1f)
                CalamityUtils.HomeInOnNPC(Projectile, Projectile.tileCollide, 500f, 15f, 20f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 180);

        public override Color? GetAlpha(Color lightColor) => new Color(200, 100, 250, Projectile.alpha);

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawStarTrail(Color.Coral, Color.White);
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            return true;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(50);

            int spawnDustAmount = 3;
            for (int i = 0; i < spawnDustAmount; i++)
            {
                Color newColor = Main.hslToRgb(1f, 1f, 0.5f);
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.RainbowMk2, 0f, 0f, 0, newColor);
                Main.dust[dust].position = Projectile.Center + Main.rand.NextVector2Circular((float)Projectile.width, (float)Projectile.height);
                Main.dust[dust].velocity *= Main.rand.NextFloat() * 2.4f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].fadeIn = 0.6f + Main.rand.NextFloat();
                Main.dust[dust].scale = 1.4f;
                if (dust != Main.maxDust)
                {
                    Dust dust2 = Dust.CloneDust(dust);
                    dust2.scale /= 2f;
                    dust2.fadeIn *= 0.85f;
                    dust2.color = new Color(255, 255, 255, 255);
                }
            }

            for (int i = 0; i < 3; i++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100);
                Main.dust[idx].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[idx].scale = 0.5f;
                    Main.dust[idx].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                int idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.5f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 5f;
                idx = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100);
                Main.dust[idx].velocity *= 2f;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                for (int i = 0; i < 3; i++)
                    Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Projectile.velocity * 0.05f, Main.rand.Next(16, 18), 1f);
            }
        }
    }
}
