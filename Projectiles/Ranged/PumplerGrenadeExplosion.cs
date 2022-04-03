using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.Ranged
{
    public class PumplerGrenadeExplosion : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private bool NPCHit => Projectile.ai[0] == 1;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cucurbitace Blast");
        }

        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.timeLeft = 5;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        private void SmokeBoom()
        {
            for (int i = 0; i < 15; i++)
            {
                Particle smoke = new SmallSmokeParticle(Projectile.Center + Main.rand.NextVector2Circular(15f, 15f), Vector2.Zero, Color.Orange, new Color(40, 40, 40), Main.rand.NextFloat(0.8f, 1.6f), 145 - Main.rand.Next(30));
                smoke.Velocity = (smoke.Position - Projectile.Center) * 0.2f + Projectile.velocity;
                GeneralParticleHandler.SpawnParticle(smoke);
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            float blastRadius = Projectile.height / 2f;

            //Get the absolute value of the distance between the target's hitbox center & the explosion's center
            float distanceX = Math.Abs(Projectile.Center.X - target.Hitbox.Center.X);
            float distanceY = Math.Abs(Projectile.Center.Y - target.Hitbox.Center.Y);

            //If the distance is just too big for the two to intersect, return false
            if (distanceX > (target.Hitbox.Width / 2f + blastRadius) || distanceY > (target.Hitbox.Height / 2f + blastRadius))
                return false;

            //If either distance is too small (aka if the projectile's center is inside of the target's hitbox, return true
            if (distanceX <= (target.Hitbox.Width / 2f) || distanceY <= (target.Hitbox.Height / 2f))
                return true;

            //Pythagorean theorem stuff to determine the litteral edge cases
            float squaredCornerDisance = (float)(Math.Pow(distanceX - target.Hitbox.Width / 2f, 2) + Math.Pow(distanceY - target.Hitbox.Height / 2f, 2));
            return (squaredCornerDisance <= Math.Pow(blastRadius, 2));
        }

        public override void AI()
        {
            //Bigger explosion if no npc gets hit
            if (!NPCHit)
            {
                Projectile.width = 120;
                Projectile.height = 120;
            }

            if (Projectile.timeLeft == 5)
                SmokeBoom();
        }
    }
}
