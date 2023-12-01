using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class NightBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.extraUpdates = 20;
            Projectile.friendly = true;
            Projectile.timeLeft = 30;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            for (int i = 0; i < 2; i++)
            {
                Vector2 projPos = Projectile.position;
                projPos -= Projectile.velocity * ((float)i * 0.25f);
                int nightDust = Dust.NewDust(projPos, 1, 1, 27, 0f, 0f, 0, default, 1.25f);
                Main.dust[nightDust].position = projPos;
                Main.dust[nightDust].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                Main.dust[nightDust].velocity *= 0.1f;
            }
        }
    }
}
