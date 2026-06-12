using System.IO;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.Items.Weapons.Magic.Omicron;

namespace CalamityMod.Projectiles.Magic
{
    public class OmicronHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<Omicron>();
        public override Vector2 GunTipPosition => base.GunTipPosition - Vector2.UnitY.RotatedBy(Projectile.rotation) * 4f * Projectile.spriteDirection;
        public override float MaxOffsetLengthFromArm => 10f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -5f;
        public override float OffsetYDownwards => 5f;

        public ref float ShootingTimer => ref Projectile.ai[0];
        public ref float PostFireCooldown => ref Projectile.ai[1];
        public ref float MaxFireRateShots => ref Projectile.ai[2];

        public float Windup { get; set; } = StarterWinup;
        public Color EffectsColor { get; set; } = Color.MediumVioletRed;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void KillHoldoutLogic()
        {
            if (Owner.CantUseHoldout() && PostFireCooldown <= 0)
                Projectile.Kill();
        }

        public override void HoldoutAI()
        {
            if (PostFireCooldown > 0)
                PostFiringCooldown();

            if (Owner.Calamity().mouseRight && PostFireCooldown <= 0)
            {
                if (Owner.CheckMana(Owner.HeldItem, (int)(HeldItem.mana * Owner.manaCost) * 13, true))
                {
                    PostFireCooldown = 100;
                    Shoot(true);
                    ShootingTimer = 0;
                }
                else
                {
                    if (Projectile.soundDelay <= 0)
                    {
                        SoundEngine.PlaySound(SoundID.MaxMana with { Pitch = -0.5f }, Projectile.Center);
                        Projectile.soundDelay = 50;
                        ShootingTimer = 0;
                    }
                }
            }
            else if (ShootingTimer >= FireRate)
            {
                if (Owner.CheckMana(Owner.HeldItem, -1, true) && PostFireCooldown <= 0)
                {
                    MaxFireRateShots++;

                    if (MaxFireRateShots == 5)
                    {
                        Windup = 60;
                        MaxFireRateShots = 1;
                    }

                    Shoot(false);

                    ShootingTimer = 0;

                    if (Windup > 10 && MaxFireRateShots > 0)
                        Windup -= 12;
                    else
                        Windup = 10;
                }
                else if (PostFireCooldown <= 0)
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
            Vector2 firingVelocity1 = (shootDirection * 8).RotatedBy(0.1f * Utils.GetLerpValue(10, 55, Windup, true));
            Vector2 firingVelocity2 = (shootDirection * 8).RotatedBy(-0.1f * Utils.GetLerpValue(10, 55, Windup, true));
            Vector2 firingVelocity3 = (shootDirection * 10);

            if (yBeam)
            {
                SoundStyle fire = new("CalamityMod/Sounds/Item/OmicronBeam");
                SoundEngine.PlaySound(fire with { Volume = 0.9f }, Projectile.Center);

                for (int k = 0; k < 6; k++)
                {
                    Particle pulse2 = new GlowSparkParticle(GunTipPosition, shootDirection * 28, false, 8, 0.087f, EffectsColor, new Vector2(2.3f, 0.9f), true);
                    GeneralParticleHandler.SpawnParticle(pulse2);
                }

                Owner.SetScreenshake(6.5f);
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, firingVelocity3, ModContent.ProjectileType<OmicronBeam>(), Projectile.damage * 32, Projectile.knockBack, Projectile.owner, 0, 0);

                for (int i = 0; i < 8; i++)
                {
                    SparkParticle spark2 = new SparkParticle(GunTipPosition + Main.rand.NextVector2Circular(10, 10), firingVelocity3 * Main.rand.NextFloat(0.7f, 1.3f), false, Main.rand.Next(20, 30), Main.rand.NextFloat(0.4f, 0.55f), EffectsColor);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
            }
            else
            {
                SoundStyle fire = new("CalamityMod/Sounds/Item/ArcNovaDiffuserBigShot");
                SoundEngine.PlaySound(fire with { Volume = 0.2f, Pitch = 0.9f }, Projectile.Center);

                if (Main.myPlayer == Projectile.owner)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        firingVelocity3 = (shootDirection * 10).RotatedBy((0.035f * (i + 1)) * Utils.GetLerpValue(0, 55, Windup, true));
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, firingVelocity3 * (1 - i * 0.1f), ModContent.ProjectileType<WingmanShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 2);
                    }
                    for (int i = 0; i < 5; i++)
                    {
                        firingVelocity3 = (shootDirection * 10).RotatedBy((-0.035f * (i + 1)) * Utils.GetLerpValue(0, 55, Windup, true));
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, firingVelocity3 * (1 - i * 0.1f), ModContent.ProjectileType<WingmanShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 2);
                    }
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
                OffsetLengthFromArm -= 32f;
            else
                OffsetLengthFromArm -= 6f;
        }

        public void PostFiringCooldown()
        {
            Owner.channel = true;
            if (PostFireCooldown > 0 && Main.rand.NextBool())
            {
                Vector2 smokeVel = new Vector2(0, -8) * Main.rand.NextFloat(0.1f, 1.1f);
                Particle smoke = new HeavySmokeParticle(GunTipPosition, smokeVel, EffectsColor, Main.rand.Next(30, 50 + 1), Main.rand.NextFloat(0.1f, 0.4f), 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextBool(), required: true);
                GeneralParticleHandler.SpawnParticle(smoke);

                Dust dust = Dust.NewDustPerfect(GunTipPosition, DustID.SteampunkSteam, smokeVel.RotatedByRandom(0.1f), 80, default, Main.rand.NextFloat(0.2f, 0.8f));
                dust.noGravity = false;
                dust.color = EffectsColor;
            }
            ShootingTimer = 0;
            PostFireCooldown--;
        }

        public override void SendExtraAIHoldout(BinaryWriter writer) => writer.Write(Windup);

        public override void ReceiveExtraAIHoldout(BinaryReader reader) => Windup = reader.ReadSingle();
    }
}
