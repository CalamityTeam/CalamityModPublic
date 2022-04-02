using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class SakuraBullet : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sakura Bullet");
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.netImportant = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 150;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.extraUpdates = 1;
            projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.02f;

            Vector2 center = projectile.Center;
            float maxDistance = 400f;
            bool homeIn = false;

            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if ((npc.CanBeChasedBy(projectile, false) || npc.type == NPCID.DukeFishron) && npc.active)
                {
                    float extraDistance = (npc.width / 2) + (npc.height / 2);

                    if (Vector2.Distance(npc.Center, projectile.Center) < (maxDistance + extraDistance))
                    {
                        center = npc.Center;
                        homeIn = true;
                    }
                }
            }
            else if (projectile.ai[0] != -1f)
            {
                NPC npc = Main.npc[(int)projectile.ai[0]];
                if ((npc.CanBeChasedBy(projectile, false) || npc.type == NPCID.DukeFishron) && npc.active)
                {
                    float extraDistance = (npc.width / 2) + (npc.height / 2);

                    if (Vector2.Distance(npc.Center, projectile.Center) < (maxDistance + extraDistance))
                    {
                        center = npc.Center;
                        homeIn = true;
                    }
                }
            }
            if (!homeIn)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if ((npc.CanBeChasedBy(projectile, false) || (npc.type == NPCID.DukeFishron && (!npc.dontTakeDamage || npc.ai[0] > 9f))) && npc.active)
                    {
                        float extraDistance = (npc.width / 2) + (npc.height / 2);

                        if (Vector2.Distance(npc.Center, projectile.Center) < (maxDistance + extraDistance))
                        {
                            center = npc.Center;
                            homeIn = true;
                            break;
                        }
                    }
                }
            }

            if (homeIn)
            {
                Vector2 moveDirection = projectile.SafeDirectionTo(center, Vector2.UnitY);
                projectile.velocity = (projectile.velocity * 20f + moveDirection * 11f) / (21f);
            }

            int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 73, 0f, 0f, 100, default, 0.6f);
            Main.dust[num458].noGravity = true;
            Main.dust[num458].velocity *= 0.5f;
            Main.dust[num458].velocity += projectile.velocity * 0.1f;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item25, projectile.position);
            int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 73, 0f, 0f, 100, default, 1f);
            Main.dust[num622].velocity *= 0.5f;
            if (Main.rand.NextBool(2))
            {
                Main.dust[num622].scale = 0.5f;
                Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
            }
            int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 73, 0f, 0f, 100, default, 1.4f);
            Main.dust[num624].noGravity = true;
            num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 73, 0f, 0f, 100, default, 0.8f);
        }
    }
}
