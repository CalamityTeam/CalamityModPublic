using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class RelicOfConvergenceCrystal : ModProjectile
    {
        public const int SoundInterval = 15;
        public const int TotalCrystalsToDraw = 3;
        public const int CrystalsDrawTime = 90;
        public const float MaxCrystalOffsetRadius = 80f;
        public const float MaxDustOffsetRadius = 70f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Relic of Convergence");
        }

        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 46;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
            projectile.timeLeft = 240;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (!player.channel)
            {
                projectile.Kill();
                return;
            }
            // Restrict player movement (as a penalty).
            player.velocity.X = MathHelper.Clamp(player.velocity.X, -5f, 5f);
            if (player.velocity.Y < 0f)
            {
                player.velocity.Y = MathHelper.Clamp(player.velocity.Y, -6f, 0f);
            }
            UpdatePlayerVisuals(player);
            // Make a constant "magical" sound.
            if (projectile.soundDelay <= 0)
            {
                Main.PlaySound(SoundID.Item4, projectile.Center);
                projectile.soundDelay = SoundInterval;
            }
            projectile.ai[0]++;
            // Make a sound when fully charged.
            if (projectile.ai[0] == CrystalsDrawTime)
            {
                Main.PlaySound(SoundID.DD2_DarkMageHealImpact, projectile.Center);
            }
            // Create a circle of dust. The circle expands outward at first until it reaches its "destination" radius.
            // Once the circle is at its maximum size, some of the dust moves inward.
            if (projectile.ai[0] >= CrystalsDrawTime)
            {
                GeneratePassiveDust(player);
            }
            if (projectile.timeLeft == 1)
            {
                for (int i = 0; i < 45; i++)
                {
                    Dust dust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.ProfanedFire);
                    dust.velocity = Utils.NextVector2Circular(Main.rand, 28f, 28f);
                    dust.fadeIn = Main.rand.NextFloat(3f, 4f);
                    dust.noGravity = true;
                }
                player.HealEffect(70, false);
                player.statLife += 70;
                if (player.statLife > player.statLifeMax2)
                {
                    player.statLife = player.statLifeMax2;
                }
                Main.PlaySound(SoundID.DD2_DarkMageHealImpact, projectile.Center);
            }
        }

        public void UpdatePlayerVisuals(Player player)
        {
            projectile.Center = player.Center + Vector2.UnitX * 15f * player.direction;

            // The crystal is a holdout projectile, so change the player's variables to reflect that
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
        }

        public void GeneratePassiveDust(Player player)
        {
            float radius = 45f;
            if (projectile.ai[0] < CrystalsDrawTime + 30f)
            {
                radius = MathHelper.Lerp(0f, 45f, (projectile.ai[0] - CrystalsDrawTime) / 30f);
            }
            for (float angle = 0f; angle <= MathHelper.TwoPi; angle += MathHelper.ToRadians(Main.rand.NextFloat(6f, 8f)))
            {
                Dust dust = Dust.NewDustPerfect(projectile.Center + angle.ToRotationVector2() * radius, (int)CalamityDusts.ProfanedFire);
                dust.position = projectile.Center + angle.ToRotationVector2() * radius;
                dust.scale = Main.rand.NextFloat(0.8f, 1.1f);
                dust.noGravity = true;
                dust.velocity = player.velocity;
                if (Main.rand.NextBool(15) || projectile.timeLeft == 1)
                {
                    dust.velocity = projectile.DirectionFrom(dust.position) * Main.rand.NextFloat(4f, 5f);
                    if (projectile.timeLeft == 1)
                        dust.velocity *= 1.7f;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            if (projectile.ai[0] >= CrystalsDrawTime)
                return true;
            float opacity = projectile.ai[0] / CrystalsDrawTime;
            Texture2D crystalTexture = ModContent.GetTexture(Texture);
            for (int i = 0; i < TotalCrystalsToDraw; i++)
            {
                float angle = MathHelper.TwoPi / TotalCrystalsToDraw * i + projectile.ai[0] / 10f;
                float radius = MathHelper.Lerp(MaxCrystalOffsetRadius, 0f, projectile.ai[0] / CrystalsDrawTime);
                Vector2 drawPositionOffset = angle.ToRotationVector2() * radius;
                Vector2 drawPosition = projectile.Center + drawPositionOffset;
                spriteBatch.Draw(crystalTexture,
                                 drawPosition - Main.screenPosition,
                                 null,
                                 Color.White * opacity,
                                 projectile.rotation,
                                 projectile.Size * 0.5f,
                                 projectile.scale,
                                 SpriteEffects.None,
                                 0f);
            }
            return false;
        }
    }
}
