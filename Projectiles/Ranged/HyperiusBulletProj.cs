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
            projectile.width = 4;
            projectile.height = 4;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.extraUpdates = 3;
            aiType = ProjectileID.Bullet;
            projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.basePointBlankShotDuration;
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
            CalamityUtils.DrawAfterimagesFromEdge(projectile, 0, lightColor);
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            OnHitEffects(target.Center);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            OnHitEffects(target.Center);
        }

        private void OnHitEffects(Vector2 targetPos)
        {
            if (projectile.owner == Main.myPlayer)
            {
                CalamityUtils.ProjectileBarrage(projectile.Center, targetPos, Main.rand.NextBool(), 800f, 800f, 0f, 800f, 10f, ModContent.ProjectileType<HyperiusSplit>(), (int)(projectile.damage * 0.6), 1f, projectile.owner, true);
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
