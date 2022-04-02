using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class GalaxyStar : ModProjectile
    {
        public bool madeCoolMagicSound = false;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Star");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 42;
            projectile.height = 42;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 160;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 8;
        }
        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 1f, 1f, 1f);
            if (!madeCoolMagicSound)
            {
                Main.PlaySound(SoundID.Item9, projectile.position); // Starfury sound
                madeCoolMagicSound = true;
            }
            projectile.ai[0]++;
            if (projectile.ai[0] % 5 == 0)
            {
                for (int i = 0; i < Main.rand.Next(2, 4); i++) //2-3 stars
                {
                    Vector2 randVector = Vector2.One.RotatedByRandom(Math.PI * 2.0) * 0.7f;
                    Dust.NewDust(projectile.Center, 4, 4, 58, randVector.X, randVector.Y, 0, default, 1f);
                }
            }
            projectile.rotation += projectile.velocity.Length() / 19f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }
    }
}
