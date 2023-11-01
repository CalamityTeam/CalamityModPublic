using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Melee
{
    public class AegisBeam : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 120;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Melee;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.25f, 0.25f, 0f);
            Projectile.rotation += 1f;
            Projectile.alpha -= 25;
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            if (Projectile.localAI[0] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item73, Projectile.position);
                Projectile.localAI[0] += 1f;
            }
            int goldDust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 246, 0f, 0f, 100, new Color(255, Main.DiscoG, 53), 0.8f);
            Main.dust[goldDust].noGravity = true;
            Main.dust[goldDust].velocity *= 0.5f;
            Main.dust[goldDust].velocity += Projectile.velocity * 0.1f;
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 64;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
            for (int d = 0; d <= 30; d++)
            {
                float rando = (float)Main.rand.Next(-10, 11);
                float rando2 = (float)Main.rand.Next(-10, 11);
                float speed = (float)Main.rand.Next(3, 9);
                float randoAdjuster = (float)Math.Sqrt((double)(rando * rando + rando2 * rando2));
                randoAdjuster = speed / randoAdjuster;
                rando *= randoAdjuster;
                rando2 *= randoAdjuster;
                int deathDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 246, 0f, 0f, 100, new Color(255, Main.DiscoG, 53), 1.2f);
                Dust dust = Main.dust[deathDust];
                dust.noGravity = true;
                dust.position.X = Projectile.Center.X;
                dust.position.Y = Projectile.Center.Y;
                dust.position.X += (float)Main.rand.Next(-10, 11);
                dust.position.Y += (float)Main.rand.Next(-10, 11);
                dust.velocity.X = rando;
                dust.velocity.Y = rando2;
            }
            int flameAmt = Main.rand.Next(2, 4);
            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < flameAmt; i++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, ModContent.ProjectileType<AegisFlame>(), (int)(Projectile.damage * 0.75), 0f, Projectile.owner, 0f, 0f);
                }
            }
        }
    }
}
