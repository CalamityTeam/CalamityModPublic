using CalamityMod.Dusts;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class AetherfluxCannonHoldout : BaseGunHoldoutProjectile
    {
        public override int AssociatedItemID => ModContent.ItemType<AetherfluxCannon>();
        public override float MaxOffsetLengthFromArm => 10f;
        public override float OffsetXUpwards => -15f;
        public override float OffsetXDownwards => 10f;
        public override float BaseOffsetY => -15f;
        public override float OffsetYUpwards => 8f;
        public override float OffsetYDownwards => 15f;
        public override string Texture => "CalamityMod/Projectiles/Magic/AetherfluxCannonHoldout";

        private ref float DeployedFrames => ref Projectile.ai[0];
        private ref float AnimationRate => ref Projectile.ai[1];
        private ref float LastShootAttemptTime => ref Projectile.localAI[0];
        private ref float LastAnimationTime => ref Projectile.localAI[1];

        public float postShotFade = 0;

        public override void SetStaticDefaults() => Main.projFrames[Type] = 8;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void KillHoldoutLogic()
        {
            base.KillHoldoutLogic();
            bool actuallyShoot = DeployedFrames >= (HeldItem?.useAnimation ?? AetherfluxCannon.UseTime);
            bool manaOK = !actuallyShoot || Owner.CheckMana(Owner.HeldItem);
            if (!manaOK)
                Projectile.Kill();
        }

        public override void HoldoutAI()
        {
            // Play a sound frame 1.
            if (DeployedFrames <= 0f)
                SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal with { Volume = SoundID.DD2_DarkMageCastHeal.Volume * 1.5f }, Projectile.Center);

            // Update damage based on curent magic damage stat (so Mana Sickness affects it)
            Projectile.damage = HeldItem is null ? 0 : Owner.GetWeaponDamage(HeldItem);

            // Get the original weapon's use time.
            int itemUseTime = HeldItem?.useAnimation ?? AetherfluxCannon.UseTime;
            // 36, base use time, will result in 5. Speed increasing reforges push it to 4.
            int framesPerShot = itemUseTime / 7;

            // Update time.
            DeployedFrames += 1f;

            // Determine animation rate. If the gun is spinning up, it increases linearly. Otherwise it's maxed.
            AnimationRate = DeployedFrames >= itemUseTime ? 2f : MathHelper.Lerp(7f, 2f, DeployedFrames / itemUseTime);

            // Update the animation. This occurs even when firing is not occurring.
            if (DeployedFrames - LastAnimationTime >= AnimationRate)
            {
                LastAnimationTime = DeployedFrames;
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
            }

            // Once past the initial spin-up time, fire constantly as long as mana is available.
            // Before the spin-up is done, sparks are produced but no lasers come out (and no mana is consumed).
            if (DeployedFrames - LastShootAttemptTime >= framesPerShot)
            {
                LastShootAttemptTime = DeployedFrames;
                bool actuallyShoot = DeployedFrames >= itemUseTime;
                bool manaOK = !actuallyShoot || Owner.CheckMana(Owner.HeldItem, -1, true, false);
                if (manaOK)
                {
                    if (actuallyShoot)
                    {
                        postShotFade = 1;
                        SoundStyle fire = new("CalamityMod/Sounds/Item/MagnaCannonShot");
                        SoundEngine.PlaySound(fire with { Volume = 0.4f, Pitch = Main.rand.NextFloat(0.1f, 0.3f) }, Projectile.Center);
                    }


                    int projID = ModContent.ProjectileType<PhasedGodRay>();
                    float shootSpeed = HeldItem.shootSpeed;
                    Vector2 shootDirection = Projectile.velocity.SafeNormalize(Vector2.UnitY);
                    Vector2 shootVelocity = shootDirection * shootSpeed;

                    // Waving beams need to start offset so they cross each other neatly.
                    float waveSideOffset = Main.rand.NextFloat(18f, 28f);
                    Vector2 perp = shootDirection.RotatedBy(-MathHelper.PiOver2) * waveSideOffset;

                    // Dust chaotically sheds off the crystal while charging or firing.
                    float dustInaccuracy = 0.045f;

                    for (int i = -1; i <= 1; i += 2)
                    {
                        Vector2 laserStartPos = GunTipPosition + i * perp + Main.rand.NextVector2CircularEdge(6f, 6f);
                        Vector2 dustOnlySpread = Main.rand.NextVector2Circular(shootSpeed, shootSpeed);
                        Vector2 dustVelocity = shootVelocity + dustInaccuracy * dustOnlySpread;
                        if (actuallyShoot && Main.myPlayer == Projectile.owner)
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), laserStartPos, shootVelocity, projID, Projectile.damage, Projectile.knockBack, Projectile.owner, i * 0.5f);
                        SpawnFiringDust(GunTipPosition, dustVelocity);
                    }
                }
            }
            postShotFade *= 0.86f;
        }

        private void SpawnFiringDust(Vector2 GunTipPosition, Vector2 laserVelocity)
        {
            int dustID = ModContent.DustType<LightDust>();
            int dustRadius = 12;
            float dustRandomness = 11f;
            int dustDiameter = 2 * dustRadius;
            Vector2 dustCorner = GunTipPosition - Vector2.One * dustRadius;
            for (int i = 0; i < 2; i++)
            {
                Vector2 dustVel = laserVelocity + Main.rand.NextVector2Circular(dustRandomness, dustRandomness);
                Dust d = Dust.NewDustDirect(dustCorner, dustDiameter, dustDiameter, dustID, dustVel.X, dustVel.Y);
                d.velocity *= 0.18f;
                d.noGravity = true;
                d.scale = 0.6f;
                d.color = Color.Lerp(AetherfluxCannon.accentColor, AetherfluxCannon.mainColor, Main.rand.NextFloat());
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Vector2 vel = Projectile.velocity * 10;
            Vector2 drawPosition = (Projectile.Center - Main.screenPosition + vel) + vel * (1 - postShotFade);
            Rectangle frame = texture.Frame(verticalFrames: Main.projFrames[Type], frameY: Projectile.frame);
            float drawRotation = Projectile.rotation + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f) + MathHelper.PiOver2;
            Vector2 rotationPoint = frame.Size() * 0.5f;
            SpriteEffects flipSprite = (Projectile.spriteDirection * Owner.gravDir == -1) ? SpriteEffects.FlipVertically : SpriteEffects.None;
            Texture2D glowTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/BloomCircle").Value;
            for (int i = 0; i < 5; i++)
            {
                float glowScale = 0.3f;
                Main.EntitySpriteDraw(glowTexture, drawPosition + vel * 1.7f, null, Color.Lerp(AetherfluxCannon.accentColor, AetherfluxCannon.mainColor, postShotFade) with { A = 0 }, vel.ToRotation(), glowTexture.Size() * 0.5f, new Vector2(1.4f, 0.75f) * (glowScale - i * 0.05f) * postShotFade * 2.5f, SpriteEffects.None, 0);
            }
            for (int i = 0; i < 15; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 15f).ToRotationVector2() * 6.5f * postShotFade;
                Main.EntitySpriteDraw(texture, drawPosition + drawOffset, frame, Color.Lerp(AetherfluxCannon.accentColor, AetherfluxCannon.mainColor, postShotFade) with { A = 0 }, drawRotation, rotationPoint, Projectile.scale * Owner.gravDir, flipSprite);
            }
            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), drawRotation, rotationPoint, Projectile.scale * Owner.gravDir, flipSprite);

            return false;
        }
    }
}
