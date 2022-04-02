using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BouncyBol : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/BouncySpikyBall";

        private bool hasHitEnemy = false;
        private int targetNPC = -1;
        private List<int> previousNPCs = new List<int>() { -1 };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bouncy Ball");
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = 8;
            projectile.timeLeft = 600;
            projectile.Calamity().rogue = true;
            projectile.aiStyle = 14;
        }

        public override void AI()
        {
            if (hasHitEnemy == true && projectile.timeLeft < 575)
            {
                projectile.velocity.X *= 1.005f; //you broke up, time to yeet yourself out
                projectile.velocity.Y *= 1.005f;
                if (projectile.velocity.X > 16f)
                {
                    projectile.velocity.X = 16f;
                }
                if (projectile.velocity.Y > 16f)
                {
                    projectile.velocity.Y = 16f;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            float minDist = 999f;
            int index = 0;
            for (int i = 0; i < Main.npc.Length; i++)
            {
                bool hasHitNPC = false;
                for (int j = 0; j < previousNPCs.Count; j++)
                {
                    if (previousNPCs[j] == i)
                    {
                        hasHitNPC = true;
                    }
                }

                NPC npc = Main.npc[i];
                if (npc == target)
                {
                    previousNPCs.Add(i);
                }
                if (npc.CanBeChasedBy(projectile, false) && npc != target && !hasHitNPC)
                {
                    float dist = (projectile.Center - npc.Center).Length();
                    if (dist < minDist)
                    {
                        minDist = dist;
                        index = i;
                    }
                }
            }

            Vector2 velocityNew;
            if (minDist < 999f)
            {
                if (projectile.Calamity().stealthStrike && hasHitEnemy == false)
                {
                    projectile.damage = (int)(projectile.damage * 1.5f);
                }
                else
                {
                    projectile.damage = (int)(projectile.damage * 1.1f);
                }
                hasHitEnemy = true;
                targetNPC = index;
                velocityNew = Main.npc[index].Center - projectile.Center;
                velocityNew.Normalize();
                velocityNew *= 15f;
                projectile.velocity = velocityNew;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity.X != oldVelocity.X)
            {
                projectile.velocity.X = -oldVelocity.X * 1.001f;
            }
            if (projectile.velocity.Y != oldVelocity.Y)
            {
                projectile.velocity.Y = -oldVelocity.Y * 1.001f;
            }
            return false;
        }
    }
}
