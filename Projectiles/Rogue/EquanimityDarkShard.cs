using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Rogue
{
    public class EquanimityDarkShard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dark Shard");
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.alpha = 0;
            projectile.penetrate = 1;
            projectile.tileCollide = true;
            projectile.timeLeft = 120;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.velocity.Y += 0.1f;
            projectile.rotation += 0.4f * projectile.direction;
            if (projectile.timeLeft < 130)
            {
                float minDist = 999f;
                int index = 0;
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(projectile, false))
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
                    velocityNew = Main.npc[index].Center - projectile.Center;
                    velocityNew.Normalize();
                    projectile.velocity += velocityNew;
                    if (projectile.velocity.Length() > 10f)
                    {
                        projectile.velocity.Normalize();
                        projectile.velocity *= 10f;
                    }
                }
            }

            if (projectile.timeLeft < 51)
            {
                projectile.alpha += 5;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (projectile.timeLeft < 130)
            {
                return null;
            }
            else
            {
                return false;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, projectile.position);
            projectile.Kill();
            return true;
        }
    }
}
