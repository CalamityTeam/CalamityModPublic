using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class NorfleetCannon : ModProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Norfleet>();
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/Norfleet";

        public Color StaticEffectsColor = Color.Gray;
        private ref float ShootingTimer => ref Projectile.ai[0];
        private float PostFireCooldown = 0;
        private bool HasLetGo = false;
        private SlotId NorfleetRecharge;

        private ref float OffsetLength => ref Projectile.localAI[0];

        private Player Owner;

        private float MaxOffsetLength = 5f;
        private const float MaxCharge = 237f;
        public bool recharging = false;
        public Color variedColor = Color.White;
        public Color mainColor = Color.White;
        public Color randomColor = Color.White;
        public int colorTimer = 0;
        public bool hasFired = false;
        public bool PUNISHMENTMODE = false;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 142;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.netImportant = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ContinuouslyUpdateDamageStats = true;
        }

        public override void AI()
        {
            Item heldItem = Owner.HeldItem;

            if (Owner.dead || Owner is null)
                Projectile.Kill();

                randomColor = Main.rand.Next(3) switch
            {
                0 => Color.OrangeRed,
                1 => Color.Aqua,
                _ => Color.GreenYellow,
            };
            if (mainColor == Color.White)
            {
                mainColor = randomColor;
                if (Projectile.ai[1] == 1000)
                    PUNISHMENTMODE = true;
            }

            if (ShootingTimer % 15 == 0)
            {
                variedColor = colorTimer switch
                {
                    0 => Color.OrangeRed,
                    1 => Color.Aqua,
                    _ => Color.GreenYellow,
                };
                colorTimer++;
                if (colorTimer >= 3)
                    colorTimer = 0;
            }

            mainColor = Color.Lerp(mainColor, variedColor, (PostFireCooldown <= 20 || recharging) ? 0.07f : 0);

            // If there's no player, or the player is the server, or the owner's stunned, there'll be no holdout.
            if (Owner.CantUseHoldout() && !HasLetGo)
            {
                if (SoundEngine.TryGetActiveSound(NorfleetRecharge, out var hum) && hum.IsPlaying)
                {
                    hum?.Stop();
                }
                if (Owner.Calamity().NorfleetCounter < 2)
                {
                    ShootRocket(heldItem);
                    PostFireCooldown = PUNISHMENTMODE ? 1000 : 55;
                    ShootingTimer = 1;
                    mainColor = PUNISHMENTMODE ? Color.Red : Color.MediumOrchid;
                }
                else
                {
                    ShootRocket(heldItem);
                    PostFireCooldown = PUNISHMENTMODE ? 1000 : 262;
                    ShootingTimer = 1;
                    mainColor = PUNISHMENTMODE ? Color.Red : Color.MediumOrchid;
                }
                    

                NetUpdate();
                HasLetGo = true;
            }
            if (HasLetGo)
            {
                PostFiringCooldown();
            }

            // The center of the player, taking into account if they have a mount or not.
            Vector2 ownerPosition = Owner.MountedCenter;

            // 15NOV2024: Ozzatron: clamped mouse position unnecessary, only used for direction
            // The vector between the player and the mouse.
            Vector2 ownerToMouse = Owner.Calamity().mouseWorld - ownerPosition;

            // Deals with the holdout's rotation and direction, the owner's arms, etc.
            ManageHoldout(ownerPosition, ownerToMouse);

            // When we change the distance of the gun from the arms for the recoil,
            // recover to the original position smoothly.
            if (OffsetLength != MaxOffsetLength)
                OffsetLength = MathHelper.Lerp(OffsetLength, MaxOffsetLength, 0.1f);

            ShootingTimer++;

            // Inside here go all the things that dedicated servers shouldn't spend resources on.
            // Like visuals and sounds.
            if (Main.dedServ)
                return;

            Vector2 shootDirection = Projectile.velocity.SafeNormalize(Vector2.Zero) * 20;
        }

        private void ManageHoldout(Vector2 mountedCenter, Vector2 ownerToMouse)
        {
            Vector2 rotationVector = Projectile.rotation.ToRotationVector2();
            float velocityRotation = Projectile.velocity.ToRotation();
            float proximityLookingUpwards = Vector2.Dot(ownerToMouse.SafeNormalize(Vector2.Zero), -Vector2.UnitY);
            int direction = MathF.Sign(ownerToMouse.X);

            Vector2 armPosition = Owner.RotatedRelativePoint(mountedCenter, true);
            Vector2 lengthOffset = rotationVector * OffsetLength;
            Vector2 armOffset = new Vector2(Utils.Remap(proximityLookingUpwards, -1f, 1f, 0f, -12f) * direction, -10f + Utils.Remap(MathF.Abs(proximityLookingUpwards), 0f, 1f, 0f, proximityLookingUpwards > 0f ? 15f : 0f));
            Projectile.Center = armPosition + lengthOffset + armOffset;
            Projectile.velocity = velocityRotation.AngleTowards(ownerToMouse.ToRotation(), 0.2f).ToRotationVector2();
            Projectile.rotation = velocityRotation;
            Projectile.timeLeft = 2;

            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();

            Projectile.spriteDirection = Projectile.direction = direction;
            Owner.ChangeDir(direction);

            float armRotation = Projectile.rotation - MathHelper.PiOver2; // -Pi/2 because the arms rotation starts with arms pointing down.
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, armRotation);
            Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, armRotation + MathHelper.ToRadians(15f) * direction);

            // Rumble (only while channeling)
            float rumble = 1f;
            if (!Owner.CantUseHoldout() && PostFireCooldown <= 0)
                Projectile.Center += Main.rand.NextVector2Circular(rumble, rumble);
            if (PostFireCooldown < 297 && PostFireCooldown > 15 && recharging)
            {
                rumble = 5f * Utils.GetLerpValue(297, 0, PostFireCooldown, true);
                Projectile.Center += Main.rand.NextVector2Circular(rumble, rumble);
            }
        }

        private void ShootRocket(Item item)
        {
            if (!hasFired)
            {
                // We use the velocity of this projectile as its direction vector.
                Vector2 shootDirection = Projectile.velocity.SafeNormalize(Vector2.Zero);

                // The position of the tip of the gun.
                Vector2 tipPosition = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(-0.05f * Projectile.direction) * 73f;

                // Spawns the projectile.
                SoundStyle fire = new("CalamityMod/Sounds/Item/NorfleetFire");
                SoundEngine.PlaySound(fire with { Volume = 0.9f, PitchVariance = 0.25f }, Projectile.Center);
                Owner.Calamity().NorfleetCounter++;

                for (int i = 0; i < 3; i++)
                {
                    Vector2 firingVelocity = shootDirection.RotatedByRandom((0.1f * i) + 0.02f) * 5f * Main.rand.NextFloat(0.8f, 1.2f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, firingVelocity, ModContent.ProjectileType<NorfleetComet>(), Projectile.damage / 3, Projectile.knockBack, Projectile.owner, 0, i, PUNISHMENTMODE ? 1 : 0);
                }

                NetUpdate();

                // Inside here go all the things that dedicated servers shouldn't spend resources on.
                // Like visuals and sounds.
                if (Main.dedServ)
                    return;

                for (int k = 0; k < 30; k++)
                {
                    Vector2 shootVel = (shootDirection * 10).RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 1.8f);

                    Dust dust2 = Dust.NewDustPerfect(tipPosition, Main.rand.NextBool(4) ? 264 : 66, shootVel);
                    dust2.scale = Main.rand.NextFloat(1.15f, 1.45f);
                    dust2.noGravity = true;
                    if (dust2.type == 66)
                        dust2.color = Main.rand.NextBool() ? Color.DarkViolet : Color.MediumOrchid;
                    else
                        dust2.color = Color.White;
                }
                for (int k = 0; k < 15; k++)
                {
                    Particle pulse = new GlowSparkParticle((tipPosition - shootDirection * 14) + Main.rand.NextVector2Circular(12, 12), shootDirection * 17 * Main.rand.NextFloat(0.35f, 1.35f), false, Main.rand.Next(7, 11 + 1), 0.025f * Main.rand.NextFloat(0.45f, 1.25f), Color.MediumOrchid, new Vector2(1.5f, 0.9f), true);
                    GeneralParticleHandler.SpawnParticle(pulse);
                }

                // By decreasing the offset length of the gun from the arms, we give an effect of recoil.
                OffsetLength -= 34f;
                hasFired = true;
            }
            else
            {
                if (Owner.Calamity().NorfleetCounter >= 1000)
                    Owner.Calamity().NorfleetCounter = 4;
                else
                    Owner.Calamity().NorfleetCounter = 1000;

                Projectile.Kill();
            }
        }
        private void PostFiringCooldown()
        {
            Owner.channel = true;
            Vector2 tipPosition = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(-0.05f * Projectile.direction) * 73f;

            if (PUNISHMENTMODE)
            {
                PostFireCooldown = 1000; 
                Owner.AddBuff(ModContent.BuffType<MiracleBlight>(), 900);
                Owner.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 900);
                if (Owner.statLife > 1)
                    Owner.statLife = (int)(Owner.statLife * 0.99f);
            }

            if (PostFireCooldown == 207)
            {
                OffsetLength -= 24f;
                SoundStyle charge = new("CalamityMod/Sounds/Item/NorfleetRecharge");
                NorfleetRecharge = SoundEngine.PlaySound(charge with { Volume = 1f }, Projectile.Center);
                recharging = true;
            }
            if (PostFireCooldown > 0 && !recharging)
            {
                Vector2 smokeVel = new Vector2(0, -8) * Main.rand.NextFloat(0.1f, 1.1f);
                Particle smoke = new HeavySmokeParticle(tipPosition, smokeVel, PUNISHMENTMODE ? Color.Red : StaticEffectsColor, Main.rand.Next(40, 60 + 1), Main.rand.NextFloat(0.3f, 0.6f), 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextBool(), required: true);
                GeneralParticleHandler.SpawnParticle(smoke);

                Dust dust = Dust.NewDustPerfect(tipPosition, DustID.SteampunkSteam, smokeVel.RotatedByRandom(0.1f), 80, default, Main.rand.NextFloat(0.4f, 1.3f));
                dust.noGravity = false;
                dust.color = PUNISHMENTMODE ? Color.Red : Color.White;
            }
            else if (!recharging)
            {
                if (SoundEngine.TryGetActiveSound(NorfleetRecharge, out var hum) && hum.IsPlaying)
                {
                    hum?.Stop();
                }
                Projectile.Kill();
                NetUpdate();
            }
            else
            {
                if (PostFireCooldown == 297)
                {
                    OffsetLength -= 18f;
                }
                if (PostFireCooldown == 15)
                {
                    OffsetLength -= 11f;

                    int points = 3;
                    float radians = MathHelper.TwoPi / points;
                    float randRot = Main.rand.NextFloat(-5, 5);
                    Vector2 spinningPoint = Vector2.Normalize(new Vector2(-1f, -1f).RotatedBy(randRot));
                    for (int k = 0; k < points; k++)
                    {
                        Color glowColor = k == 0 ? Color.OrangeRed : k == 1 ? Color.Aqua : Color.GreenYellow;
                        Vector2 velocity = spinningPoint.RotatedBy(radians * k).RotatedBy(-0.45f);
                        Particle pulse = new GlowSparkParticle(tipPosition, velocity, false, 10, 0.065f, glowColor, new Vector2(1.3f, 0.2f), true);
                        GeneralParticleHandler.SpawnParticle(pulse);
                    }

                    SoundStyle click = new("CalamityMod/Sounds/Item/DudFire");
                    SoundEngine.PlaySound(click with { Volume = 1f, Pitch = 0.7f }, Projectile.Center);
                    Owner.Calamity().NorfleetCounter = 0;
                }
                if (PostFireCooldown > 15)
                {
                    Vector2 dustVel = Projectile.velocity.RotatedByRandom(100) * Main.rand.NextFloat(5.1f, 25.8f);
                    Dust dust = Dust.NewDustPerfect(tipPosition + dustVel * 5, DustID.RainbowMk2, -dustVel * 0.5f, 0, default, Main.rand.NextFloat(0.5f, 1f));
                    dust.noGravity = true;
                    dust.color = mainColor;
                }

                if (PostFireCooldown <= 0)
                    recharging = false;
            }
            PostFireCooldown--;

            if (SoundEngine.TryGetActiveSound(NorfleetRecharge, out var hum2) && hum2.IsPlaying)
            {
                hum2.Position = Projectile.Center;
            }
        }

        private void NetUpdate() => Projectile.ForceNetUpdate(false);

        public override void OnSpawn(IEntitySource source)
        {
            Owner = Main.player[Projectile.owner];
            OffsetLength = MaxOffsetLength;

            if (Main.zenithWorld && Main.rand.NextBool(1000))
            {
                PUNISHMENTMODE = true;
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/NuhUhUh"), Owner.Center);
            }
        }

        // Because we use the velocity as a direction, we don't need it to change its position.
        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            if (ShootingTimer <= 0)
                return false;

            Vector2 tipPosition = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(-0.05f * Projectile.direction) * 73f;

            Texture2D glowTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/NorfleetGhostGlowmask").Value;
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Ranged/Norfleet").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Color drawColor = Projectile.GetAlpha(lightColor);
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (recharging)
            {
                for (int i = 0; i < 3; i++)
                {
                    Color auraColor = mainColor * 0.25f;
                    Texture2D centerTexture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/NorfleetGhost").Value;
                    Vector2 rotationalDrawOffset = (MathHelper.TwoPi * i / 7f + Main.GlobalTimeWrappedHourly * 50f).ToRotationVector2();
                    rotationalDrawOffset *= MathHelper.Lerp(3f, 5.25f, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 4f) * 0.5f + 0.5f);
                    Main.EntitySpriteDraw(centerTexture, Projectile.Center - Main.screenPosition + rotationalDrawOffset, null, auraColor, drawRotation, centerTexture.Size() * 0.5f, Projectile.scale * MathHelper.Clamp(1.35f * Utils.GetLerpValue(-200, 297, PostFireCooldown, true), 1, 1.35f), flipSprite, 0f);
                }
            }

            Texture2D rechargeTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;

            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            Main.EntitySpriteDraw(glowTexture, drawPosition, null, PUNISHMENTMODE ? Color.Red : mainColor, drawRotation, rotationPoint, Projectile.scale, flipSprite);
            if (PostFireCooldown > 15 && recharging)
            {
                float randSize = Main.rand.NextFloat(0.8f, 1.2f);
                Main.EntitySpriteDraw(rechargeTexture, tipPosition - Main.screenPosition, null, mainColor with { A = 0 }, Projectile.rotation, rechargeTexture.Size() * 0.5f, 0.55f * Utils.GetLerpValue(-100, 297, PostFireCooldown, true) * randSize, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(rechargeTexture, tipPosition - Main.screenPosition, null, Color.White with { A = 0 }, Projectile.rotation, rechargeTexture.Size() * 0.5f, 0.35f * Utils.GetLerpValue(-100, 297, PostFireCooldown, true) * randSize, SpriteEffects.None, 0);
            }
            return false;
        }

    }
}
