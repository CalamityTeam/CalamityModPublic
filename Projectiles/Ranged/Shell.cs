using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Ranged
{
    public class Shell : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shell");
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 18;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 5;
            Projectile.aiStyle = ProjAIStyleID.Arrow;
            Projectile.arrow = true;
            Projectile.timeLeft = 600;
            Projectile.Calamity().pointBlankShotDuration = CalamityGlobalProjectile.DefaultPointBlankDuration;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.9995f;
        }

        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item10, Projectile.position);
            Projectile.position.X = Projectile.position.X + (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y + (float)(Projectile.height / 2);
            Projectile.width = 34;
            Projectile.height = 18;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            for (int num621 = 0; num621 < 10; num621++)
            {
                int num195 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 14, Projectile.oldVelocity.X / 4, Projectile.oldVelocity.Y / 4, 0, new Color(0, 255, 255), 1.5f);
                Main.dust[num195].noGravity = true;
                Main.dust[num195].velocity *= 3f;
            }
        }
    }
}
