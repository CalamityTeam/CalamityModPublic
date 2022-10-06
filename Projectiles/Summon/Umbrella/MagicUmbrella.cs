using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon.Umbrella
{
    public class MagicUmbrella : ModProjectile
   {
        public float Behavior = 0f;
        public override void SetStaticDefaults()
		{
            DisplayName.SetDefault("Umbrella");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
		{
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 0f;
            Projectile.timeLeft = 180;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
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
            Projectile.rotation += 0.075f;
            Projectile.alpha -= 50;

			if (Main.player[Projectile.owner].Calamity().magicHat)
			{
				Projectile.timeLeft = 2;
			}
			Projectile.ai[0] -= MathHelper.ToRadians(4f);

			List<int> blackListedTargets = new List<int> {};

			DelegateMethods.v3_1 = Color.Transparent.ToVector3();
			Point point = Projectile.Center.ToTileCoordinates();
			DelegateMethods.CastLightOpen(point.X, point.Y);

			blackListedTargets.Clear();
			AI_156_Think(blackListedTargets);
        }

		private void AI_156_Think(List<int> blacklist)
		{
			int num = 40;
			int num2 = num - 1;
			int num3 = num + 40;
			int num4 = num3 - 1;
			int num5 = num + 1;

			Player player = Main.player[Projectile.owner];

			// Idle AI when too far away from the player
			if (player.active && Vector2.Distance(player.Center, Projectile.Center) > 2000f)
			{
				Behavior = 0f;
				Projectile.ai[1] = 0f;
				Projectile.netUpdate = true;
			}

			// Idle AI when Behavior is -1
			if (Behavior == -1f)
			{
				IdleAI();
				return;
			}

			// Idle AI when Behavior is 0
			if (Behavior == 0f)
			{
				IdleAI();
				// Look for a target
				int targetIdx = FindATarget(blacklist);
				if (targetIdx != -1)
				{
					Behavior = Main.rand.NextFromList<int>(num, num3);
					Behavior = num3;
					Projectile.ai[1] = targetIdx;
					Projectile.netUpdate = true;
				}
				return;
			}

			int num13 = 0;
			int num14 = num2;
			int num15 = 0;
			if (Behavior >= (float)num5)
			{
				num13 = 1;
				num14 = num4;
				num15 = num5;
			}

			int currentTarget = (int)Projectile.ai[1];
			if (!Main.npc.IndexInRange(currentTarget))
			{
				int targetIdx = FindATarget(blacklist);
				if (targetIdx != -1)
			{
					Behavior = Main.rand.NextFromList<int>(num, num3);
					Projectile.ai[1] = targetIdx;
					Projectile.netUpdate = true;
				}
				else
				{
					Behavior = -1f;
					Projectile.ai[1] = 0f;
					Projectile.netUpdate = true;
				}

				return;
			}

			NPC npc = Main.npc[currentTarget];
			if (!npc.CanBeChasedBy(this))
			{
				int targetIdx = FindATarget(blacklist);
				if (targetIdx != -1)
				{
					Behavior = Main.rand.NextFromList<int>(num, num3);
					Projectile.ai[1] = targetIdx;
					Projectile.netUpdate = true;
				}
				else
				{
					Behavior = -1f;
					Projectile.ai[1] = 0f;
					Projectile.netUpdate = true;
				}

				return;
			}

			Behavior -= 1f;
			if (Behavior >= (float)num14)
			{
				Projectile.direction = ((Projectile.Center.X < npc.Center.X) ? 1 : (-1));
				if (Behavior == (float)num14)
				{
					Projectile.localAI[0] = Projectile.Center.X;
					Projectile.localAI[1] = Projectile.Center.Y;
				}
			}

			float lerpValue2 = Utils.GetLerpValue(num14, num15, Behavior, clamped: true);
			if (num13 == 0)
			{
				Vector2 vector6 = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
				if (lerpValue2 >= 0.5f)
					vector6 = Vector2.Lerp(npc.Center, Main.player[Projectile.owner].Center, 0.5f);

				Vector2 center2 = npc.Center;
				float num19 = (center2 - vector6).ToRotation();
				float num20 = (Projectile.direction == 1) ? (-(float)Math.PI) : ((float)Math.PI);
				float num21 = num20 + (0f - num20) * lerpValue2 * 2f;
				Vector2 vector7 = num21.ToRotationVector2();
				vector7.Y *= 0.5f;
				vector7.Y *= 0.8f + (float)Math.Sin((float)Projectile.identity * 2.3f) * 0.2f;
				vector7 = vector7.RotatedBy(num19);
				float scaleFactor2 = (center2 - vector6).Length() / 2f;
				Vector2 vector9 = Projectile.Center = Vector2.Lerp(vector6, center2, 0.5f) + vector7 * scaleFactor2;
				float num22 = MathHelper.WrapAngle(num19 + num21 + 0f);
				Projectile.rotation = num22 + (float)Math.PI / 2f + MathHelper.PiOver4;
				Vector2 vector10 = Projectile.velocity = num22.ToRotationVector2() * 10f;
				Projectile.position -= Projectile.velocity;
			}

			if (num13 == 1)
			{
				Vector2 vector11 = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
				vector11 += new Vector2(0f, Utils.GetLerpValue(0f, 0.4f, lerpValue2, clamped: true) * -100f);
				Vector2 v = npc.Center - vector11;
				Vector2 value = v.SafeNormalize(Vector2.Zero) * MathHelper.Clamp(v.Length(), 60f, 150f);
				Vector2 value2 = npc.Center + value;
				float lerpValue3 = Utils.GetLerpValue(0.4f, 0.6f, lerpValue2, clamped: true);
				float lerpValue4 = Utils.GetLerpValue(0.6f, 1f, lerpValue2, clamped: true);
				float targetAngle = v.SafeNormalize(Vector2.Zero).ToRotation() + (float)Math.PI / 2f;
				Projectile.rotation = Projectile.rotation.AngleTowards(targetAngle, (float)Math.PI / 5f) + MathHelper.PiOver4;
				Projectile.Center = Vector2.Lerp(vector11, npc.Center, lerpValue3);
				if (lerpValue4 > 0f)
					Projectile.Center = Vector2.Lerp(npc.Center, value2, lerpValue4);
			}

			if (Behavior == (float)num15)
			{
				int targetIdx = FindATarget(blacklist);
				if (targetIdx != -1)
				{
					Behavior = Main.rand.NextFromList<int>(num, num3);
					Projectile.ai[1] = targetIdx;
					Projectile.netUpdate = true;
				}
				else
				{
					Behavior = -1f;
					Projectile.ai[1] = 0f;
					Projectile.netUpdate = true;
				}
			}
		}

		private int FindATarget(List<int> blackListedTargets)
		{
			Vector2 center = Main.player[Projectile.owner].Center;
			int target = -1;
			float closestDist = -1f;
			NPC selectedTarget = Projectile.OwnerMinionAttackTargetNPC;
			if (selectedTarget != null && selectedTarget.CanBeChasedBy(this))
			{
				bool flag = true;
				if (!selectedTarget.boss && blackListedTargets.Contains(selectedTarget.whoAmI))
					flag = false;

				if (selectedTarget.Distance(center) > MagicHat.Range)
					flag = false;

				if (flag)
					return selectedTarget.whoAmI;
			}

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.CanBeChasedBy(this) && (npc.boss || !blackListedTargets.Contains(i)))
				{
					float npcDist = npc.Distance(center);
					if (npcDist <= MagicHat.Range && (npcDist <= closestDist || closestDist == -1f))
					{
						closestDist = npcDist;
						target = i;
					}
				}
			}

			return target;
		}

		private void IdleAI()
		{
			Player player = Main.player[Projectile.owner];

			const float outwardPosition = 180f;
			Vector2 returnPos = player.Center + Projectile.ai[0].ToRotationVector2() * outwardPosition;

			bool returningToPlayer = Behavior == -1f;

			// Player distance calculations
			Vector2 playerVec = returnPos - Projectile.Center;
			float playerDist = playerVec.Length();

			float playerHomeSpeed = 40f;
			//Return to normal if close enough to the player
			if (playerDist < 150f && returningToPlayer && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
			{
				Behavior = 0f;
				Projectile.netUpdate = true;
			}
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
			}
			else
			{
				Projectile.Center = returnPos;
				Projectile.rotation = Projectile.ai[0] + MathHelper.PiOver4;
			}
		}

        public override Color? GetAlpha(Color lightColor) => new Color(75, 255, 255, Projectile.alpha);

        public override bool PreDraw(ref Color lightColor)
		{
            CalamityUtils.DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
		{
            for (int i = 0; i < 10; i++)
			{
                Vector2 dspeed = new Vector2(Main.rand.NextFloat(-7f, 7f), Main.rand.NextFloat(-7f, 7f));
                int dust = Dust.NewDust(Projectile.Center, 1, 1, 66, dspeed.X, dspeed.Y, 160, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 0.75f);
                Main.dust[dust].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
            if (Main.rand.NextBool(4))
			{
                SpawnProjectileballBats(target.Center);
            }
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
		{
            if (Main.rand.NextBool(4))
			{
                SpawnProjectileballBats(target.Center);
            }
        }

        private void SpawnProjectileballBats(Vector2 targetPos)
		{
			int batAmt = Main.rand.Next(1, 3);
            for (int n = 0; n < batAmt; n++) //1 to 2 baseball bats
			{
                float x = targetPos.X + Main.rand.Next(-400, 401);
                float y = targetPos.Y - Main.rand.Next(500, 801);
                Vector2 source = new Vector2(x, y);
                Vector2 velocity = targetPos - source;
                velocity.X += Main.rand.Next(-100, 101);
                float speed = 29f;
                float targetDist = velocity.Length();
                targetDist = speed / targetDist;
                velocity.X *= targetDist;
                velocity.Y *= targetDist;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), source, velocity, ModContent.ProjectileType<MagicBat>(), (int)(Projectile.damage * Main.rand.NextFloat(0.3f, 0.6f)), Projectile.knockBack * Main.rand.NextFloat(0.7f, 1f), Projectile.owner, 0f, 0f);
            }
        }
    }
}
