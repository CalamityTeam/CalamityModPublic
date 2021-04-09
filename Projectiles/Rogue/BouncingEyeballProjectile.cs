using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static System.Math;

namespace CalamityMod.Projectiles.Rogue
{
    public class BouncingEyeballProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/BouncingEyeball";

        private int Bounces = 5;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eyeball");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.tileCollide = true;
            projectile.timeLeft = 300;
            projectile.penetrate = 2;
            projectile.Calamity().rogue = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Main.PlaySound(SoundID.NPCHit, (int)projectile.Center.X, (int)projectile.Center.Y, 19, 0.7f);
            Bounces--;
            if (Bounces <= 0)
            {
                projectile.Kill();
            }
            else
            {
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
            }
            return false;
        }
        public override void AI()
        {
            if (projectile.velocity.Y <= 10f)
                projectile.velocity.Y += 0.15f;
            projectile.rotation += MathHelper.ToRadians(5f) * Sign(projectile.velocity.X);
        }
        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.NPCHit, (int)projectile.Center.X, (int)projectile.Center.Y, 19, 0.7f);
            int dustCount = Main.rand.Next(8, 16);
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
