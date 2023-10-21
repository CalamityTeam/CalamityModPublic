using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class WaterStream2 : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 80;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.15f / 255f, (255 - Projectile.alpha) * 0.25f / 255f);
            Projectile.scale -= 0.002f;
            if (Projectile.scale <= 0f)
            {
                Projectile.Kill();
            }
            if (Projectile.ai[0] <= 3f)
            {
                Projectile.ai[0] += 1f;
                return;
            }
            Projectile.velocity.Y = Projectile.velocity.Y + 0.075f;
            for (int i = 0; i < 3; i++)
            {
                float shortXVel = Projectile.velocity.X / 3f * (float)i;
                float shortYVel = Projectile.velocity.Y / 3f * (float)i;
                int dustPosOffset = 14;
                int waterDust = Dust.NewDust(new Vector2(Projectile.position.X + (float)dustPosOffset, Projectile.position.Y + (float)dustPosOffset), Projectile.width - dustPosOffset * 2, Projectile.height - dustPosOffset * 2, 14, 0f, 0f, 100, new Color(0, 255, 255), 1f);
                Dust dust = Main.dust[waterDust];
                dust.noGravity = true;
                dust.velocity *= 0.1f;
                dust.velocity += Projectile.velocity * 0.5f;
                dust.position.X -= shortXVel;
                dust.position.Y -= shortYVel;
            }
            if (Main.rand.NextBool(8))
            {
                int extraDustPosOffset = 16;
                int extraWaterDust = Dust.NewDust(new Vector2(Projectile.position.X + (float)extraDustPosOffset, Projectile.position.Y + (float)extraDustPosOffset), Projectile.width - extraDustPosOffset * 2, Projectile.height - extraDustPosOffset * 2, 14, 0f, 0f, 100, new Color(0, 255, 255), 0.5f);
                Main.dust[extraWaterDust].velocity *= 0.25f;
                Main.dust[extraWaterDust].velocity += Projectile.velocity * 0.5f;
            }
        }
    }
}
