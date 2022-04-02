using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class HowlsHeartFireball : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fireball");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.MinionShot[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.netImportant = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
            projectile.minion = true;
            projectile.minionSlots = 0f;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];

            //Cycle through animation
            projectile.frameCounter++;
            if (projectile.frameCounter >= 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }

            Vector2 center = projectile.Center;
            float maxDistance = 500f;
            bool homeIn = false;
            int target = (int)projectile.ai[0];

            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float extraDistance = (npc.width / 2) + (npc.height / 2);

                    bool canHit = true;
                    if (extraDistance < maxDistance)
                        canHit = Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1);

                    if (Vector2.Distance(npc.Center, projectile.Center) < (maxDistance + extraDistance) && canHit)
                    {
                        center = npc.Center;
                        homeIn = true;
                    }
                }
            }
            else if (Main.npc[target].CanBeChasedBy(projectile, false))
            {
                NPC npc = Main.npc[target];

                float extraDistance = (npc.width / 2) + (npc.height / 2);

                bool canHit = true;
                if (extraDistance < maxDistance)
                    canHit = Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1);

                if (Vector2.Distance(npc.Center, projectile.Center) < (maxDistance + extraDistance) && canHit)
                {
                    center = npc.Center;
                    homeIn = true;
                }
            }
            if (!homeIn)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float extraDistance = (npc.width / 2) + (npc.height / 2);

                        bool canHit = true;
                        if (extraDistance < maxDistance)
                            canHit = Collision.CanHit(projectile.Center, 1, 1, npc.Center, 1, 1);

                        if (Vector2.Distance(npc.Center, projectile.Center) < (maxDistance + extraDistance) && canHit)
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
                projectile.velocity = (projectile.velocity * 20f + moveDirection * 21f) / (21f);
            }

            int blueT = Dust.NewDust(projectile.position, projectile.width, projectile.height, 59, 0f, 0f, 100, default, 0.6f);
            Main.dust[blueT].noGravity = true;
            Main.dust[blueT].velocity *= 0.5f;
            Main.dust[blueT].velocity += projectile.velocity * 0.1f;
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item45, projectile.position);
            int blue = Dust.NewDust(projectile.position, projectile.width, projectile.height, 59, 0f, 0f, 100, default, 1f);
            Main.dust[blue].velocity *= 0.5f;
            if (Main.rand.NextBool(2))
            {
                Main.dust[blue].scale = 0.5f;
                Main.dust[blue].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
            }
            int torch = Dust.NewDust(projectile.position, projectile.width, projectile.height, 59, 0f, 0f, 100, default, 1.4f);
            Main.dust[torch].noGravity = true;
            Dust.NewDust(projectile.position, projectile.width, projectile.height, 59, 0f, 0f, 100, default, 0.8f);
        }
    }
}
