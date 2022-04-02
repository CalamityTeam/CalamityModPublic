using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class UnstableMatter : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public bool HasCollidedWithATile
        {
            get => projectile.ai[0] == 1f;
            set => projectile.ai[0] = value.ToInt();
        }

        public float Time
        {
            get => projectile.ai[1];
            set => projectile.ai[1] = value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Unstable Matter");
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 480;
        }

        public override void AI()
        {
            // Cast some lime light at the projectile's position.
            Lighting.AddLight(projectile.Center, Color.GreenYellow.ToVector3());
            Time++;
            if (Time >= 10f)
                GenerateIdleDust();
        }

        public void GenerateIdleDust()
        {
            if (Main.dedServ)
                return;

            // Release some dust outward.
            // The faster the projectile itself is, the slower the dust, and the longer it lasts.
            // This is done to give the projectile a "streak" movement the faster it is versus a expansion.
            int dustCount = HasCollidedWithATile ? 5 : 12;
            float baseSpeedRatio = Utils.InverseLerp(6f, 14f, projectile.velocity.Length(), true);
            float speed = MathHelper.Lerp(6f, 1.8f, (float)Math.Pow(baseSpeedRatio, 3f));
            if (!HasCollidedWithATile)
                speed += MathHelper.Lerp(-6f, 4f, Utils.InverseLerp(10f, 150f, Time, true));
            float persistence = MathHelper.Lerp(0f, 0.8f, baseSpeedRatio);
            for (int i = 0; i < dustCount; i++)
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center, 107);
                dust.velocity = Main.rand.NextVector2CircularEdge(speed, speed) - projectile.velocity;
                dust.fadeIn = persistence;
                dust.scale = 0.9f;
                dust.noGravity = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (!HasCollidedWithATile)
                damage /= 3;
            else if (projectile.penetrate == -1)
                projectile.penetrate = 1;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Register the tile collision, if one has not happened yet.
            if (!HasCollidedWithATile)
            {
                HasCollidedWithATile = true;

                // And bounce off the tile. Or towards a nearby enemy, if there is one.
                NPC potentialTarget = projectile.Center.ClosestNPCAt(700f, false);
                if (potentialTarget != null)
                    projectile.velocity = projectile.SafeDirectionTo(potentialTarget.Center);
                else
                {
                    if (projectile.velocity.X != oldVelocity.X)
                        projectile.velocity.X = -oldVelocity.X;
                    if (projectile.velocity.Y != oldVelocity.Y)
                        projectile.velocity.Y = -oldVelocity.Y;
                }

                projectile.velocity = projectile.velocity.SafeNormalize(Vector2.UnitY) * 14f;

                projectile.netUpdate = true;
                return false;
            }
            return true;
        }
    }
}
