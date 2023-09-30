using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon.Umbrella
{
    public class MagicArrow : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public float Behavior = 0f;
		private const float drawOffset = -MathHelper.PiOver4 + MathHelper.Pi;
        public VertexStrip TrailDrawer;

        public float GetOffsetAngle
        {
            get
            {
                return MathHelper.TwoPi * 1 / 5 + Main.projectile[(int)Projectile.ai[0]].ai[0] / 27f;
            }
        }

        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Behavior);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Behavior = reader.ReadSingle();
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
				IdleAI();
			}
			else
			{
				AttackEnemy(targetIndex);
			}
        }

		private void AttackEnemy(int targetIndex)
		{
			NPC npc = Main.npc[targetIndex];
			Behavior = 0f;
            // Go to the target
			Projectile.velocity = (Projectile.velocity * 5f + Projectile.SafeDirectionTo(npc.Center) * 40f) / 6f;
			Projectile.rotation = Projectile.AngleTo(npc.Center) + drawOffset;
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
				Behavior = 1f;
			}
		}

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public Color TrailColorFunction(float completionRatio)
        {
            float opacity = (float)Math.Pow(Utils.GetLerpValue(1f, 0.45f, completionRatio, true), 4D) * Projectile.Opacity * 0.48f;
            return new Color(211, 8, 8) * opacity;
        }

        public float TrailWidthFunction(float completionRatio) => 2f;

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
			Vector2 origin = frame.Size() * 0.5f;
			Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
			SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			bool shouldDrawTrail = Behavior != 1f;
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

            // Draw the bat.
			Main.spriteBatch.Draw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, direction, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                int dust = Dust.NewDust(Projectile.Center, 1, 1, 67, dspeed.X, dspeed.Y, 50, default, 1.2f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => OnHitEffect();

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => OnHitEffect();

        private void OnHitEffect()
		{
			if (Behavior == 1f)
				return;

            for (int i = 0; i < 10; i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                int dust = Dust.NewDust(Projectile.Center, 1, 1, 67, dspeed.X, dspeed.Y, 50, default, 1.2f);
                Main.dust[dust].noGravity = true;
            }
			Vector2 returnPos = Main.player[Projectile.owner].Center + GetOffsetAngle.ToRotationVector2() * 180f;
			Projectile.Center = returnPos;
            for (int i = 0; i < 10; i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                int dust = Dust.NewDust(Projectile.Center, 1, 1, 67, dspeed.X, dspeed.Y, 50, default, 1.2f);
                Main.dust[dust].noGravity = true;
            }
		}
    }
}
