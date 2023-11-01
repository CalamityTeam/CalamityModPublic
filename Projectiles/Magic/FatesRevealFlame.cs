using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class FatesRevealFlame : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = true;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            float scaleFactor = 15f;
            if (Projectile.timeLeft > 30 && Projectile.alpha > 0)
            {
                Projectile.alpha -= 25;
            }
            if (Projectile.timeLeft > 30 && Projectile.alpha < 128 && Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
            {
                Projectile.alpha = 128;
            }
            if (Projectile.alpha < 0)
            {
                Projectile.alpha = 0;
            }
            int inc = Projectile.frameCounter + 1;
            Projectile.frameCounter = inc;
            if (inc > 4)
            {
                Projectile.frameCounter = 0;
                inc = Projectile.frame + 1;
                Projectile.frame = inc;
                if (inc >= 4)
                {
                    Projectile.frame = 0;
                }
            }
            float dustScale = 0.5f;
            if (Projectile.timeLeft < 120)
            {
                dustScale = 1.1f;
            }
            if (Projectile.timeLeft < 60)
            {
                dustScale = 1.6f;
            }
            float[] var_2_2A211_cp_0 = Projectile.ai;
            int var_2_2A211_cp_1 = 1;
            float dustIncr = var_2_2A211_cp_0[var_2_2A211_cp_1];
            var_2_2A211_cp_0[var_2_2A211_cp_1] = dustIncr + 1f;
            for (float j = 0f; j < 3f; j = dustIncr + 1f)
            {
                if (Main.rand.Next(3) != 0)
                {
                    return;
                }
                Dust fateful = Main.dust[Dust.NewDust(Projectile.Center, 0, 0, 60, 0f, -2f, 0, default, 1f)];
                fateful.position = Projectile.Center + Vector2.UnitY.RotatedBy((double)(j * 6.28318548f / 3f + Projectile.ai[1]), default) * 10f;
                fateful.noGravity = true;
                fateful.velocity = Projectile.DirectionFrom(fateful.position);
                fateful.scale = dustScale;
                fateful.fadeIn = 0.5f;
                fateful.alpha = 200;
                dustIncr = j;
            }
            if (Projectile.timeLeft < 4)
            {
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 180;
                Projectile.Center = Projectile.position;
                // Ozzatron 15FEB2021 -- What the actual fuck is this
                // projectile.damage = 800;
                for (int i = 0; i < 10; i = inc + 1)
                {
                    Dust fateful = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 60, 0f, -2f, 0, default, 1f)];
                    fateful.noGravity = true;
                    if (fateful.position != Projectile.Center)
                        fateful.velocity = Projectile.SafeDirectionTo(fateful.position) * 3f;

                    inc = i;
                }
            }
            if (Main.player[Projectile.owner].active && !Main.player[Projectile.owner].dead)
            {
                if (Projectile.Distance(Main.player[Projectile.owner].Center) > 160f)
                {
                    Vector2 moveDirection = Projectile.SafeDirectionTo(Main.player[Projectile.owner].Center, Vector2.UnitY);
                    Projectile.velocity = (Projectile.velocity * 9f + moveDirection * scaleFactor) / 10f;
                }
            }
            else
            {
                if (Projectile.timeLeft > 30)
                {
                    Projectile.timeLeft = 30;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft < 30)
            {
                float aboutToDieAlpha = (float)Projectile.timeLeft / 30f;
                Projectile.alpha = (int)(255f - 255f * aboutToDieAlpha);
            }
            return new Color(255 - Projectile.alpha, 255 - Projectile.alpha, 255 - Projectile.alpha, 128 - Projectile.alpha / 2);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);

            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 84;
            Projectile.position.X = Projectile.position.X - (float)(Projectile.width / 2);
            Projectile.position.Y = Projectile.position.Y - (float)(Projectile.height / 2);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.Damage();

            // Dust code
            int inc;
            for (int i = 0; i < 3; i = inc + 1)
            {
                int redFate = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 100, default, 1.5f);
                Main.dust[redFate].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                inc = i;
            }
            for (int j = 0; j < 10; j = inc + 1)
            {
                int redFate2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 0, default, 2.5f);
                Main.dust[redFate2].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                Main.dust[redFate2].noGravity = true;
                Dust dust = Main.dust[redFate2];
                dust.velocity *= 2f;
                inc = j;
            }
            for (int k = 0; k < 5; k = inc + 1)
            {
                int redFate3 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 60, 0f, 0f, 0, default, 1.5f);
                Main.dust[redFate3].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 2f;
                Main.dust[redFate3].noGravity = true;
                Dust dust = Main.dust[redFate3];
                dust.velocity *= 2f;
                inc = k;
            }
        }
    }
}
