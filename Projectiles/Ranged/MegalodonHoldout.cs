using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Projectiles.Ranged
{
    public class MegalodonHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<Megalodon>();
        public override float MaxOffsetLengthFromArm => 24f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -1f;
        public override float OffsetYDownwards => 5f;
        public override Vector2 GunTipPosition => Projectile.Center + (Projectile.velocity * 50).RotatedBy(0.13f * Projectile.direction);

        public int Time = 0;
        public int shotCounter = 0;
        public int framesBetweenShots = 0;
        public bool swapType = false;

        public override void KillHoldoutLogic()
        {
            //If the player is dead, kill the holdout.
            if (Owner.CantUseHoldout() || HeldItem.type != AssociatedItemID)
                Projectile.Kill();
        }

        public override void HoldoutAI()
        {
            //Pause to give the effect of reloading
            if (Time == 30)
            {
                Player Owner = Main.player[Projectile.owner];
                SoundEngine.PlaySound(SoundID.Item149, Projectile.Center);
                if (Main.netMode != NetmodeID.Server)
                {
                    string goreType = "MegalodonMag";
                    Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center + (-Projectile.velocity * 18), Projectile.velocity.RotatedBy(2f * -Owner.direction) * Main.rand.NextFloat(0.6f, 0.7f), Mod.Find<ModGore>(goreType).Type);
                }
                //Set the scaling to 0 whenever it reloads
                Owner.Calamity().sharkGunDamageScaling = 0;
            }
            if (Time >= 90)
            {
                if (framesBetweenShots == 0 && shotCounter <= 31)
                {
                    Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 19;
                    #region Debug Text Display
                    //Main.NewText(shotCounter);
                    //Main.NewText(Owner.Calamity().sharkGunDamageScaling);
                    #endregion
                    #region Visuals and Sounds
                    SoundStyle fire = new("CalamityMod/Sounds/Item/GunShotTiny");
                    SoundEngine.PlaySound(fire with { Volume = 0.3f, Pitch = 0.4f, PitchVariance = 0.1f, MaxInstances = -1 }, Projectile.Center);
                    Particle sparker = new CritSpark(GunTipPosition, Vector2.Zero, Color.Gold, Color.LightGoldenrodYellow, 1.7f, 3, 0.5f, 3f);
                    GeneralParticleHandler.SpawnParticle(sparker);
                    for (int i = 0; i <= 4; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? 303 : 244, (shootVelocity * Main.rand.NextFloat(0.2f, 1.1f)).RotatedByRandom(0.4f));
                        dust.noGravity = true;
                        dust.scale = Main.rand.NextFloat(0.8f, 1.4f);
                    }
                    #endregion

                    //How many frames between firing projectiles, and how far the gun moves backward to give the effect of recoil. Change this number to edit fire rate
                    framesBetweenShots = 4;
                    OffsetLengthFromArm -= 3f;
                    //Here we detect which ammo the bullets will use
                    Owner.PickAmmo(Owner.HeldItem, out int bulletAMMO, out float SpeedNoUse, out int bulletDamage, out float kBackNoUse, out _);
                    //Alternate between shooting bullets and water streams. We seperate it into two different projectiles due to the bullets needing to use a Global Projectile to track the damage multiplier
                    if (Main.myPlayer == Projectile.owner)
                    {
                        if (!swapType)
                        {
                            Projectile scalingShot = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity.RotatedByRandom(MathHelper.ToRadians(1.5f)), bulletAMMO, Projectile.damage, Projectile.knockBack, Projectile.owner);
                            CalamityGlobalProjectile cgp = scalingShot.Calamity();
                            cgp.sharkBullets = true;
                            //Only eject casings from the gun if bullets are fired, preventing too many at once
                            if (Main.netMode != NetmodeID.Server)
                            {
                                string goreType = "MegalodonCasing";
                                Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center + (-Projectile.velocity * 18), -Projectile.velocity * 4f, Mod.Find<ModGore>(goreType).Type);
                            }
                        }
                        if (swapType)
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity.RotatedByRandom(MathHelper.ToRadians(1.5f)), ModContent.ProjectileType<MegalodonShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        }
                        swapType = !swapType;
                    }
                    shotCounter++;
                    //Allow a pause before firing the rocket. This allows the final bullet a chance to hit before the rocket is fired. Lowering this number reduces the delay, but may also cause the gun to become inconsistent
                    if (shotCounter == 30)
                        framesBetweenShots = 18;
                }
                if (framesBetweenShots > 0)
                    framesBetweenShots--;
            }
            if (shotCounter == 30 && framesBetweenShots == 0)
            {
                Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 30;
                //Debug Text
                //Main.NewText(Owner.Calamity().sharkGunDamageScaling);
                #region Visuals and Sounds
                SoundStyle fire = new("CalamityMod/Sounds/Item/GunShotHeavy");
                SoundEngine.PlaySound(fire with { Volume = 0.6f, Pitch = 0.2f }, Projectile.Center);
                for (int i = 0; i <= 13; i++)
                {
                    Dust dust = Dust.NewDustPerfect(GunTipPosition, 303, shootVelocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.2f, 1.1f), 80, default, Main.rand.NextFloat(0.4f, 1.3f));
                    dust.noGravity = false;
                    dust.color = Color.White;

                    Dust dust2 = Dust.NewDustPerfect(GunTipPosition, 278, shootVelocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 0.8f));
                    dust2.noGravity = true;
                    dust2.scale = Main.rand.NextFloat(0.52f, 0.72f);
                    dust2.color = Color.Lerp(Color.Orange, Color.Gold, Main.rand.NextFloat(0f, 1f));
                }
                for (int i = 0; i < 14; i++)
                {
                    Particle smoke = new HeavySmokeParticle(GunTipPosition, shootVelocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.2f, 1.5f), Color.RoyalBlue, Main.rand.Next(40, 60 + 1), Main.rand.NextFloat(0.3f, 0.6f), 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextBool(), required: true);
                    GeneralParticleHandler.SpawnParticle(smoke);
                }
                float rotation = Projectile.velocity.ToRotation();
                Particle pulse = new DirectionalPulseRing(GunTipPosition, shootVelocity / 4, Color.Gray, new Vector2(1f, 2.5f), rotation, 0.03f, 0.3f, 20);
                GeneralParticleHandler.SpawnParticle(pulse);
                //Extra sound and muzzle flash if the scaling is at max
                if (Owner.Calamity().sharkGunDamageScaling == 30)
                {
                    SoundStyle fireperfect = new("CalamityMod/Sounds/Item/ShadowboltReflect");
                    SoundEngine.PlaySound(fireperfect with { Volume = 0.6f, Pitch = 0f }, GunTipPosition);
                    GenericSparkle sparker = new GenericSparkle(GunTipPosition, Vector2.Zero, Color.DarkGoldenrod, Color.Gold, Main.rand.NextFloat(1.1f, 1.8f), 10, 2f, 2.68f);
                    GeneralParticleHandler.SpawnParticle(sparker);
                }
                #endregion

                //If the player hasn't hit any shots, increase the multiplier from 0 to 1 to avoid the rocket doing 0 damage
                if (Owner.Calamity().sharkGunDamageScaling == 0)
                {
                    Owner.Calamity().sharkGunDamageScaling++;
                }
                //Fire the rocket. Damage is 50% of base, multiplied by how many shots the player hits
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity, ModContent.ProjectileType<MegalodonRocket>(), (int)(Projectile.damage * 0.5f) * Owner.Calamity().sharkGunDamageScaling, Projectile.knockBack, Projectile.owner);

                //After firing the rocket, reset all variables to allow left click to be held down
                Time = 2;
                shotCounter = 0;
            }
            Time++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Time < 2)
            return false;
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale * Owner.gravDir, flipSprite);
            return false;
        }
    }
}
