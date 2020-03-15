using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class SiriusMinion : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sirius");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 50;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 1f, 0.5f, 5f);
            bool flag64 = projectile.type == ModContent.ProjectileType<SiriusMinion>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            player.AddBuff(ModContent.BuffType<SiriusBuff>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.sirius = false;
                }
                if (modPlayer.sirius)
                {
                    projectile.timeLeft = 2;
                }
            }
            projectile.minionSlots = projectile.ai[0];
            projectile.position.X = player.Center.X - (float)(projectile.width / 2);
            projectile.position.Y = player.Center.Y - (float)(projectile.height / 2) + player.gfxOffY - 60f;
            if (player.gravDir == -1f)
            {
                projectile.position.Y = projectile.position.Y + 150f;
                projectile.rotation = 3.14f;
            }
            else
            {
                projectile.rotation = 0f;
            }
            projectile.position.X = (float)(int)projectile.position.X;
            projectile.position.Y = (float)(int)projectile.position.Y;
            float num395 = (float)Main.mouseTextColor / 200f - 0.35f;
            num395 *= 0.2f;
            projectile.scale = num395 + 0.95f;
            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = (player.allDamage + player.minionDamage - 1f);
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num501 = 50;
                for (int num502 = 0; num502 < num501; num502++)
                {
                    int num503 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 20, 0f, 0f, 0, default, 1f);
                    Main.dust[num503].velocity *= 2f;
                    Main.dust[num503].scale *= 1.15f;
                }
                projectile.localAI[0] += 1f;
            }
            if ((player.allDamage + player.minionDamage - 1f) != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    (player.allDamage + player.minionDamage - 1f));
                projectile.damage = damage2;
            }
            if (projectile.owner == Main.myPlayer)
            {
                if (projectile.ai[1] != 0f)
                {
                    projectile.ai[1] -= 1f;
                    return;
                }
                float num396 = projectile.position.X;
                float num397 = projectile.position.Y;
                float minDistance = 7000f;
                bool flag11 = false;
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float num539 = npc.position.X + (float)(npc.width / 2);
                        float num540 = npc.position.Y + (float)(npc.height / 2);
                        float num541 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num539) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num540);
                        if (num541 < minDistance)
                        {
                            num396 = num539;
                            num397 = num540;
                            flag11 = true;
                        }
                    }
                }
                else
                {
                    for (int num399 = 0; num399 < Main.npc.Length; num399++)
                    {
                        if (Main.npc[num399].CanBeChasedBy(projectile, false))
                        {
                            float num400 = Main.npc[num399].position.X + (float)(Main.npc[num399].width / 2);
                            float num401 = Main.npc[num399].position.Y + (float)(Main.npc[num399].height / 2);
                            float num402 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num400) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num401);
                            if (num402 < minDistance)
                            {
                                minDistance = num402;
                                num396 = num400;
                                num397 = num401;
                                flag11 = true;
                            }
                        }
                    }
                }
                if (flag11)
                {
                    float num403 = 15f;
                    Vector2 vector29 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num404 = num396 - vector29.X;
                    float num405 = num397 - vector29.Y;
                    float num406 = (float)Math.Sqrt((double)(num404 * num404 + num405 * num405));
                    num406 = num403 / num406;
                    num404 *= num406;
                    num405 *= num406;
					float damageMult = ((float)Math.Log(projectile.ai[0], MathHelper.E)) + 1f;
                    int beam = Projectile.NewProjectile(projectile.Center.X - 4f, projectile.Center.Y, num404, num405, ModContent.ProjectileType<SiriusBeam>(), (int)(projectile.damage * damageMult), projectile.knockBack, projectile.owner);
					Main.projectile[beam].penetrate = (int)projectile.ai[0];
                    projectile.ai[1] = 30f;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 200);
        }

        public override bool CanDamage()
        {
            return false;
        }
    }
}
