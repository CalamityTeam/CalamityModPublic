using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class SphereBlue : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Thruster Sphere");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.tileCollide = false;
            projectile.Calamity().rogue = true;
            projectile.extraUpdates = 1;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0f, 1f);
            if (Main.rand.NextBool(5))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 229, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 100);
            }
            projectile.ai[0] += 1f;
            if (projectile.ai[0] >= 5f)
            {
                projectile.tileCollide = true;
            }
            projectile.rotation += projectile.velocity.X * 0.02f;
            projectile.velocity.Y = projectile.velocity.Y + 0.085f;
            projectile.velocity.X = projectile.velocity.X * 0.99f;
            CalamityGlobalProjectile.HomeInOnNPC(projectile, true, 300f, 12f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 192;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            Main.PlaySound(SoundID.NPCDeath37, projectile.position);
            for (int num625 = 0; num625 < 3; num625++)
            {
                float scaleFactor10 = 0.33f;
                if (num625 == 1)
                {
                    scaleFactor10 = 0.66f;
                }
                if (num625 == 2)
                {
                    scaleFactor10 = 1f;
                }
                int defectiveBruh = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default, Main.rand.Next(660, 662), 1f);
                Main.gore[defectiveBruh].velocity *= scaleFactor10;
                Main.gore[defectiveBruh].velocity += projectile.velocity;
            }
            for (int num194 = 0; num194 < 25; num194++)
            {
                int dustType = Utils.SelectRandom(Main.rand, new int[]
                {
                    226,
                    229
                });
                int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 1f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 0f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
