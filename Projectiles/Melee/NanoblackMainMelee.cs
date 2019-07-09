using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Weapons;

namespace CalamityMod.Projectiles.Melee
{
    public class NanoblackMainMelee : ModProjectile
    {
        private static float RotationIncrement = 0.22f;
        private static int Lifetime = 240;
        private static float ReboundTime = 50f;

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nanoblack Reaper");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 56;
            projectile.height = 56;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.extraUpdates = 2;
            projectile.timeLeft = Lifetime;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 2;
        }
        
        public override void AI()
        {
            drawOffsetX = -11;
            drawOriginOffsetY = -4;
            drawOriginOffsetX = 0;

            // Produces electricity and green firework sparks constantly while in flight.
            if (Main.rand.Next(3) == 0)
            {
                int dustType = (Main.rand.Next(5) == 0) ? 226 : 220;
                float scale = 0.8f + Main.rand.NextFloat(0.3f);
                float velocityMult = Main.rand.NextFloat(0.3f, 0.6f);
                int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity = velocityMult * projectile.velocity;
                Main.dust[idx].scale = scale;
            }

            // ai[0] stores whether the scythe is returning. If 0, it isn't. If 1, it is.
            if (projectile.ai[0] == 0f)
			{
				projectile.ai[1] += 1f;
				if (projectile.ai[1] >= ReboundTime)
				{
					projectile.ai[0] = 1f;
					projectile.ai[1] = 0f;
					projectile.netUpdate = true;
				}
        	}
        	else
			{
				projectile.tileCollide = false;
                float returnSpeed = NanoblackReaperMelee.Speed;
				float acceleration = 2.4f;
                Player owner = Main.player[projectile.owner];

                // Delete the scythe if it's excessively far away.
                Vector2 playerCenter = owner.Center;
				float xDist = playerCenter.X - projectile.Center.X;
				float yDist = playerCenter.Y - projectile.Center.Y;
				float dist = (float)Math.Sqrt(xDist * xDist + yDist * yDist);
				if (dist > 3000f)
					projectile.Kill();

				dist = returnSpeed / dist;
				xDist *= dist;
				yDist *= dist;

                // Home back in on the player.
				if (projectile.velocity.X < xDist)
				{
					projectile.velocity.X = projectile.velocity.X + acceleration;
					if (projectile.velocity.X < 0f && xDist > 0f)
						projectile.velocity.X += acceleration;
				}
				else if (projectile.velocity.X > xDist)
				{
					projectile.velocity.X = projectile.velocity.X - acceleration;
					if (projectile.velocity.X > 0f && xDist < 0f)
						projectile.velocity.X -= acceleration;
				}
				if (projectile.velocity.Y < yDist)
				{
					projectile.velocity.Y = projectile.velocity.Y + acceleration;
					if (projectile.velocity.Y < 0f && yDist > 0f)
						projectile.velocity.Y += acceleration;
				}
				else if (projectile.velocity.Y > yDist)
				{
					projectile.velocity.Y = projectile.velocity.Y - acceleration;
					if (projectile.velocity.Y > 0f && yDist < 0f)
						projectile.velocity.Y -= acceleration;
				}

                // Delete the projectile if it touches its owner.
				if (Main.myPlayer == projectile.owner)
					if (projectile.Hitbox.Intersects(owner.Hitbox))
						projectile.Kill();
        	}

            // Create nanoblack energy blades while in flight after a delay.
            if(Lifetime - projectile.timeLeft >= 4 && Main.rand.Next(16) == 0)
                SpawnEnergyBlade();

            // Rotate the scythe as it flies.
            float spin = (projectile.direction <= 0) ? -1f : 1f;
        	projectile.rotation += spin * RotationIncrement;
			return;
        }

        private void SpawnEnergyBlade()
        {
            int bladeID = mod.ProjectileType("NanoblackSplitMelee");
            int bladeDamage = NanoblackReaperMelee.BaseDamage / 5;
            float bladeKB = 3f;
            float spin = (projectile.direction <= 0) ? -1f : 1f;
            float d = 16f;
            float velocityMult = 0.9f;
            Vector2 directOffset = new Vector2(Main.rand.NextFloat(-d, d), Main.rand.NextFloat(-d, d));
            Vector2 velocityOffset = Main.rand.NextFloat(-velocityMult, velocityMult) * projectile.velocity;
            Vector2 pos = projectile.Center + directOffset + velocityOffset;
            if (projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(pos, Vector2.Zero, bladeID, bladeDamage, bladeKB, projectile.owner, 0f, spin);
        }
    }
}
