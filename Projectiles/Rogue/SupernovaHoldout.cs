using System;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SupernovaHoldout : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Supernova";
        public override bool useAttackSpeed => true;
        public override bool useMeleeSize => false;
        public override int swingWidth => 180;
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<Supernova>()).Item;
        public override int AfterImageLength => 0;

        public override int StartupTime { get; set; }
        public override int CooldownTime { get; set; }

        int explodeTimer = 0;

        public override void Defaults()
        {
            Projectile.width = 106;
            Projectile.height = 112;
            Projectile.MaxUpdates = 4;
            Projectile.noEnchantmentVisuals = true;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }

        public Color randomColor => Main.rand.Next(4) switch
            {
                0 => Color.Red,
                1 => Color.MediumTurquoise,
                2 => Color.Orange,
                _ => Color.LawnGreen,
            };
    public override void Spawn()
        {
            var player = Main.player[Projectile.owner];
            var modplayer = player.GetModPlayer<BaseSwordHoldoutPlayer>();
            StartupTime = 50;
            CooldownTime = 10;
            swingTime -= StartupTime + CooldownTime;
            OffsetDistance = 48;
            modplayer.swingNum = 0;
            RotateInStartup = 1;
            Projectile.scale = 0.4f;
        }

        public override void AdditionalAI()
        {
            Lighting.AddLight(Projectile.Center, new Vector3(1, 1, 1));
            var player = Main.player[Projectile.owner];
            var cplayer = player.Calamity();
            if (Projectile.Opacity > 0)
            {
                cplayer.temporaryStealthMax = 1.3f;
                cplayer.temporaryStealthTimer = 2;
            }

            var avgStealth = 0.8f * cplayer.stealthGenMoving + 0.2f * cplayer.stealthGenStandstill;
            var explodeTimeGoal = CalamityUtils.SecondsToFrames(Supernova.StealthTimeInSeconds) / avgStealth + 30;
            var stealthTime = explodeTimeGoal - 30;


            cplayer.rogueStealth = Projectile.Opacity <= 0 ? 0 : Math.Max(cplayer.temporaryStealthMax, cplayer.rogueStealthMax) * MathHelper.Clamp(explodeTimer / stealthTime, 0f, 1f);

            if (player.channel && inStartup)
            {

                if (timer >= StartupTime - 1)
                {
                    if (Projectile.FinalExtraUpdate() && explodeTimer < explodeTimeGoal)
                        explodeTimer++;
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
                        var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, angle * -16 * cplayer.rogueVelocity, ModContent.ProjectileType<SupernovaBomb>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
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
