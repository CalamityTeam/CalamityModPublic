using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class AtaraxiaMain : ModProjectile
    {
        private static int NumAnimationFrames = 5;
        private static int AnimationFrameTime = 9;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Not Exoblade");
            Main.projFrames[projectile.type] = NumAnimationFrames;
        }

        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 2;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            drawOffsetX = -40;
            drawOriginOffsetY = -3;
            drawOriginOffsetX = 18;
            projectile.rotation = projectile.velocity.ToRotation();

            // Light
            Lighting.AddLight(projectile.Center, 0.45f, 0.1f, 0.1f);

            // Spawn dust with a 1/2 chance
            if (Main.rand.NextBool())
            {
                int idx = Dust.NewDust(projectile.Center, 1, 1, 90);
                Main.dust[idx].position = projectile.Center;
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 0.6f;
            }

            // Update animation
            projectile.frameCounter++;
            if (projectile.frameCounter > AnimationFrameTime)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= NumAnimationFrames)
                projectile.frame = 0;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.ShadowFlame, 180);
        }

        // Explodes like Exoblade's Exobeams
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCDeath55, projectile.Center);

            // Transform the projectile's hitbox into a big explosion
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 140;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);

            Vector2 corner = new Vector2(projectile.position.X, projectile.position.Y);
            for (int i = 0; i < 50; i++)
            {
                int idx = Dust.NewDust(corner, projectile.width, projectile.height, 86, 0f, 0f, 0, new Color(210, 0, 255), 2.2f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 2.5f;

                idx = Dust.NewDust(corner, projectile.width, projectile.height, 118, 0f, 0f, 100, new Color(210, 0, 255), 1.8f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 1.8f;

                idx = Dust.NewDust(corner, projectile.width, projectile.height, 71, 0f, 0f, 100, new Color(210, 0, 255), 1.0f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 4.0f;
            }

            // Make the projectile ignore iframes while exploding
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
        }
    }
}
