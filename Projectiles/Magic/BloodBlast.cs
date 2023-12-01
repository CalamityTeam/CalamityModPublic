using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class BloodBlast : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.alpha = 255;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 2;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.5f, 0f, 0f);
            for (int i = 0; i < 2; i++)
            {
                float shortXVel = Projectile.velocity.X / 3f * (float)i;
                float shortYVel = Projectile.velocity.Y / 3f * (float)i;
                int fourConst = 4;
                int bloody = Dust.NewDust(new Vector2(Projectile.position.X + (float)fourConst, Projectile.position.Y + (float)fourConst), Projectile.width - fourConst * 2, Projectile.height - fourConst * 2, 5, 0f, 0f, 100, default, 1.2f);
                Dust dust = Main.dust[bloody];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += Projectile.velocity * 0.1f;
                dust.position.X -= shortXVel;
                dust.position.Y -= shortYVel;
            }
            if (Main.rand.NextBool(5))
            {
                int otherFourConst = 4;
                int graphicContent = Dust.NewDust(new Vector2(Projectile.position.X + (float)otherFourConst, Projectile.position.Y + (float)otherFourConst), Projectile.width - otherFourConst * 2, Projectile.height - otherFourConst * 2, 5, 0f, 0f, 100, default, 0.6f);
                Main.dust[graphicContent].velocity *= 0.25f;
                Main.dust[graphicContent].velocity += Projectile.velocity * 0.5f;
            }
            if (Projectile.ai[1] >= 20f)
            {
                Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
            }
            else
            {
                Projectile.rotation += 0.3f * (float)Projectile.direction;
            }
        }
    }
}
