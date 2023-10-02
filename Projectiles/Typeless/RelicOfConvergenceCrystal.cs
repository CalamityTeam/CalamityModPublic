using CalamityMod.Dusts;
using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
    public class RelicOfConvergenceCrystal : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<RelicOfConvergence>();
        public const int SoundInterval = 15;
        public const int TotalCrystalsToDraw = 3;
        public const int CrystalsDrawTime = 90;
        public const float MaxCrystalOffsetRadius = 80f;
        public const float MaxDustOffsetRadius = 70f;
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 46;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 240;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (!player.channel)
            {
                Projectile.Kill();
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
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);
                Projectile.soundDelay = SoundInterval;
            }
            Projectile.ai[0]++;
            // Make a sound when fully charged.
            if (Projectile.ai[0] == CrystalsDrawTime)
            {
                SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, Projectile.Center);
            }
            // Create a circle of dust. The circle expands outward at first until it reaches its "destination" radius.
            // Once the circle is at its maximum size, some of the dust moves inward.
            if (Projectile.ai[0] >= CrystalsDrawTime)
            {
                GeneratePassiveDust(player);
            }
            if (Projectile.timeLeft == 1)
            {
                for (int i = 0; i < 45; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, (int)CalamityDusts.ProfanedFire);
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
                SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, Projectile.Center);
            }
        }

        public void UpdatePlayerVisuals(Player player)
        {
            Projectile.Center = player.Center + Vector2.UnitX * 15f * player.direction;

            // The crystal is a holdout projectile, so change the player's variables to reflect that
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
        }

        public void GeneratePassiveDust(Player player)
        {
            float radius = 45f;
            if (Projectile.ai[0] < CrystalsDrawTime + 30f)
            {
                radius = MathHelper.Lerp(0f, 45f, (Projectile.ai[0] - CrystalsDrawTime) / 30f);
            }
            for (float angle = 0f; angle <= MathHelper.TwoPi; angle += MathHelper.ToRadians(Main.rand.NextFloat(6f, 8f)))
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + angle.ToRotationVector2() * radius, (int)CalamityDusts.ProfanedFire);
                dust.position = Projectile.Center + angle.ToRotationVector2() * radius;
                dust.scale = Main.rand.NextFloat(0.8f, 1.1f);
                dust.noGravity = true;
                dust.velocity = player.velocity;
                if (Main.rand.NextBool(15) || Projectile.timeLeft == 1)
                {
                    dust.velocity = Projectile.DirectionFrom(dust.position) * Main.rand.NextFloat(4f, 5f);
                    if (Projectile.timeLeft == 1)
                        dust.velocity *= 1.7f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] >= CrystalsDrawTime)
                return true;
            float opacity = Projectile.ai[0] / CrystalsDrawTime;
            Texture2D crystalTexture = ModContent.Request<Texture2D>(Texture).Value;
            for (int i = 0; i < TotalCrystalsToDraw; i++)
            {
                float angle = MathHelper.TwoPi / TotalCrystalsToDraw * i + Projectile.ai[0] / 10f;
                float radius = MathHelper.Lerp(MaxCrystalOffsetRadius, 0f, Projectile.ai[0] / CrystalsDrawTime);
                Vector2 drawPositionOffset = angle.ToRotationVector2() * radius;
                Vector2 drawPosition = Projectile.Center + drawPositionOffset;
                Main.EntitySpriteDraw(crystalTexture,
                                 drawPosition - Main.screenPosition,
                                 null,
                                 Color.White * opacity,
                                 Projectile.rotation,
                                 Projectile.Size * 0.5f,
                                 Projectile.scale,
                                 SpriteEffects.None,
                                 0);
            }
            return false;
        }
    }
}
