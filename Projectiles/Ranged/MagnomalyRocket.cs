using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Ranged
{
    public class MagnomalyRocket : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        private bool spawnedAura = false;
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            //Lighting
            Lighting.AddLight(Projectile.Center, Main.DiscoR * 0.25f / 255f, Main.DiscoG * 0.25f / 255f, Main.DiscoB * 0.25f / 255f);

            //Animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 7)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }

            //Rotation
            Projectile.spriteDirection = Projectile.direction = (Projectile.velocity.X > 0).ToDirectionInt();
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? 0f : MathHelper.Pi) + MathHelper.ToRadians(90) * Projectile.direction;

            int dustType = Main.rand.NextBool() ? 107 : 234;
            if (Main.rand.NextBool(4))
            {
                dustType = 269;
            }
            if (Projectile.owner == Main.myPlayer && !spawnedAura)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MagnomalyAura>(), (int)(Projectile.damage * 0.5f), Projectile.knockBack * 0.5f, Projectile.owner, Projectile.identity, 0f);
                spawnedAura = true;
            }
            float dustOffsetX = Projectile.velocity.X * 0.5f;
            float dustOffsetY = Projectile.velocity.Y * 0.5f;
            if (Main.rand.NextBool())
            {
                int exo = Dust.NewDust(new Vector2(Projectile.position.X + 3f + dustOffsetX, Projectile.position.Y + 3f + dustOffsetY) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, dustType, 0f, 0f, 100, default, 0.5f);
                Main.dust[exo].scale *= (float)Main.rand.Next(10) * 0.1f;
                Main.dust[exo].velocity *= 0.2f;
                Main.dust[exo].noGravity = true;
                Main.dust[exo].noLight = true;
            }
            else
            {
                int exo = Dust.NewDust(new Vector2(Projectile.position.X + 3f + dustOffsetX, Projectile.position.Y + 3f + dustOffsetY) - Projectile.velocity * 0.5f, Projectile.width - 8, Projectile.height - 8, dustType, 0f, 0f, 100, default, 0.25f);
                Main.dust[exo].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[exo].velocity *= 0.05f;
                Main.dust[exo].noGravity = true;
                Main.dust[exo].noLight = true;
            }
            CalamityUtils.HomeInOnNPC(Projectile, true, 300f, 12f, 20f);
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Projectile.ExpandHitboxBy(192);
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                //DO NOT REMOVE THIS PROJECTILE
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<MagnomalyExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);

                int dustType = Main.rand.NextBool() ? 107 : 234;
                if (Main.rand.NextBool(4))
                {
                    dustType = 269;
                }
                for (int d = 0; d < 30; d++)
                {
                    int exo = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 1f);
                    Main.dust[exo].velocity *= 3f;
                    Main.dust[exo].noGravity = true;
                    Main.dust[exo].noLight = true;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[exo].scale = 0.5f;
                        Main.dust[exo].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int d = 0; d < 40; d++)
                {
                    int exo = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 0.5f);
                    Main.dust[exo].noGravity = true;
                    Main.dust[exo].noLight = true;
                    Main.dust[exo].velocity *= 5f;
                    exo = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 0.75f);
                    Main.dust[exo].velocity *= 2f;
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    Vector2 goreSource = Projectile.Center;
                    int goreAmt = 9;
                    Vector2 source = new Vector2(goreSource.X - 24f, goreSource.Y - 24f);
                    for (int goreIndex = 0; goreIndex < goreAmt; goreIndex++)
                    {
                        float velocityMult = 0.33f;
                        if (goreIndex < (goreAmt / 3))
                        {
                            velocityMult = 0.66f;
                        }
                        if (goreIndex >= (2 * goreAmt / 3))
                        {
                            velocityMult = 1f;
                        }
                        Mod mod = ModContent.GetInstance<CalamityMod>();
                        int type = Main.rand.Next(61, 64);
                        int smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                        Gore gore = Main.gore[smoke];
                        gore.velocity *= velocityMult;
                        gore.velocity.X += 1f;
                        gore.velocity.Y += 1f;
                        type = Main.rand.Next(61, 64);
                        smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                        gore = Main.gore[smoke];
                        gore.velocity *= velocityMult;
                        gore.velocity.X -= 1f;
                        gore.velocity.Y += 1f;
                        type = Main.rand.Next(61, 64);
                        smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                        gore = Main.gore[smoke];
                        gore.velocity *= velocityMult;
                        gore.velocity.X += 1f;
                        gore.velocity.Y -= 1f;
                        type = Main.rand.Next(61, 64);
                        smoke = Gore.NewGore(Projectile.GetSource_Death(), source, default, type, 1f);
                        gore = Main.gore[smoke];
                        gore.velocity *= velocityMult;
                        gore.velocity.X -= 1f;
                        gore.velocity.Y -= 1f;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            OnHitEffects();
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            OnHitEffects();
            target.AddBuff(ModContent.BuffType<MiracleBlight>(), 300);
        }

        private void OnHitEffects()
        {
            if (Projectile.owner == Main.myPlayer)
            {
                float random = Main.rand.Next(30, 90);
                float spread = random * 0.0174f;
                double startAngle = Math.Atan2(Projectile.velocity.X, Projectile.velocity.Y) - spread / 2;
                double deltaAngle = spread / 8f;
                for (int i = 0; i < 4; i++)
                {
                    double offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                    int proj1 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<MagnomalyBeam>(), Projectile.damage / 4, Projectile.knockBack / 4, Projectile.owner, 0f, 1f);
                    int proj2 = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), ModContent.ProjectileType<MagnomalyBeam>(), Projectile.damage / 4, Projectile.knockBack / 4, Projectile.owner, 0f, 1f);
                }
            }
        }
    }
}
