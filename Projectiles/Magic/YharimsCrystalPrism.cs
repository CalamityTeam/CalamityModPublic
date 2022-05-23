using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

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
            Main.projFrames[Projectile.type] = 6;
            ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 22;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.ignoreWater = true;
        }

        // TODO -- Make property wrappers for these variables instead of needing to explain them.
        // ai[0] is a frame counter. ai[1] is the next frame mana will be consumed.
        // localAI[0] is the number of frames between mana consumptions.
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);

            // Update damage based on curent magic damage stat (so Mana Sickness affects it)
            Projectile.damage = (int)player.GetDamage<MagicDamageClass>().ApplyTo(player.ActiveItem()?.damage ?? 0);

            // ai[0] is the overall frame counter.
            Projectile.ai[0] += 1f;
            float chargeRatio = MathHelper.Clamp(Projectile.ai[0] / MaxCharge, 0f, 1f);

            // Update the crystal's animation, with the animation accelerating as the crystal charges
            Projectile.frameCounter++;
            int framesPerAnimationUpdate = Projectile.ai[0] >= MaxCharge ? 2 : Projectile.ai[0] >= (MaxCharge * 0.66f) ? 3 : 4;
            if (Projectile.frameCounter >= framesPerAnimationUpdate)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 6)
                    Projectile.frame = 0;
            }

            // Make sound intermittently while the crystal is in use
            if (Projectile.soundDelay <= 0)
            {
                Projectile.soundDelay = SoundInterval;
                // Don't play the continuous beam sound the first time around
                if (Projectile.ai[0] > 1f)
                    SoundEngine.PlaySound(SoundID.Item15, Projectile.position);
            }

            // Once the crystal reaches a certain charge, start producing dust. More charge = more dust.
            if (Projectile.ai[0] > DustStart && Main.rand.NextFloat() < chargeRatio)
                SpawnEjectionDust(chargeRatio);

            UpdatePlayerVisuals(player, rrp);

            // Update the crystal's existence: project beams on frame 1, and despawn if out of mana.
            if (Projectile.owner == Main.myPlayer)
            {
                // Scale seemingly never changes, so this just scales with shoot speed (Yharim's Crystal is 30 by default)
                float speedTimesScale = player.ActiveItem().shootSpeed * Projectile.scale;
                UpdateAim(rrp, speedTimesScale);

                // CheckMana returns true if the mana cost can be paid. If mana isn't consumed this frame, the CheckMana short-circuits out of being evaluated.
                bool allowContinuedUse = !ShouldConsumeMana() || player.CheckMana(player.ActiveItem(), -1, true, false);
                bool crystalStillInUse = player.channel && allowContinuedUse && !player.noItems && !player.CCed;

                // The beams are only projected once (on frame 1).
                if (crystalStillInUse && Projectile.ai[0] == 1f)
                {
                    Vector2 beamVelocity = Vector2.Normalize(Projectile.velocity);
                    if (beamVelocity.HasNaNs())
                        beamVelocity = -Vector2.UnitY;

                    int damage = Projectile.damage;
                    float kb = Projectile.knockBack; // should always be 0
                    for (int b = 0; b < NumBeams; b++)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, beamVelocity, ModContent.ProjectileType<YharimsCrystalBeam>(), damage, kb, Projectile.owner, b, Projectile.GetByUUID(Projectile.owner, Projectile.whoAmI));
                    Projectile.netUpdate = true;
                }
                else if (!crystalStillInUse)
                    Projectile.Kill();
            }

            // Ensures the crystal will disappear immediately if anything goes wrong
            Projectile.timeLeft = 2;
        }

        private bool ShouldConsumeMana()
        {
            // If the mana consumption timer hasn't been initialized yet, initialize it and consume mana on frame 1.
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.ai[1] = Projectile.localAI[0] = MaxManaConsumptionDelay;
                return true;
            }
            bool consume = Projectile.ai[0] == Projectile.ai[1];
            if(consume)
            {
                Projectile.localAI[0] = MathHelper.Clamp(Projectile.localAI[0] - 1f, MinManaConsumptionDelay, MaxManaConsumptionDelay);
                Projectile.ai[1] += Projectile.localAI[0];
            }
            return consume;
        }

        // Gently adjusts the aim vector of the crystal to point towards the mouse.
        private void UpdateAim(Vector2 source, float speed)
        {
            Vector2 aimVector = Vector2.Normalize(Main.MouseWorld - source);
            if (aimVector.HasNaNs())
                aimVector = -Vector2.UnitY;
            aimVector = Vector2.Normalize(Vector2.Lerp(aimVector, Vector2.Normalize(Projectile.velocity), AimResponsiveness));
            aimVector *= speed;

            if (aimVector != Projectile.velocity)
                Projectile.netUpdate = true;
            Projectile.velocity = aimVector;
        }

        private void UpdatePlayerVisuals(Player player, Vector2 rrp)
        {
            // Place the projectile directly into the player's hand at all times
            Projectile.Center = rrp;
            // The beam comes out of the tip of the crystal, not the side
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.spriteDirection = Projectile.direction;

            // The crystal is a holdout projectile, so change the player's variables to reflect that
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;

            // Multiplying by projectile.direction is required due to vanilla spaghetti.
            player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }

        private void SpawnEjectionDust(float charge)
        {
            Vector2 projDir = Vector2.Normalize(Projectile.velocity);
            int dustType = 90;
            float dustAngle = MathHelper.Pi * 0.76f * (Main.rand.NextBool() ? 1f : -1f);
            float scale = Main.rand.NextFloat(0.9f, 1.2f);
            float speed = 18f * charge;
            Vector2 dustVel = projDir.RotatedBy(dustAngle) * speed;
            float dustForwardOffset = 11f;
            Vector2 dustOrigin = Projectile.Center + dustForwardOffset * projDir;
            Dust d = Dust.NewDustDirect(dustOrigin, 1, 1, dustType, dustVel.X, dustVel.Y);
            d.position += Main.rand.NextVector2Circular(2f, 2f);
            d.noGravity = true;
            d.scale = scale;
        }

        // Completely custom drawcode because it's a holdout projectile. The projectile is also fullbright.
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteEffects eff = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
            int frameHeight = ModContent.Request<Texture2D>(Texture).Value.Height / Main.projFrames[Projectile.type];
            int texYOffset = frameHeight * Projectile.frame;
            Vector2 sheetInsertVec = (Projectile.Center + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition).Floor();
            Main.spriteBatch.Draw(tex, sheetInsertVec, new Rectangle?(new Rectangle(0, texYOffset, tex.Width, frameHeight)), Color.White, Projectile.rotation, new Vector2(tex.Width / 2f, frameHeight / 2f), Projectile.scale, eff, 0f);
            return false;
        }
    }
}
