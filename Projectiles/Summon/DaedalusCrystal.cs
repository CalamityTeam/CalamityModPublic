using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class DaedalusCrystal : ModProjectile
    {
        public int dust = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Crystal");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 46;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
        }

        public override void AI()
        {
            bool flag64 = projectile.type == ModContent.ProjectileType<DaedalusCrystal>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.daedalusCrystal)
            {
                projectile.active = false;
                return;
            }
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.dCrystal = false;
                }
                if (modPlayer.dCrystal)
                {
                    projectile.timeLeft = 2;
                }
            }
            dust--;
            if (dust >= 0)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num501 = 50;
                for (int num502 = 0; num502 < num501; num502++)
                {
                    int num503 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 173, 0f, 0f, 0, default, 1f);
                    Main.dust[num503].velocity *= 2f;
                    Main.dust[num503].scale *= 1.15f;
                }
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.35f / 255f, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0.75f / 255f);
            projectile.Center = player.Center + Vector2.UnitY * (player.gfxOffY - 60f);
            if (player.gravDir == -1f)
            {
                projectile.position.Y += 120f;
                projectile.rotation = MathHelper.Pi;
            }
            else
            {
                projectile.rotation = 0f;
            }
            projectile.position.X = (int)projectile.position.X;
            projectile.position.Y = (int)projectile.position.Y;
            float num395 = (float)Main.mouseTextColor / 200f - 0.35f;
            num395 *= 0.2f;
            projectile.scale = num395 + 0.95f;
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
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float num539 = npc.position.X + (float)(npc.width / 2);
                        float num540 = npc.position.Y + (float)(npc.height / 2);
                        float num541 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num539) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num540);
                        if (num541 < num508 && Collision.CanHit(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                        {
                            num506 = num539;
                            num507 = num540;
                            flag18 = true;
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
                    int projectileType = ModContent.ProjectileType<DaedalusCrystalShot>();
                    float num403 = Main.rand.Next(10, 15); //modify the speed the projectile are shot.  Lower number = slower projectile.
                    Vector2 vector29 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
                    float num404 = num516 - vector29.X;
                    float num405 = num517 - vector29.Y;
                    float num406 = (float)Math.Sqrt((double)(num404 * num404 + num405 * num405));
                    num406 = num403 / num406;
                    num404 *= num406;
                    num405 *= num406;
                    Projectile.NewProjectile(projectile.Center.X - 4f, projectile.Center.Y, num404, num405, projectileType, projectile.damage, 5f, projectile.owner, 0f, 0f);
                    projectile.ai[0] = 50f;
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
