using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
	public class HyperiusBulletProj : ModProjectile
    {
        private Color currentColor = Color.Black;
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hyperius Bullet");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 1;
            aiType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            if (currentColor == Color.Black)
            {
                int startPoint = Main.rand.Next(6);
                projectile.localAI[0] = startPoint;
                currentColor = GetStartingColor(startPoint);
            }
            Visuals(projectile, ref currentColor);
        }

        internal static void Visuals(Projectile projectile, ref Color c)
        {
            CalamityUtils.IterateDisco(ref c, ref projectile.localAI[0], 15);
            Vector3 compositeColor = 0.1f * Color.White.ToVector3() + 0.05f * c.ToVector3();
            Lighting.AddLight(projectile.Center, compositeColor);
        }

        internal static Color GetStartingColor(int startPoint = 0)
        {
            switch (startPoint)
            {
                default: return new Color(1f, 0f, 0f, 0f); // Color.Red
                case 1: return new Color(1f, 1f, 0f, 0f); // Color.Yellow
                case 2: return new Color(0f, 1f, 0f, 0f); // Color.Green
                case 3: return new Color(0f, 1f, 1f, 0f); // Color.Turquoise
                case 4: return new Color(0f, 0f, 1f, 0f); // Color.Blue
                case 5: return new Color(1f, 0f, 1f, 0f); // Color.Violet
            }
        }

        // This projectile is always fullbright.
        public override Color? GetAlpha(Color lightColor)
        {
            return currentColor;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            float xPos = Main.rand.NextBool(2) ? projectile.position.X + 800 : projectile.position.X - 800;
            Vector2 vector2 = new Vector2(xPos, projectile.position.Y + Main.rand.Next(-800, 801));
            float num80 = xPos;
            float speedX = (float)target.position.X - vector2.X;
            float speedY = (float)target.position.Y - vector2.Y;
            float dir = 10 / num80;
            speedX *= dir * 150;
            speedY *= dir * 150;
            if (speedX > 15f)
            {
                speedX = 15f;
            }
            if (speedX < -15f)
            {
                speedX = -15f;
            }
            if (speedY > 15f)
            {
                speedY = 15f;
            }
            if (speedY < -15f)
            {
                speedY = -15f;
            }
            if (projectile.owner == Main.myPlayer)
            {
                int splitDamage = (int)(projectile.damage * 0.6f);
                float splitKB = 1f;
                Projectile.NewProjectile(vector2, new Vector2(speedX, speedY), ModContent.ProjectileType<HyperiusSplit>(), splitDamage, splitKB, projectile.owner);
            }
        }

        public override void Kill(int timeLeft)
        {
            const int killDust = 3;
            int[] dustTypes = new int[] { 60, 61, 59 };
            for (int i = 0; i < killDust; ++i)
            {
                int dustType = dustTypes[Main.rand.Next(3)];
                float scale = Main.rand.NextFloat(0.4f, 0.9f);
                float velScale = Main.rand.NextFloat(3f, 5.5f);
                int dustID = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType);
                Main.dust[dustID].noGravity = true;
                Main.dust[dustID].scale = scale;
                Main.dust[dustID].velocity *= velScale;
            }
        }
    }
}
