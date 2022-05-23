using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Rogue
{
    public class BrickFragment : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brick Fragment");
            Main.projFrames[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = 13;
            Projectile.height = 13;
            Projectile.DamageType = RogueDamageClass.Instance;
        }
        public override void AI()
        {
            //Choose the sprite of the projectile
            if (Projectile.ai[0] != 1)
            {
                Projectile.frame = Main.rand.Next(2);
                Projectile.ai[0] += 1;
            }
            //Rotation and gravity
            Projectile.rotation += 0.6f * Projectile.direction;
            Projectile.velocity.Y = Projectile.velocity.Y + 0.27f;
            if (Projectile.velocity.Y > 16f)
            {
                Projectile.velocity.Y = 16f;
            }

        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, (int)Projectile.position.X, (int)Projectile.position.Y);
            //Dust effect
            int splash = 0;
            while (splash < 4)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 9, -Projectile.velocity.X * 0.15f, -Projectile.velocity.Y * 0.10f, 150, default, 0.9f);
                splash += 1;
            }
        }
    }
}
