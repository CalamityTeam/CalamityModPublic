using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class DreadmineTurret : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public float count = 0;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
            }
            if (Projectile.frame >= Main.projFrames[Projectile.type])
            {
                Projectile.frame = 0;
            }
            Projectile.velocity = new Vector2(0f, (float)Math.Sin((double)(6.28318548f * Projectile.ai[0] / 300f)) * 0.5f);
            Projectile.ai[0] += 1f;
            if (Projectile.ai[0] >= 300f)
            {
                Projectile.ai[0] = 0f;
                Projectile.netUpdate = true;
            }
            if (Main.rand.NextBool(15))
            {
                int mineAmt = 0;
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == ModContent.ProjectileType<Dreadmine>())
                    {
                        mineAmt++;
                    }
                }
                if (Main.rand.Next(15) >= mineAmt && mineAmt < 10)
                {
                    int spawnVariance = 24;
                    int moreSpawnVariance = 90;
                    for (int j = 0; j < 50; j++)
                    {
                        int randSpawn = Main.rand.Next(200 - j * 2, 400 + j * 2);
                        Vector2 center = Projectile.Center;
                        center.X += (float)Main.rand.Next(-randSpawn, randSpawn + 1);
                        center.Y += (float)Main.rand.Next(-randSpawn, randSpawn + 1);
                        if (!Collision.SolidCollision(center, spawnVariance, spawnVariance))
                        {
                            center.X += (float)(spawnVariance / 2);
                            center.Y += (float)(spawnVariance / 2);
                            if (Collision.CanHit(new Vector2(Projectile.Center.X, Projectile.position.Y), 1, 1, center, 1, 1) ||
                                Collision.CanHit(new Vector2(Projectile.Center.X, Projectile.position.Y - 50f), 1, 1, center, 1, 1))
                            {
                                int tileX = (int)center.X / 16;
                                int tileY = (int)center.Y / 16;
                                bool canSpawnMine = false;
                                if (Main.rand.NextBool(3) && Main.tile[tileX, tileY] != null && Main.tile[tileX, tileY].WallType > 0)
                                {
                                    canSpawnMine = true;
                                }
                                else
                                {
                                    center.X -= (float)(moreSpawnVariance / 2);
                                    center.Y -= (float)(moreSpawnVariance / 2);
                                    if (Collision.SolidCollision(center, moreSpawnVariance, moreSpawnVariance))
                                    {
                                        center.X += (float)(moreSpawnVariance / 2);
                                        center.Y += (float)(moreSpawnVariance / 2);
                                        canSpawnMine = true;
                                    }
                                }
                                if (canSpawnMine)
                                {
                                    for (int k = 0; k < Main.maxProjectiles; k++)
                                    {
                                        if (Main.projectile[k].active && Main.projectile[k].owner == Main.myPlayer &&
                                            Main.projectile[k].type == ModContent.ProjectileType<Dreadmine>() && (center - Main.projectile[k].Center).Length() < 48f)
                                        {
                                            canSpawnMine = false;
                                            break;
                                        }
                                    }
                                    if (canSpawnMine && Main.myPlayer == Projectile.owner)
                                    {
                                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), center, Vector2.Zero, ModContent.ProjectileType<Dreadmine>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public override bool? CanDamage() => false;
    }
}
