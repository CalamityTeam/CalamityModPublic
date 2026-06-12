using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace CalamityMod.Projectiles.Ranged
{
    public class OmniGunHoldout : BaseGunHoldoutProjectile
    {

        public static Asset<Texture2D> MuzzleFlash;
        public override int AssociatedItemID => ModContent.ItemType<OmniGun>();
        public override float MaxOffsetLengthFromArm => 24f;
        public override float OffsetXUpwards => -5f;
        public override float BaseOffsetY => -1f;
        public override float OffsetYDownwards => 5f;
        public override Vector2 GunTipPosition => Projectile.Center + (Projectile.velocity * 51);
        public override float RecoilResolveSpeed => CurrentRecoilResolveSpeed;

        public float CurrentRecoilResolveSpeed = 0;
        public int Time = 0;
        public int shotCounter = 0;
        public int framesBetweenShots = 0;
        private float flashVariant;
        public override void Load()
        {
            MuzzleFlash = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Ranged/M1GarandMuzzleFlash", AssetRequestMode.ImmediateLoad);
        }

        public override void KillHoldoutLogic()
        {
            //If the player is dead, kill the holdout.
            if (Owner.CantUseHoldout() || HeldItem.type != AssociatedItemID)
                Projectile.Kill();
        }

        public override void HoldoutAI()
        {
            //Pause to give the effect of reloading
            if (Time == 20)
            {
                Player Owner = Main.player[Projectile.owner];
                SoundEngine.PlaySound(SoundID.Item149, Projectile.Center);
            }
            if (Time >= 70)
            {
                if (framesBetweenShots == 0)
                {
                    //Here we detect which ammo the bullets will use
                    Owner.PickAmmo(Owner.HeldItem, out int bulletAMMO, out float SpeedNoUse, out int bulletDamage, out float kBackNoUse, out _);
                    #region Sniper Shot
                    if (shotCounter == 0)
                    {
                        Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 19;
                        #region Visuals and Sounds
                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/GunShotBig") with { Volume = 1f, Pitch = -0.1f }, Projectile.Center);
                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Item/GunShotHeavy") with { Volume = 0.35f, Pitch = -0.15f }, Projectile.Center);

                        if (Main.netMode != NetmodeID.Server)
                        {
                            string goreType = "OmniSniperCasing";
                            Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center + (-Projectile.velocity * 6), -Projectile.velocity * 4f, Mod.Find<ModGore>(goreType).Type);
                        }
                        for (int i = 0; i <= 4; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? 303 : 244, (shootVelocity * Main.rand.NextFloat(0.2f, 1.1f)).RotatedByRandom(0.4f));
                            dust.noGravity = true;
                            dust.scale = Main.rand.NextFloat(0.8f, 1.4f);
                        }
                        // "Smoke" ring
                        Particle shootPulse = new DirectionalPulseRing(GunTipPosition, Projectile.velocity.SafeNormalize(Vector2.UnitY) * 4f, Color.Gray * 0.5f, new Vector2(0.5f, 1f), Projectile.velocity.ToRotation(), 0.1f, 0.475f, 18);
                        GeneralParticleHandler.SpawnParticle(shootPulse);
                        #endregion
                        OffsetLengthFromArm -= 20f;
                        CurrentRecoilResolveSpeed = 0.14f;

                        if (Main.myPlayer == Projectile.owner)
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity.RotatedByRandom(MathHelper.ToRadians(1.5f)), ModContent.ProjectileType<OmniSniperShot>(), (int)(Projectile.damage * 6f), Projectile.knockBack, Projectile.owner);
                        shotCounter++;
                        framesBetweenShots = 40;
                    }
                    #endregion
                    #region AR Burst
                    if (shotCounter >= 1 && shotCounter <= 10 && framesBetweenShots == 0)
                    {
                        Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 19;
                        #region Visuals and Sounds
                        SoundStyle fire = new("CalamityMod/Sounds/Item/GunShotSmallAlt");
                        SoundEngine.PlaySound(fire with { Volume = 0.7f, Pitch = 0.1f}, Projectile.Center);
                        if (Main.netMode != NetmodeID.Server)
                        {
                            string goreType = "OmniARCasing";
                            Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center + (-Projectile.velocity * 9), Projectile.velocity.RotatedBy(2f * -Owner.direction) * Main.rand.NextFloat(0.6f, 0.7f), Mod.Find<ModGore>(goreType).Type);
                        }
                        // Direct smoke
                        for (int i = 0; i < 2; i++)
                        {
                            Particle smoke = new HeavySmokeParticle(GunTipPosition, Projectile.velocity.SafeNormalize(Vector2.UnitY) * 7f * Main.rand.NextFloat(0.4f, 1.5f), Color.Gray, Main.rand.Next(30, 50), Main.rand.NextFloat(0.22f, 0.44f), 0.35f, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextBool(), required: true);
                            GeneralParticleHandler.SpawnParticle(smoke);
                        }
                        for (int i = 0; i <= 4; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? 303 : 244, (shootVelocity * Main.rand.NextFloat(0.2f, 1.1f)).RotatedByRandom(0.4f));
                            dust.noGravity = true;
                            dust.scale = Main.rand.NextFloat(0.8f, 1.4f);
                        }
                        #endregion
                        framesBetweenShots = 8;
                        OffsetLengthFromArm -= 3f;
                        CurrentRecoilResolveSpeed = 0.3f;
                        if (Main.myPlayer == Projectile.owner)
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity.RotatedByRandom(MathHelper.ToRadians(4f)), bulletAMMO, Projectile.damage, Projectile.knockBack, Projectile.owner);
                        shotCounter++;
                        if (shotCounter > 10)
                            framesBetweenShots = 30;
                    }
                    #endregion
                    #region Shotgun Blast
                    if (shotCounter == 11 && framesBetweenShots == 0)
                    {
                        Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 19;
                        #region Visuals and Sounds
                        SoundStyle FireSound = new("CalamityMod/Sounds/Item/Hydra");
                        SoundEngine.PlaySound(FireSound with { Volume = 0.7f }, Projectile.Center);
                        if (Main.netMode != NetmodeID.Server)
                        {
                            string goreType = "OmniShotgunShell";
                            Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center + (-Projectile.velocity * 9), Projectile.velocity.RotatedBy(2f * -Owner.direction) * Main.rand.NextFloat(0.6f, 0.7f), Mod.Find<ModGore>(goreType).Type);
                        }
                        for (int i = 0; i <= 4; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(GunTipPosition, Main.rand.NextBool(3) ? 303 : 244, (shootVelocity * Main.rand.NextFloat(0.2f, 1.1f)).RotatedByRandom(0.4f));
                            dust.noGravity = true;
                            dust.scale = Main.rand.NextFloat(0.8f, 1.4f);
                        }
                        // Direct smoke
                        for (int i = 0; i < 16; i++)
                        {
                            Particle smoke = new HeavySmokeParticle(GunTipPosition, Projectile.velocity.SafeNormalize(Vector2.UnitY) * 7f * Main.rand.NextFloat(0.4f, 1.5f), Color.Gray, Main.rand.Next(30, 50), Main.rand.NextFloat(0.22f, 0.44f), 0.35f, Main.rand.NextFloat(-0.2f, 0.2f), Main.rand.NextBool(), required: true);
                            GeneralParticleHandler.SpawnParticle(smoke);
                        }
                        // "Smoke" ring
                        Particle shootPulse = new DirectionalPulseRing(GunTipPosition, Projectile.velocity.SafeNormalize(Vector2.UnitY) * 4f, Color.Gray * 0.5f, new Vector2(0.5f, 1f), Projectile.velocity.ToRotation(), 0.1f, 0.475f, 18);
                        GeneralParticleHandler.SpawnParticle(shootPulse);
                        #endregion
                        framesBetweenShots = 8;
                        OffsetLengthFromArm -= 20f;
                        CurrentRecoilResolveSpeed = 0.12f;

                        // Propel the owner backward
                        Owner.velocity += -Projectile.velocity * 7f;
                        if (Main.myPlayer == Projectile.owner)
                        {
                            for (int i = 0; i < 8; i++)
                                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.5f, 1.5f), ModContent.ProjectileType<OmniShotgunShot>(), (int)(Projectile.damage * 3f), Projectile.knockBack, Projectile.owner);
                        }
                        shotCounter++;
                        if (shotCounter > 11)
                            framesBetweenShots = 30;
                    }
                    #endregion
                    #region Grenade
                    if (shotCounter == 12 && framesBetweenShots == 0)
                    {
                        Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 30;
                        #region Visuals and Sounds
                        SoundStyle fire = new("CalamityMod/Sounds/Item/LauncherHeavyShot");
                        SoundEngine.PlaySound(fire with { Volume = 0.8f, PitchVariance = 0.1f }, Projectile.Center);
                        if (Main.netMode != NetmodeID.Server)
                        {
                            string goreType = "OmniGrenadeShell";
                            Gore.NewGore(Projectile.GetSource_FromAI(), Projectile.Center + (-Projectile.velocity * 6), -Projectile.velocity * 4f, Mod.Find<ModGore>(goreType).Type);
                        }
                        for (int i = 0; i <= 13; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(GunTipPosition, 303, shootVelocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.2f, 0.4f), 80, default, Main.rand.NextFloat(0.4f, 1.3f));
                            dust.noGravity = false;
                            dust.color = Color.White;

                            Dust dust2 = Dust.NewDustPerfect(GunTipPosition, 278, shootVelocity.RotatedByRandom(0.5f) * Main.rand.NextFloat(0.1f, 0.5f));
                            dust2.noGravity = true;
                            dust2.scale = Main.rand.NextFloat(0.52f, 0.72f);
                            dust2.color = Color.Lerp(Color.Orange, Color.Gold, Main.rand.NextFloat(0f, 1f));
                        }
                        ;
                        #endregion
                        OffsetLengthFromArm -= 24f;
                        CurrentRecoilResolveSpeed = 0.15f;
                        if (Main.myPlayer == Projectile.owner)
                            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), GunTipPosition, shootVelocity, ModContent.ProjectileType<OmniGrenade>(), (int)(Projectile.damage * 9f), Projectile.knockBack, Projectile.owner);
                        Time = 0;
                        shotCounter = 0;
                    }
                    #endregion
                }
                if (framesBetweenShots > 0)
                    framesBetweenShots--;
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
            float scale = Projectile.scale * Owner.gravDir;
            Vector2 rotationPoint = texture.Size() * 0.5f;
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Flash
            if (Time == 70 || shotCounter == 11 && framesBetweenShots == 0)
            {
                Texture2D flashTex = MuzzleFlash.Value;
                int flashFrameY = (int)flashVariant;
                Vector2 flashPos = GunTipPosition + Vector2.UnitX.RotatedBy(Projectile.rotation) * MuzzleFlash.Width() * 0.4f - Main.screenPosition;
                Rectangle flashFrame = flashTex.Frame(verticalFrames: 3, frameY: flashFrameY);
                Main.EntitySpriteDraw(flashTex, flashPos, flashFrame, Color.White, drawRotation, flashFrame.Size() * 0.5f, scale * 0.8f, flipSprite);
            }

            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, scale, flipSprite);
            return false;
        }
    }
}
