using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class ClimaxBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.extraUpdates = 100;
            Projectile.friendly = true;
            Projectile.timeLeft = 90;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Vector2 projPos = Projectile.position;
            projPos -= Projectile.velocity * 0.25f;
            int boltDust = Dust.NewDust(projPos, 1, 1, 206, 0f, 0f, 0, default, 1.25f);
            Main.dust[boltDust].position = projPos;
            Main.dust[boltDust].scale = (float)Main.rand.Next(70, 110) * 0.013f;
            Main.dust[boltDust].velocity *= 0.1f;
        }
    }
}
