using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PlasmaCasterShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int dust1 = 107; //160
        private int dust2 = 110; //187

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.MaxUpdates = 7;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0.6f, 0f);

            int dustTypeOnTimer = dust1;

            float createDustVar = 10f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > createDustVar)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustRotation = Projectile.position;
                    dustRotation -= Projectile.velocity * (i * 0.25f);
                    int dustSpawn = Dust.NewDust(dustRotation, 1, 1, dustTypeOnTimer, 0f, 0f, 0, default, 1f);
                    Main.dust[dustSpawn].noGravity = true;
                    Main.dust[dustSpawn].position = dustRotation;
                    Main.dust[dustSpawn].scale = Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[dustSpawn].velocity *= 0.2f;
                }

                Projectile.ai[0] += 1f;
                if (Projectile.ai[0] == 48f)
                {
                    Projectile.ai[0] = 0f;

                    if (dustTypeOnTimer == dust1)
                        dustTypeOnTimer = dust2;
                    else
                        dustTypeOnTimer = dust1;
                }
                else
                {
                    Vector2 dustRotateVector = new Vector2(5f, 10f);

                    for (int j = 0; j < 2; j++)
                    {
                        Vector2 dustRotate = Vector2.UnitX * -12f;
                        dustRotate = -Vector2.UnitY.RotatedBy(Projectile.ai[0] * 0.1308997f + j * MathHelper.Pi) * dustRotateVector * 0.75f;
                        int plasmaDust = Dust.NewDust(Projectile.Center, 0, 0, dust1, 0f, 0f, 160, default, 1f);
                        Main.dust[plasmaDust].scale = 0.6f;
                        Main.dust[plasmaDust].noGravity = true;
                        Main.dust[plasmaDust].position = Projectile.Center + dustRotate;
                        Main.dust[plasmaDust].velocity = Projectile.velocity;
                    }

                    for (int j = 0; j < 2; j++)
                    {
                        Vector2 dustRotate = Vector2.UnitX * -12f;
                        dustRotate = -Vector2.UnitY.RotatedBy(Projectile.ai[0] * 0.1308997f + j * MathHelper.Pi) * dustRotateVector * 1.5f;
                        int plasmaDust = Dust.NewDust(Projectile.Center, 0, 0, dust2, 0f, 0f, 160, default, 1f);
                        Main.dust[plasmaDust].scale = 0.6f;
                        Main.dust[plasmaDust].noGravity = true;
                        Main.dust[plasmaDust].position = Projectile.Center + dustRotate;
                        Main.dust[plasmaDust].velocity = Projectile.velocity;
                    }
                }
            }

            if (Projectile.localAI[0] == createDustVar)
                PlasmaBurst(1f, 1.6f);
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.ExpandHitboxBy(240);
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();

            SoundEngine.PlaySound(SoundID.Item93, Projectile.Center);

            PlasmaBurst(1.8f, 3.6f); // 60 dusts

            for (int i = 0; i < 400; i++)
            {
                float dustScale = 16f;
                if (i < 300)
                    dustScale = 12f;
                if (i < 200)
                    dustScale = 8f;
                if (i < 100)
                    dustScale = 4f;

                int deathDust = Dust.NewDust(Projectile.Center, 6, 6, Main.rand.NextBool() ? dust1 : dust2, 0f, 0f, 100, default, 1f);
                float deathDustX = Main.dust[deathDust].velocity.X;
                float deathDustY = Main.dust[deathDust].velocity.Y;

                if (deathDustX == 0f && deathDustY == 0f)
                    deathDustX = 1f;

                float deathDustVel = (float)Math.Sqrt(deathDustX * deathDustX + deathDustY * deathDustY);
                deathDustVel = dustScale / deathDustVel;
                deathDustX *= deathDustVel;
                deathDustY *= deathDustVel;

                float scale = 1f;
                switch ((int)dustScale)
                {
                    case 4:
                        scale = 1.2f;
                        break;
                    case 8:
                        scale = 1.1f;
                        break;
                    case 12:
                        scale = 1f;
                        break;
                    case 16:
                        scale = 0.9f;
                        break;
                    default:
                        break;
                }

                Dust dust = Main.dust[deathDust];
                dust.velocity *= 0.5f;
                dust.velocity.X = dust.velocity.X + deathDustX;
                dust.velocity.Y = dust.velocity.Y + deathDustY;
                dust.scale = scale;
                dust.noGravity = true;
            }
        }

        private void PlasmaBurst(float speed1, float speed2)
        {
            float angleRandom = 0.35f;

            for (int i = 0; i < 40; i++)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);
                int randomDustType = Main.rand.Next(2) == 0 ? dust1 : dust2;

                int plasmaBurstDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 200, default, 1.7f);
                Main.dust[plasmaBurstDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                Main.dust[plasmaBurstDust].noGravity = true;

                Dust dust = Main.dust[plasmaBurstDust];
                dust.velocity *= 3f;
                dust = Main.dust[plasmaBurstDust];

                plasmaBurstDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 100, default, 0.8f);
                Main.dust[plasmaBurstDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;

                dust = Main.dust[plasmaBurstDust];
                dust.velocity *= 2f;

                Main.dust[plasmaBurstDust].noGravity = true;
                Main.dust[plasmaBurstDust].fadeIn = 1f;
                Main.dust[plasmaBurstDust].color = Color.Green * 0.5f;

                dust = Main.dust[plasmaBurstDust];
            }
            for (int j = 0; j < 20; j++)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);
                int randomDustType = Main.rand.Next(2) == 0 ? dust1 : dust2;

                int plasmaBurstDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 0, default, 2f);
                Main.dust[plasmaBurstDust2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 3f;
                Main.dust[plasmaBurstDust2].noGravity = true;

                Dust dust = Main.dust[plasmaBurstDust2];
                dust.velocity *= 0.5f;
                dust = Main.dust[plasmaBurstDust2];
            }
        }
    }
}
