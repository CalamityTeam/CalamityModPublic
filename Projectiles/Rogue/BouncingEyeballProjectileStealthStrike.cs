using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class BouncingEyeballProjectileStealthStrike : ModProjectile
    {
        public const float Bounciness = 1.35f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eyeball");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.timeLeft = 280;
            projectile.penetrate = -1;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation();
            if (Math.Abs(projectile.velocity.X) > 23f)
            {
                projectile.velocity.X = Math.Sign(projectile.velocity.X) * 23f;
            }
            if (Math.Abs(projectile.velocity.Y) > 23f)
            {
                projectile.velocity.Y = Math.Sign(projectile.velocity.Y) * 23f;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.velocity != oldVelocity)
            {
                projectile.velocity = Main.rand.NextFloat(-1.15f, -0.85f) * oldVelocity * Bounciness;
            }
            Main.PlaySound(SoundID.NPCHit, (int)projectile.Center.X, (int)projectile.Center.Y, 19, 0.7f);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            // Explode into a large display of blood.
            Main.PlaySound(SoundID.NPCHit, (int)projectile.Center.X, (int)projectile.Center.Y, 19, 0.7f);
            int dustCount = Main.rand.Next(15, 26);
            for (int index = 0; index < dustCount; index++)
            {
                Vector2 velocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(4f, 9f) + projectile.velocity / 2f;
                Dust.NewDust(projectile.Center, 4, 4, DustID.Blood, velocity.X, velocity.Y);
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 2);
            return false;
        }
    }
}
