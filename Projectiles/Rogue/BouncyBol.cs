using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
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
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.penetrate = 8;
            Projectile.timeLeft = 600;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.aiStyle = ProjAIStyleID.GroundProjectile;
        }

        public override void AI()
        {
            if (hasHitEnemy && Projectile.timeLeft < 575)
            {
                Projectile.velocity.X *= 1.005f; //you broke up, time to yeet yourself out
                Projectile.velocity.Y *= 1.005f;
                if (Projectile.velocity.X > 16f)
                {
                    Projectile.velocity.X = 16f;
                }
                if (Projectile.velocity.Y > 16f)
                {
                    Projectile.velocity.Y = 16f;
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
                if (npc.CanBeChasedBy(Projectile, false) && npc != target && !hasHitNPC)
                {
                    float dist = (Projectile.Center - npc.Center).Length();
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
                Projectile.damage = (int)(Projectile.damage * 1.1f);
                hasHitEnemy = true;
                targetNPC = index;
                velocityNew = Main.npc[index].Center - Projectile.Center;
                velocityNew.Normalize();
                velocityNew *= 15f;
                Projectile.velocity = velocityNew;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
            {
                Projectile.velocity.X = -oldVelocity.X * 1.001f;
            }
            if (Projectile.velocity.Y != oldVelocity.Y)
            {
                Projectile.velocity.Y = -oldVelocity.Y * 1.001f;
            }
            return false;
        }
    }
}
