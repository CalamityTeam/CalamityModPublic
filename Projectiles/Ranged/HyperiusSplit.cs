using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class HyperiusSplit : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/Ranged/HyperiusBulletProj";
        private Color currentColor = Color.Black;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            // Intentionally large bullet hitbox to make Hyperius swarm more forgiving with hits
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 360;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            if (currentColor == Color.Black)
            {
                int startPoint = Main.rand.Next(6);
                Projectile.localAI[0] = startPoint;
                currentColor = HyperiusBulletProj.GetStartingColor(startPoint);
            }
            HyperiusBulletProj.Visuals(Projectile, ref currentColor);
        }

        // This projectile is always fullbright.
        public override Color? GetAlpha(Color lightColor)
        {
            return currentColor;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.timeLeft == 360)
                return false;
            CalamityUtils.DrawAfterimagesFromEdge(Projectile, 0, lightColor);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            const int killDust = 3;
            int[] dustTypes = new int[] { 60, 61, 59 };
            for (int i = 0; i < killDust; ++i)
            {
                int dustType = dustTypes[Main.rand.Next(3)];
                float scale = Main.rand.NextFloat(0.4f, 0.9f);
                float velScale = Main.rand.NextFloat(3f, 5.5f);
                int dustID = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType);
                Main.dust[dustID].noGravity = true;
                Main.dust[dustID].scale = scale;
                Main.dust[dustID].velocity *= velScale;
            }
        }
    }
}
