using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon.Umbrella
{
    public class MagicHammer : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public float Behavior = 0f;
		public float PivotPointX = 0f;
		public float PivotPointY = 0f;
		private int counter = 0;
		private const int projSize = 60;
		private const float drawOffset = -MathHelper.PiOver4 + MathHelper.Pi;
        public static readonly SoundStyle StylishSound = new("CalamityMod/Sounds/Custom/Stylish");

        public VertexStrip TrailDrawer;

        public float GetOffsetAngle
        {
            get
            {
                return MathHelper.TwoPi * 2 / 5 + Main.projectile[(int)Projectile.ai[0]].ai[0] / 27f;
            }
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = projSize;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 40;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Behavior);
            writer.Write(PivotPointX);
            writer.Write(PivotPointY);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Behavior = reader.ReadSingle();
            PivotPointX = reader.ReadSingle();
            PivotPointY = reader.ReadSingle();
        }

        public override void AI()
        {
			Player player = Main.player[Projectile.owner];
            Projectile.alpha -= 50;

			// Set the timeLeft if the player has the hat
			if (player.Calamity().magicHat)
			{
				Projectile.timeLeft = 2;
			}

            float homingRange = MagicHat.Range;
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
                    if (targetIndex == -1 && targetDist < (homingRange + extraDist))
                    {
                        homingRange = targetDist;
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
                            targetIndex = npc.whoAmI;
                        }
                    }
                }
            }

			if (targetIndex == -1)
			{
				Behavior = 0f;
				IdleAI();
			}
			else
			{
				MoveToEnemy(targetIndex);
			}
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
				Projectile.rotation = Projectile.velocity.ToRotation() + drawOffset;
			}
			else
			{
				Projectile.Center = returnPos;
				Projectile.rotation = GetOffsetAngle + drawOffset;
			}
			// Return to normal size
			if (Projectile.scale != 1f)
			{
				Projectile.scale = 1f;
				Projectile.ExpandHitboxBy((int)(projSize * Projectile.scale));
			}
		}

		private void MoveToEnemy(int targetIndex)
		{
			if (Behavior == 0f)
				Behavior = 1f;

			// Target distance calculations
			NPC npc = Main.npc[targetIndex];
			Vector2 targetVec = npc.Center - Projectile.Center;
			float targetDist = targetVec.Length();

			float moveSpeed = 40f;
			// If more than 60 pixels away, move toward the target
			if (targetDist > 60f && Behavior == 1f)
			{
				targetVec.Normalize();
				targetVec *= moveSpeed;
				Projectile.velocity = (Projectile.velocity * 1f + targetVec) / 2f;
				Projectile.rotation = Projectile.velocity.ToRotation() + drawOffset;
			}
			else
			{
				// Get bigger and prepare the pivot position
				if (Behavior == 1f)
				{
					if (Projectile.scale == 1f)
					{
						Projectile.scale = 3f;
						Projectile.ExpandHitboxBy(Projectile.scale);
					}
					Projectile.rotation = drawOffset + MathHelper.ToRadians(20f) * (targetVec.X > 0 ? 3f : -3f);
					Projectile.ai[1] = drawOffset + MathHelper.ToRadians(20f) * (targetVec.X > 0 ? 3f : -3f);
					PivotPointX = Projectile.Bottom.X;
					PivotPointY = Projectile.Bottom.Y;
					Behavior = 2f;
				}
				// Decide which way to swing
				if (Behavior == 2f)
				{
					Behavior = targetVec.X > 0 ? 3f : 4f;
				}
				// Big Bonk Energy
				if (Behavior == 3f || Behavior == 4f)
				{
					float swingTime = 20f;
					Projectile.ai[1] += MathHelper.ToRadians(200f / swingTime) * (Behavior == 3f ? 1f : -1f);
					counter++;
					float outwardPosition = 50f;
					Vector2 pivot = new Vector2(PivotPointX, PivotPointY);
					Projectile.Center = pivot + Projectile.ai[1].ToRotationVector2() * outwardPosition;
					Projectile.rotation = Projectile.ai[1] + drawOffset;
					if (counter > swingTime)
					{
						Projectile.ai[1] = MathHelper.PiOver4;
						counter = 0;
						Behavior = 5f;
					}
				}
				// Head back to the player as a reset between bonks
				if (Behavior == 5f)
					MoveBackToPlayer();
			}
		}

		private void MoveBackToPlayer()
		{
			Player player = Main.player[Projectile.owner];

			const float outwardPosition = 180f;
			Vector2 returnPos = player.Center + GetOffsetAngle.ToRotationVector2() * outwardPosition;

			// Player distance calculations
			Vector2 playerVec = returnPos - Projectile.Center;
			float playerDist = playerVec.Length();

			float playerHomeSpeed = 40f;
			// If more than 60 pixels away, move toward the player
			if (playerDist > 60f)
			{
				playerVec.Normalize();
				playerVec *= playerHomeSpeed;
				Projectile.velocity = (Projectile.velocity * 1f + playerVec) / 2f;
				Projectile.rotation = Projectile.velocity.ToRotation() + drawOffset;
			}
			else
			{
				Behavior = 0f;
			}
			// Return to normal size
			if (Projectile.scale != 1f)
			{
				Projectile.scale = 1f;
				Projectile.ExpandHitboxBy(projSize);
			}
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			// On bonk, there is a small chance for confetti and Stylish energy
            if ((Behavior == 3f || Behavior == 4f) && Main.rand.NextBool(20) && Projectile.soundDelay <= 0)
			{
                Rectangle location = new Rectangle((int)target.position.X, (int)target.position.Y, target.width, target.height);
                CombatText.NewText(location, new Color(239, 113, 152), CalamityUtils.GetTextValue("Misc.StylishHammerHit"), true);
				SoundEngine.PlaySound(StylishSound with { Volume = 0.35f }, target.Center);
                Projectile.soundDelay = 60;
				for (int i = 0; i < 5; i++)
				{
					int confettiDust = Main.rand.Next(139, 143);
					int confetti = Dust.NewDust(target.Center, target.width, target.height, confettiDust, target.velocity.X, target.velocity.Y, 0, new Color(), 1.2f);
					Main.dust[confetti].velocity.X *= Main.rand.NextFloat(0.5f, 1.5f);
					Main.dust[confetti].velocity.Y *= Main.rand.NextFloat(0.5f, 1.5f);
					Main.dust[confetti].velocity.X += Main.rand.NextFloat(-2.5f, 2.5f);
					Main.dust[confetti].velocity.Y += Main.rand.NextFloat(-2.5f, 2.5f);
					Main.dust[confetti].scale *= Main.rand.NextFloat(0.7f, 1.3f);
					if (Main.netMode != NetmodeID.Server && Main.rand.NextBool())
					{
						int confettiGore = Main.rand.Next(276, 283);
						int idx = Gore.NewGore(Projectile.GetSource_FromThis(), target.Center, target.velocity, confettiGore, 1f);
						Main.gore[idx].velocity.X *= Main.rand.NextFloat(0.5f, 1.5f);
						Main.gore[idx].velocity.Y *= Main.rand.NextFloat(0.5f, 1.5f);
						Main.gore[idx].velocity.X += Main.rand.NextFloat(-2.5f, 2.5f);
						Main.gore[idx].velocity.Y += Main.rand.NextFloat(-2.5f, 2.5f);
						Main.gore[idx].scale *= Main.rand.NextFloat(0.8f, 1.2f);
					}
				}
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, Projectile.alpha);

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                int dust = Dust.NewDust(Projectile.Center, 1, 1, 67, dspeed.X, dspeed.Y, 50, default, 1.2f);
                Main.dust[dust].noGravity = true;
            }
        }

        public Color TrailColorFunction(float completionRatio)
        {
            float opacity = (float)Math.Pow(Utils.GetLerpValue(1f, 0.45f, completionRatio, true), 4D) * Projectile.Opacity * 0.48f;
            return new Color(255, 56, 0) * opacity;
        }

        public float TrailWidthFunction(float completionRatio) => 2f;

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
			Vector2 origin = frame.Size() * 0.5f;
			Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
			SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			bool shouldDrawTrail = Behavior != 0f;
			if (shouldDrawTrail)
			{
				// Draw the afterimage trail.
				TrailDrawer ??= new();
				GameShaders.Misc["EmpressBlade"].UseShaderSpecificData(new Vector4(1f, 0f, 0f, 0.6f));
				GameShaders.Misc["EmpressBlade"].Apply(null);
				TrailDrawer.PrepareStrip(Projectile.oldPos, Projectile.oldRot, TrailColorFunction, TrailWidthFunction, Projectile.Size * 0.5f - Main.screenPosition, Projectile.oldPos.Length, true);
				TrailDrawer.DrawTrail();
				Main.pixelShader.CurrentTechnique.Passes[0].Apply();
			}

            // Draw the hammer.
			Main.spriteBatch.Draw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, direction, 0);
            return false;
        }
    }
}
