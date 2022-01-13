using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class UrchinSpikeFugu : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Urchin Spike");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 90;
            projectile.noEnchantments = true;
        }

        public override void AI()
        {
            projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
            if (projectile.ai[0] == 0f)
            {
                float maxRange = 100f;
                int npcIndex = -1;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
                    {
                        float targetDist = (npc.Center - projectile.Center).Length();
                        if (targetDist < maxRange)
                        {
                            npcIndex = i;
                            maxRange = targetDist;
                        }
                    }
                }
                projectile.ai[0] = (float)(npcIndex + 1);
                if (projectile.ai[0] == 0f)
                {
                    projectile.ai[0] = -15f;
                }
                if (projectile.ai[0] > 0f)
                {
                    float scaleFactor5 = (float)Main.rand.Next(35, 75) / 30f;
                    projectile.velocity = (projectile.velocity * 20f + Vector2.Normalize(Main.npc[(int)projectile.ai[0] - 1].Center - projectile.Center + new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101))) * scaleFactor5) / 21f;
                    projectile.netUpdate = true;
                }
            }
            else if (projectile.ai[0] > 0f)
            {
                Vector2 value16 = Vector2.Normalize(Main.npc[(int)projectile.ai[0] - 1].Center - projectile.Center);
                projectile.velocity = (projectile.velocity * 40f + value16 * 12f) / 41f;
            }
            else
            {
                projectile.ai[0] += 1f;
                projectile.alpha -= 25;
                if (projectile.alpha < 0)
                {
                    projectile.alpha = 0;
                }
                projectile.velocity.Y = projectile.velocity.Y + 0.015f;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Venom, 120);
        }
    }
}
