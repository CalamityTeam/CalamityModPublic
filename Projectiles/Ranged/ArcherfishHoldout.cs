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
using CalamityMod.Projectiles.Melee;

namespace CalamityMod.Projectiles.Ranged
{
    public class ArcherfishHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<Archerfish>();
        public override float MaxOffsetLengthFromArm => 24f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -1f;
        public override float OffsetYDownwards => 5f;

        public int Time = 0;
        public int shotCounter = 0;
        public int framesBetweenShots = 0;
        public bool PostVolley = false;

        public override void KillHoldoutLogic()
        {
            //If the player is dead, kill the holdout.
            if (Owner.CantUseHoldout() || !PostVolley && HeldItem.type != Owner.HeldItem.type)
                Projectile.Kill();
        }
        
        public override void HoldoutAI()
        {
            //Set the scaling to 0 as soon as the holdout spawns
            if (Time == 1)
            {
                Owner.Calamity().sharkGunDamageScaling = 0;
            }
            if (Time >= 10)
            {
                if (framesBetweenShots == 0 && shotCounter <= 25)
                {
                    Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 15;
                    #region Debug Text Display
                    //Main.NewText(shotCounter);
                    //Main.NewText(Owner.Calamity().sharkGunDamageScaling);
                    #endregion
                    #region Visuals and Sounds
                    SoundEngine.PlaySound(SoundID.Item85, Projectile.Center);
                    #endregion

                    //How many frames between firing projectiles, and how far the gun moves backward to give the effect of recoil. Change this number to edit fire rate
                    framesBetweenShots = 6;
                    OffsetLengthFromArm -= 3f;
                    //Here we detect which ammo the bullets will use
                    Owner.PickAmmo(Owner.HeldItem, out int bulletAMMO, out float SpeedNoUse, out int bulletDamage, out float kBackNoUse, out _, !Main.rand.NextBool(4));
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity.RotatedByRandom(MathHelper.ToRadians(1.5f)), ModContent.ProjectileType<ArcherfishShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                    shotCounter++;
                    //Frame time is much longer due to Archerfish working differently, and handling both the shot and the "reload" on the same step
                    if (shotCounter == 25)
                        framesBetweenShots = 100;
                }
                if (framesBetweenShots > 0)
                    framesBetweenShots--;
            }
            if (shotCounter == 25 && framesBetweenShots > 0)
            {   
                //Holdout cannot be killed during the final shot and cooldown
                PostVolley = true;
                Owner.channel = true;
                //If the player hasn't hit any shots, increase the multiplier from 0 to 1 to avoid the final shot doing 0 damage
                if (Owner.Calamity().sharkGunDamageScaling == 0)
                {
                    Owner.Calamity().sharkGunDamageScaling++;
                }
                //Pauses before triggering the final water blast. This allows extra water streams time to hit. Increasing this number reduces the delay, but may also cause the gun to become inconsistent
                if (framesBetweenShots == 80)
                {
                    SoundEngine.PlaySound((Main.rand.NextBool(2) ? SoundID.Item85 : SoundID.Item86).WithPitchOffset(-0.5f), Owner.Center);
                    SoundEngine.PlaySound(SoundID.NPCDeath14.WithPitchOffset(1f), Owner.Center);
                    //Fires the water jet. Damage is 20% of base, multiplied by how many shots hit.
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), GunTipPosition, Projectile.velocity.SafeNormalize(Vector2.UnitY) * 30, ModContent.ProjectileType<MantisClawJet>(), (int)(Projectile.damage * 0.2f) * Owner.Calamity().sharkGunDamageScaling, Projectile.knockBack, Projectile.owner);
                }
                #region Post Firing Visuals and Sounds
                //After the water blast is fired, make the fish "sweat" during the reload
                //I don't think this is how science works
                if (framesBetweenShots % 6 == 0 && framesBetweenShots <= 80)
                {
                    for (int i = 0; i < 2; ++i)
                    {
                        int bloodLifetime = Main.rand.Next(12, 15);
                        float bloodScale = Main.rand.NextFloat(0.4f, 0.6f);
                        Color bloodColor = Color.Lerp(Color.LightBlue * 0.7f, Color.LightBlue, Main.rand.NextFloat());
                        bloodColor = Color.Lerp(bloodColor, new Color(51, 22, 94), Main.rand.NextFloat(0.65f));

                        if (Main.rand.NextBool(20))
                            bloodScale *= 1.5f;

                        float randomSpeedMultiplier = Main.rand.NextFloat(0.8f, 1.5f);
                        Vector2 bloodVelocity = Main.rand.NextVector2Unit() * 2 * randomSpeedMultiplier;
                        bloodVelocity.Y -= 3f;
                        BloodParticle blood = new BloodParticle(Projectile.Center + (Projectile.velocity * 14f).RotatedBy(-0.8f * Projectile.direction), bloodVelocity, bloodLifetime, bloodScale, bloodColor);
                        GeneralParticleHandler.SpawnParticle(blood);
                    }
                }
                //Takes a heavy breath every 25 frames after the final shot is fired
                if (framesBetweenShots % 25 == 0 && framesBetweenShots <= 75)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 4;
                        Particle smoke = new HeavySmokeParticle(GunTipPosition, shootVelocity.RotatedByRandom(0.6f) * Main.rand.NextFloat(0.2f, 1.5f), Color.White, Main.rand.Next(20, 40 + 1), Main.rand.NextFloat(0.2f, 0.3f), 0.5f, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextBool(), required: true);
                        GeneralParticleHandler.SpawnParticle(smoke);
                    }
                    SoundEngine.PlaySound(SoundID.Item111 with { Pitch = 0.05f, PitchVariance = 0.25f, MaxInstances = -1 }, Projectile.Center);
                    OffsetLengthFromArm -= 4.5f;
                }
                #endregion

            }
            if (shotCounter == 25 && framesBetweenShots == 0)
            {
                if (framesBetweenShots == 0)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath19, Projectile.Center);
                    //After firing the water blast, kill the projectile to allow left click to be held down
                    Projectile.Kill();
                }
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
