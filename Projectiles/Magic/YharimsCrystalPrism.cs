using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class YharimsCrystalPrism : ModProjectile
    {
        public const int NumBeams = 6;
        public const float MaxCharge = 180f;
        public const float DamageStart = 30f;
        private const float DustStart = 30f;
        private const float AimResponsiveness = 0.89f; // Last Prism is 0.92f. Lower makes the prism turn faster.
        private const int SoundInterval = 20;
        private const float MaxManaConsumptionDelay = 15f;
        private const float MinManaConsumptionDelay = 5f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yermes Christal");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.NeedsUUID[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 22;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.ignoreWater = true;
        }

        // TODO -- Make property wrappers for these variables instead of needing to explain them.
        // ai[0] is a frame counter. ai[1] is the next frame mana will be consumed.
        // localAI[0] is the number of frames between mana consumptions.
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);

            // Update damage based on curent magic damage stat (so Mana Sickness affects it)
            projectile.damage = (int)((player.ActiveItem()?.damage ?? 0) * player.MagicDamage());

            // ai[0] is the overall frame counter.
            projectile.ai[0] += 1f;
            float chargeRatio = MathHelper.Clamp(projectile.ai[0] / MaxCharge, 0f, 1f);

            // Update the crystal's animation, with the animation accelerating as the crystal charges
            projectile.frameCounter++;
            int framesPerAnimationUpdate = projectile.ai[0] >= MaxCharge ? 2 : projectile.ai[0] >= (MaxCharge * 0.66f) ? 3 : 4;
            if (projectile.frameCounter >= framesPerAnimationUpdate)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 6)
                    projectile.frame = 0;
            }

            // Make sound intermittently while the crystal is in use
            if (projectile.soundDelay <= 0)
            {
                projectile.soundDelay = SoundInterval;
                // Don't play the continuous beam sound the first time around
                if (projectile.ai[0] > 1f)
                    Main.PlaySound(SoundID.Item15, projectile.position);
            }

            // Once the crystal reaches a certain charge, start producing dust. More charge = more dust.
            if (projectile.ai[0] > DustStart && Main.rand.NextFloat() < chargeRatio)
                SpawnEjectionDust(chargeRatio);

            UpdatePlayerVisuals(player, rrp);

            // Update the crystal's existence: project beams on frame 1, and despawn if out of mana.
            if (projectile.owner == Main.myPlayer)
            {
                // Scale seemingly never changes, so this just scales with shoot speed (Yharim's Crystal is 30 by default)
                float speedTimesScale = player.ActiveItem().shootSpeed * projectile.scale;
                UpdateAim(rrp, speedTimesScale);

                // CheckMana returns true if the mana cost can be paid. If mana isn't consumed this frame, the CheckMana short-circuits out of being evaluated.
                bool allowContinuedUse = !ShouldConsumeMana() || player.CheckMana(player.ActiveItem().mana, true, false);
                bool crystalStillInUse = player.channel && allowContinuedUse && !player.noItems && !player.CCed;

                // The beams are only projected once (on frame 1).
                if (crystalStillInUse && projectile.ai[0] == 1f)
                {
                    Vector2 beamVelocity = Vector2.Normalize(projectile.velocity);
                    if (beamVelocity.HasNaNs())
                        beamVelocity = -Vector2.UnitY;

                    int damage = projectile.damage;
                    float kb = projectile.knockBack; // should always be 0
                    for (int b = 0; b < NumBeams; b++)
                        Projectile.NewProjectile(projectile.Center, beamVelocity, ModContent.ProjectileType<YharimsCrystalBeam>(), damage, kb, projectile.owner, b, Projectile.GetByUUID(projectile.owner, projectile.whoAmI));
                    projectile.netUpdate = true;
                }
                else if (!crystalStillInUse)
                    projectile.Kill();
            }

            // Ensures the crystal will disappear immediately if anything goes wrong
            projectile.timeLeft = 2;
        }

        private bool ShouldConsumeMana()
        {
            // If the mana consumption timer hasn't been initialized yet, initialize it and consume mana on frame 1.
            if (projectile.localAI[0] == 0f)
            {
                projectile.ai[1] = projectile.localAI[0] = MaxManaConsumptionDelay;
                return true;
            }
            bool consume = projectile.ai[0] == projectile.ai[1];
            if(consume)
            {
                projectile.localAI[0] = MathHelper.Clamp(projectile.localAI[0] - 1f, MinManaConsumptionDelay, MaxManaConsumptionDelay);
                projectile.ai[1] += projectile.localAI[0];
            }
            return consume;
        }

        // Gently adjusts the aim vector of the crystal to point towards the mouse.
        private void UpdateAim(Vector2 source, float speed)
        {
            Vector2 aimVector = Vector2.Normalize(Main.MouseWorld - source);
            if (aimVector.HasNaNs())
                aimVector = -Vector2.UnitY;
            aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(projectile.velocity), AimResponsiveness));
            aimVector *= speed;

            if (aimVector != projectile.velocity)
                projectile.netUpdate = true;
            projectile.velocity = aimVector;
        }

        private void UpdatePlayerVisuals(Player player, Vector2 rrp)
        {
            // Place the projectile directly into the player's hand at all times
            projectile.Center = rrp;
            // The beam comes out of the tip of the crystal, not the side
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
            projectile.spriteDirection = projectile.direction;

            // The crystal is a holdout projectile, so change the player's variables to reflect that
            player.ChangeDir(projectile.direction);
            player.heldProj = projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            // Multiplying by projectile.direction is required due to vanilla spaghetti.
            player.itemRotation = (projectile.velocity * projectile.direction).ToRotation();
        }

        private void SpawnEjectionDust(float charge)
        {
            Vector2 projDir = Vector2.Normalize(projectile.velocity);
            int dustType = 90;
            float dustAngle = MathHelper.Pi * 0.76f * (Main.rand.NextBool() ? 1f : -1f);
            float scale = Main.rand.NextFloat(0.9f, 1.2f);
            float speed = 18f * charge;
            Vector2 dustVel = projDir.RotatedBy(dustAngle) * speed;
            float dustForwardOffset = 11f;
            Vector2 dustOrigin = projectile.Center + dustForwardOffset * projDir;
            Dust d = Dust.NewDustDirect(dustOrigin, 1, 1, dustType, dustVel.X, dustVel.Y);
            d.position += Main.rand.NextVector2Circular(2f, 2f);
            d.noGravity = true;
            d.scale = scale;
        }

        // Completely custom drawcode because it's a holdout projectile. The projectile is also fullbright.
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects eff = projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D tex = Main.projectileTexture[projectile.type];
            int frameHeight = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
            int texYOffset = frameHeight * projectile.frame;
            Vector2 sheetInsertVec = (projectile.Center + Vector2.UnitY * projectile.gfxOffY - Main.screenPosition).Floor();
            Main.spriteBatch.Draw(tex, sheetInsertVec, new Rectangle?(new Rectangle(0, texYOffset, tex.Width, frameHeight)), Color.White, projectile.rotation, new Vector2(tex.Width / 2f, frameHeight / 2f), projectile.scale, eff, 0f);
            return false;
        }
    }
}
