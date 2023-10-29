using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Boss
{
    public class ApolloRocket : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Boss";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.Calamity().DealsDefenseDamage = true;
            Projectile.width = 28;
            Projectile.height = 28;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            CooldownSlot = ImmunityCooldownID.Bosses;
            Projectile.timeLeft = 600;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
        }

        public override void AI()
        {
            if (Projectile.position.Y > Projectile.ai[1])
                Projectile.tileCollide = true;

            // Animation
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame > 4)
                Projectile.frame = 0;

            // Rotation
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + MathHelper.PiOver2;

            // Spawn effects
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;

                float randDustSpeed1 = 1.8f;
                float randDustSpeed2 = 2.8f;
                float angleRandom = 0.35f;

                for (int i = 0; i < 20; i++)
                {
                    float dustSpeed = Main.rand.NextFloat(randDustSpeed1, randDustSpeed2);
                    Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                    dustVel = dustVel.RotatedBy(-angleRandom);
                    dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);
                    int randomDustType = Main.rand.NextBool() ? 107 : 110;

                    int plasmaDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 200, default, 1.7f);
                    Main.dust[plasmaDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                    Main.dust[plasmaDust].noGravity = true;

                    Dust dust = Main.dust[plasmaDust];
                    dust.velocity *= 3f;
                    dust = Main.dust[plasmaDust];

                    plasmaDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 100, default, 0.8f);
                    Main.dust[plasmaDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;

                    dust = Main.dust[plasmaDust];
                    dust.velocity *= 2f;

                    Main.dust[plasmaDust].noGravity = true;
                    Main.dust[plasmaDust].fadeIn = 1f;
                    Main.dust[plasmaDust].color = Color.Green * 0.5f;

                    dust = Main.dust[plasmaDust];
                }
                for (int j = 0; j < 10; j++)
                {
                    float dustSpeed = Main.rand.NextFloat(randDustSpeed1, randDustSpeed2);
                    Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                    dustVel = dustVel.RotatedBy(-angleRandom);
                    dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);
                    int randomDustType = Main.rand.NextBool() ? 107 : 110;

                    int plasmaDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 0, default, 2f);
                    Main.dust[plasmaDust2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 3f;
                    Main.dust[plasmaDust2].noGravity = true;

                    Dust dust = Main.dust[plasmaDust2];
                    dust.velocity *= 0.5f;
                    dust = Main.dust[plasmaDust2];
                }
            }

            // Difficulty modes
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            // Light
            Lighting.AddLight(Projectile.Center, 0.2f, 1f, 0f);

            // Get a target and calculate distance from it
            int target = Player.FindClosest(Projectile.Center, 1, 1);
            Vector2 distanceFromTarget = Main.player[target].Center - Projectile.Center;

            // Set AI to stop homing, start accelerating
            float stopHomingDistance = bossRush ? 120f : death ? 160f : revenge ? 180f : expertMode ? 200f : 240f;
            if (distanceFromTarget.Length() < stopHomingDistance || Projectile.ai[0] == 1f || Projectile.timeLeft < 480)
            {
                Projectile.ai[0] = 1f;

                if (Projectile.velocity.Length() < 24f)
                    Projectile.velocity *= 1.05f;

                return;
            }

            // Home in on target
            float scaleFactor = Projectile.velocity.Length();
            float inertia = bossRush ? 6f : death ? 8f : revenge ? 9f : expertMode ? 10f : 12f;
            distanceFromTarget.Normalize();
            distanceFromTarget *= scaleFactor;
            Projectile.velocity = (Projectile.velocity * inertia + distanceFromTarget) / (inertia + 1f);
            Projectile.velocity.Normalize();
            Projectile.velocity *= scaleFactor;

            // Fly away from other rockets
            float pushForce = bossRush ? 0.07f : death ? 0.06f : revenge ? 0.055f : expertMode ? 0.05f : 0.04f;
            float pushDistance = bossRush ? 120f : death ? 100f : revenge ? 90f : expertMode ? 80f : 60f;
            for (int k = 0; k < Main.maxProjectiles; k++)
            {
                Projectile otherProj = Main.projectile[k];
                // Short circuits to make the loop as fast as possible
                if (!otherProj.active || k == Projectile.whoAmI)
                    continue;

                // If the other projectile is indeed the same owned by the same player and they're too close, nudge them away.
                bool sameProjType = otherProj.type == Projectile.type;
                float taxicabDist = Vector2.Distance(Projectile.Center, otherProj.Center);
                if (sameProjType && taxicabDist < pushDistance)
                {
                    if (Projectile.position.X < otherProj.position.X)
                        Projectile.velocity.X -= pushForce;
                    else
                        Projectile.velocity.X += pushForce;

                    if (Projectile.position.Y < otherProj.position.Y)
                        Projectile.velocity.Y -= pushForce;
                    else
                        Projectile.velocity.Y += pushForce;
                }
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (info.Damage <= 0)
                return;

            target.AddBuff(BuffID.OnFire, 360);
            target.AddBuff(BuffID.CursedInferno, 180);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int height = texture.Height / Main.projFrames[Projectile.type];
            int drawStart = height * Projectile.frame;
            Vector2 origin = Projectile.Size / 2;
            Main.EntitySpriteDraw(ModContent.Request<Texture2D>("CalamityMod/Projectiles/Boss/ApolloRocketGlow").Value, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, drawStart, texture.Width, height)), Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
        }

        public override void OnKill(int timeLeft)
        {
            // Rocket explosion
            int height = 90;
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = height;
            Projectile.Center = Projectile.position;
            Projectile.Damage();

            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int i = 0; i < 12; i++)
            {
                int randomDustType = Main.rand.NextBool() ? 107 : 110;
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, randomDustType, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 3f;
                if (Main.rand.NextBool())
                {
                    Main.dust[dust].scale = 0.5f;
                    Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int j = 0; j < 15; j++)
            {
                int randomDustType = Main.rand.NextBool() ? 107 : 110;
                int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, randomDustType, 0f, 0f, 100, default, 3f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].velocity *= 5f;
                dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, randomDustType, 0f, 0f, 100, default, 2f);
                Main.dust[dust].velocity *= 2f;
            }

            if (Main.netMode != NetmodeID.Server)
            {
                Vector2 goreSource = Projectile.Center;
                int goreAmt = 3;
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
}
