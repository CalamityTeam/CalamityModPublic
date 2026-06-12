using System;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    internal class VitriolicViperHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<VitriolicViper>();
        public override float MaxOffsetLengthFromArm => 40f;
        public override float BaseOffsetY => 0f;
        public override float RecoilResolveSpeed => 0.4f;
        public override string Texture => "CalamityMod/Items/Weapons/Magic/VitriolicViper";
        public override Vector2 GunTipPosition => base.GunTipPosition - Vector2.UnitX.RotatedBy(Projectile.rotation) * -20;
        
        private ref float CurrentChargingFrames => ref Projectile.ai[0];
        private ref float CurrentOverchargeFrames => ref Projectile.ai[2];
        public int FirstChargeFrames = 30;
        public int FullyChargedFrames = 30;
        private bool FirstCharge => CurrentChargingFrames >= FirstChargeFrames;
        private bool FullCharge => CurrentOverchargeFrames >= FullyChargedFrames;
        public SlotId ChargeSlot;
        public static float BulletSpeed = 15f;
        public int time = 0;
        public float chargePower = 0;
        public float overchargePower = 0;
        public Color bColor = Color.Chartreuse; // Base color

        public float vortexRotation = 0;

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void KillHoldoutLogic()
        {
            if (Owner.CantUseHoldout(false) || HeldItem.type != Owner.HeldItem.type)
                Projectile.Kill();
        }

        public override void HoldoutAI()
        {
            if (SoundEngine.TryGetActiveSound(ChargeSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                ChargeSound.Position = Projectile.Center;

            chargePower = Utils.GetLerpValue(0, FirstChargeFrames, CurrentChargingFrames, true) * (FullCharge ? 1.4f : 1);
            if (CurrentOverchargeFrames < FullyChargedFrames)
                overchargePower = MathHelper.Clamp(Utils.GetLerpValue(FullyChargedFrames * 2, 0, CurrentOverchargeFrames, true), 0.5f, 1);
            else
                overchargePower = MathHelper.Lerp(overchargePower, 1, 0.09f);

            vortexRotation += 0.4f * chargePower;

            // Fire if the owner stops channeling or otherwise cannot use the weapon.
            if (Owner.CantUseHoldout())
            {
                KeepRefreshingLifetime = false;

                if (Projectile.ai[1] != 1f)
                {
                    Projectile.timeLeft = 30;

                    ChargeSound?.Stop();

                    Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * BulletSpeed;
                    if (FullCharge)
                    {
                        OffsetLengthFromArm -= 25;
                        SoundStyle fire = new("CalamityMod/Sounds/Item/ViperSpit");
                        SoundEngine.PlaySound(fire with { Pitch = -0.3f + Main.rand.NextFloat(-0.1f, 0.1f) + (FirstCharge ? 0.2f : 0) }, Projectile.Center);
                        SoundStyle fire2 = new("CalamityMod/Sounds/Item/MeldShoot");
                        SoundEngine.PlaySound(fire2 with { Pitch = -0.3f, Volume = 0.5f }, Projectile.Center);

                        // Vipers can apparently have 33 teeth or something like that
                        Owner.SetScreenshake(4f);
                        if (Main.myPlayer == Projectile.owner)
                        {
                            for (int i = 0; i < 33; i++)
                                Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, (shootVelocity * 2).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.8f, 1.2f), ModContent.ProjectileType<VitriolicViperFang>(), Projectile.damage / 10, Projectile.knockBack, Projectile.owner, 0);
                        }

                        Particle pulse = new CustomPulse(GunTipPosition, Vector2.Zero, bColor, "CalamityMod/Particles/HighResFoggyCircleHardEdge", Vector2.One, Main.rand.NextFloat(-10f, 10f), 0f, 0.05f, 14);
                        GeneralParticleHandler.SpawnParticle(pulse);

                        for (int i = 0; i < 17; i++)
                        {
                            Dust chargefull = Dust.NewDustPerfect(GunTipPosition, DustID.FireworksRGB);
                            chargefull.velocity = Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(5, 25);
                            chargefull.scale = Main.rand.NextFloat(0.45f, 0.75f);
                            chargefull.noGravity = true;
                            chargefull.color = Color.Lerp(Color.White, Main.rand.NextBool(4) ? Color.Green : bColor, 0.7f);
                        }

                        Vector2 shootDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
                        for (int i = 0; i <= 18; i++)
                        {
                            Vector2 sparkVelocity = shootVelocity / 2f;

                            float sparkScale1 = Main.rand.NextFloat(0.3f, 0.8f);
                            Vector2 sparkvelocity1 = sparkVelocity.RotatedByRandom(0.45f) * Main.rand.NextFloat(0.5f, 0.7f);
                            Particle spark1 = new LineParticle(GunTipPosition, sparkvelocity1, false, 40, sparkScale1, Main.rand.NextBool() ? bColor : Color.Green);
                            GeneralParticleHandler.SpawnParticle(spark1);

                            float sparkScale2 = Main.rand.NextFloat(0.4f, 1f);
                            Vector2 sparkvelocity2 = sparkVelocity.RotatedByRandom(0.2f) * Main.rand.NextFloat(0.9f, 1.6f);
                            Particle spark2 = new LineParticle(GunTipPosition, sparkvelocity2, false, 40, sparkScale2, Main.rand.NextBool() ? bColor : Color.Green);
                            GeneralParticleHandler.SpawnParticle(spark2);
                        }
                    }
                    else
                    {
                        OffsetLengthFromArm -= 15;
                        SoundStyle fire = new("CalamityMod/Sounds/Item/ViperSpit");
                        SoundEngine.PlaySound(fire with { Pitch = (chargePower * 0.3f) + Main.rand.NextFloat(-0.1f, 0.1f), Volume = 0.5f }, Projectile.Center);

                        if (Main.myPlayer == Projectile.owner)
                        {
                            Projectile hiss = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity * MathHelper.Clamp(chargePower, 0.3f, 1), ModContent.ProjectileType<VitriolicViperSpit>(), (int)(Projectile.damage * MathHelper.Clamp(chargePower, 0.3f, 1)), Projectile.knockBack, Projectile.owner, 0, 0, chargePower);
                            hiss.extraUpdates = (int)(Utils.Remap(chargePower, 0, 1, 2, 13, true));
                        }
                        for (int i = 0; i < 17; i++)
                        {
                            Dust chargefull = Dust.NewDustPerfect(GunTipPosition, DustID.FireworksRGB);
                            chargefull.velocity = Projectile.velocity.RotatedByRandom(0.4f) * Main.rand.NextFloat(5, 25);
                            chargefull.scale = Main.rand.NextFloat(0.55f, 0.75f) * chargePower + 0.1f;
                            chargefull.noGravity = true;
                            chargefull.color = Color.Lerp(Color.White, Main.rand.NextBool(4) ? Color.Green : bColor, 0.7f);
                        }

                        Vector2 shootDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);
                        Particle pulse3 = new GlowSparkParticle(GunTipPosition, shootDirection * 18, false, 6, 0.057f * chargePower, bColor, new Vector2(1.7f, 0.8f), true);
                        GeneralParticleHandler.SpawnParticle(pulse3);
                    }
                    Projectile.ai[1] = 1f;
                }
            }
            else
            {
                if (Projectile.ai[1] != 1f)
                {
                    if (!FirstCharge)
                        CurrentChargingFrames++;
                    else
                        CurrentOverchargeFrames++;
                }

                // Charge-up visuals
                if (CurrentChargingFrames >= 10)
                {
                    if (FirstCharge)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            Vector2 dustPos = GunTipPosition + (i * MathHelper.Pi + vortexRotation * 0.35f + MathHelper.PiOver2).ToRotationVector2() * 20f * chargePower * overchargePower;
                            Dust dust = Dust.NewDustPerfect(dustPos, (int)CalamityDusts.SulphurousSeaAcid, (i * MathHelper.Pi + vortexRotation * 0.35f * Math.Sign(Projectile.velocity.Length())).ToRotationVector2() * 4);
                            dust.noGravity = true;
                            dust.scale = Main.rand.NextFloat(0.85f, 0.9f);
                        }
                    }
                    else
                    {
                        Dust dust = Dust.NewDustPerfect(GunTipPosition, (int)CalamityDusts.SulphurousSeaAcid, new Vector2(12, 12).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1f) * chargePower * overchargePower);
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(0.6f, 0.7f);
                    }
                }

                // Full charge effects
                if (CurrentChargingFrames == FirstChargeFrames && CurrentOverchargeFrames == 0)
                {
                    SoundStyle fire = new("CalamityMod/Sounds/NPCHit/NuclearTerrorHit");
                    SoundEngine.PlaySound(fire with { Volume = 1f, Pitch = -0.2f }, Projectile.Center);
                    for (int i = 0; i < 18; i++)
                    {
                        Vector2 dustVel = Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(1, 5);
                        Dust dust2 = Dust.NewDustPerfect(GunTipPosition + dustVel, DustID.FireworksRGB, dustVel * 0.7f);
                        dust2.scale = Main.rand.NextFloat(0.45f, 0.9f);
                        dust2.noGravity = true;
                        dust2.color = Color.Lerp(Color.White, Main.rand.NextBool(4) ? bColor : Color.Green, 0.7f);
                    }
                }
                if (CurrentOverchargeFrames == FullyChargedFrames)
                {
                    SoundStyle fire = new("CalamityMod/Sounds/NPCHit/NuclearTerrorHit");
                    SoundEngine.PlaySound(fire with { Volume = 1f, Pitch = 0.2f }, Projectile.Center);
                    for (int i = 0; i < 12; i++)
                    {
                        Dust dust2 = Dust.NewDustPerfect(GunTipPosition, DustID.FireworksRGB, Vector2.One.RotatedByRandom(100) * Main.rand.NextFloat(2f, 5.5f));
                        dust2.scale = Main.rand.NextFloat(0.75f, 1.1f);
                        dust2.noGravity = false;
                        dust2.color = Color.Lerp(Color.White, Main.rand.NextBool(4) ? Color.Green : bColor, 0.7f);
                    }
                }
            }
            if (Projectile.ai[1] == 1f)
            {
                CurrentChargingFrames = 0;
                CurrentOverchargeFrames = 0;
            }

            Lighting.AddLight(GunTipPosition, bColor.ToVector3() * 1.5f * chargePower);

            time++;
        }

        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(ChargeSlot, out var ChargeSound))
                ChargeSound?.Stop();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (time < 2)
                return false;

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f) - (Owner.gravDir == -1 ? MathHelper.PiOver2 * Owner.direction : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Texture2D rechargeTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleVortex").Value;

            // Glow Orb
            for (int i = 0; i < 3; i++)
                Main.EntitySpriteDraw(rechargeTexture, GunTipPosition - Main.screenPosition, null, Color.Chartreuse with { A = 0 } * 0.4f, vortexRotation * (i * 0.3f), rechargeTexture.Size() * 0.5f, (0.5f * 0.14f + i * 0.015f) * chargePower * overchargePower, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(rechargeTexture, GunTipPosition - Main.screenPosition, null, Color.White with { A = 0 } * chargePower, vortexRotation, rechargeTexture.Size() * 0.5f, 0.25f * chargePower * 0.14f * overchargePower, SpriteEffects.None, 0);

            // Main staff
            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation + (MathHelper.ToRadians(45f * (Projectile.spriteDirection))), rotationPoint, Projectile.scale * Owner.gravDir, flipSprite);
            return false;
        }
    }
}
