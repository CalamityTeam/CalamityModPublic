using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Ranged
{
    public class TerraArrowMain : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Items/Ammo/TerraArrow";

        private bool initialized = false;
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.arrow = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            if (!initialized)
            {
                Projectile.velocity *= 0.5f;
                initialized = true;
            }
            if (Projectile.FinalExtraUpdate() && initialized)
            {
                Projectile.velocity *= 1.003f;
                if (Projectile.velocity.Length() >= 22f)
                {
                    Projectile.Kill();
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(32);
            SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
            for (int d = 0; d < 3; d++)
            {
                int terra = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 107, 0f, 0f, 100, default, 2f);
                Main.dust[terra].velocity *= 1.2f;
                if (Main.rand.NextBool())
                {
                    Main.dust[terra].scale = 0.5f;
                    Main.dust[terra].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            if (Projectile.owner == Main.myPlayer)
            {
                for (int a = 0; a < 2; a++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<TerraArrowSplit>(), (int)(Projectile.damage * 0.25), 0f, Projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
