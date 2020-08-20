using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class MadAlchemistsCocktailGasCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mad Alchemist's Cocktail Gas Cloud");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 32;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 30;
        }

        public override void AI()
        {
            projectile.ai[1] += 1f;
            if (projectile.ai[1] > 60f)
            {
                projectile.ai[0] += 10f;
            }
            if (projectile.ai[0] > 255f)
            {
                projectile.Kill();
                projectile.ai[0] = 255f;
            }
            projectile.alpha = (int)(100.0 + (double)projectile.ai[0] * 0.7);
            projectile.rotation += projectile.velocity.X * 0.1f;
            projectile.rotation += (float)projectile.direction * 0.003f;
            projectile.velocity *= 0.96f;

            Rectangle cloudHitbox = new Rectangle((int)projectile.position.X, (int)projectile.position.Y, projectile.width, projectile.height);
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
				Projectile otherProj = Main.projectile[i];
				// Short circuits to make the loop as fast as possible
				if (!otherProj.active || otherProj.owner != projectile.owner || !otherProj.minion || i == projectile.whoAmI)
					continue;

                if (otherProj.type == projectile.type)
                {
                    Rectangle otherProjHitbox = new Rectangle((int)otherProj.position.X, (int)otherProj.position.Y, otherProj.width, otherProj.height);
                    if (cloudHitbox.Intersects(otherProjHitbox))
                    {
                        Vector2 projDistance = otherProj.Center - projectile.Center;
                        if (projDistance.X == 0f && projDistance.Y == 0f)
                        {
                            if (i < projectile.whoAmI)
                            {
                                projDistance.X = -1f;
                                projDistance.Y = 1f;
                            }
                            else
                            {
                                projDistance.X = 1f;
                                projDistance.Y = -1f;
                            }
                        }
                        projDistance.Normalize();
                        projDistance *= 0.005f;
                        projectile.velocity -= projDistance;
                        otherProj.velocity += projDistance;
                    }
                }
            }
        }
    }
}
