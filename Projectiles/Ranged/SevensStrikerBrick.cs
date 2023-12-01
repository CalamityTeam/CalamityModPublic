using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class SevensStrikerBrick : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/ThrowingBrick";

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            //Constant rotation and gravity
            Projectile.rotation += 0.4f * Projectile.direction;
            Projectile.velocity.Y = Projectile.velocity.Y + 0.3f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }
            //Dust trail
            if (Main.rand.NextBool(13))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 22, Projectile.velocity.X * 0.25f, Projectile.velocity.Y * 0.25f, 150, default, 0.9f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item50, Projectile.position);
            //Dust on impact
            int dust_splash = 0;
            while (dust_splash < 9)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 9, -Projectile.velocity.X * 0.15f, -Projectile.velocity.Y * 0.15f, 120, default, 1.5f);
                dust_splash += 1;
            }
        }
    }
}
