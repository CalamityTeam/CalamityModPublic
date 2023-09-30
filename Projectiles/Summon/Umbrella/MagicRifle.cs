using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon.Umbrella
{
    public class MagicRifle : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public VertexStrip TrailDrawer;
		public bool drawTrail = false;
		public bool leftSide = false;
		public int swapCooldown = 0;
		public float sineCounter = 0f;
		public ref float SwapSides => ref Projectile.localAI[0];
		public ref float SpinCounter => ref Projectile.localAI[1];
		public ref float ShootCooldown => ref Projectile.ai[1];

        public float GetOffsetAngle
        {
            get
            {
                return MathHelper.TwoPi * 5 / 5 + Main.projectile[(int)Projectile.ai[0]].ai[0] / 27f;
            }
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 50;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
        {
            // Set player namespace
            Player player = Main.player[Projectile.owner];

			// Set timeLeft if the hat exists
			if (player.Calamity().magicHat)
			{
				Projectile.timeLeft = 2;
			}

			// Increment counters for swap cooldown and sine counter
			if (swapCooldown > 0)
				swapCooldown--;
			if (swapCooldown == 1)
				leftSide = !leftSide;
			sineCounter++;

            float homingRange = MagicHat.Range;
            Vector2 targetVec = Projectile.position;
            int targetIndex = -1;
            // If targeting something, prioritize that enemy
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(Projectile, false))
                {
                    float extraDist = (npc.width / 2) + (npc.height / 2);
                    // Calculate distance between target and the projectile to know if it's too far or not
                    float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                    if (targetDist < (homingRange + extraDist))
                    {
                        homingRange = targetDist;
                        targetVec = npc.Center;
                        targetIndex = npc.whoAmI;
                    }
                }
            }
            if (targetIndex == -1)
            {
                for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                {
                    NPC npc = Main.npc[npcIndex];
                    if (npc.CanBeChasedBy(Projectile, false))
                    {
                        float extraDist = (npc.width / 2) + (npc.height / 2);
                        // Calculate distance between target and the projectile to know if it's too far or not
                        float targetDist = Vector2.Distance(npc.Center, Projectile.Center);
                        if (targetDist < (homingRange + extraDist))
                        {
                            homingRange = targetDist;
                            targetVec = npc.Center;
							targetIndex = npc.whoAmI;
                        }
                    }
                }
            }

            if (targetIndex == -1)
            {
				IdleAI();
            }
			else
			{
				AttackMovement(targetIndex);
			}

            // Return if on swap cooldown
            if (swapCooldown != 0f)
                return;

            // Increment attack cooldown
            if (ShootCooldown > 0f)
            {
                ShootCooldown += 1f;
            }
            // Set the minion to be ready for attack
            if (ShootCooldown > 45f)
            {
                ShootCooldown = 0f;
                Projectile.netUpdate = true;
            }

            // Return if on attack cooldown, has no target
            if (ShootCooldown != 0f || targetIndex == -1)
                return;

			// Do a little spin before firing a big shot
			if (SwapSides > 5f)
			{
				SpinCounter += MathHelper.ToRadians(60f) * Projectile.spriteDirection;
				if (Math.Abs(SpinCounter) > MathHelper.ToRadians(720f))
				{
					SpinCounter = 0f;
					SwapSides = -1f;
				}
				return;
			}

            // Shoot a bullet
            if (Main.myPlayer == Projectile.owner)
            {
                float projSpeed = 6f;
                int projType = ModContent.ProjectileType<MagicBullet>();
				int damage = Projectile.damage;
				float kback = 0f;
				if (SwapSides == -1f)
				{
					projType = ModContent.ProjectileType<MagicBulletBig>();
					damage *= 2;
					swapCooldown = 30;
					kback = Projectile.knockBack;
				}
                ShootCooldown += 1f;
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 velocity = targetVec - Projectile.Center;
                    velocity.Normalize();
                    velocity *= projSpeed;

                    SoundEngine.PlaySound(SwapSides == -1f ? CommonCalamitySounds.LargeWeaponFireSound : SoundID.Item40, Projectile.position);

                    int bullet = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity, projType, damage, kback, Projectile.owner);
					// For unknown reasons, the bullet does not spawn at the desired position, so let's set that manually
					// I don't think this works regardless
					if (Main.projectile.IndexInRange(bullet))
						Main.projectile[bullet].Center = Projectile.Center;
                    Projectile.netUpdate = true;
                }
                SwapSides += 1f;
            }
        }

		private void AttackMovement(int targetIndex)
		{
			Player player = Main.player[Projectile.owner];
			NPC target = Main.npc[targetIndex];

			Vector2 returnPos = Vector2.Zero;
			Vector2 returnPos1 = target.Right + new Vector2(300f, 0f);
			Vector2 returnPos2 = target.Left - new Vector2(300f, 0f);

			// Swap sides after shooting a big bullet
			returnPos = !leftSide ? returnPos1 : returnPos2;
			if (player.Center.X - target.Center.X < 0)
				returnPos = !leftSide ? returnPos2 : returnPos1;

			// Target distance calculations
			Vector2 targetVec = returnPos - Projectile.Center;
			float targetDist = targetVec.Length();

			float targetHomeSpeed = 60f;
			// If more than 100 pixels away, move toward the target
			if (targetDist > 100f)
			{
				targetVec.Normalize();
				targetVec *= targetHomeSpeed;
				Projectile.velocity = (Projectile.velocity * 10f + targetVec) / 11f;
                Projectile.spriteDirection = Projectile.direction = ((returnPos.X - Projectile.Center.X) > 0).ToDirectionInt();
				Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? MathHelper.ToRadians(-135f) : MathHelper.ToRadians(-45f));
				ShootCooldown = 40f;
			}
			else
			{
                Projectile.spriteDirection = Projectile.direction = ((target.Center.X - Projectile.Center.X) > 0).ToDirectionInt();
				float angle = Projectile.AngleTo(target.Center) + (Projectile.spriteDirection == 1 ? MathHelper.ToRadians(-135f) : MathHelper.ToRadians(-45f));
                Projectile.rotation = (SpinCounter != 0f ? angle + SpinCounter : Projectile.rotation.AngleTowards(angle, 0.3f));
				Projectile.Center = returnPos + new Vector2(0f, ((float)Math.Sin(MathHelper.TwoPi * 0.5f + sineCounter / 50f) * 0.5f + 0.5f) * 40f);
			}
			drawTrail = true;
		}

		private void IdleAI()
		{
			Player player = Main.player[Projectile.owner];

			Vector2 returnPos = player.Center + GetOffsetAngle.ToRotationVector2() * 180f;

			// Player distance calculations
			Vector2 playerVec = returnPos - Projectile.Center;
			float playerDist = playerVec.Length();

			float playerHomeSpeed = 40f;
			// Teleport to the player if abnormally far
			if (playerDist > 2000f)
			{
				Projectile.Center = returnPos;
				Projectile.netUpdate = true;
			}
			// If more than 60 pixels away, move toward the player
			if (playerDist > 60f)
			{
				playerVec.Normalize();
				playerVec *= playerHomeSpeed;
				Projectile.velocity = (Projectile.velocity * 10f + playerVec) / 11f;
				Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == 1 ? MathHelper.ToRadians(-135f) : MathHelper.ToRadians(-45f));
			}
			else
			{
				Projectile.spriteDirection = Projectile.direction = 0;
				Projectile.rotation = GetOffsetAngle + MathHelper.PiOver4;
				drawTrail = false;
				Projectile.Center = returnPos;
			}
			SwapSides = 0f;
		}

        public override bool? CanDamage() => false;

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                int dust = Dust.NewDust(Projectile.Center, 1, 1, 66, dspeed.X, dspeed.Y, 160, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 0.75f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public Color TrailColorFunction(float completionRatio)
        {
            float opacity = (float)Math.Pow(Utils.GetLerpValue(1f, 0.45f, completionRatio, true), 4D) * Projectile.Opacity * 0.48f;
            return new Color(148, 0, 211) * opacity;
        }

        public float TrailWidthFunction(float completionRatio) => 2f;

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
			Vector2 origin = frame.Size() * 0.5f;
			Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
			SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			if (drawTrail)
			{
				// Draw the afterimage trail.
				TrailDrawer ??= new();
				GameShaders.Misc["EmpressBlade"].UseShaderSpecificData(new Vector4(1f, 0f, 0f, 0.6f));
				GameShaders.Misc["EmpressBlade"].Apply(null);
				TrailDrawer.PrepareStrip(Projectile.oldPos, Projectile.oldRot, TrailColorFunction, TrailWidthFunction, Projectile.Size * 0.5f - Main.screenPosition, Projectile.oldPos.Length, true);
				TrailDrawer.DrawTrail();
				Main.pixelShader.CurrentTechnique.Passes[0].Apply();

				direction |= SpriteEffects.FlipVertically;
			}

            // Draw the rifle.
			Main.spriteBatch.Draw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, direction, 0);
            return false;
        }
    }
}
