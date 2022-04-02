using CalamityMod.Buffs.Summon;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class AstrageldonSummon : ModProjectile
    {
        public bool dust = false;
        private int attackCounter = 1;
        private int teleportCounter = 400;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astrageldon");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 64;
            projectile.height = 62;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 1;
            projectile.alpha = 75;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.aiStyle = 26;
            aiType = ProjectileID.BabySlime;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            //for platform collision?
            fallThrough = false;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];;
            CalamityPlayer modPlayer = player.Calamity();
            CalamityGlobalProjectile modProj = projectile.Calamity();

            //hitbox size scaling
            float scale = (float)Math.Log(projectile.minionSlots, 10f) + 1f;
            if (projectile.scale != scale)
                projectile.scale = scale;
            projectile.width = (int)(64f * projectile.scale);
            projectile.height = (int)(62f * projectile.scale);

            //on spawn effects and flexible minions
            if (!dust)
            {
                modProj.spawnedPlayerMinionDamageValue = player.MinionDamage();
                modProj.spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int dustAmt = 16;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(dustIndex - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int dusty = Dust.NewDust(vector6 + vector7, 0, 0, ModContent.DustType<AstralOrange>(), vector7.X * 1f, vector7.Y * 1f, 100, default, 1.1f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].noLight = true;
                    Main.dust[dusty].velocity = vector7;
                }

                dust = true;
            }
            if (player.MinionDamage() != modProj.spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)modProj.spawnedPlayerMinionProjectileDamageValue /
                    modProj.spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }

            //Bool setup
            bool flag64 = projectile.type == ModContent.ProjectileType<AstrageldonSummon>();
            player.AddBuff(ModContent.BuffType<AstrageldonBuff>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.aSlime = false;
                }
                if (modPlayer.aSlime)
                {
                    projectile.timeLeft = 2;
                }
            }

            if (projectile.frame == 0 || projectile.frame == 1)
            {
                float mindistance = 1000f;
                float longdistance = 2000f;
                float longestdistance = 3000f;
                Vector2 objectivepos = projectile.position;
                bool gotoenemy = false;
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        bool lineOfSight = Collision.CanHit(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
                        float disttoobjective = Vector2.Distance(npc.Center, projectile.Center);
                        if ((!gotoenemy && disttoobjective < mindistance) && lineOfSight)
                        {
                            
                            mindistance = disttoobjective;
                            objectivepos = npc.Center;
                            gotoenemy = true;
                        }
                    }
                }
                else
                {
                    for (int num645 = 0; num645 < Main.npc.Length; num645++)
                    {
                        NPC npc = Main.npc[num645];
                        if (npc.CanBeChasedBy(projectile, false))
                        {
                            bool lineOfSight = Collision.CanHit(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
                            float disttoobjective = Vector2.Distance(npc.Center, projectile.Center);
                            if ((!gotoenemy && disttoobjective < mindistance) && lineOfSight)
                            {
                                
                                mindistance = disttoobjective;
                                objectivepos = npc.Center;
                                gotoenemy = true;
                            }
                        }
                    }
                }
                float maxdisttoenemy = longdistance;
                if (gotoenemy)
                {
                    maxdisttoenemy = longestdistance;
                }
                if (gotoenemy)
                {
                    float teleportRange = objectivepos.Length();
                    float scaleAddition = projectile.scale * 5f;
                    if (teleportCounter <= 0 && teleportRange >= 800f)
                    {
                        float num461 = 50f;
                        int num462 = 0;
                        while ((float)num462 < num461)
                        {
                            int dustType = Utils.SelectRandom(Main.rand, new int[]
                            {
                                ModContent.DustType<AstralBlue>(),
                                ModContent.DustType<AstralOrange>()
                            });
                            float num463 = (float)Main.rand.Next(-10, 11);
                            float num464 = (float)Main.rand.Next(-10, 11);
                            float num465 = (float)Main.rand.Next(3, 9);
                            float num466 = (float)Math.Sqrt((double)(num463 * num463 + num464 * num464));
                            num466 = num465 / num466;
                            num463 *= num466;
                            num464 *= num466;
                            int num467 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 2f);
                            Dust dust = Main.dust[num467];
                            dust.noGravity = true;
                            dust.position.X = projectile.Center.X;
                            dust.position.Y = projectile.Center.Y;
                            dust.position.X += (float)Main.rand.Next(-10, 11);
                            dust.position.Y += (float)Main.rand.Next(-10, 11);
                            dust.velocity.X = num463;
                            dust.velocity.Y = num464;
                            num462++;
                        }
                        projectile.position.X = objectivepos.X - (float)(projectile.width / 2) + Main.rand.NextFloat(-100f, 100f);
                        projectile.position.Y = objectivepos.Y - (float)(projectile.height / 2) - Main.rand.NextFloat(0f + scaleAddition, 200f + scaleAddition);
                        projectile.netUpdate = true;
                        teleportCounter = 600;
                    }
                    if (teleportCounter > 0)
                        teleportCounter -= Main.rand.Next(1, 4);
                }

                if (attackCounter > 0)
                {
                    attackCounter += Main.rand.Next(1, 4);
                }
                if (attackCounter > 300)
                {
                    attackCounter = 0;
                    projectile.netUpdate = true;
                }
                float scaleFactor3 = 6f;
                int projType = ModContent.ProjectileType<AstrageldonLaser>();
                if (gotoenemy && attackCounter == 0)
                {
                    attackCounter += 2;
                    if (Main.myPlayer == projectile.owner)
                    {
                        Vector2 laserVel = objectivepos - projectile.Center;
                        laserVel.Normalize();
                        laserVel *= scaleFactor3;
                        int laser = Projectile.NewProjectile(projectile.Center, laserVel, projType, projectile.damage, 0f, projectile.owner);
                        projectile.netUpdate = true;
                    }
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int height = texture.Height / Main.projFrames[projectile.type];
            int y6 = height * projectile.frame;
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture.Width / 2f, (float)height / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
