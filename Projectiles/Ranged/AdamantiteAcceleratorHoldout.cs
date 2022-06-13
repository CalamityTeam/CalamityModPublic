using CalamityMod.Items.Weapons.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria.Audio;
using System;

namespace CalamityMod.Projectiles.Ranged
{
    public class AdamantiteAcceleratorHoldout : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/AdamantiteParticleAccelerator";
        const float maxTimeAnim = 35;
        public Player Owner => Main.player[Projectile.owner];
        public float Timer => maxTimeAnim - Projectile.timeLeft;
        public ref float runTimer => ref Projectile.ai[0];
        public ref float delayTimer => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Adamantite Particle Accelerator");
        }

        public override void SetDefaults()
        {
            Projectile.width = 54;
            Projectile.height = 52;
            // This projectile has no hitboxes and no damage type.
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 900;
            Projectile.velocity = Vector2.Zero;
        }

        // ai[0] is a time-dilated frame counter. ai[1] is the frame counter for when the beams are fired.
        // localAI[0] is the rate at which the "frame" counter increases.
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);

            Projectile.rotation = Owner.MountedCenter.AngleTo(Owner.Calamity().mouseWorld);

            // Calculate how quickly the gun should charge. Charge increases by some number close to 1 every frame.
            // Speed increasing reforges make this number greater than 1. Slowing reforges make it smaller than 1.
            if (Projectile.localAI[0] == 0f)
                Projectile.localAI[0] = 44f / player.ActiveItem().useTime;

            // Increment the timer for the gun. If the timer has passed 44, destroy it.
            runTimer += Projectile.localAI[0];
            int maxTime = AdamantiteParticleAccelerator.ChargeFrames + AdamantiteParticleAccelerator.CooldownFrames;
            if (runTimer > maxTime)
            {
                Projectile.Kill();
                return;
            }

            // Compute the weapon's charge.
            float chargeLevel = MathHelper.Clamp(Projectile.ai[0] / AdamantiteParticleAccelerator.ChargeFrames, 0f, 1f);

            // Common code among holdouts to keep the holdout projectile directly in the player's hand
            UpdatePlayerVisuals(player, rrp);

            // Firing or charging?
            if (chargeLevel >= 1f)
            {
                // Compute the gem position, which is needed for visual effects
                float angle = Projectile.rotation;
                Vector2 dirVector = Owner.direction >= 0 ? new Vector2(1f, -0.35f) : new Vector2(1f, 0.35f);
                Vector2 gemOffset = dirVector * AdamantiteParticleAccelerator.GemDistance; // distance to gem on gun
                Vector2 gemPos = Projectile.Center + gemOffset.RotatedBy(angle);

                //Fires the first beam of the positive polarity on the 1st frame
                if (delayTimer == 0f)
                {
                    FiringEffects(gemPos, new Color(235, 40, 121));
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile redLaser = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), gemPos, Projectile.rotation.ToRotationVector2(), ModContent.ProjectileType<AdamAcceleratorBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, 120);
                        redLaser.Center = gemPos;
                    }
                }

                //Fires the second beam of the negative polarity 8 frames after the first frame
                else if (delayTimer == 8f)
                {
                    FiringEffects(gemPos, new Color(49, 161, 246));
                    if (Projectile.owner == Main.myPlayer)
                    {
                        Projectile blueLaser = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), gemPos, Projectile.rotation.ToRotationVector2(), ModContent.ProjectileType<AdamAcceleratorBeam>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0, -120);
                        blueLaser.Center = gemPos;
                    }
                }
                delayTimer += 1f;
            }
        }

        private void UpdatePlayerVisuals(Player player, Vector2 rrp)
        {
            // Place the projectile directly into the player's hand at all times
            Projectile.Center = rrp;
            
            // The gun is a holdout projectile, so change the player's variables to reflect that
            player.ChangeDir(Math.Sign(Projectile.rotation.ToRotationVector2().X));
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            
        }

        private void FiringEffects(Vector2 center, Color color)
        {
            SoundEngine.PlaySound(SoundID.Item92, center);
            SoundEngine.PlaySound(SoundID.Item60, center);
            int numDust = 36;
            int dustID = 73;
            for (int i = 0; i < numDust; ++i)
            {
                Dust d = Dust.NewDustDirect(center, 0, 0, dustID, 0f, 0f, 64, color);
                d.velocity = (i * MathHelper.TwoPi / numDust).ToRotationVector2() * 2.2f;
                d.scale = 1.4f;
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D gun = Request<Texture2D>("CalamityMod/Items/Weapons/Ranged/AdamantiteParticleAccelerator").Value;

            SpriteEffects flip = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            float drawAngle = Projectile.rotation + (Owner.direction < 0 ? MathHelper.Pi : 0);
            Vector2 drawOrigin = new Vector2(Owner.direction < 0 ? gun.Width-20f : 33f, 33f);
            Vector2 drawOffset = Owner.MountedCenter + drawAngle.ToRotationVector2() * 10f - Main.screenPosition;

            Main.EntitySpriteDraw(gun, drawOffset, null, lightColor, drawAngle, drawOrigin, Projectile.scale, flip, 0);

            return false;
        }
    }
}
