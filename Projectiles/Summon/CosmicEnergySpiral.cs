using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class CosmicEnergySpiral : ModProjectile
    {
        private bool justSpawned = true;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cosmic Energy");
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 78;
            projectile.height = 78;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 10f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, (float)Main.DiscoR / 255f, (float)Main.DiscoG / 255f, (float)Main.DiscoB / 255f);
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }
            bool flag64 = projectile.type == ModContent.ProjectileType<CosmicEnergySpiral>();
            player.AddBuff(ModContent.BuffType<CosmicEnergy>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.cEnergy = false;
                }
                if (modPlayer.cEnergy)
                {
                    projectile.timeLeft = 2;
                }
            }
            float num633 = 1400f; //700
            float num634 = 1600f; //800
            float num635 = 2400f; //1200
            float num636 = 800f;
            projectile.rotation += projectile.velocity.X * 0.1f;
            Vector2 vector46 = projectile.position;
            bool flag25 = false;
            int target = 0;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float num646 = Vector2.Distance(npc.Center, projectile.Center);
                    if (!flag25 && num646 < num633)
                    {
                        vector46 = npc.Center;
                        flag25 = true;
                        target = npc.whoAmI;
                    }
                }
            }
            else
            {
                for (int num645 = 0; num645 < Main.maxNPCs; num645++)
                {
                    NPC nPC2 = Main.npc[num645];
                    if (nPC2.CanBeChasedBy(projectile, false))
                    {
                        float num646 = Vector2.Distance(nPC2.Center, projectile.Center);
                        if (!flag25 && num646 < num633)
                        {
                            num633 = num646;
                            vector46 = nPC2.Center;
                            flag25 = true;
                            target = num645;
                        }
                    }
                }
            }
            float num647 = num634;
            if (flag25)
            {
                num647 = num635;
            }
            if (Vector2.Distance(player.Center, projectile.Center) > num647)
            {
                projectile.ai[1] = 1f;
                projectile.netUpdate = true;
            }
            if (flag25 && projectile.ai[1] == 0f)
            {
                Vector2 vector47 = vector46 - projectile.Center;
                float num648 = vector47.Length();
                vector47.Normalize();
                if (num648 > 200f)
                {
                    float scaleFactor2 = 6f; //6
                    vector47 *= scaleFactor2;
                    projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
                }
                else
                {
                    float num649 = 4f; //4
                    vector47 *= -num649;
                    projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
                }
            }
            else
            {
                bool flag26 = false;
                if (!flag26)
                {
                    flag26 = projectile.ai[1] == 1f;
                }
                float num650 = 6f;
                if (flag26)
                {
                    num650 = 15f;
                }
                Vector2 center2 = projectile.Center;
                Vector2 vector48 = player.Center - center2 + new Vector2(0f, -60f);
                float num651 = vector48.Length();
                if (num651 > 200f && num650 < 8f)
                {
                    num650 = 8f;
                }
                if (num651 < num636 && flag26 && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    projectile.ai[1] = 0f;
                    projectile.netUpdate = true;
                }
                if (num651 > 2000f) //2000
                {
                    projectile.position.X = Main.player[projectile.owner].Center.X - (float)(projectile.width / 2);
                    projectile.position.Y = Main.player[projectile.owner].Center.Y - (float)(projectile.height / 2);
                    projectile.netUpdate = true;
                }
                if (num651 > 70f)
                {
                    vector48.Normalize();
                    vector48 *= num650;
                    projectile.velocity = (projectile.velocity * 40f + vector48) / 41f;
                }
                else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                {
                    projectile.velocity.X = -0.15f;
                    projectile.velocity.Y = -0.05f;
                }
            }
            float num395 = (float)Main.mouseTextColor / 200f - 0.35f;
            num395 *= 0.2f;
            projectile.scale = num395 + 0.95f;
            if (justSpawned)
            {
                justSpawned = false;
                projectile.ai[0] = 100f;
            }
            if (projectile.owner == Main.myPlayer)
            {
                if (projectile.ai[0] != 0f)
                {
                    projectile.ai[0] -= 1f;
                    return;
                }
                float num396 = projectile.position.X;
                float num397 = projectile.position.Y;
                float num398 = 1200f;
                bool flag11 = false;
                for (int num399 = 0; num399 < Main.maxNPCs; num399++)
                {
                    if (Main.npc[num399].CanBeChasedBy(projectile, false))
                    {
                        float num400 = Main.npc[num399].position.X + (float)(Main.npc[num399].width / 2);
                        float num401 = Main.npc[num399].position.Y + (float)(Main.npc[num399].height / 2);
                        float num402 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num400) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num401);
                        if (num402 < num398)
                        {
                            num398 = num402;
                            num396 = num400;
                            num397 = num401;
                            flag11 = true;
                        }
                    }
                }
                if (flag11)
                {
                    Main.PlaySound(SoundID.Item105, (int)projectile.position.X, (int)projectile.position.Y);
                    int blastAmt = Main.rand.Next(5, 8);
                    for (int b = 0; b < blastAmt; b++)
                    {
                        Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                        Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<CosmicBlast>(), (int)(projectile.damage * 0.5), 2f, projectile.owner, (float)target, 0f);
                    }
                    float speed = 15f;
                    float num404 = num396 - projectile.Center.X;
                    float num405 = num397 - projectile.Center.Y;
                    float num406 = (float)Math.Sqrt((double)(num404 * num404 + num405 * num405));
                    num406 = speed / num406;
                    num404 *= num406;
                    num405 *= num406;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num404, num405, ModContent.ProjectileType<CosmicBlastBig>(), projectile.damage, 3f, projectile.owner, (float)target, 0f);
                    projectile.ai[0] = 100f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 255);
        }

        public override bool CanDamage()
        {
            return false;
        }
    }
}
