using System;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class LemonNadeHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/LemonNade";
        public override bool useAttackSpeed => true;
        public override bool useMeleeSize => false;
        public override int swingWidth => 180;
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<LemonNade>()).Item;
        public override int AfterImageLength => 0;

        public override int StartupTime { get; set; }
        public override int CooldownTime { get; set; }

        int explodeTimer = 0;

        public override void Defaults()
        {
            Projectile.width = 22; Projectile.height = 28;
            Projectile.MaxUpdates = 4;
            Projectile.noEnchantmentVisuals = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }
        public override void Spawn()
        {
            var player = Main.player[Projectile.owner];
            var modplayer = player.GetModPlayer<BaseSwordHoldoutPlayer>();
            StartupTime = 10;
            CooldownTime = 10;
            swingTime -= StartupTime + CooldownTime;
            OffsetDistance = 24;
            modplayer.swingNum = 0;
            RotateInStartup = 1;
        }

        public override void AdditionalAI()
        {
            var player = Main.player[Projectile.owner];
            var cplayer = player.Calamity();
            if (Projectile.Opacity > 0)
            {
                cplayer.temporaryStealthMax = 0.5f;
                cplayer.temporaryStealthTimer = 2;
            }

            var avgStealth = 0.8f * cplayer.stealthGenMoving + 0.2f * cplayer.stealthGenStandstill;
            var stealthTime = CalamityUtils.SecondsToFrames(2) / avgStealth;
            var explodeTimeGoal = stealthTime + 30;

            if (Projectile.FinalExtraUpdate() && inStartup)
                if (explodeTimer < explodeTimeGoal)
                    explodeTimer++;
                else
                {
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, angle * -10, ModContent.ProjectileType<LemonNadeProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, explodeTimer, explodeTimeGoal).hostile = true;
                    Projectile.Opacity = 0;
                    return;
                }

            cplayer.rogueStealth = Projectile.Opacity <= 0 ? 0 : Math.Max(cplayer.temporaryStealthMax, cplayer.rogueStealthMax) * MathHelper.Clamp(explodeTimer / stealthTime, 0f, 1f);

            if (timer == 1)
            {


                if (!Main.dedServ)
                {
                    SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack with { pitch = 1f });
                    Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.UnitY * -8 + angle, Mod.Find<ModGore>("LemonNadePin").Type, 1);
                }
            }

            if (player.channel && inStartup)
            {
                if (timer >= StartupTime - 1)
                {
                    timer--;
                    Projectile.timeLeft++;
                }
            }
            // Code copied from Violence / Chalice.
            float bloodVelMult = 1;
            if (Main.rand.NextFloat() < explodeTimer / explodeTimeGoal && Projectile.Opacity > 0 && Projectile.FinalExtraUpdate())
            {
                int bloodLifetime = Main.rand.Next(5, 15);
                float bloodScale = Main.rand.NextFloat(0.6f, 0.8f);
                Color bloodColor = Color.Lerp(Color.Yellow, Color.Goldenrod, Main.rand.NextFloat());
                bloodColor = Color.Lerp(bloodColor, new Color(51, 22, 94), Main.rand.NextFloat(0.65f));
                float randomSpeedMultiplier = Main.rand.NextFloat(1.25f, 2.25f);
                Vector2 bloodVelocity = Main.rand.NextVector2Unit() * bloodVelMult * randomSpeedMultiplier;
                bloodVelocity.Y -= 5f;
                BloodParticle blood = new BloodParticle(Projectile.Center, bloodVelocity.RotatedBy(Projectile.rotation + MathHelper.PiOver4 * Projectile.spriteDirection), bloodLifetime, bloodScale, bloodColor);
                GeneralParticleHandler.SpawnParticle(blood);
            }
            if (explodeTimer >= explodeTimeGoal && inCooldown && !Projectile.FinalExtraUpdate())
            {
                timer--;
                Projectile.timeLeft++;
            }

            if (inSwing)
            {
                if (swingTimer == (int)(swingTime * 0.75f))
                {
                    if (Projectile.owner == Main.myPlayer && Projectile.Opacity > 0)
                    {
                        var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, angle * -10 * cplayer.rogueVelocity, ModContent.ProjectileType<LemonNadeProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, explodeTimer - 25, explodeTimeGoal);
                        if (player.Calamity().StealthStrikeAvailable())
                        {
                            p.Calamity().stealthStrike = true;
                        }
                    }
                    Projectile.Opacity = 0;
                }
            }
            Projectile.rotation += MathHelper.PiOver2 * Projectile.spriteDirection;
        }

        public override float SwingFunction()
        {
            if (inStartup)
            {
                return swingWidth * -0.5f + (MathF.Sin(Main.GlobalTimeWrappedHourly * 30) * 0.2f);
            }
            if (inCooldown)
                return MathHelper.ToRadians(MathHelper.Lerp(swingWidth * 0.2f, swingWidth * 0.33f, 1 - MathF.Pow(1 - CooldownCompletion, 3f)));
            return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * -0.7f, swingWidth * 0.2f, SwingCompletion));
        }

        public override bool? CanHitNPC(NPC target)
        {
            return false;
        }

        public override bool CanHitPlayer(Player target)
        {
            return false;
        }
    }
}
