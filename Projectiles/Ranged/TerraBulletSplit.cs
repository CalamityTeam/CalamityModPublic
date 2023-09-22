using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class TerraBulletSplit : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        private float speed = 0f;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.alpha = 255;
            Projectile.timeLeft = 120;
            Projectile.extraUpdates = 1;
            AIType = ProjectileID.Bullet;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 90 && target.CanBeChasedBy(Projectile);

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesFromEdge(Projectile, 0, lightColor);
            return false;
        }

        public override void AI()
        {
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
            if (Projectile.alpha > 0)
            {
                Projectile.alpha -= 85;
            }
            float x2 = Projectile.position.X - Projectile.velocity.X / 10f;
            float y2 = Projectile.position.Y - Projectile.velocity.Y / 10f;
            int terraDust = Dust.NewDust(new Vector2(x2, y2), 1, 1, 74, 0f, 0f, 0, default, 0.8f);
            Main.dust[terraDust].alpha = Projectile.alpha;
            Main.dust[terraDust].position.X = x2;
            Main.dust[terraDust].position.Y = y2;
            Main.dust[terraDust].velocity *= 0f;
            Main.dust[terraDust].noGravity = true;

            if (speed == 0f)
                speed = Projectile.velocity.Length();

            if (Projectile.timeLeft < 90) {
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 450f, speed, 12f);
            }
        }
    }
}
