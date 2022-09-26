using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Ranged
{
    public class PlanetaryAnnihilationProj : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int dustType = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ball");
        }

        public override void SetDefaults()
        {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 1;
            Projectile.timeLeft = 600;
            Projectile.arrow = true;
        }

        public override void AI()
        {
            switch ((int)Projectile.ai[1])
            {
                case 0:
                    dustType = 15;
                    break;
                case 1:
                    dustType = 74;
                    break;
                case 2:
                    dustType = 73;
                    break;
                case 3:
                    dustType = 162;
                    break;
                case 4:
                    dustType = 90;
                    break;
                case 5:
                    dustType = 173;
                    break;
                case 6:
                    dustType = 57;
                    break;
            }
            int num469 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, 2.2f);
            Main.dust[num469].noGravity = true;
            Main.dust[num469].velocity *= 0f;

            CalamityUtils.HomeInOnNPC(Projectile, !Projectile.tileCollide, 200f, 12f, 20f);
        }

        public override void Kill(int timeLeft)
        {
            int height = 40;
            float num50 = 2.1f;
            float num51 = 1.1f;
            float num52 = 2.5f;
            Vector2 value3 = (Projectile.rotation - 1.57079637f).ToRotationVector2();
            Vector2 value4 = value3 * Projectile.velocity.Length() * (float)Projectile.MaxUpdates;
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = height;
            Projectile.Center = Projectile.position;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.damage /= 2;
            Projectile.Damage();
            int num3;
            for (int num53 = 0; num53 < 20; num53 = num3 + 1)
            {
                int num54 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 200, default, num50);
                Main.dust[num54].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                Main.dust[num54].noGravity = true;
                Dust dust = Main.dust[num54];
                dust.velocity *= 3f;
                dust = Main.dust[num54];
                dust.velocity += value4 * Main.rand.NextFloat();
                num54 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 100, default, num51);
                Main.dust[num54].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust = Main.dust[num54];
                dust.velocity *= 2f;
                Main.dust[num54].noGravity = true;
                Main.dust[num54].fadeIn = 1f;
                dust = Main.dust[num54];
                dust.velocity += value4 * Main.rand.NextFloat();
                num3 = num53;
            }
            for (int num55 = 0; num55 < 10; num55 = num3 + 1)
            {
                int num56 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dustType, 0f, 0f, 0, default, num52);
                Main.dust[num56].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 3f;
                Main.dust[num56].noGravity = true;
                Dust dust = Main.dust[num56];
                dust.velocity *= 0.5f;
                dust = Main.dust[num56];
                dust.velocity += value4 * (0.6f + 0.6f * Main.rand.NextFloat());
                num3 = num55;
            }
        }
    }
}
