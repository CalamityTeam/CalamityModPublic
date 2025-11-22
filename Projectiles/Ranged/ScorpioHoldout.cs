using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using static CalamityMod.Items.Weapons.Ranged.Scorpio;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Ranged
{
    public class ScorpioHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ItemType<Scorpio>();
        public override Vector2 GunTipPosition => base.GunTipPosition - Vector2.UnitY.RotatedBy(Projectile.rotation) * 10f * Projectile.spriteDirection * Owner.gravDir;
        public override float RecoilResolveSpeed => 0.1f;
        public override float MaxOffsetLengthFromArm => 15f;
        public override float OffsetXUpwards => -12f;
        public override float BaseOffsetY => -10f;
        public override float OffsetYDownwards => 10f;

        public ref float ShootingTimer => ref Projectile.ai[0];
        public ref float TimerBetweenBursts => ref Projectile.ai[1];
        public ref float ChargeLV => ref Projectile.ai[2];

        // The type of dust used in the effects and the color of the particles that it'll use depending on the type of rocket used.
        // It'll carry over to the projectiles that it shoots.
        public static int DustEffectsID { get; set; }
        public static Color EffectsColor { get; set; }
        public static Color StaticEffectsColor { get; set; } = Color.Turquoise;

        public override void HoldoutAI()
        {
            // If the player's pressing RMB, it'll shoot the big rocket.
            if (Owner.Calamity().mouseRight && ChargeLV >= 3)
            {
                ShootRocket(HeldItem, true);
                ShootingTimer = HeldItem.useAnimation; // If you time your nukes well, you can reset the burst rocket cooldown
                ChargeLV = -1;
            }

            if (ShootingTimer >= HeldItem.useAnimation)
            {
                if (ShootingTimer == HeldItem.useAnimation && ChargeLV < 3)
                    ChargeLV++;

                // Shooting the burst of small rockets.
                // The time is adapted to the speed modifier from the reforge.
                int adaptedTimeBetweenBursts = TimeBetweenBursts * HeldItem.useAnimation / OriginalUseTime;

                if (ShootingTimer % adaptedTimeBetweenBursts == 0f)
                    ShootRocket(HeldItem, false);

                // Resets the timers when everything has been shot.
                if (ShootingTimer >= HeldItem.useAnimation + adaptedTimeBetweenBursts * (ProjectilesPerBurst - 1))
                {
                    ShootingTimer = 0f;
                    TimerBetweenBursts = 0f;
                }
            }

            ShootingTimer++;

            if (Main.dedServ)
                return;

            // Charge level visuals here
            if (ChargeLV == 1 && ShootingTimer % 3 == 0)
            {
                Vector2 position = GunTipPosition - Projectile.velocity * 95;
                Vector2 velocity = (-Projectile.velocity * 5).RotatedByRandom(0.3f) * Main.rand.NextFloat(0.6f, 1.4f);
                SquishyLightParticle energy = new(position, velocity, Main.rand.NextFloat(0.12f, 0.14f), StaticEffectsColor, Main.rand.Next(3, 5 + 1), 1, 1.5f);
                GeneralParticleHandler.SpawnParticle(energy);
                Dust dust = Dust.NewDustPerfect(position, DustEffectsID, velocity, 0, default, Main.rand.NextFloat(1.2f, 1.7f));
                dust.noGravity = true;
            }
            if (ChargeLV == 2 & ShootingTimer % 2 == 0)
            {
                Vector2 position = GunTipPosition - Projectile.velocity * 95;
                Vector2 velocity = (-Projectile.velocity * 5).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.9f, 1.9f);
                SquishyLightParticle energy = new(position, velocity, Main.rand.NextFloat(0.18f, 0.22f), StaticEffectsColor, Main.rand.Next(3, 5 + 1), 1, 1.5f);
                GeneralParticleHandler.SpawnParticle(energy);
                Dust dust = Dust.NewDustPerfect(position, DustEffectsID, velocity, 0, default, Main.rand.NextFloat(1.2f, 1.7f));
                dust.noGravity = true;
            }
            if (ChargeLV >= 3)
            {
                Vector2 position = GunTipPosition - Projectile.velocity * 95;
                Vector2 velocity = (-Projectile.velocity * 5).RotatedByRandom(0.75f) * Main.rand.NextFloat(1.2f, 2.3f);
                SquishyLightParticle energy = new(position, velocity, Main.rand.NextFloat(0.24f, 0.34f), StaticEffectsColor, Main.rand.Next(3, 5 + 1), 1, 1.5f);
                GeneralParticleHandler.SpawnParticle(energy);
                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(position, DustEffectsID, velocity, 0, default, Main.rand.NextFloat(1.6f, 2.1f));
                    dust.noGravity = true;
                }
            }
            if (ChargeLV == 0 && ShootingTimer % 3 == 0)
            {
                Vector2 position = GunTipPosition - Projectile.velocity * 95;
                Vector2 velocity = (-Projectile.velocity * 4).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.9f, 2f);
                Particle smoke = new HeavySmokeParticle(position, velocity, Color.SlateGray, Main.rand.Next(40, 60 + 1), Main.rand.NextFloat(0.3f, 0.6f), 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), true, required: true);
                GeneralParticleHandler.SpawnParticle(smoke);

                for (int i = 0; i < 2; i++)
                {
                    Dust dust = Dust.NewDustPerfect(position, 303, velocity * 0.5f, 200, default, Main.rand.NextFloat(0.9f, 1.3f));
                    dust.noGravity = false;
                }
            }
        }

        public void ShootRocket(Item item, bool isRMB)
        {
            // We use the velocity of this projectile as its direction vector.
            Vector2 projectileVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);

            // Every time we shoot we use ammo.
            // With this method we also use the item's stats, like the shoot speed, or the type of ammo it was used.
            Owner.PickAmmo(item, out _, out float projSpeed, out int damage, out float knockback, out int rocketType, Main.rand.Next(100) > 70); // 70% ammo conservation

            // Decides the color of the effects depending on the type of rocket used.
            switch (rocketType)
            {
                case ItemID.WetRocket:
                    DustEffectsID = 45;
                    EffectsColor = Color.RoyalBlue;
                    break;
                case ItemID.LavaRocket:
                    DustEffectsID = DustID.Torch;
                    EffectsColor = Color.Red;
                    break;
                case ItemID.HoneyRocket:
                    DustEffectsID = DustID.Honey;
                    EffectsColor = Color.Yellow;
                    break;
                default:
                    DustEffectsID = 302;
                    EffectsColor = Color.Aquamarine;
                    break;
            }

            if (Main.myPlayer == Projectile.owner)
            {
                // Spawns the projectile.
                Projectile.NewProjectileDirect(
                    Projectile.GetSource_FromThis(),
                    GunTipPosition,
                    projectileVelocity.RotatedByRandom(isRMB ? 0f : MathHelper.PiOver4) * projSpeed * (isRMB ? 1f : Main.rand.NextFloat(0.8f, 1f)),
                    isRMB ? ProjectileType<ScorpioLargeRocket>() : ProjectileType<ScorpioRocket>(),
                    damage,
                    knockback,
                    Projectile.owner,
                    rocketType,
                    projSpeed);
            }

            // Inside here go all the things that dedicated servers shouldn't spend resources on.
            // Like visuals and sounds.
            if (Main.dedServ)
                return;

            SoundStyle RightClickSound = new("CalamityMod/Sounds/Item/RealityRuptureStealth") { Volume = 0.45f };
            if (isRMB)
                SoundEngine.PlaySound(RightClickSound with { Pitch = -0.1f }, Projectile.Center);
            else
                SoundEngine.PlaySound(RocketShoot with { Pitch = ChargeLV * 0.055f }, Projectile.Center);

            // By decreasing the offset length of the gun from the arms, we give an effect of recoil.
            OffsetLengthFromArm -= isRMB ? 30f : 5f;

            int dustAmount = Main.rand.Next(10, 15 + 1);
            for (int i = 0; i < dustAmount; i++)
            {
                Dust shootDust = Dust.NewDustPerfect(
                    GunTipPosition,
                    DustEffectsID,
                    projectileVelocity.RotatedByRandom(MathHelper.PiOver2 - MathHelper.ToRadians(15f)) * Main.rand.NextFloat(6f, 10f));
                shootDust.noGravity = true;
                shootDust.noLight = true;
                shootDust.noLightEmittence = true;
            }

            Particle shootPulse = new DirectionalPulseRing(
                GunTipPosition,
                Vector2.Zero,
                Color.Gray * 0.7f,
                new Vector2(0.5f, 1f),
                Projectile.rotation,
                0.1f,
                0.4f,
                20);
            GeneralParticleHandler.SpawnParticle(shootPulse);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowTexture = Request<Texture2D>(GlowTexture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Projectile.GetAlpha(lightColor);
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, drawRotation, rotationPoint, Projectile.scale * Owner.gravDir, flipSprite);
            Main.EntitySpriteDraw(glowTexture, drawPosition, null, Color.White, drawRotation, rotationPoint, Projectile.scale * Owner.gravDir, flipSprite);

            return false;
        }
    }
}
