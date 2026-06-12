using System;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SealedSingularityHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/SealedSingularity";
        public override bool useAttackSpeed => true;
        public override bool useMeleeSize => false;
        public override int swingWidth => 180;
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<SealedSingularity>()).Item;
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
                cplayer.temporaryStealthMax = 1.2f;
                cplayer.temporaryStealthTimer = 2;
            }

            var avgStealth = 0.8f * cplayer.stealthGenMoving + 0.2f * cplayer.stealthGenStandstill;
            var explodeTimeGoal = CalamityUtils.SecondsToFrames(2) / avgStealth + 30;
            var stealthTime = explodeTimeGoal - 30;

            if (Projectile.FinalExtraUpdate() && inStartup)
                if (explodeTimer < explodeTimeGoal)
                    explodeTimer++;
                else
                {
                    if (Main.myPlayer == Projectile.owner)
                        Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, angle * -10, ModContent.ProjectileType<SealedSingularityProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, explodeTimer, explodeTimeGoal, 1).hostile = true;
                    Projectile.Opacity = 0;
                    return;
                }

            cplayer.rogueStealth = Projectile.Opacity <= 0 ? 0 : Math.Max(cplayer.temporaryStealthMax, cplayer.rogueStealthMax) * MathHelper.Clamp(explodeTimer / stealthTime, 0f, 1f);

            if (player.channel && inStartup)
            {
                if (timer >= StartupTime - 1)
                {
                    timer--;
                    Projectile.timeLeft++;
                }
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
                        var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, angle * -15 * cplayer.rogueVelocity, ModContent.ProjectileType<SealedSingularityProjectile>(), Projectile.damage, Projectile.knockBack, Projectile.owner, explodeTimer - 30, explodeTimeGoal);
                        if (player.Calamity().StealthStrikeAvailable())
                        {
                            p.Calamity().stealthStrike = true;
                        }
                    }
                    Projectile.Opacity = 0;
                }
            }
        }

        public override float SwingFunction()
        {
            if (inStartup)
            {
                return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * -0.2f, swingWidth * -0.5f, StartupCompletion));
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
