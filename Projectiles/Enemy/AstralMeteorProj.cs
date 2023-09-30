using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;

namespace CalamityMod.Projectiles.Enemy
{
    public class AstralMeteorProj : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Enemy";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 360;
            Projectile.Opacity = 0f;
        }

        public override void AI()
        {
            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + 0.1f, 0f, 1f);
            Projectile.rotation += Projectile.velocity.X * 0.04f;

            // Release astal sparks.
            int dustID = ModContent.DustType<AstralOrange>();
            Dust astralParticle = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustID, 0f, 0f, 100, default, 1.2f);
            astralParticle.velocity = Main.rand.NextVector2Circular(4f, 4f);
            astralParticle.noGravity = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath14, Projectile.position);
            Projectile.ExpandHitboxBy(60);

            for (int i = 0; i < 15; i++)
            {
                int dustID = Main.rand.NextBool(3) ? ModContent.DustType<AstralOrange>() : ModContent.DustType<AstralBlue>();
                Dust astralParticle = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, dustID, 0f, 0f, 100, default, 1.2f);
                astralParticle.velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    astralParticle.scale = 0.5f;
                    astralParticle.fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }

            if (Main.netMode != NetmodeID.Server)
            {
                int goreCount = 3;
                Vector2 goreSource = Projectile.Center;
                Vector2 source = new Vector2(goreSource.X - 24f, goreSource.Y - 24f);
                for (int goreIndex = 0; goreIndex < goreCount; goreIndex++)
                {
                    float smokeSpeed = 0.33f;
                    if (goreIndex < (goreCount / 3))
                        smokeSpeed = 0.66f;

                    if (goreIndex >= (2 * goreCount / 3))
                        smokeSpeed = 1f;

                    int goreID = Main.rand.Next(61, 64);
                    int smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, goreID, 1f);
                    Gore gore = Main.gore[smoke];
                    gore.velocity *= smokeSpeed;
                    gore.velocity.X += 1f;
                    gore.velocity.Y += 1f;
                    goreID = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, goreID, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= smokeSpeed;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y += 1f;
                    goreID = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, goreID, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= smokeSpeed;
                    gore.velocity.X += 1f;
                    gore.velocity.Y -= 1f;
                    goreID = Main.rand.Next(61, 64);
                    smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, goreID, 1f);
                    gore = Main.gore[smoke];
                    gore.velocity *= smokeSpeed;
                    gore.velocity.X -= 1f;
                    gore.velocity.Y -= 1f;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 45);
		}
    }
}
