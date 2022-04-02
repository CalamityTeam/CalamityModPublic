using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class MiniGuardianDefense : ModProjectile
    {
        private int ai = 3;
        private void updateDamage(int type)
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            float baseDamage = (modPlayer.profanedCrystal && !modPlayer.profanedCrystalBuffs) ? 0f : (75f +
                        (modPlayer.profanedCrystalBuffs ? 420f : 0f));
            projectile.damage = baseDamage == 0 ? 0 : (int)(baseDamage * player.MinionDamage());
            ai = type;
            if (baseDamage >= 420f)
            {
                projectile.localNPCHitCooldown = 6;
            }
            else if (baseDamage == 0)
            {
                projectile.localNPCHitCooldown = 69;
            }
            else
            {
                projectile.localNPCHitCooldown = 9;
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(ai);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            ai = reader.ReadInt32();
        }

        private void AI(int type, float num535, float num536, Player player)
        {
            updateDamage(type);
            switch (ai)
            {
                case 1: //defensive bab (profaned soul artifact)
                case 2: //Empowered bab WEEEEEEEEEE (profaned soul crystal)
                    if (projectile.ai[1] <= -1f)
                    {
                        projectile.ai[1] = 17f;
                    }
                    if (projectile.ai[1] > 0f)
                    {
                        projectile.ai[1] -= type;
                    }
                    if (projectile.ai[1] == 0f)
                    {
                        float num550 = 24f; //12
                        Vector2 vector43 = projectile.Center;
                        float num551 = num535 - vector43.X;
                        float num552 = num536 - vector43.Y;
                        float num553 = (float)Math.Sqrt((double)(num551 * num551 + num552 * num552));
                        if (num553 < 100f)
                        {
                            num550 = 28f; //14
                        }
                        if (type == 2)
                            num550 *= 2f;
                        else if (player.Calamity().gOffense)
                            num550 *= 0.95f;
                        num553 = num550 / num553;
                        num551 *= num553;
                        num552 *= num553;
                        projectile.velocity.X = (projectile.velocity.X * (type == 1 ? 12f : 20f) + num551) / (type == 1 ? 13f : 21f);
                        projectile.velocity.Y = (projectile.velocity.Y * (type == 1 ? 12f : 20f) + num552) / (type == 1 ? 13f : 21f);
                    }
                    else
                    {
                        if (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y) < 10f)
                        {
                            projectile.velocity *= 1.05f;
                        }
                    }
                    break;
                case 3: //bored bab - (idle)
                    float num16 = 0.5f;
                    projectile.tileCollide = false;
                    int num17 = 100;
                    Vector2 vector3 = projectile.Center;
                    float num18 = player.Center.X - vector3.X;
                    float num19 = player.Center.Y - vector3.Y;
                    num19 += (float)Main.rand.Next(-10, 21);
                    num18 += (float)Main.rand.Next(-10, 21);
                    num18 += (float)(60 * -(float)player.direction);
                    num19 -= 60f;
                    float num20 = (float)Math.Sqrt((double)(num18 * num18 + num19 * num19));
                    float num21 = 18f;

                    if (num20 < (float)num17 && player.velocity.Y == 0f &&
                        projectile.position.Y + (float)projectile.height <= player.position.Y + (float)player.height &&
                        !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                    {
                        projectile.ai[0] = 0f;
                        if (projectile.velocity.Y < -6f)
                        {
                            projectile.velocity.Y = -6f;
                        }
                    }
                    if (num20 > 2000f)
                    {
                        projectile.position = player.position;
                        projectile.netUpdate = true;
                    }
                    if (num20 < 50f)
                    {
                        if (Math.Abs(projectile.velocity.X) > 2f || Math.Abs(projectile.velocity.Y) > 2f)
                        {
                            projectile.velocity *= 0.90f;
                        }
                        num16 = 0.01f;
                    }
                    else
                    {
                        if (num20 < 100f)
                        {
                            num16 = 0.1f;
                        }
                        if (num20 > 300f)
                        {
                            num16 = 1f;
                        }
                        num20 = num21 / num20;
                        num18 *= num20;
                        num19 *= num20;
                    }

                    if (projectile.velocity.X < num18)
                    {
                        projectile.velocity.X += num16;
                        if (num16 > 0.05f && projectile.velocity.X < 0f)
                        {
                            projectile.velocity.X += num16;
                        }
                    }
                    if (projectile.velocity.X > num18)
                    {
                        projectile.velocity.X -= num16;
                        if (num16 > 0.05f && projectile.velocity.X > 0f)
                        {
                            projectile.velocity.X -= num16;
                        }
                    }
                    if (projectile.velocity.Y < num19)
                    {
                        projectile.velocity.Y += num16;
                        if (num16 > 0.05f && projectile.velocity.Y < 0f)
                        {
                            projectile.velocity.Y += num16 * 2f;
                        }
                    }
                    if (projectile.velocity.Y > num19)
                    {
                        projectile.velocity.Y -= num16;
                        if (num16 > 0.05f && projectile.velocity.Y > 0f)
                        {
                            projectile.velocity.Y -= num16 * 2f;
                        }
                    }
                    break;
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Defensive Guardian");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.tileCollide = false;
            projectile.width = 62;
            projectile.height = 80;
            projectile.minionSlots = 0f;
            projectile.minion = true;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 18000;
            projectile.timeLeft *= 5;
            projectile.usesLocalNPCImmunity = true;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override bool? CanCutTiles()
        {
            if (projectile.damage == 0)
                return false;
            return null;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (Main.player[projectile.owner].Calamity().profanedCrystalBuffs && !Main.player[projectile.owner].Calamity().endoCooper)
            { 
                CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

            if (player.dead)
            {
                modPlayer.gDefense = false;
            }
            if (modPlayer.gDefense)
            {
                projectile.timeLeft = 2;
            }
            if (!modPlayer.pArtifact || (modPlayer.minionSlotStat < 10 && !modPlayer.profanedCrystal))
            {
                modPlayer.gDefense = false;
                projectile.active = false;
                return;
            }
            projectile.MinionAntiClump();
            float num535 = projectile.position.X;
            float num536 = projectile.position.Y;
            float num537 = 3000f;
            bool flag19 = false;
            NPC ownerMinionAttackTargetNPC2 = projectile.OwnerMinionAttackTargetNPC;
            if (ownerMinionAttackTargetNPC2 != null && ownerMinionAttackTargetNPC2.CanBeChasedBy(projectile, false))
            {
                float num539 = ownerMinionAttackTargetNPC2.Center.X;
                float num540 = ownerMinionAttackTargetNPC2.Center.Y;
                float num541 = Math.Abs(projectile.Center.X - num539) + Math.Abs(projectile.Center.Y - num540);
                if (num541 < num537)
                {
                    num537 = num541;
                    num535 = num539;
                    num536 = num540;
                    flag19 = true;
                }
            }
            if (!flag19)
            {
                int num3;
                for (int num542 = 0; num542 < Main.maxNPCs; num542 = num3 + 1)
                {
                    if (Main.npc[num542].CanBeChasedBy(projectile, false))
                    {
                        float num543 = Main.npc[num542].Center.X;
                        float num544 = Main.npc[num542].Center.Y;
                        float num545 = Math.Abs(projectile.Center.X - num543) + Math.Abs(projectile.Center.Y - num544);
                        if (num545 < num537)
                        {
                            num537 = num545;
                            num535 = num543;
                            num536 = num544;
                            flag19 = true;
                        }
                    }
                    num3 = num542;
                }
            }
            if (!flag19 || projectile.damage == 0)
            {
                AI(3, num535, num536, player);
            }
            else
            {
                if (player.Calamity().profanedCrystalBuffs)
                    AI(2, num535, num536, player);
                else
                    AI(1, num535, num536, player);
            }
            if (projectile.velocity.X > 0.25f)
            {
                projectile.direction = -1;
            }
            else if (projectile.velocity.X < -0.25f)
            {
                projectile.direction = 1;
            }

            if (Math.Abs(projectile.velocity.X) > 0.2f)
            {
                projectile.spriteDirection = -projectile.direction;
            }

            projectile.frameCounter++;
            if (projectile.frameCounter > 5)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
                if (projectile.frame % 2 == 0)
                    projectile.netUpdate = true;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (Main.player[projectile.owner].Calamity().angelicAlliance)
                target.AddBuff(ModContent.BuffType<BanishingFire>(), 300);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            if (Main.player[projectile.owner].Calamity().angelicAlliance)
                target.AddBuff(ModContent.BuffType<BanishingFire>(), 300);
        }
    }
}
