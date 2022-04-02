using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class AtaraxiaSide : ModProjectile
    {
        private static int NumAnimationFrames = 5;
        private static int AnimationFrameTime = 9;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Also Not Exoblade");
            Main.projFrames[projectile.type] = NumAnimationFrames;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 2;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            drawOffsetX = -28;
            drawOriginOffsetY = -2;
            drawOriginOffsetX = 12;
            projectile.rotation = projectile.velocity.ToRotation();

            // Light
            Lighting.AddLight(projectile.Center, 0.3f, 0.1f, 0.45f);

            // Spawn dust with a 3/4 chance
            if (Main.rand.Next(4) != 3)
            {
                int idx = Dust.NewDust(projectile.Center, 1, 1, 70);
                Main.dust[idx].position = projectile.Center;
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity *= 0.25f;
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

        // Spawns 6 smaller projectiles that slowly glide outward and ignore iframes
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item89, projectile.Center);

            // Individual split projectiles deal 5% damage per hit.
            int numSplits = 6;
            int splitID = ModContent.ProjectileType<AtaraxiaSplit>();
            int damage = (int)(projectile.damage * 0.05f);
            float angleVariance = MathHelper.TwoPi / numSplits;
            Vector2 projVec = new Vector2(4.5f, 0f).RotatedByRandom(MathHelper.TwoPi);

            for (int i = 0; i < numSplits; ++i)
            {
                projVec = projVec.RotatedBy(angleVariance);
                if (projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(projectile.Center, projVec, splitID, damage, 1.5f, Main.myPlayer);
            }
        }
    }
}
