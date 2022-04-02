using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class StormfrontRazorProjectile : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/StormfrontRazor";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stormfront Knife");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 5;
            projectile.timeLeft = 300;
            projectile.extraUpdates = 1;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.frameCounter++;
            if (projectile.frameCounter >= 6)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 4)
            {
                projectile.frame = 0;
            }
            drawOriginOffsetX = 50;
            drawOriginOffsetY = 20;
            projectile.ai[0]++;
            projectile.spriteDirection = projectile.direction = (projectile.velocity.X > 0).ToDirectionInt();
            projectile.rotation = projectile.velocity.ToRotation() + (projectile.spriteDirection == 1 ? 0f : MathHelper.Pi);
            projectile.rotation += projectile.spriteDirection * MathHelper.ToRadians(45f);
            if (projectile.ai[1] == 0)
            {
                projectile.ai[1] = 1;
            }
            float sparkFreq = 125f / projectile.ai[1];
            if (projectile.ai[0] >= sparkFreq)
            {
                Vector2 sparkS = new Vector2(Main.rand.NextFloat(-14f, 14f), Main.rand.NextFloat(-14f, 14f));
                Projectile.NewProjectile(projectile.Center, sparkS, ModContent.ProjectileType<Stormfrontspark>(), projectile.damage, 3f, projectile.owner);
                projectile.ai[0] = 0;
            }
            if (Main.rand.NextBool(10))
            {
                int d = Dust.NewDust(projectile.Center, projectile.width, projectile.height, 246, 0f, 0f, 100, new Color(255, Main.DiscoG, 53), 1f);
                Main.dust[d].scale += (float)Main.rand.Next(50) * 0.01f;
                Main.dust[d].noGravity = true;
                Main.dust[d].position = projectile.Center;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(projectile.position + projectile.velocity, projectile.velocity, projectile.width, projectile.height);
            Main.PlaySound(SoundID.Dig, projectile.position);
            return true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
