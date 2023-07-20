using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Dusts;
using System;
using CalamityMod.NPCs.TownNPCs;
using Microsoft.Xna.Framework.Graphics;
using CalamityMod.Particles;
using System.IO;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Typeless;

namespace CalamityMod.Projectiles.Ranged
{
    public class StarmageddonStar : ModProjectile, ILocalizedModType
    {
        private bool start = true;

        public new string LocalizationCategory => "Projectiles.Ranged";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 10000;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 38;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 3600;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.localAI[0]);
            writer.Write(start);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.localAI[0] = reader.ReadSingle();
            start = reader.ReadBoolean();
        }

        public override void AI()
        {
            Projectile hostProjectile = Main.projectile[(int)Projectile.ai[0]];
            if (Projectile.type != ModContent.ProjectileType<StarmageddonStar>() || !hostProjectile.active || hostProjectile.type != ModContent.ProjectileType<StarmageddonBinaryStarCenter>())
            {
                Projectile.Kill();
                return;
            }

            Lighting.AddLight(Projectile.Center, 0.5f, 0.4f, 0.05f);

            bool hostProjectileAttachedToEnemy = hostProjectile.ai[1] == 1f;

            if (start)
            {
                Projectile.ai[2] = Projectile.ai[1];
                start = false;
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;

            double deg = Projectile.ai[2];
            double rad = deg * (Math.PI / 180);
            double dist = StarmageddonBinaryStarCenter.StarDistanceFromCenter;
            Projectile.position.X = hostProjectile.Center.X - (int)(Math.Cos(rad) * dist) - Projectile.width / 2;
            Projectile.position.Y = hostProjectile.Center.Y - (int)(Math.Sin(rad) * dist) - Projectile.height / 2;
            Projectile.ai[2] += StarmageddonBinaryStarCenter.StarRotationRate;

            if (hostProjectileAttachedToEnemy)
            {
                Projectile.localAI[0] += 1f;
                for (int i = 0; i < StarmageddonBinaryStarCenter.ParticleStreamsPerStar; i++)
                {
                    bool top = i == 0;
                    if (Projectile.localAI[0] % StarmageddonBinaryStarCenter.ParticleSpawnRate == 0f)
                    {
                        int numParticles = Main.rand.Next(2, 4);
                        for (int j = 0; j < numParticles; j++)
                        {
                            float dustVelocityX = Main.rand.NextFloat(-StarmageddonBinaryStarCenter.ParticleSpreadMax, StarmageddonBinaryStarCenter.ParticleSpreadMax);
                            float dustVelocityY = top ? -StarmageddonBinaryStarCenter.ParticleVelocityMax : StarmageddonBinaryStarCenter.ParticleVelocityMax;
                            Vector2 dustVelocity = new Vector2(dustVelocityX, dustVelocityY);
                            GeneralParticleHandler.SpawnParticle(new SparkParticle(Projectile.Center + Vector2.Normalize(dustVelocity) * StarmageddonBinaryStarCenter.ParticleSpawnOffset, dustVelocity, false, 180, 1f, Color.Orange));
                        }
                    }

                    if (Projectile.localAI[0] % StarmageddonBinaryStarCenter.DustCloudSpawnRate == 0f)
                    {
                        float cloudVelocityX = Main.rand.NextFloat(-StarmageddonBinaryStarCenter.DustCloudSpreadMax, StarmageddonBinaryStarCenter.DustCloudSpreadMax);
                        float cloudVelocityY = top ? -StarmageddonBinaryStarCenter.DustCloudVelocityMax : StarmageddonBinaryStarCenter.DustCloudVelocityMax;
                        Vector2 cloudVelocity = new Vector2(cloudVelocityX, cloudVelocityY);
                        GeneralParticleHandler.SpawnParticle(new MediumMistParticle(Projectile.Center + Vector2.Normalize(cloudVelocity) * StarmageddonBinaryStarCenter.ParticleSpawnOffset, cloudVelocity, Color.Orange, Color.Red, 1f, 255));
                    }

                    if (Main.myPlayer == Projectile.owner)
                    {
                        if (Projectile.localAI[0] % StarmageddonBinaryStarCenter.SuckedProjectileSpawnRate == 0f)
                        {
                            float suckYDistanceMax = StarmageddonBinaryStarCenter.SuckedProjectileDistanceFromStars * 0.5f;
                            Vector2 position = Projectile.Center + new Vector2(StarmageddonBinaryStarCenter.SuckedProjectileDistanceFromStars * (top ? -1f : 1f), Main.rand.NextFloat(-suckYDistanceMax, suckYDistanceMax));
                            Vector2 speed = Vector2.Normalize(Projectile.Center - position) * 12f;

                            for (int l = 0; l < 12; l++)
                            {
                                Vector2 vector3 = Vector2.UnitX * (float)-(float)Projectile.width / 2f;
                                vector3 += -Vector2.UnitY.RotatedBy((double)((float)l * MathHelper.Pi / 6f), default) * new Vector2(8f, 16f);
                                vector3 = vector3.RotatedBy((double)(speed.ToRotation()), default);
                                int num9 = Dust.NewDust(position, 0, 0, 221, 0f, 0f, 0, default, 1f);
                                Main.dust[num9].noGravity = true;
                                Main.dust[num9].position = position + vector3;
                                Main.dust[num9].velocity = speed * 0.1f;
                                Main.dust[num9].velocity = Vector2.Normalize(position - speed * 3f - Main.dust[num9].position) * 1.25f;
                            }

                            int type = Utils.SelectRandom(Main.rand, new int[]
                            {
                                ModContent.ProjectileType<PlasmaBlast>(),
                                ModContent.ProjectileType<AstralStar>(),
                                ModContent.ProjectileType<GalacticaComet>(),
                                ProjectileID.StarCannonStar,
                                ProjectileID.Starfury
                            });

                            int star = Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, speed, type, Projectile.damage / 2, Projectile.knockBack * 0.5f, Projectile.owner);
                            if (star.WithinBounds(Main.maxProjectiles))
                            {
                                if (type == ModContent.ProjectileType<PlasmaBlast>() || type == ModContent.ProjectileType<AstralStar>() || type == ModContent.ProjectileType<GalacticaComet>())
                                    Main.projectile[star].ai[0] = 1f;

                                Main.projectile[star].extraUpdates += 4;
                                Main.projectile[star].penetrate = 1;
                                Main.projectile[star].timeLeft = 300;
                                Main.projectile[star].DamageType = DamageClass.Ranged;
                                Main.projectile[star].tileCollide = false;
                                Main.projectile[star].netUpdate = true;
                            }
                        }
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            int height = texture.Height / Main.projFrames[Projectile.type];
            int drawStart = height * Projectile.frame;
            Vector2 origin = Projectile.Size / 2;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, drawStart, texture.Width, height)), Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(176);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            for (int i = 0; i < 4; i++)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 31, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[dust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
            }

            for (int i = 0; i < 30; i++)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 200, default(Color), 3.7f);
                Main.dust[dust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                Main.dust[dust].noGravity = true;
                Dust dust2 = Main.dust[dust];
                dust2.velocity *= 3f;
                dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[dust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                dust2 = Main.dust[dust];
                dust2.velocity *= 2f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].fadeIn = 2.5f;
            }

            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 6, 0f, 0f, 0, default(Color), 2.7f);
                Main.dust[dust].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 2f;
                Main.dust[dust].noGravity = true;
                Dust dust2 = Main.dust[dust];
                dust2.velocity *= 3f;
            }

            for (int i = 0; i < 10; i++)
            {
                int dust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 31, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[dust].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 2f;
                Main.dust[dust].noGravity = true;
                Dust dust2 = Main.dust[dust];
                dust2.velocity *= 3f;
            }

            for (int i = 0; i < 2; i++)
            {
                int gore = Gore.NewGore(Projectile.GetSource_Death(), Projectile.position + new Vector2((float)(Projectile.width * Main.rand.Next(100)) / 100f, (float)(Projectile.height * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, default(Vector2), Main.rand.Next(61, 64));
                Main.gore[gore].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                Gore gore2 = Main.gore[gore];
                gore2.velocity *= 0.3f;
                Main.gore[gore].velocity.X += (float)Main.rand.Next(-10, 11) * 0.05f;
                Main.gore[gore].velocity.Y += (float)Main.rand.Next(-10, 11) * 0.05f;
            }
        }
    }
}
