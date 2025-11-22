using System;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using static CalamityMod.Items.Weapons.Ranged.FlakKraken;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Ranged
{
    public class FlakKrakenHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ItemType<FlakKraken>();
        public override float RecoilResolveSpeed => 0.05f;
        public override float MaxOffsetLengthFromArm => 30f;
        public override float OffsetXUpwards => -18f;
        public override float OffsetXDownwards => 15f;
        public override float BaseOffsetY => -20f;
        public override float OffsetYUpwards => 20f;
        public override float OffsetYDownwards => 10f;

        public ref float ShootingTimer => ref Projectile.ai[0];
        public ref float TimerBetweenBursts => ref Projectile.ai[1];

        // The type of dust used in the effects and the color of the particles that it'll use depending on the type of rocket used.
        // It'll carry over to the projectiles that it shoots.
        public static int DustEffectsID { get; set; }
        public static Color EffectsColor { get; set; }

        public override void HoldoutAI()
        {
            // The vector between the player and the mouse.
            Vector2 ownerToMouse = Owner.Calamity().mouseWorld - Owner.MountedCenter;

            // If the timer reaches Item.useTime, it'll shoot.
            // It'll shoot once immediately as the timer reaches.
            if (ShootingTimer >= HeldItem.useAnimation)
            {
                float adaptiveTimeBetweenShots = MathF.Floor(TimeBetweenShots * HeldItem.useAnimation / OriginalUseTime);
                // Every X frames it'll shoot a projectile.
                if (ShootingTimer % adaptiveTimeBetweenShots == 0f)
                {
                    // We use the velocity of this projectile as its direction vector.
                    Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);

                    // Every time we shoot we use ammo.
                    // With this method we also use the item's stats, like the shoot speed, or the type of ammo it was used.
                    Owner.PickAmmo(Owner.ActiveItem(), out _, out float itemShootSpeed, out int itemDamage, out float itemKnockback, out int rocketTypeShot);

                    // Decides the color of the effects depending on the type of rocket used.
                    switch (rocketTypeShot)
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
                            DustEffectsID = 109;
                            EffectsColor = Color.Black;
                            break;
                    }

                    if (Main.myPlayer == Projectile.owner)
                    {
                        // Spawns the projectile.
                        Projectile.NewProjectileDirect(
                            Projectile.GetSource_FromThis(),
                            GunTipPosition,
                            direction * itemShootSpeed,
                            ProjectileType<FlakKrakenProjectile>(),
                            itemDamage,
                            itemKnockback,
                            Projectile.owner,
                            rocketTypeShot,
                            ownerToMouse.Length());
                    }

                    // Applies the knockback to the player.
                    Owner.velocity += ownerToMouse.SafeNormalize(Vector2.UnitY) * -OwnerKnockbackStrength;

                    // Inside here go all the things that dedicated servers shouldn't spend resources on.
                    // Like visuals and sounds.
                    if (!Main.dedServ)
                    {
                        // By decreasing the offset length of the gun from the arms, we give an effect of recoil.
                        OffsetLengthFromArm = 10f;

                        Owner.Calamity().GeneralScreenShakePower = 3.5f;

                        Particle shootPulse = new DirectionalPulseRing(GunTipPosition,
                            Vector2.Zero,
                            Color.Gray * 0.7f,
                            new Vector2(0.5f, 1f),
                            Projectile.rotation,
                            0.1f,
                            0.4f,
                            20);
                        GeneralParticleHandler.SpawnParticle(shootPulse);

                        int smokeAmount = Main.rand.Next(12, 16 + 1);
                        for (int i = 0; i < smokeAmount; i++)
                        {
                            Particle smoke = new HeavySmokeParticle(
                                GunTipPosition,
                                direction.RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(2f, 30f),
                                EffectsColor * 0.1f,
                                Main.rand.Next(45, 61),
                                Main.rand.NextFloat(.6f, 1.3f),
                                Main.rand.NextFloat(0.2f, 0.35f));
                            GeneralParticleHandler.SpawnParticle(smoke);
                        }

                        int dustAmount = Main.rand.Next(15, 20 + 1);
                        for (int i = 0; i < dustAmount; i++)
                        {
                            Dust shootDust = Dust.NewDustPerfect(
                                GunTipPosition,
                                DustEffectsID,
                                direction.RotatedByRandom(MathHelper.PiOver4 * 0.5f) * Main.rand.NextFloat(2f, 12f),
                                Scale: Main.rand.NextFloat(0.8f, 1f));
                            shootDust.fadeIn = 100f;
                            shootDust.noLight = false;
                            shootDust.noLightEmittence = false;
                        }

                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/FlakKrakenShoot") { Volume = 0.6f }, GunTipPosition);
                    }
                }

                // When it has shot all the projectiles in the burst, reset all the variables and back to shooting.
                if (ShootingTimer >= HeldItem.useAnimation + adaptiveTimeBetweenShots * (ProjectilesPerBurst - 1))
                {
                    ShootingTimer = 0f;
                    TimerBetweenBursts = 0f;
                }
            }

            ShootingTimer++;
        }

        public override void OnSpawn(IEntitySource source)
        {
            base.OnSpawn(source);
            FrontArmStretch = Player.CompositeArmStretchAmount.ThreeQuarters;
        }
    }
}
