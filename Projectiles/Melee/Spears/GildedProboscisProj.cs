using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using CalamityMod.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class GildedProboscisProj : BaseSwordHoldoutProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Melee";
        public override bool useAttackSpeed => false;
        public override bool useMeleeSize => false;
        public override int swingWidth => 360;
        public override Item BaseItem => ModContent.GetModItem(ModContent.ItemType<GildedProboscis>()).Item;
        public override int AfterImageLength => 0;

        public override int StartupTime { get; set; }
        public override int CooldownTime { get; set; }


        public override float lineCollisionLength => 196;

        bool isChannelable = false;

        int channelCharge = 0;

        public override void Defaults()
        {
            Projectile.width = Projectile.height = 138;
            Projectile.extraUpdates = 5; //ExtraUpdates help make the VFX smoother
            Projectile.noEnchantmentVisuals = true;
        }
        public override void Spawn()
        {
            //This sets variables for the spear in general, as well as the secondary attack
            //The secondary attack is the "default" because it was coded first
            var player = Main.player[Projectile.owner];
            var modplayer = player.GetModPlayer<BaseSwordHoldoutPlayer>();
            StartupTime = 20;
            CooldownTime = 41;
            swingTime -= StartupTime + CooldownTime;
            modplayer.swingNum = 0;
            OffsetDistance = 70;
            angle = new Vector2(angle.X > 0 ? 1 : -1, 0);
            if (player.dashDelay == -1)
            {
                angle = new Vector2(-MathF.Sign(player.velocity.X), 0 );
            }
            RotateInStartup = 0;
            RotateInCooldown = 0;

            UseSound = SoundID.DD2_BetsysWrathImpact;
            //This adjusts variables for the primary attack
            if (player.altFunctionUse == 0)
            {
                UseSound = SoundID.DD2_JavelinThrowersAttack;
                isChannelable = true;
                RotateInStartup = 0.5f;
                OffsetDistance = 10;
            }
        }

        public override void AdditionalAI()
        {
            var player = Main.player[Projectile.owner];
            //Primary attack is a chargable spear stab
            //The DPS is worse than the secondary due to being able to charge it while out of range
            //The goal gameplay loop is for the primary to be used when getting up close, than the secondary to be used while close
            #region Primary Attack
            if (isChannelable)
            {
                //When channeling, the internal timer will not progress & the charge timer will go up
                if (player.channel)
                {
                    if (timer >= StartupTime - 1)
                    {
                        timer--;
                        Projectile.timeLeft++;
                    }
                    if (Projectile.FinalExtraUpdate() && channelCharge < CalamityUtils.SecondsToFrames(5))
                        channelCharge++;
                }
                //Once not channeling, set the damage at the end of the startup
                else
                {
                    if (timer == StartupTime - 1)
                    {
                        Projectile.originalDamage = (int)(Projectile.originalDamage * 0.75 * (channelCharge / 75f)); //scales from 0x to 3x power
                        Projectile.damage = (int)(Projectile.damage * 0.75 * (channelCharge / 75f)); //Both are needed 
                    }
                }
                //Make the sprite rotation look right in game
                Projectile.rotation -= (MathHelper.PiOver2) * (angle.X > 0 ? 1 : -1);

                //Adjust the offset distance (how close/far the weapon is held to the player) depending on charge during startup
                //In cooldown or during the swing, these are based on how far completed the corresponding step is
                if (inStartup)
                {
                    OffsetDistance = (int)MathHelper.SmoothStep(40, 5, channelCharge / 300f);
                }
                if (inCooldown)
                {
                    OffsetDistance = (int)MathHelper.SmoothStep(90, 60, MathF.Pow(CooldownCompletion, 1));

                    if (CooldownCompletion > 0.5f && Main.LocalPlayer.whoAmI == Projectile.owner && player.HeldItem.type == ModContent.ItemType<GildedProboscis>() && Main.mouseRight)
                    {
                        player.itemAnimation = 1;
                        player.itemTime = 1;
                        Projectile.timeLeft = 1;
                        timer = StartupTime + swingTime + CooldownTime;
                        return;
                    }
                }

                if (inSwing)
                {
                    OffsetDistance = (int)MathHelper.Lerp(-40, 90, MathF.Pow(SwingCompletion, 1.7f));
                    //Spawn the little sparks
                    var veloc = oldPlayerOffset - (Projectile.Center - Main.player[Projectile.owner].Center);
                    veloc.Normalize();
                    if (swingTimer % 4 == 0)
                    {
                        for (int i2 = -1; i2 <= 1; i2 += 2)
                        {
                            int sparkLifetime = Main.rand.Next(15, 23);

                            Vector2 sparkVel = Vector2.UnitY * -9f;
                            float maxRotationDeviance = 0.4f;
                            float rotationAngle = Main.rand.NextFloat(-maxRotationDeviance, maxRotationDeviance);
                            sparkVel = sparkVel.RotatedBy(rotationAngle) * Main.rand.NextFloat(0.3f, 1.0f);

                            float sparkScale = Main.rand.NextFloat(0.007f, 0.015f);

                            Vector2 compensatedSparkVel = angle.RotatedBy(i2 * -0.15f) * Main.rand.NextFloat(5, 10);
                            Vector2 pos = player.Center + angle.RotatedBy(MathHelper.Pi) * (OffsetDistance + Main.rand.NextFloat(30, 100)) * Projectile.scale + new Vector2(0, 10 * i2).RotatedByRandom(MathHelper.Pi - 0.2f);

                            Particle spark = new GlowSparkParticle(pos, compensatedSparkVel, false, sparkLifetime, sparkScale, Main.rand.NextBool() ? Color.Red : Color.Gold, new Vector2(0.5f, 1.3f));
                            GeneralParticleHandler.SpawnParticle(spark);
                        }

                    }
                    //Spawn the large glow V
                    var sparkAngle = angle.RotatedBy(MathHelper.Pi);
                    int dir = MathF.Sign(sparkAngle.X);
                    var color = Color.Lerp(Color.Gold, Color.Crimson, SwingCompletion);
                    if (swingTimer % 2 == 0 || (swingTimer == swingTime - 1))
                        for (int i = -1; i <= 1; i += 2)
                        {
                            Vector2 velocity = -sparkAngle.RotatedBy(i * -0.3f) * 10f;
                            Vector2 position = Projectile.Center + new Vector2(75, i * 22).RotatedBy(sparkAngle.ToRotation());
                            Particle spark = new CustomSpark(position, velocity, "CalamityMod/Particles/BloomCircle", false, 10, 0.3f, color, new Vector2(0.3f, 3f), shrinkSpeed: 0.2f);
                            GeneralParticleHandler.SpawnParticle(spark);
                        }
                }
                return;
            }
            #endregion
            //The secondary attack is based on Byleth's side-special from Smash Ultimate
            //It moves the player forward when used, and is meant to be the higher DPS option of the two
            #region Secondary Attack

            //Sets the max dash velocity depending on keys
            //Base of 12, 4 when holding against the dash & 20 when holding with the dash
            var dashStrength = 12;
            if ((player.direction == 1 && player.controlLeft) || (player.direction == -1 && player.controlRight))
                dashStrength -= 8;
            if ((player.direction == 1 && player.controlRight) || (player.direction == -1 && player.controlLeft))
                dashStrength += 8;
            if (inStartup)
            {
                //Scale % offset distance during startup
                Projectile.scale = baseScale * MathHelper.Lerp(0.9f, 0.65f, MathF.Pow(StartupCompletion, 2f));
                OffsetDistance = (int)MathHelper.Lerp(60f, 40f, MathF.Pow(StartupCompletion, 2f));
                ref var vel = ref Main.player[Projectile.owner].velocity;
                //accel for player during dash
                if (StartupCompletion > 0.5f && -vel.X * angle.X < dashStrength)
                    vel.X -= angle.X;
            }
            else if (inCooldown)
            {
                //Scale & offset distance during cooldown
                Projectile.scale = baseScale * MathHelper.Lerp(1, 0.9f, 1 - MathF.Pow(1 - CooldownCompletion, 2));
                OffsetDistance = (int)MathHelper.Lerp(70, 60, 1 - MathF.Pow(1 - CooldownCompletion, 2));
                ref var vel = ref Main.player[Projectile.owner].velocity;
                //decel for player after dash
                if (CooldownCompletion < 0.25f && -vel.X * angle.X > 5)
                    vel.X *= 0.9f;
            }
            else
            {
                //scale & offset for the damaging swing part of the anim
                //This keeps "am" (short for animation) at 0 or 1 most of the time, with a quick but smooth transition
                float am = MathHelper.Clamp(MathF.Pow((SwingCompletion - (0.5f - (1 / 3f))) * 3f, 3f), 0, 1);
                if (float.IsNaN(am))
                    am = 0;
                Projectile.scale = baseScale * MathHelper.Lerp(0.65f, 1, am);
                am = MathHelper.Clamp(MathF.Pow((SwingCompletion - (0.3f - (1 / 3f))) * 3f, 3f), 0, 1);
                if (float.IsNaN(am))
                    am = 0;
                OffsetDistance = (int)MathHelper.Lerp(40, 70, am);
                //accel for player during dash
                ref var vel = ref Main.player[Projectile.owner].velocity;
                if (-vel.X * angle.X < dashStrength)
                    vel.X -= angle.X;

                var veloc = oldPlayerOffset - (Projectile.Center - Main.player[Projectile.owner].Center);
                veloc.Normalize();
                //Little sparks
                for (var i = 0; i < 2; i++)
                {
                    int sparkLifetime = Main.rand.Next(15, 23);

                    Vector2 sparkVel = Vector2.UnitY * -9f;
                    float maxRotationDeviance = 0.4f;
                    float rotationAngle = Main.rand.NextFloat(-maxRotationDeviance, maxRotationDeviance);
                    sparkVel = sparkVel.RotatedBy(rotationAngle) * Main.rand.NextFloat(0.3f, 1.0f);

                    float sparkScale = Main.rand.NextFloat(0.007f, 0.015f);

                    Vector2 compensatedSparkVel = veloc * Main.rand.NextFloat(2, 5);
                    Particle spark = new GlowSparkParticle(Projectile.Center + new Vector2(-angle.X, Main.rand.NextFloat(-0.05f, 0.05f)).RotatedBy(Projectile.rotation) * Main.rand.NextFloat(50, 120) * Projectile.scale, compensatedSparkVel, false, sparkLifetime, sparkScale, Main.rand.NextBool() ? Color.Red : Color.Gold, new Vector2(0.5f, 1.3f));
                    GeneralParticleHandler.SpawnParticle(spark);

                }
                // Smoke/fire effect
                Color[] smokeColor = { Color.Red, Color.Goldenrod, Color.Crimson };
                Vector2 smokePosition = Projectile.Center + new Vector2(-angle.X, Main.rand.NextFloat(-0.05f, 0.05f)).RotatedBy(Projectile.rotation) * 80 * Projectile.scale;
                for (var i = 0; i < 3; i++)
                {
                    Particle beam4 = new CustomSpark(smokePosition, smokePosition.DirectionTo(Main.player[Projectile.owner].Center), "CalamityMod/ExtraTextures/Trails/ScarletDevilStreak", false, 10, 0.6f * Projectile.scale, smokeColor[timer % 3], new Vector2(1f, 1), true, false, 0, false, false, 0f);
                    GeneralParticleHandler.SpawnParticle(beam4);
                }
            }
            Projectile.rotation -= (MathHelper.PiOver2) * angle.X;
            #endregion
        }

        public override float SwingFunction()
        {
            if (isChannelable) //Primary attack
            {
                if (inStartup && channelCharge >= CalamityUtils.SecondsToFrames(5))
                {
                    //shake when fully charged
                    return Main.rand.NextFloat(-0.025f, 0.025f);
                }
                return 0; //Primary attack should face the mouse direction directly
            }
            //Everything else is secondary attack
            if (inStartup)
                return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * 0.7f, swingWidth * 0.33f, MathF.Pow(StartupCompletion, 4f)));
            if (inCooldown)
                return MathHelper.ToRadians(MathHelper.Lerp(-swingWidth * 0.2f, -swingWidth * 0.3f, 1 - MathF.Pow(1 - CooldownCompletion, 3f)));
            return MathHelper.ToRadians(MathHelper.SmoothStep(swingWidth * 0.33f, -swingWidth * 0.2f, SwingCompletion));
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<VermillionFlux>(), 900);
            Main.player[Projectile.owner].SpawnLifeStealProjectile(target, Projectile, ProjectileID.VampireHeal, (int)Math.Round(hit.Damage * 0.0015), 0.5f);
            Projectile.originalDamage = (int)(Projectile.originalDamage * 0.925f);
            Projectile.damage = (int)(Projectile.damage * 0.925f); //Both are needed as it doesn't recalculate till next frame
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
            => modifiers.ApplyScalingForcedCrit(Projectile);
    }
}
