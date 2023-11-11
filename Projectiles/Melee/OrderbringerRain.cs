using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class OrderbringerRain : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.extraUpdates = 2;
            Projectile.penetrate = 2;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            if (Projectile.ai[1] == 0f)
            {
                Projectile.ai[1] = 1f;
                SoundEngine.PlaySound(SoundID.Item125, Projectile.Center);
            }

            Lighting.AddLight((int)Projectile.Center.X / 16, (int)Projectile.Center.Y / 16, (float)Main.DiscoR / 200f, (float)Main.DiscoG / 200f, (float)Main.DiscoB / 200f);

            for (int i = 0; i < 2; i++)
            {
                int rainbow = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 66, 0f, 0f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
                Main.dust[rainbow].noGravity = true;
                Main.dust[rainbow].velocity *= 0.5f;
                Main.dust[rainbow].velocity += Projectile.velocity * 0.1f;
            }

            CalamityUtils.HomeInOnNPC(Projectile, true, 200f, 12f, 20f);
        }
    }
}
