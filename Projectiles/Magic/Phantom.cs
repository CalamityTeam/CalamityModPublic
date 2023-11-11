using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class Phantom : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 2;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 120;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Projectile.rotation += 0.01f;

            Lighting.AddLight(Projectile.Center, 0.2f, 0.2f, 0.2f);

            for (int i = 0; i < 2; i++)
            {
                int spectre = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 175, 0f, 0f, 100, default, 1f);
                Main.dust[spectre].noGravity = true;
            }

            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 300f, 12f, 20f);
        }
    }
}
