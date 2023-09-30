using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.Projectiles.Magic
{
    public class Vehemence : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Magic";
        public ref float Time => ref Projectile.ai[0];
        public Player Owner => Main.player[Projectile.owner];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 300;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, Color.DarkRed.ToVector3());

            if (Time == 0f)
                GenerateInitialBurstDust();

            GenerateHelicalDust();
            Time++;
        }

        private void GenerateInitialBurstDust()
        {
            if (Main.dedServ)
                return;

            for (int i = 0; i < 40; i++)
            {
                float angle = MathHelper.TwoPi * i / 40f;

                Dust brimstoneMagic = Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * 7f, 27);
                brimstoneMagic.velocity = angle.ToRotationVector2() * 15f;
                brimstoneMagic.color = Color.Lerp(Color.Red, Color.MediumPurple, (float)Math.Sin(angle) * 0.5f + 0.5f);
                brimstoneMagic.scale = 1.6f;
                brimstoneMagic.noGravity = true;
            }
        }

        private void GenerateHelicalDust()
        {
            if (Main.dedServ)
                return;

            // Helical brimstone dust from the back of the projectile.
            for (int i = -1; i <= 1; i += 2)
            {
                float helixOffset = (float)Math.Sin(Time / 45f * MathHelper.TwoPi) * i * 8f;
                Vector2 spawnOffset = new Vector2(helixOffset, 10f).RotatedBy(Projectile.rotation);

                Dust brimstoneMagic = Dust.NewDustPerfect(Projectile.Center + spawnOffset, (int)CalamityDusts.Brimstone);
                brimstoneMagic.velocity = Vector2.Zero;
                brimstoneMagic.scale = 1.1f;
                brimstoneMagic.noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item74, Projectile.position);
            if (Main.myPlayer == Projectile.owner)
            {
                int skullID = ModContent.ProjectileType<VehemenceSkull>();
                int damage = (int)(Projectile.damage * Items.Weapons.Magic.Vehemence.SkullRatio);
                for (int i = 0; i < 18; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2Circular(12f, 12f), skullID, damage, Projectile.knockBack, Projectile.owner);
            }

            if (!Main.dedServ)
            {
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 shootVelocity = Projectile.velocity.RotatedBy(MathHelper.Lerp(-0.35f, 0.35f, j / 4f)) * Main.rand.NextFloat(0.75f, 1.1f);
                        Vector2 spawnPosition = Projectile.Center + shootVelocity.SafeNormalize(Vector2.Zero) * 10f;

                        Dust blood = Dust.NewDustPerfect(spawnPosition, DustID.Blood);
                        blood.velocity = shootVelocity;
                        blood.scale = MathHelper.Lerp(1.7f, 0.85f, i / 20f);
                    }
                }
                for (int i = 0; i < 60; i++)
                {
                    Dust brimstoneMagic = Dust.NewDustPerfect(Projectile.Center, Main.rand.NextBool() ? (int)CalamityDusts.Brimstone : 27);
                    brimstoneMagic.velocity = Main.rand.NextVector2Circular(18f, 18f);
                    brimstoneMagic.scale = 1.7f;
                    brimstoneMagic.noGravity = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 2);
            return false;
        }
    }
}
