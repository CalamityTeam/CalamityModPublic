using System.IO;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.Items.Weapons.Magic.Genesis;

namespace CalamityMod.Projectiles.Magic
{
    public class GenesisHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<Genesis>();
        public override Vector2 GunTipPosition => base.GunTipPosition - Vector2.UnitY.RotatedBy(Projectile.rotation) * 2.5f * Projectile.spriteDirection;
        public override float MaxOffsetLengthFromArm => 10f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -5f;
        public override float OffsetYDownwards => 5f;

        public ref float ShootingTimer => ref Projectile.ai[0];
        public ref float MaxFirerateShots => ref Projectile.ai[1];
        public bool FireYBeam
        {
            get => Projectile.ai[2] == 1f;
            set => Projectile.ai[2] = value == true ? 1f : 0f;
        }

        public float Windup { get; set; } = StarterWindup;
        public Color EffectsColor { get; set; } = Color.MediumSlateBlue;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void KillHoldoutLogic()
        {
            if (HeldItem.type != Owner.HeldItem.type || Owner.CantUseHoldout())
                Projectile.Kill();
        }

        public override void HoldoutAI()
        {
            if (ShootingTimer >= FireRate)
            {
                if (Owner.CheckMana(HeldItem, -1, true, false))
                {
                    MaxFirerateShots++;

                    if (MaxFirerateShots == 6f)
                    {
                        FireYBeam = true;
                        Windup = 60f;
                    }

                    if (FireYBeam)
                    {
                        Shoot(true);
                        MaxFirerateShots = 0f;
                    }
                    else
                        Shoot(false);

                    ShootingTimer = 0f;

                    if (Windup > 10f)
                    {
                        if (!FireYBeam)
                            Windup -= 12f;
                    }
                    else
                        Windup = 10f;

                    FireYBeam = false;
                }
                else
                {
                    SoundEngine.PlaySound(SoundID.MaxMana with { Pitch = -0.5f }, Projectile.Center);
                    Projectile.Kill();
                }
            }

            ShootingTimer++;
        }

        public void Shoot(bool yBeam)
        {
            Vector2 shootDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);

            // Spawns the projectile.

            Vector2 firingVelocity1 = (shootDirection * 8).RotatedBy(0.1f * Utils.GetLerpValue(10, 55, Windup, true));
            Vector2 firingVelocity2 = (shootDirection * 8).RotatedBy(-0.1f * Utils.GetLerpValue(10, 55, Windup, true));
            Vector2 firingVelocity3 = (shootDirection * 10);

            if (yBeam)
            {
                SoundStyle fire = new("CalamityMod/Sounds/Item/LanceofDestinyStrong");
                SoundEngine.PlaySound(fire with { Volume = 0.35f, Pitch = 1f, PitchVariance = 0.15f }, Projectile.Center);

                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, firingVelocity3, ModContent.ProjectileType<GenesisBeam>(), Projectile.damage * 9, Projectile.knockBack, Projectile.owner, 0, 0);

                Particle pulse3 = new GlowSparkParticle(GunTipPosition, shootDirection * 18, false, 6, 0.057f, EffectsColor, new Vector2(1.7f, 0.8f), true);
                GeneralParticleHandler.SpawnParticle(pulse3);
                for (int i = 0; i < 8; i++)
                {
                    SparkParticle spark2 = new SparkParticle(GunTipPosition + Main.rand.NextVector2Circular(10, 10), firingVelocity3 * Main.rand.NextFloat(0.7f, 1.3f), false, Main.rand.Next(20, 30), Main.rand.NextFloat(0.4f, 0.55f), EffectsColor);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
            }
            else
            {
                SoundStyle fire = new("CalamityMod/Sounds/Item/MagnaCannonShot");
                SoundEngine.PlaySound(fire with { Volume = 0.25f, Pitch = 1f, PitchVariance = 0.35f }, Projectile.Center);

                for (int i = 0; i < 4; i++)
                {
                    firingVelocity3 = (shootDirection * 10).RotatedBy((0.05f * (i + 1)) * Utils.GetLerpValue(0, 55, Windup, true));

                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, firingVelocity3 * (1 - i * 0.1f), ModContent.ProjectileType<WingmanShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 1);
                }
                for (int i = 0; i < 4; i++)
                {
                    firingVelocity3 = (shootDirection * 10).RotatedBy((-0.05f * (i + 1)) * Utils.GetLerpValue(0, 55, Windup, true));

                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, firingVelocity3 * (1 - i * 0.1f), ModContent.ProjectileType<WingmanShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 1);
                }

                Particle pulse3 = new GlowSparkParticle(GunTipPosition, shootDirection * 18, false, 6, 0.057f, EffectsColor, new Vector2(1.7f, 0.8f), true);
                GeneralParticleHandler.SpawnParticle(pulse3);
            }

            // Inside here go all the things that dedicated servers shouldn't spend resources on.
            // Like visuals and sounds.
            if (Main.dedServ)
                return;

            for (int k = 0; k < 10; k++)
            {
                Vector2 shootVel = (shootDirection * 15).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 1.8f);

                Dust dust2 = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(4) ? 267 : 66, shootVel);
                dust2.scale = Main.rand.NextFloat(1.15f, 1.45f);
                dust2.noGravity = true;
                dust2.color = Main.rand.NextBool() ? Color.Lerp(EffectsColor, Color.White, 0.5f) : EffectsColor;
            }

            // By decreasing the offset length of the gun from the arms, we give an effect of recoil.
            if (yBeam)
                OffsetLengthFromArm -= 27f;
            else
                OffsetLengthFromArm -= 5f;
        }

        public override void SendExtraAIHoldout(BinaryWriter writer) => writer.Write(Windup);

        public override void ReceiveExtraAIHoldout(BinaryReader reader) => Windup = reader.ReadSingle();
    }
}
