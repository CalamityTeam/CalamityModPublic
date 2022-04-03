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
            get => Projectile.ai[0] == 1f;
            set => Projectile.ai[0] = value.ToInt();
        }

        public float Time
        {
            get => Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Unstable Matter");
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 480;
        }

        public override void AI()
        {
            // Cast some lime light at the projectile's position.
            Lighting.AddLight(Projectile.Center, Color.GreenYellow.ToVector3());
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
            float baseSpeedRatio = Utils.InverseLerp(6f, 14f, Projectile.velocity.Length(), true);
            float speed = MathHelper.Lerp(6f, 1.8f, (float)Math.Pow(baseSpeedRatio, 3f));
            if (!HasCollidedWithATile)
                speed += MathHelper.Lerp(-6f, 4f, Utils.InverseLerp(10f, 150f, Time, true));
            float persistence = MathHelper.Lerp(0f, 0.8f, baseSpeedRatio);
            for (int i = 0; i < dustCount; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 107);
                dust.velocity = Main.rand.NextVector2CircularEdge(speed, speed) - Projectile.velocity;
                dust.fadeIn = persistence;
                dust.scale = 0.9f;
                dust.noGravity = true;
            }
        }

        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (!HasCollidedWithATile)
                damage /= 3;
            else if (Projectile.penetrate == -1)
                Projectile.penetrate = 1;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            // Register the tile collision, if one has not happened yet.
            if (!HasCollidedWithATile)
            {
                HasCollidedWithATile = true;

                // And bounce off the tile. Or towards a nearby enemy, if there is one.
                NPC potentialTarget = Projectile.Center.ClosestNPCAt(700f, false);
                if (potentialTarget != null)
                    Projectile.velocity = Projectile.SafeDirectionTo(potentialTarget.Center);
                else
                {
                    if (Projectile.velocity.X != oldVelocity.X)
                        Projectile.velocity.X = -oldVelocity.X;
                    if (Projectile.velocity.Y != oldVelocity.Y)
                        Projectile.velocity.Y = -oldVelocity.Y;
                }

                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 14f;

                Projectile.netUpdate = true;
                return false;
            }
            return true;
        }
    }
}
