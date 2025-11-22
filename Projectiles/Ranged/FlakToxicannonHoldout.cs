using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using static CalamityMod.Items.Weapons.Ranged.FlakToxicannon;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles.Ranged
{
    public class FlakToxicannonHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ItemType<FlakToxicannon>();
        public override float MaxOffsetLengthFromArm => 38f;
        public override float RecoilResolveSpeed => 0.1f;
        public override float OffsetXUpwards => -10f;
        public override float OffsetXDownwards => 2f;
        public override float BaseOffsetY => -10f;
        public override float OffsetYUpwards => 10f;
        public override float OffsetYDownwards => 5f;

        public ref float ShootingTimer => ref Projectile.ai[0];
        public ref float TimerBetweenBursts => ref Projectile.ai[1];

        // The type of dust used in the effects and the color of the particles that it'll use depending on the type of rocket used.
        // It'll carry over to the projectiles that it shoots.
        public static int DustEffectsID { get; set; }
        public static Color EffectsColor { get; set; }

        public override void HoldoutAI()
        {
            // The center of the player, taking into account if they have a mount or not.
            Vector2 mountedCenter = Owner.MountedCenter;

            // The vector between the player and the mouse.
            Vector2 ownerToMouse = Owner.Calamity().mouseWorld - mountedCenter;

            // If the timer reaches Item.useTime, it'll shoot.
            // It'll shoot once immediately as the timer reaches.
            if (ShootingTimer >= HeldItem.useAnimation)
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
                        DustEffectsID = 298;
                        EffectsColor = Color.GreenYellow;
                        break;
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    // Spawns the projectile.
                    Projectile.NewProjectileDirect(
                        Projectile.GetSource_FromThis(),
                        GunTipPosition,
                        direction * itemShootSpeed,
                        ProjectileType<FlakToxicannonProjectile>(),
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
                    OffsetLengthFromArm = 25f;

                    Owner.Calamity().GeneralScreenShakePower = 2f;

                    Particle shootPulse = new DirectionalPulseRing(GunTipPosition,
                        Vector2.Zero,
                        Color.Gray * 0.7f,
                        new Vector2(0.5f, 1f),
                        Projectile.rotation,
                        0.1f,
                        0.4f,
                        20);
                    GeneralParticleHandler.SpawnParticle(shootPulse);

                    int smokeAmount = Main.rand.Next(8, 12 + 1);
                    for (int i = 0; i < smokeAmount; i++)
                    {
                        Particle smoke = new HeavySmokeParticle(
                            GunTipPosition,
                            direction.RotatedByRandom(MathHelper.ToRadians(25f)) * Main.rand.NextFloat(2f, 30f),
                            EffectsColor * 0.4f,
                            Main.rand.Next(45, 61),
                            Main.rand.NextFloat(.6f, 1.3f),
                            Main.rand.NextFloat(0.2f, 0.35f),
                            0,
                            true);
                        GeneralParticleHandler.SpawnParticle(smoke);
                    }

                    int dustAmount = Main.rand.Next(10, 15 + 1);
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

                    SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/FlakKrakenShoot") { Pitch = 0.65f, Volume = 0.5f }, GunTipPosition);
                }

                ShootingTimer = 0f;

            }

            ShootingTimer++;
        }
    }
}
