using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class ProfanedEnergy : ModProjectile
    {
        public override string Texture => "CalamityMod/NPCs/NormalNPCs/ImpiousImmolator";

        private float count = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Profaned Energy");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 60;
            projectile.height = 60;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.sentry = true;
            projectile.timeLeft = Projectile.SentryLifeTime;
            projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            projectile.frameCounter++;
            if (projectile.frameCounter > 5)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame > 3)
            {
                projectile.frame = 0;
            }
            if (count == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                Main.PlaySound(SoundID.Item20, projectile.Center);
                for (int i = 0; i < 5; i++)
                {
                    int holy = Dust.NewDust(projectile.Center, projectile.width, projectile.height, (int)CalamityDusts.ProfanedFire, 0f, 0f, 100, default, 2f);
                    Main.dust[holy].velocity *= 3f;
                    Main.dust[holy].position = projectile.Center;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[holy].scale = 0.5f;
                        Main.dust[holy].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 10; j++)
                {
                    int fire = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 246, 0f, 0f, 100, default, 3f);
                    Main.dust[fire].noGravity = true;
                    Main.dust[fire].velocity *= 5f;
                    Main.dust[fire].position = projectile.Center;
                    fire = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 246, 0f, 0f, 100, default, 2f);
                    Main.dust[fire].velocity *= 2f;
                    Main.dust[fire].position = projectile.Center;
                }
                count += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }
            if (projectile.owner == Main.myPlayer)
            {
                if (projectile.ai[0] != 0f)
                {
                    projectile.ai[0] -= 1f;
                    return;
                }
                bool flag18 = false;
                float num506 = projectile.Center.X;
                float num507 = projectile.Center.Y;
                float num508 = 1000f;
                int target = 0;
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float num539 = npc.Center.X;
                        float num540 = npc.Center.Y;
                        float num541 = Math.Abs(projectile.Center.X - num539) + Math.Abs(projectile.Center.Y - num540);
                        if (num541 < num508 && Collision.CanHit(projectile.Center, projectile.width, projectile.height, npc.Center, npc.width, npc.height))
                        {
                            num506 = num539;
                            num507 = num540;
                            flag18 = true;
                            target = npc.whoAmI;
                        }
                    }
                }
                if (!flag18)
                {
                    for (int num512 = 0; num512 < Main.maxNPCs; num512++)
                    {
                        if (Main.npc[num512].CanBeChasedBy(projectile, false))
                        {
                            float num513 = Main.npc[num512].position.X + (float)(Main.npc[num512].width / 2);
                            float num514 = Main.npc[num512].position.Y + (float)(Main.npc[num512].height / 2);
                            float num515 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num513) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num514);
                            if (num515 < num508 && Collision.CanHit(projectile.position, projectile.width, projectile.height, Main.npc[num512].position, Main.npc[num512].width, Main.npc[num512].height))
                            {
                                num508 = num515;
                                num506 = num513;
                                num507 = num514;
                                flag18 = true;
                                target = num512;
                            }
                        }
                    }
                }
                if (flag18)
                {
                    float num516 = num506;
                    float num517 = num507;
                    num506 -= projectile.Center.X;
                    num507 -= projectile.Center.Y;
                    if (num506 < 0f)
                    {
                        projectile.spriteDirection = 1;
                    }
                    else
                    {
                        projectile.spriteDirection = -1;
                    }
                    int projectileType = Utils.SelectRandom(Main.rand, new int[]
                    {
                        ModContent.ProjectileType<FlameBlast>(),
                        ModContent.ProjectileType<FlameBurst>()
                    });
                    float speed = 25f;
                    Vector2 vector29 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num404 = num516 - vector29.X;
                    float num405 = num517 - vector29.Y;
                    float num406 = (float)Math.Sqrt((double)(num404 * num404 + num405 * num405));
                    num406 = speed / num406;
                    num404 *= num406;
                    num405 *= num406;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num404, num405, projectileType, projectile.damage, projectile.knockBack, projectile.owner, (float)target, 0f);
                    projectile.ai[0] = 16f;
                }
            }
        }

        public override bool CanDamage()
        {
            return false;
        }
    }
}
