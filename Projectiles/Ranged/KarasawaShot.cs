using CalamityMod.Buffs.StatDebuffs;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
namespace CalamityMod.Projectiles.Ranged
{
    public class KarasawaShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int dust1 = 187; //160
        private int dust2 = 229; //187

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 600;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0f, 0.5f, 0.5f);

            int dustTypeOnTimer = dust1;

            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > 15f)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustPos = Projectile.position;
                    dustPos -= Projectile.velocity * ((float)i * 0.25f);
                    Projectile.alpha = 255;
                    int dusty = Dust.NewDust(dustPos, 1, 1, dustTypeOnTimer, 0f, 0f, 0, default, 1f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].position = dustPos;
                    Main.dust[dusty].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[dusty].velocity *= 0.2f;
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
                    Vector2 randVector = new Vector2(5f, 10f);

                    for (int j = 0; j < 2; j++)
                    {
                        int dustType = j == 0 ? dust1 : dust2;
                        Vector2 randDustPos = Vector2.UnitX * -12f;
                        randDustPos = -Vector2.UnitY.RotatedBy((double)(Projectile.ai[0] * 0.1308997f + (float)j * 3.14159274f), default) * randVector * 1.5f;
                        int dusty2 = Dust.NewDust(Projectile.Center, 0, 0, dustType, 0f, 0f, 160, default, 1f);
                        Main.dust[dusty2].scale = 0.75f;
                        Main.dust[dusty2].noGravity = true;
                        Main.dust[dusty2].position = Projectile.Center + randDustPos;
                        Main.dust[dusty2].velocity = Projectile.velocity;
                    }
                }
            }

            if (Projectile.localAI[0] == 15f)
            {
                float angleRandom = 0.1f;

                for (int k = 0; k < 40; k++)
                {
                    float dustSpeed = Main.rand.NextFloat(6.0f, 12.0f);
                    Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                    dustVel = dustVel.RotatedBy(-angleRandom);
                    dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);
                    int randomDustType = Main.rand.NextBool() ? dust1 : dust2;

                    int newDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 200, default, 1.7f);
                    Dust dust = Main.dust[newDust];
                    dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                    dust.noGravity = true;

                    dust.velocity *= randomDustType == dust2 ? 2f : 4f;

                    newDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 100, default, 0.8f);
                    dust.position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;

                    dust.velocity *= randomDustType == dust2 ? 1.33f : 2.66f;

                    dust.noGravity = true;
                    dust.fadeIn = 1f;
                    dust.color = Color.Blue * 0.5f;
                }
                for (int l = 0; l < 20; l++)
                {
                    float dustSpeed = Main.rand.NextFloat(6.0f, 12.0f);
                    Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                    dustVel = dustVel.RotatedBy(-angleRandom);
                    dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);
                    int randomDustType = Main.rand.NextBool() ? dust1 : dust2;

                    int newDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, randomDustType, dustVel.X, dustVel.Y, 0, default, 2f);
                    Dust dust = Main.dust[newDust2];
                    dust.position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default) * (float)Projectile.width / 3f;
                    dust.noGravity = true;

                    dust.velocity *= randomDustType == dust2 ? 0.33f : 0.66f;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            int height = 150;

            SoundEngine.PlaySound(SoundID.NPCDeath43, Projectile.Center);

            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = height;
            Projectile.Center = Projectile.position;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();

            int constant = 36;
            for (int i = 0; i < constant; i++) // 108 dusts
            {
                Vector2 rotate = Vector2.Normalize(Projectile.velocity) * new Vector2((float)Projectile.width / 2f, (float)Projectile.height) * 0.3f;
                rotate = rotate.RotatedBy((double)((float)(i - (constant / 2 - 1)) * 6.28318548f / (float)constant), default) + Projectile.Center;
                Vector2 faceDirection = rotate - Projectile.Center;

                int killDust = Dust.NewDust(rotate + faceDirection, 0, 0, Main.rand.NextBool() ? dust1 : dust2, faceDirection.X * 0.3f, faceDirection.Y * 0.3f, 100, default, 2f);
                Main.dust[killDust].noGravity = true;

                int killDust2 = Dust.NewDust(rotate + faceDirection, 0, 0, Main.rand.NextBool() ? dust1 : dust2, faceDirection.X * 0.2f, faceDirection.Y * 0.2f, 100, default, 2f);
                Main.dust[killDust2].noGravity = true;

                int killDust3 = Dust.NewDust(rotate + faceDirection, 0, 0, Main.rand.NextBool() ? dust1 : dust2, faceDirection.X * 0.1f, faceDirection.Y * 0.1f, 100, default, 2f);
                Main.dust[killDust3].noGravity = true;
            }

            bool random = Main.rand.NextBool();
            float angleStart = Main.rand.NextFloat(0f, MathHelper.TwoPi);
            for (float angle = 0f; angle < MathHelper.TwoPi; angle += 0.05f) // 125 dusts
            {
                random = !random;
                Vector2 velocity = angle.ToRotationVector2() * (2f + (float)(Math.Sin(angleStart + angle * 3f) + 1) * 2.5f) * Main.rand.NextFloat(0.95f, 1.05f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, random ? dust1 : dust2, velocity);
                d.noGravity = true;
                d.customData = 0.025f;
                d.scale = 2f;
            }
        }
    }
}
