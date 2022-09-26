using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class SevensStrikerCherry : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cherry");
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 480;
        }

        public override void AI()
        {
            Projectile.rotation += 0.1f * Projectile.direction;
        }

        public override void Kill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                for (int f = 0; f < 3; f++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<SevensStrikerCherrySplit>(), (int)(Projectile.damage * 0.4), 0f, Projectile.owner);
                }
            }
            SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.position);
        }
    }
}
