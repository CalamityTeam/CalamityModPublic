using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static CalamityMod.Items.Weapons.Ranged.TheHive;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Ranged
{
    public class TheHiveHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ItemType<TheHive>();
        public override float RecoilResolveSpeed => 0.1f;
        public override float MaxOffsetLengthFromArm => 15f;
        public override float OffsetXUpwards => -12f;
        public override float BaseOffsetY => -10f;
        public override float OffsetYUpwards => 15f;

        // The type of dust used in the effects and the color of the particles that it'll use depending on the type of rocket used.
        // It'll carry over to the projectiles that it shoots.
        public static int DustEffectsID { get; set; }
        public static Color EffectsColor { get; set; }
        public static Color StaticEffectsColor { get; set; } = Color.Lime;

        public ref float ShootingTimer => ref Projectile.ai[0];
        public ref float PostFireCooldown => ref Projectile.ai[1];
        public bool HasLetGo
        {
            get => Projectile.ai[2] == 1f;
            set => Projectile.ai[2] = value == true ? 1f : 0f;
        }

        public bool FireNuke => ShootingTimer > MaxCharge;
        public SlotId HiveHum { get; set; }

        public override void KillHoldoutLogic()
        {
            if (HasLetGo)
                PostFiringCooldown();
        }

        public override void HoldoutAI()
        {
            // If there's no player, or the player is the server, or the owner's stunned, there'll be no holdout.
            if (Owner.CantUseHoldout() && !HasLetGo)
            {
                if (SoundEngine.TryGetActiveSound(HiveHum, out var hum) && hum.IsPlaying)
                {
                    hum?.Stop();
                }
                if (HeldItem.type == ItemType<TheHive>())
                    ShootRocket();
                HasLetGo = true;
            }

            if (!HasLetGo)
            {
                ShootingTimer++;
                if (SoundEngine.TryGetActiveSound(HiveHum, out var hum) && hum.IsPlaying)
                {
                    hum.Position = Projectile.Center;
                    hum.Pitch = MathHelper.Lerp(0f, 0.8f, Utils.GetLerpValue(0f, MaxCharge, ShootingTimer, true));
                }
            }

            // Inside here go all the things that dedicated servers shouldn't spend resources on.
            // Like visuals and sounds.
            if (Main.dedServ)
                return;

            Vector2 shootDirection = Projectile.velocity.SafeNormalize(Vector2.Zero) * 20;
            if (ShootingTimer >= MaxCharge && !HasLetGo)
            {
                for (int k = 0; k < 2; k++)
                {
                    GlowOrbParticle spark = new GlowOrbParticle(GunTipPosition + Projectile.velocity * 1.5f, Vector2.Zero, false, 2, Main.rand.NextFloat(0.5f, 1.1f), Color.Red);
                    GeneralParticleHandler.SpawnParticle(spark);
                }
                for (int e = 0; e < 2; e++)
                {
                    Dust dust2 = Dust.NewDustPerfect(GunTipPosition + Projectile.velocity * 1.5f, 90, shootDirection * Main.rand.NextFloat(0.01f, 0.8f));
                    dust2.scale = Main.rand.NextFloat(0.45f, 0.75f);
                    dust2.noGravity = true;
                }
            }

            if (ShootingTimer == MaxCharge && !HasLetGo)
            {
                SoundStyle fullCharge = new("CalamityMod/Sounds/Custom/PlagueSounds/PBGAttackSwitchShort");
                SoundEngine.PlaySound(fullCharge with { Volume = 0.9f }, Projectile.Center);
                for (int k = 0; k < 15; k++)
                {
                    Dust dust2 = Dust.NewDustPerfect(GunTipPosition, 90, new Vector2(4, 4).RotatedByRandom(100) * Main.rand.NextFloat(0.05f, 0.8f));
                    dust2.scale = Main.rand.NextFloat(0.85f, 1.15f);
                    dust2.noGravity = true;
                }
            }
        }

        public void ShootRocket()
        {
            // We use the velocity of this projectile as its direction vector.
            Vector2 shootDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
            float VelocityMultiplier = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0f, MaxCharge, ShootingTimer, true));

            // Every time we shoot we use ammo.
            // With this method we also use the item's stats, like the shoot speed, or the type of ammo it was used.
            Owner.PickAmmo(HeldItem, out _, out float projSpeed, out int damage, out float knockback, out int rocketType);

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
                    DustEffectsID = 131;
                    EffectsColor = Color.LawnGreen;
                    break;
            }

            // Spawns the projectile.
            if (FireNuke)
            {
                SoundStyle fire = new("CalamityMod/Sounds/Custom/PlagueSounds/PBGBarrageLaunch");
                SoundEngine.PlaySound(fire with { Volume = 0.5f, Pitch = 0.1f }, Projectile.Center);

                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GunTipPosition, shootDirection * projSpeed * 0.3f, ProjectileType<HiveNuke>(), damage * 10, knockback, Projectile.owner, rocketType);

                PostFireCooldown = 75;
            }
            else
            {
                SoundStyle fire = new("CalamityMod/Sounds/Custom/PlagueSounds/PBGBarrageLaunch");
                SoundEngine.PlaySound(fire with { Volume = 0.4f, Pitch = 0.7f }, Projectile.Center);

                if (Main.myPlayer == Projectile.owner)
                {
                    int numProj = 4;
                    float rotation = MathHelper.ToRadians(MathHelper.Clamp(35 - VelocityMultiplier * 26, 2, 25));
                    for (int i = 0; i < numProj; i++)
                    {
                        Vector2 perturbedSpeed = (shootDirection).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                        Projectile.NewProjectileDirect(
                        Projectile.GetSource_FromThis(),
                        GunTipPosition,
                        perturbedSpeed * projSpeed * VelocityMultiplier,
                        ProjectileType<HiveMissile>(),
                        damage,
                        knockback,
                        Projectile.owner,
                        rocketType);
                    }
                }
                PostFireCooldown = 30;
            }

            // Inside here go all the things that dedicated servers shouldn't spend resources on.
            // Like visuals and sounds.
            if (Main.dedServ)
                return;

            if (FireNuke)
            {
                for (int k = 0; k < 10; k++)
                {
                    float pulseScale = Main.rand.NextFloat(0.35f, 0.55f);
                    DirectionalPulseRing pulse = new DirectionalPulseRing(GunTipPosition, (shootDirection * 25).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.5f, 1.2f), (Main.rand.NextBool(3) ? EffectsColor : StaticEffectsColor) * 0.8f, new Vector2(1, 1), pulseScale - 0.25f, pulseScale, 0f, 20);
                    GeneralParticleHandler.SpawnParticle(pulse);
                }
                for (int i = 0; i <= 15; i++)
                {
                    Dust dust = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? DustEffectsID : 303, (shootDirection * 20).RotatedByRandom(0.3f) * Main.rand.NextFloat(0.5f, 1.2f), 0, default, Main.rand.NextFloat(0.5f, 0.9f));
                    dust.noGravity = false;
                    if (dust.type != DustEffectsID)
                        dust.color = Main.rand.NextBool(3) ? EffectsColor : StaticEffectsColor;
                }
            }
            else
            {
                for (int k = 0; k < 6; k++)
                {
                    float pulseScale = Main.rand.NextFloat(0.2f, 0.4f);
                    DirectionalPulseRing pulse = new DirectionalPulseRing(GunTipPosition, (shootDirection * 20).RotatedByRandom(0.25f) * Main.rand.NextFloat(0.5f, 1.2f), (Main.rand.NextBool(3) ? EffectsColor : StaticEffectsColor) * 0.8f, new Vector2(1, 1), pulseScale - 0.25f, pulseScale, 0f, 20);
                    GeneralParticleHandler.SpawnParticle(pulse);
                }
            }

            // By decreasing the offset length of the gun from the arms, we give an effect of recoil.
            OffsetLengthFromArm -= FireNuke ? 16f : 6f;
        }

        public void PostFiringCooldown()
        {
            Owner.channel = true;
            if (PostFireCooldown > 0)
            {
                PostFireCooldown--;
                Vector2 smokeVel = new Vector2(0, -8) * Main.rand.NextFloat(0.1f, 1.1f);
                Particle smoke = new HeavySmokeParticle(GunTipPosition, smokeVel, StaticEffectsColor, Main.rand.Next(40, 60 + 1), Main.rand.NextFloat(0.3f, 0.6f), 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextBool(), required: true);
                GeneralParticleHandler.SpawnParticle(smoke);

                Dust dust = Dust.NewDustPerfect(GunTipPosition, 303, smokeVel.RotatedByRandom(0.1f), 80, default, Main.rand.NextFloat(0.4f, 1.3f));
                dust.noGravity = false;
                dust.color = StaticEffectsColor;
            }
            else
            {
                if (SoundEngine.TryGetActiveSound(HiveHum, out var hum) && hum.IsPlaying)
                    hum?.Stop();
                Projectile.Kill();
            }
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            SoundStyle charge = new("CalamityMod/Sounds/Item/LowHum");
            FrontArmStretch = Player.CompositeArmStretchAmount.Quarter;
            ExtraBackArmRotation = MathHelper.ToRadians(15f);
            HiveHum = SoundEngine.PlaySound(charge with { Volume = 1.6f, IsLooped = true }, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (ShootingTimer <= 0)
                return false;

            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowTexture = Request<Texture2D>(GlowTexture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Projectile.GetAlpha(lightColor);
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (!Owner.CantUseHoldout() && PostFireCooldown <= 0)
            {
                float rumble = Utils.GetLerpValue(0f, MaxCharge, ShootingTimer, true) * ShootingTimer >= MaxCharge ? 2 : 0.8f;
                drawPosition += Main.rand.NextVector2Circular(rumble, rumble);
            }

            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, drawRotation, rotationPoint, Projectile.scale * Owner.gravDir, flipSprite);
            Main.EntitySpriteDraw(glowTexture, drawPosition, null, Color.White, drawRotation, rotationPoint, Projectile.scale * Owner.gravDir, flipSprite);

            return false;
        }
    }
}
