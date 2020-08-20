using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class ReaverOrb : ModProjectile
    {
        public int dust = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Orb");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 48;
            projectile.height = 50;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 18000;
            projectile.alpha = 50;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
        }

        public override void AI()
        {
            bool flag64 = projectile.type == ModContent.ProjectileType<ReaverOrb>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.reaverOrb)
            {
                projectile.active = false;
                return;
            }
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.rOrb = false;
                }
                if (modPlayer.rOrb)
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
                    int num503 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 157, 0f, 0f, 0, default, 1f);
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
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 1f / 255f, (255 - projectile.alpha) * 0f / 255f);
            projectile.position.X = player.Center.X - (float)(projectile.width / 2);
            projectile.position.Y = player.Center.Y - (float)(projectile.height / 2) + player.gfxOffY - 60f;
            if (player.gravDir == -1f)
            {
                projectile.position.Y = projectile.position.Y + 120f;
                projectile.rotation = MathHelper.Pi;
            }
            else
            {
                projectile.rotation = 0f;
            }
            projectile.position.X = (float)(int)projectile.position.X;
            projectile.position.Y = (float)(int)projectile.position.Y;
            if (projectile.owner == Main.myPlayer)
            {
                if (projectile.ai[0] != 0f)
                {
                    projectile.ai[0] -= 1f;
                    return;
                }
                bool foundTarget = false;
                float maxDist = 600f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
					NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        if (Vector2.Distance(projectile.Center, npc.Center) < maxDist && Collision.CanHit(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                        {
                            foundTarget = true;
							break;
                        }
                    }
                }
                if (foundTarget)
                {
                    int projAmt = Main.rand.Next(4, 9);
                    for (int u = 0; u < projAmt; u++)
                    {
						Vector2 source = new Vector2(projectile.Center.X - 4f, projectile.Center.Y);
						Vector2 velocity = CalamityUtils.RandomVelocity(100f, 90f, 120f);
                        int spore = Projectile.NewProjectile(source, velocity, ProjectileID.SporeGas + Main.rand.Next(3), projectile.damage, 1.5f, projectile.owner, 0f, 0f);
                        Main.projectile[spore].minionSlots = 0f;
						Main.projectile[spore].Calamity().forceMinion = true;
						Main.projectile[spore].usesLocalNPCImmunity = true;
						Main.projectile[spore].localNPCHitCooldown = 30;
                    }
                    Main.PlaySound(SoundID.Item77, projectile.position);
                    projectile.ai[0] = 50f;
                }
            }
        }

        public override bool CanDamage()
        {
            return false;
        }
    }
}
