using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon.Umbrella
{
    public class MagicUmbrella : ModProjectile
   {
        private int counter = 0;
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
            Projectile.penetrate = 10;
            Projectile.tileCollide = false;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.alpha = 255;
            Projectile.DamageType = DamageClass.Summon;
        }

        public override void AI()
		{
            Projectile.rotation += 0.075f;
            Projectile.alpha -= 50;
            counter++;
            if (counter == 30)
			{
                Projectile.netUpdate = true;
            }
            else if (counter < 30)
			{
                return;
            }

            AI_156_BatOfLight();
        }

		private void AI_156_BatOfLight()
		{
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
			if (player.active && Vector2.Distance(player.Center, Projectile.Center) > 2000f)
			{
				Projectile.ai[0] = 0f;
				Projectile.ai[1] = 0f;
				Projectile.netUpdate = true;
			}

			if (Projectile.ai[0] == -1f)
			{
				AI_GetMyGroupIndexAndFillBlackList(blacklist, out int index, out int totalIndexesInGroup);
				IdleAI();
				return;
			}

			if (Projectile.ai[0] == 0f)
			{
				AI_GetMyGroupIndexAndFillBlackList(blacklist, out int index3, out int totalIndexesInGroup3);
				IdleAI();
				if (Main.rand.Next(20) == 0)
				{
					int num7 = AI_156_TryAttackingNPCs(blacklist);
					if (num7 != -1)
					{
						Projectile.ai[0] = Main.rand.NextFromList<int>(num, num3);
						Projectile.ai[0] = num3;
						Projectile.ai[1] = num7;
						Projectile.netUpdate = true;
					}
				}
				return;
			}

			bool skipBodyCheck = true;
			int num13 = 0;
			int num14 = num2;
			int num15 = 0;
			if (Projectile.ai[0] >= (float)num5)
			{
				num13 = 1;
				num14 = num4;
				num15 = num5;
			}

			int num16 = (int)Projectile.ai[1];
			if (!Main.npc.IndexInRange(num16))
			{
				int num17 = AI_156_TryAttackingNPCs(blacklist, skipBodyCheck);
				if (num17 != -1)
			{
					Projectile.ai[0] = Main.rand.NextFromList<int>(num, num3);
					Projectile.ai[1] = num17;
					Projectile.netUpdate = true;
				}
				else
				{
					Projectile.ai[0] = -1f;
					Projectile.ai[1] = 0f;
					Projectile.netUpdate = true;
				}

				return;
			}

			NPC nPC2 = Main.npc[num16];
			if (!nPC2.CanBeChasedBy(this))
			{
				int num18 = AI_156_TryAttackingNPCs(blacklist, skipBodyCheck);
				if (num18 != -1)
				{
					Projectile.ai[0] = Main.rand.NextFromList<int>(num, num3);
					Projectile.ai[1] = num18;
					Projectile.netUpdate = true;
				}
				else
				{
					Projectile.ai[0] = -1f;
					Projectile.ai[1] = 0f;
					Projectile.netUpdate = true;
				}

				return;
			}

			Projectile.ai[0] -= 1f;
			if (Projectile.ai[0] >= (float)num14)
			{
				Projectile.direction = ((Projectile.Center.X < nPC2.Center.X) ? 1 : (-1));
				if (Projectile.ai[0] == (float)num14)
				{
					Projectile.localAI[0] = Projectile.Center.X;
					Projectile.localAI[1] = Projectile.Center.Y;
				}
			}

			float lerpValue2 = Utils.GetLerpValue(num14, num15, Projectile.ai[0], clamped: true);
			if (num13 == 0)
			{
				Vector2 vector6 = new Vector2(Projectile.localAI[0], Projectile.localAI[1]);
				if (lerpValue2 >= 0.5f)
					vector6 = Vector2.Lerp(nPC2.Center, Main.player[Projectile.owner].Center, 0.5f);

				Vector2 center2 = nPC2.Center;
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
				Vector2 v = nPC2.Center - vector11;
				Vector2 value = v.SafeNormalize(Vector2.Zero) * MathHelper.Clamp(v.Length(), 60f, 150f);
				Vector2 value2 = nPC2.Center + value;
				float lerpValue3 = Utils.GetLerpValue(0.4f, 0.6f, lerpValue2, clamped: true);
				float lerpValue4 = Utils.GetLerpValue(0.6f, 1f, lerpValue2, clamped: true);
				float targetAngle = v.SafeNormalize(Vector2.Zero).ToRotation() + (float)Math.PI / 2f;
				Projectile.rotation = Projectile.rotation.AngleTowards(targetAngle, (float)Math.PI / 5f) + MathHelper.PiOver4;
				Projectile.Center = Vector2.Lerp(vector11, nPC2.Center, lerpValue3);
				if (lerpValue4 > 0f)
					Projectile.Center = Vector2.Lerp(nPC2.Center, value2, lerpValue4);
			}

			if (Projectile.ai[0] == (float)num15)
			{
				int num23 = AI_156_TryAttackingNPCs(blacklist, skipBodyCheck);
				if (num23 != -1)
				{
					Projectile.ai[0] = Main.rand.NextFromList<int>(num, num3);
					Projectile.ai[1] = num23;
					Projectile.netUpdate = true;
				}
				else
				{
					Projectile.ai[0] = -1f;
					Projectile.ai[1] = 0f;
					Projectile.netUpdate = true;
				}
			}
		}

		private int AI_156_TryAttackingNPCs(List<int> blackListedTargets, bool skipBodyCheck = false)
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

				if (!skipBodyCheck && !Projectile.CanHitWithOwnBody(selectedTarget))
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
					if (!(npcDist > MagicHat.Range) && (!(npcDist > closestDist) || closestDist == -1f) && (skipBodyCheck || Projectile.CanHitWithOwnBody(npc)))
					{
						closestDist = npcDist;
						target = i;
					}
				}
			}

			return target;
		}

		private void AI_GetMyGroupIndexAndFillBlackList(List<int> blackListedTargets, out int index, out int totalIndexesInGroup)
		{
			index = 0;
			totalIndexesInGroup = 0;
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile proj = Main.projectile[i];
				if (proj.active && proj.owner == Projectile.owner && proj.type == Projectile.type)
				{
					if (Projectile.whoAmI > i)
						index++;

					totalIndexesInGroup++;
				}
			}
		}

		private void IdleAI()
		{
			Player player = Main.player[Projectile.owner];
			Vector2 returnOffset = new Vector2(0f, -60f);
			float safeDist = 150f;

			bool returningToPlayer = false;
			if (!returningToPlayer)
			{
				returningToPlayer = Projectile.ai[0] == 1f;
			}

			//Player distance calculations
			Vector2 playerVec = player.Center - Projectile.Center + returnOffset;
			float playerDist = playerVec.Length();

			//If the minion is actively returning, move faster
			float playerHomeSpeed = 6f;
			if (returningToPlayer)
			{
				playerHomeSpeed = 15f;
			}
			//Move somewhat faster if the player is kinda far~ish
			if (playerDist > 200f && playerHomeSpeed < 8f)
			{
				playerHomeSpeed = 8f;
			}
			//Return to normal if close enough to the player
			if (playerDist < safeDist && returningToPlayer && !Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
			{
				Projectile.ai[0] = 0f;
				Projectile.netUpdate = true;
			}
			//Teleport to the player if abnormally far
			if (playerDist > 2000f)
			{
				Projectile.position.X = player.Center.X - (float)(Projectile.width / 2);
				Projectile.position.Y = player.Center.Y - (float)(Projectile.height / 2);
				Projectile.netUpdate = true;
			}
			//If more than 70 pixels away, move toward the player
			if (playerDist > 70f)
			{
				playerVec.Normalize();
				playerVec *= playerHomeSpeed;
				Projectile.velocity = (Projectile.velocity * 40f + playerVec) / 41f;
			}
			//Minions never stay still
			else if (Projectile.velocity.X == 0f && Projectile.velocity.Y == 0f)
			{
				Projectile.velocity.X = -0.15f;
				Projectile.velocity.Y = -0.05f;
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
