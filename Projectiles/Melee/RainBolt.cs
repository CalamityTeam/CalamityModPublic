using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Melee
{
    public class RainBolt : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
        }

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft < 150 && target.CanBeChasedBy(Projectile);

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item9, Projectile.Center);
                Projectile.localAI[0] += 1f;
            }

            int rainbow = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 66, 0f, 0f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 2f);
            Main.dust[rainbow].noGravity = true;
            Main.dust[rainbow].velocity *= 0f;

            if (Projectile.timeLeft < 150)
                CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 600f, 12f, 20f);
            else
                Projectile.velocity *= 0.95f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
            for (int k = 0; k < 5; k++)
            {
                int rain = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, 66, 0f, 0f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB));
                Main.dust[rain].noGravity = true;
                Main.dust[rain].velocity *= 4f;
            }
        }
    }
}
