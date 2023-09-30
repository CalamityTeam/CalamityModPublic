using CalamityMod.Buffs.DamageOverTime;
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
    public class MagicAxe : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public float Behavior = 0f;
		public ref float ChargeCooldown => ref Projectile.ai[1];
		public ref float TreeCounter => ref Projectile.localAI[0];
		public ref float TreeReset => ref Projectile.localAI[1];
		private const float drawOffset = -MathHelper.PiOver4 + MathHelper.Pi;
        public VertexStrip TrailDrawer;

        public float GetOffsetAngle
        {
            get
            {
                return MathHelper.TwoPi * 3 / 5 + Main.projectile[(int)Projectile.ai[0]].ai[0] / 27f;
            }
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 40;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 52;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.minion = true;
            Projectile.tileCollide = false;
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
				CooldownReset(false);
				IdleAI();
			}
			else
			{
				Projectile.rotation += 0.15f;
				if (!CooldownReset(true))
					return;
				AttackEnemy(targetIndex);
			}
        }

		private bool CooldownReset(bool offense)
		{
            // Breather time between charges as like a reset
            if (Behavior == 2f)
            {
				TreeReset = 0f;
                ChargeCooldown += 1f;
                if (ChargeCooldown > 15f)
                {
                    ChargeCooldown = 1f;
                    Behavior = 0f;
                    Projectile.netUpdate = true;
                }
                else
                {
                    return false;
                }
            }
			if (offense && Behavior == 4f)
				Behavior = 0f;
			return true;
		}

		private void AttackEnemy(int targetIndex)
		{
			NPC npc = Main.npc[targetIndex];
            // Go to the target
            if (Behavior == 0f)
            {
                Vector2 targetSpot = npc.Center - Projectile.Center;
                float targetDist = targetSpot.Length();
                targetSpot.Normalize();
                // Tries to get the minion in the sweet spot of 200 pixels away but the minion also charges so idk what good it does
                if (targetDist > 200f)
                {
                    float speed = 48f;
                    targetSpot *= speed;
                    Projectile.velocity = (Projectile.velocity * 40f + targetSpot) / 41f;
                }
                else
                {
                    float speed = -24f;
                    targetSpot *= speed;
                    Projectile.velocity = (Projectile.velocity * 40f + targetSpot) / 41f; //41
                }
            }

            // Increment attack counter randomly
            if (ChargeCooldown > 0f)
            {
                ChargeCooldown++;
            }
            // If high enough, prepare to attack
            if (ChargeCooldown > 15f)
            {
                ChargeCooldown = 0f;
                Projectile.netUpdate = true;
            }

            // Charge at an enemy if not on cooldown
            if (Behavior == 0f)
            {
				Vector2 targetVec = npc.Center - Projectile.Center;
				float targetDist = targetVec.Length();
                if (ChargeCooldown == 0f && targetDist < 500f)
                {
                    ChargeCooldown += 1f;
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Behavior = 2f;
                        targetVec.Normalize();
                        Projectile.velocity = targetVec * 30f;
                        Projectile.netUpdate = true;
                    }
                }
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
				Behavior = 4f;
			}
			TreeCounter = 0f;
			TreeReset = 0f;
		}

        public override Color? GetAlpha(Color lightColor) => Color.White;

        public Color TrailColorFunction(float completionRatio)
        {
            float opacity = (float)Math.Pow(Utils.GetLerpValue(1f, 0.45f, completionRatio, true), 4D) * Projectile.Opacity * 0.48f;
            return new Color(0, 255, 111) * opacity;
        }

        public float TrailWidthFunction(float completionRatio) => 2f;

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
			Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
			Vector2 origin = frame.Size() * 0.5f;
			Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY);
			SpriteEffects direction = Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

			bool shouldDrawTrail = Behavior != 4f;
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

            // Draw the axe.
			Main.spriteBatch.Draw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, direction, 0);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                int dust = Dust.NewDust(Projectile.Center, 1, 1, 66, dspeed.X, dspeed.Y, 160, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 0.75f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => OnHitEffect(target.Center, target.whoAmI);

        public override void OnHitPlayer(Player target, Player.HurtInfo info) => OnHitEffect(target.Center, target.whoAmI);

        private void OnHitEffect(Vector2 targetPos, int whoAmI)
		{
			// Every 20 hits, spawn a tree
			// Only 1 hit per charge increments this
			if (TreeReset == 0f)
			{
				TreeCounter++;
				TreeReset = 1f;
			}

			if (TreeCounter >= 20f)
			{
				TreeCounter = 0;
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), targetPos + new Vector2(0f, -600f), Vector2.Zero, ModContent.ProjectileType<MagicTree>(), Projectile.damage * 10, Projectile.knockBack * 3f, Projectile.owner, whoAmI);
			}
		}
    }
}
