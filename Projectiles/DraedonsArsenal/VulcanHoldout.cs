using System;
using CalamityMod.Cooldowns;
using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class VulcanHoldout : BaseGunHoldoutProjectile
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override int AssociatedItemID => ModContent.ItemType<Vulcan>();

        public float offsetBase = 35;
        public override float MaxOffsetLengthFromArm => offsetBase;
        public override float RecoilResolveSpeed => 0.2f;
        public override float OffsetXUpwards => 0f;
        public override float BaseOffsetY => 0f;
        public override float OffsetYDownwards => 0f;
        public override float WeaponTurnSpeed => (shootingTimer < 0 ? 0 : 0.35f);
        public bool firingSpear => Projectile.ai[2] == 5;
        public ref float shootingTimer => ref Projectile.ai[0];
        public ref float fireRate => ref Projectile.ai[1];
        public int time = 0;
        public int maxShots = 25;
        public int shotsToFire = 0;
        public bool playRampDown = false;
        public int shotsFired = 0;
        public float heatVis = 0;
        public float flashVis1 = 0;
        public float flashVis2 = 0;
        public float spearVis = 1;
        public int cooldownGiven = 180;
        public bool failedManaCheck = false;
        public int spearFakeCooldown = 0;


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void KillHoldoutLogic()
        {

        }
        public override void HoldoutAI()
        {
            if (Owner.dead)
            {
                Projectile.Kill();
                return;
            }

            if (flashVis1 > 0) flashVis1 -= 0.2f; if (flashVis1 < 0) flashVis1 = 0;
            if (flashVis2 > 0) flashVis2 -= 0.2f; if (flashVis2 < 0) flashVis2 = 0;

            if (time == 0)
            {
                if (Owner.Calamity().arsenalCooldown > 0)
                    spearVis = 0;
                shootingTimer = -20;
                fireRate = Owner.HeldItem.useAnimation;
                shotsToFire = maxShots;
                OffsetLengthFromArm = offsetBase;
            }
            if (shootingTimer < 0)
            {
                // Light cooldown after firing
                if (shootingTimer == -20 || shootingTimer == -1)
                {
                    if ((!Main.mouseLeft && !firingSpear) || failedManaCheck)
                    {
                        Projectile.Kill();
                        return;
                    }
                }
                if (playRampDown)
                {
                    SoundStyle sound = new("CalamityMod/Sounds/Item/VulcanRampDown");
                    for (int i = 0; i < 2; i++)
                        SoundEngine.PlaySound(sound with { Volume = 0.9f, MaxInstances = -1 }, Projectile.Center);
                    playRampDown = false;
                }
                if (shootingTimer == -20)
                {
                    SoundStyle sound2 = new("CalamityMod/Sounds/Item/VulcanRampUp");
                    for (int i = 0; i < 2; i++)
                        SoundEngine.PlaySound(sound2 with { Volume = 0.9f, MaxInstances = -1 }, Projectile.Center);
                }
                if (shootingTimer == -1)
                {
                    if (!firingSpear)
                        shootingTimer = fireRate;
                    else
                        shootingTimer = 0;
                    shotsToFire = maxShots;
                    fireRate = Owner.HeldItem.useAnimation;
                    shotsFired = 0;
                    playRampDown = true;
                }
                Projectile.velocity = Owner.Center.DirectionTo(Owner.Calamity().mouseWorld);
            }
            if (Owner.Calamity().mouseRight && Owner.Calamity().arsenalCooldown <= 0)
            {
                Projectile.ai[2] = 5;
            }
            if (firingSpear && time > 20 && spearFakeCooldown <= 0)
            {
                Shoot(true);
                spearFakeCooldown = 5;
                shotsToFire = maxShots;
                Projectile.ai[2] = 0;
                Owner.Calamity().arsenalCooldown = cooldownGiven;
                Owner.AddCooldown(ArsenalPower.ID, cooldownGiven);
            }
            if (shootingTimer >= 0)
            {
                if (!Main.mouseLeft && !firingSpear)
                    shotsToFire = 0;
                
                if (!firingSpear)
                {
                    if (shotsToFire <= 0)
                        shootingTimer = -50;
                    else if (shootingTimer >= fireRate)
                    {
                        if (Owner.CheckMana(HeldItem, -1, true, false))
                        {
                            Shoot(false);

                            if (shotsFired % 2 == 0) flashVis1 = 1;
                            else flashVis2 = 1;

                            shootingTimer = -1;
                            if (shotsToFire > maxShots * 0.2f)
                            {
                                fireRate -= 1;
                                if (fireRate < 3)
                                    fireRate = 3;
                            }
                            else if (fireRate < Owner.HeldItem.useAnimation)
                                fireRate += 1;
                            if (fireRate > Owner.HeldItem.useAnimation)
                                fireRate = Owner.HeldItem.useAnimation;

                        }
                        else
                        {
                            failedManaCheck = true;
                            shotsToFire = 0;
                        }
                        
                        shotsToFire--;
                        shotsFired++;
                        if (shotsToFire <= 0)
                        {
                            shootingTimer = -50;
                        }
                    }
                }
            }
            if (shotsFired > 0 && shotsToFire > 0)
            {
                heatVis = MathHelper.Lerp(heatVis, 1, 0.007f);
            }
            else
                heatVis = MathHelper.Lerp(heatVis, 0, 0.085f);

            spearVis = Utils.GetLerpValue(cooldownGiven / 2, 0, Owner.Calamity().arsenalCooldown, true);
            spearFakeCooldown--;

            time++;
            shootingTimer++;
        }
        public void Shoot(bool isSpear)
        {
            if (isSpear)
            {
                Owner.SetScreenshake(3.5f);
                OffsetLengthFromArm -= 16;
                SoundStyle sound = new("CalamityMod/Sounds/Item/GunShotHeavy");
                SoundEngine.PlaySound(sound with { Volume = 0.8f, Pitch = 0.8f }, Projectile.Center);
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 spearPos = GunTipPosition + Projectile.velocity.RotatedBy(MathHelper.PiOver2 * Projectile.direction) * 12 - Projectile.velocity * 8;
                    Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0f) * 6f;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), spearPos, shootVelocity, ModContent.ProjectileType<VulcanSpear>(), Projectile.damage * 5, 0, Projectile.owner);
                }
            }
            else
            {
                Owner.SetScreenshake(1f);
                OffsetLengthFromArm -= 7;
                SoundStyle sound = new("CalamityMod/Sounds/Item/VulcanShot");
                SoundEngine.PlaySound(sound with { Volume = 0.6f, Pitch = Main.rand.NextFloat(-0.1f, 0), MaxInstances = 1 }, Projectile.Center);

                Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedByRandom(0f) * 6f;
                Vector2 position = GunTipPosition + Projectile.velocity.RotatedBy(MathHelper.PiOver2 * (shotsFired % 2 == 0 ? -1 : 1)) * 4;
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), position, shootVelocity, ModContent.ProjectileType<VulcanProjectile>(), Projectile.damage, 0, Projectile.owner);

                for (int i = 0; i < 4; i++)
                {
                    Dust dust = Dust.NewDustPerfect(position, Main.rand.NextBool(3) ? ModContent.DustType<SquashDust>() : Effects.ArsenalEffects.ArsenalGaussDust, (shootVelocity * 4).RotatedByRandom(0.05f) * Main.rand.NextFloat(0.1f, 0.8f), 0, default, Main.rand.NextFloat(0.7f, 1.1f));
                    dust.noGravity = true;
                    dust.color = Effects.ArsenalEffects.ArsenalGaussColor;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (time < 2)
                return false;

            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/VulcanHoldout").Value;
            Texture2D glow = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/VulcanHoldoutGlow").Value;
            Texture2D flash = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/VulcanFlash").Value;
            Texture2D spear = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/VulcanSpear").Value;
            Texture2D spearGlow = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/VulcanSpearGlow").Value;
            Texture2D heat = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/VulcanHoldoutHeatedTip").Value;
            if (Owner.Calamity().arsenalCooldown <= 0 && false)
            {
                texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/VulcanHoldoutSpear").Value;
                glow = ModContent.Request<Texture2D>("CalamityMod/Projectiles/DraedonsArsenal/VulcanHoldoutSpearGlow").Value;
            }
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Vector2 vel = Projectile.rotation.ToRotationVector2();

            float randSize = Main.rand.NextFloat(0.9f, 1f);
            Color drawColor = Projectile.GetAlpha(lightColor);
            float drawRotation = Projectile.rotation + (Projectile.direction == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            int draws = 8;

            Vector2 spearPos = GunTipPosition + vel.RotatedBy(MathHelper.PiOver2 * Projectile.direction) * 12 - vel * 27 - Main.screenPosition;
            for (int i = 0; i < draws; i++)
            {
                float fadeIn = Math.Min(Utils.GetLerpValue(0, 0.8f, spearVis), (float)Math.Pow(Utils.GetLerpValue(1, 0.8f, spearVis), 2));
                Vector2 offset = (MathHelper.TwoPi / draws * i).ToRotationVector2() * 6 * (1 - spearVis);
                Main.EntitySpriteDraw(spear, spearPos + offset, null, Effects.ArsenalEffects.ArsenalGaussColor with { A = 0 } * fadeIn * 0.5f, drawRotation, spear.Size() * 0.5f, Projectile.scale, flipSprite);
                Main.EntitySpriteDraw(spear, spearPos, null, Color.Lerp(Effects.ArsenalEffects.ArsenalGaussColor with { A = 0 }, drawColor, spearVis) * spearVis, drawRotation, spear.Size() * 0.5f, Projectile.scale, flipSprite);
            }
            Main.EntitySpriteDraw(spearGlow, spearPos, null, Color.White * spearVis, drawRotation, spearGlow.Size() * 0.5f, Projectile.scale, flipSprite);

            Main.EntitySpriteDraw(texture, drawPosition, null, drawColor, drawRotation, rotationPoint, Projectile.scale, flipSprite);

            Main.EntitySpriteDraw(glow, drawPosition, null, Color.White, drawRotation, rotationPoint, Projectile.scale, flipSprite);

            for (int i = 0; i < draws; i++)
            {
                Vector2 offset = (MathHelper.TwoPi / draws * i).ToRotationVector2() * 4 * heatVis;
                Main.EntitySpriteDraw(heat, drawPosition + offset, null, Effects.ArsenalEffects.ArsenalGaussColor with { A = 0 } * heatVis * 0.35f, drawRotation, heat.Size() * 0.5f, Projectile.scale, flipSprite);
                Main.EntitySpriteDraw(heat, drawPosition, null, Color.White with { A = 0 } * heatVis * 0.3f, drawRotation, heat.Size() * 0.5f, Projectile.scale, flipSprite);
            }

            float tipSeperation = 4;
            // Tip flash 1
            if (flashVis1 > 0)
            {
                Vector2 position = GunTipPosition + vel * (12 + 30 * (1 - flashVis1)) + vel.RotatedBy(MathHelper.PiOver2) * tipSeperation - Main.screenPosition;
                for (int i = 0; i < 3; i++)
                    Main.EntitySpriteDraw(flash, position, null, Color.White with { A = 0 } * flashVis1, drawRotation, flash.Size() * 0.5f, new Vector2(0.5f + 2f * (1 - flashVis1), 1 - 0.8f * flashVis1) * Projectile.scale, flipSprite);
            }
            // Tip flash 1
            if (flashVis2 > 0)
            {
                Vector2 position = GunTipPosition + vel * (12 + 30 * (1 - flashVis2)) + vel.RotatedBy(-MathHelper.PiOver2) * tipSeperation - Main.screenPosition;
                for (int i = 0; i < 3; i++)
                    Main.EntitySpriteDraw(flash, position, null, Color.White with { A = 0 } * flashVis2, drawRotation, flash.Size() * 0.5f, new Vector2(0.5f + 2f * (1 - flashVis2), 1 - 0.8f * flashVis2) * Projectile.scale, flipSprite);
            }
            return false;
        }
    }
}
