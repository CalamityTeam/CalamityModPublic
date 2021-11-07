using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class Vehemence : ModProjectile
    {
        public ref float Time => ref projectile.ai[0];
        public Player Owner => Main.player[projectile.owner];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vehemence");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 32;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 1;
            projectile.timeLeft = 300;
            projectile.magic = true;
        }

        public override void AI()
        {
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(projectile.Center, Color.DarkRed.ToVector3());

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

                Dust brimstoneMagic = Dust.NewDustPerfect(projectile.Center + projectile.velocity * 7f, 27);
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
                Vector2 spawnOffset = new Vector2(helixOffset, 10f).RotatedBy(projectile.rotation);

                Dust brimstoneMagic = Dust.NewDustPerfect(projectile.Center + spawnOffset, (int)CalamityDusts.Brimstone);
                brimstoneMagic.velocity = Vector2.Zero;
                brimstoneMagic.scale = 1.1f;
                brimstoneMagic.noGravity = true;
            }
        }

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(SoundID.Item74, projectile.position);
            if (Main.myPlayer == projectile.owner)
            {
                int damage = (int)(Vehemenc.VehemenceSkullDamage * Owner.MagicDamage());
                for (int i = 0; i < 18; i++)
                    Projectile.NewProjectile(projectile.Center, Main.rand.NextVector2Circular(12f, 12f), ModContent.ProjectileType<VehemenceSkull>(), damage, projectile.knockBack, projectile.owner);
            }

            if (!Main.dedServ)
            {
                for (int i = 0; i < 20; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        Vector2 shootVelocity = projectile.velocity.RotatedBy(MathHelper.Lerp(-0.35f, 0.35f, j / 4f)) * Main.rand.NextFloat(0.75f, 1.1f);
                        Vector2 spawnPosition = projectile.Center + shootVelocity.SafeNormalize(Vector2.Zero) * 10f;

                        Dust blood = Dust.NewDustPerfect(spawnPosition, DustID.Blood);
                        blood.velocity = shootVelocity;
                        blood.scale = MathHelper.Lerp(1.7f, 0.85f, i / 20f);
                    }
                }
                for (int i = 0; i < 60; i++)
                {
                    Dust brimstoneMagic = Dust.NewDustPerfect(projectile.Center, Main.rand.NextBool(2) ? (int)CalamityDusts.Brimstone : 27);
                    brimstoneMagic.velocity = Main.rand.NextVector2Circular(18f, 18f);
                    brimstoneMagic.scale = 1.7f;
                    brimstoneMagic.noGravity = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (target.life == target.lifeMax)
                target.AddBuff(ModContent.BuffType<DemonFlames>(), 18000);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }
    }
}
