using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class PlantSummon : ModProjectile
    {
		private bool initialized = false;
		private bool enraged = false;
		private int pinkSeed = ModContent.ProjectileType<PlantSeed>();
		private int greenSeed = ModContent.ProjectileType<PlantSeedGreen>();
		private int thornBall = ModContent.ProjectileType<PlantThornBall>();
		private int sporeClouds = ModContent.ProjectileType<PlantSporeCloud>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plantera");
            Main.projFrames[projectile.type] = 8;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            ProjectileID.Sets.NeedsUUID[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 38;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 3f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 12;
			projectile.extraUpdates = 0;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
			CalamityGlobalProjectile modProj = projectile.Calamity();

			if (player.statLife <= (int)(player.statLifeMax2 * 0.75))
			{
				if (Main.myPlayer == projectile.owner)
				{
					enraged = true;
				}
			}
			else
			{
				enraged = false;
				projectile.extraUpdates = 0;
			}

			Framing();

            if (!initialized)
            {
                modProj.spawnedPlayerMinionDamageValue = player.MinionDamage();
                modProj.spawnedPlayerMinionProjectileDamageValue = projectile.damage;
				SpawnDust();
				SpawnTentacles();
                initialized = true;
            }
            if (player.MinionDamage() != modProj.spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)modProj.spawnedPlayerMinionProjectileDamageValue /
                    modProj.spawnedPlayerMinionDamageValue * player.MinionDamage());
                projectile.damage = damage2;
            }

            bool correctMinion = projectile.type == ModContent.ProjectileType<PlantSummon>();
            player.AddBuff(ModContent.BuffType<PlantationBuff>(), 3600);
            if (correctMinion)
            {
                if (player.dead)
                {
                    modPlayer.plantera = false;
                }
                if (modPlayer.plantera)
                {
                    projectile.timeLeft = 2;
                }
            }

            float range = 1000f;

			//shouldn't need anti clump because there can only be one

			if (!enraged)
			{
				if (projectile.ai[0] >= 2f)
					projectile.ai[0] = 0f;
				Vector2 targetVec = projectile.position;
				bool foundTarget = false;
				if (player.HasMinionAttackTargetNPC)
				{
					NPC npc = Main.npc[player.MinionAttackTargetNPC];
					if (npc.CanBeChasedBy(projectile, false))
					{
						float extraDist = (npc.width / 2) + (npc.height / 2);
						//Calculate distance between target and the projectile to know if it's too far or not
						float npcDist = Vector2.Distance(npc.Center, projectile.Center);
						if (!foundTarget && npcDist < (range + extraDist))
						{
							range = npcDist;
							targetVec = npc.Center;
							foundTarget = true;
						}
					}
				}
				else
				{
					for (int i = 0; i < Main.maxNPCs; i++)
					{
						NPC npc = Main.npc[i];
						if (npc.CanBeChasedBy(projectile, false))
						{
							float extraDist = (npc.width / 2) + (npc.height / 2);
							//Calculate distance between target and the projectile to know if it's too far or not
							float npcDist = Vector2.Distance(npc.Center, projectile.Center);
							if (!foundTarget && npcDist < (range + extraDist))
							{
								range = npcDist;
								targetVec = npc.Center;
								foundTarget = true;
							}
						}
					}
				}

				CheckIfShouldReturnToPlayer(foundTarget);

				if (foundTarget && projectile.ai[0] == 0f)
				{
					StayCertainDistFromTarget(targetVec);
				}
				else
				{
					PassiveAI();
				}

				HandleRotation(foundTarget, targetVec);

				IncrementAttackCounter();
				if (projectile.ai[0] == 0f)
				{
					float projSpeed = 6f;
					int projType = Main.rand.NextBool(2) ? greenSeed : pinkSeed;
					int projDmg = (int)(projectile.damage * 0.7f);
					float speedMult = 1f;
					if (Main.rand.NextBool(4))
					{
						projType = thornBall;
					}
					if (projType == thornBall)
					{
						speedMult = 2f;
						projDmg = (int)(projectile.damage * 1.2f);
					}
					if (projectile.ai[1] == 0f && foundTarget && range < 500f)
					{
						Main.PlaySound(SoundID.Item20, projectile.position);
						projectile.ai[1] += 1f;
						if (Main.myPlayer == projectile.owner)
						{
							Vector2 velocity = targetVec - projectile.Center;
							if (projType != thornBall && Main.rand.NextBool(3))
							{
								FireShotgun(velocity, 0.7f);
							}
							else
							{
								velocity.Normalize();
								velocity *= projSpeed;
								velocity *= speedMult;
								Projectile.NewProjectile(projectile.Center, velocity, projType, projDmg, projectile.knockBack, projectile.owner, 0f, 0f);
							}
							projectile.netUpdate = true;
						}
					}
				}
			}
			else //enraged
			{
				bool charging = false;
				if (projectile.ai[0] == 2f)
				{
					projectile.ai[1] += 1f;
					projectile.extraUpdates = 1;
					if (projectile.ai[1] > 30f)
					{
						projectile.ai[1] = 1f;
						projectile.ai[0] = 0f;
						projectile.extraUpdates = 0;
						projectile.numUpdates = 0;
						projectile.netUpdate = true;
					}
					else
					{
						charging = true;
					}
				}
				if (charging)
				{
					return;
				}
				Vector2 targetVec = projectile.position;
				bool foundTarget = false;
				if (player.HasMinionAttackTargetNPC)
				{
					NPC npc = Main.npc[player.MinionAttackTargetNPC];
					if (npc.CanBeChasedBy(projectile, false))
					{
						float extraDist = (npc.width / 2) + (npc.height / 2);
						//Calculate distance between target and the projectile to know if it's too far or not
						float npcDist = Vector2.Distance(npc.Center, projectile.Center);
						if (!foundTarget && npcDist < (range + extraDist))
						{
							range = npcDist;
							targetVec = npc.Center;
							foundTarget = true;
						}
					}
				}
				if (!foundTarget)
				{
					for (int num645 = 0; num645 < Main.maxNPCs; num645++)
					{
						NPC npc = Main.npc[num645];
						if (npc.CanBeChasedBy(projectile, false))
						{
							float extraDist = (npc.width / 2) + (npc.height / 2);
							//Calculate distance between target and the projectile to know if it's too far or not
							float npcDist = Vector2.Distance(npc.Center, projectile.Center);
							if (!foundTarget && npcDist < (range + extraDist))
							{
								range = npcDist;
								targetVec = npc.Center;
								foundTarget = true;
							}
						}
					}
				}

				HandleRotation(foundTarget, targetVec);

				CheckIfShouldReturnToPlayer(foundTarget);

				if (foundTarget && projectile.ai[0] == 0f)
				{
					StayCertainDistFromTarget(targetVec);
				}
				else
				{
					PassiveAI();
				}
				IncrementAttackCounter();
				if (projectile.ai[0] == 0f)
				{
					if (projectile.ai[1] == 0f && foundTarget && range < 500f)
					{
						projectile.ai[1] += 1f;
						if (Main.myPlayer == projectile.owner)
						{
							projectile.ai[0] = 2f;
							Vector2 whereIsTarget = targetVec - projectile.Center;
							whereIsTarget.Normalize();
							int projType = thornBall;
							if (Main.rand.NextBool(2))
							{
								Vector2 projVelocity = whereIsTarget * 2f;
								int projDmg = (int)(projectile.damage * 1.5f);
								Projectile.NewProjectile(projectile.Center, projVelocity, projType, projDmg, projectile.knockBack, projectile.owner, 0f, 1f);
							}
							if (Main.rand.NextBool(3))
							{
								FireShotgun(whereIsTarget, 0.8f);
							}
							float chargeSpeed = 8f;
							projectile.velocity = whereIsTarget * chargeSpeed;
							projectile.netUpdate = true;
						}
					}
				}
			}
        }

		private void CheckIfShouldReturnToPlayer(bool targetLocated)
		{
			Player player = Main.player[projectile.owner];
			float separationAnxietyDist = 1300f;
			if (targetLocated)
			{
				separationAnxietyDist = 2600f;
			}
			if (Vector2.Distance(player.Center, projectile.Center) > separationAnxietyDist)
			{
				projectile.ai[0] = 1f;
				projectile.netUpdate = true;
			}
		}

		private void StayCertainDistFromTarget(Vector2 whereIsTarget)
		{
			Vector2 targetPos = whereIsTarget - projectile.Center;
			float targetDist = targetPos.Length();
			targetPos.Normalize();
			if (targetDist > 200f)
			{
				float speedMult = 8f;
				targetPos *= speedMult;
				projectile.velocity = (projectile.velocity * 40f + targetPos) / 41f;
			}
			else
			{
				float reverseSpeedMult = 4f;
				targetPos *= -reverseSpeedMult;
				projectile.velocity = (projectile.velocity * 40f + targetPos) / 41f;
			}
		}

		private void PassiveAI()
		{
			Player player = Main.player[projectile.owner];
			bool returningToPlayer = false;
			if (!returningToPlayer)
			{
				returningToPlayer = projectile.ai[0] == 1f;
			}
			float returnSpeed = 12f;
			if (returningToPlayer)
			{
				returnSpeed = 30f;
			}
			Vector2 playerVec = player.Center - projectile.Center + new Vector2(0f, -120f);
			float playerDist = playerVec.Length();
			if (playerDist > 200f && returnSpeed < 16f)
			{
				returnSpeed = 16f;
			}
			if (playerDist < 600f && returningToPlayer)
			{
				projectile.ai[0] = 0f;
				projectile.netUpdate = true;
			}
			if (playerDist > 2000f)
			{
				projectile.position.X = player.Center.X - (float)(projectile.width / 2);
				projectile.position.Y = player.Center.Y - (float)(projectile.height / 2);
				projectile.netUpdate = true;
			}
			if (playerDist > 70f)
			{
				playerVec.Normalize();
				playerVec *= returnSpeed;
				projectile.velocity = (projectile.velocity * 40f + playerVec) / 41f;
			}
			else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
			{
				projectile.velocity.X = -0.15f;
				projectile.velocity.Y = -0.05f;
			}
		}

		private void IncrementAttackCounter()
		{
			if (projectile.ai[1] > 0f)
			{
				projectile.ai[1] += (float)Main.rand.Next(1, 4);
			}
			if (projectile.ai[1] > 40f)
			{
				projectile.ai[1] = 0f;
				projectile.netUpdate = true;
			}
		}

		private void FireShotgun(Vector2 whereIsTarget, float attackMult)
		{
			whereIsTarget.Normalize();
			float projSpeedMult = 3f;
			int projType = pinkSeed;
			if (Main.rand.NextBool(2) && CalamityUtils.CountProjectiles(sporeClouds) < 9)
			{
				projType = sporeClouds;
				projSpeedMult = 10f;
			}
			else
			{
				projType = Main.rand.NextBool(2) ? greenSeed : pinkSeed;
			}
			int projDmg = (int)(projectile.damage * attackMult);
			Vector2 projVelocity = whereIsTarget * projSpeedMult;
			for (int i = -8; i <= 8; i += 8)
			{
				Vector2 perturbedSpeed = projVelocity.RotatedBy(MathHelper.ToRadians(i));
				Projectile.NewProjectile(projectile.Center, perturbedSpeed, projType, projDmg, projectile.knockBack * attackMult, projectile.owner, Main.rand.Next(3), 1f);
			}
		}

		private void HandleRotation(bool targetFound, Vector2 whereIsTarget)
		{
			if (targetFound && !enraged)
			{
				projectile.rotation = projectile.rotation.AngleTowards(projectile.AngleTo(whereIsTarget) + MathHelper.Pi, 0.1f);
			}
			else
			{
				projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi;
			}
		}

		private void Framing()
		{
            if (projectile.frameCounter++ % 8 == 7)
            {
                projectile.frame++;
            }
			if (!enraged)
			{
				if (projectile.frame >= 4)
				{
					projectile.frame = 0;
				}
			}
			else
			{
				if (projectile.frame >= 8)
				{
					projectile.frame = 4;
				}
			}
		}

		private void SpawnDust()
		{
			int dustAmt = 36;
			for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
			{
				Vector2 source = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
				source = source.RotatedBy((double)((float)(dustIndex - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
				Vector2 dustVel = source - projectile.Center;
				int terra = Dust.NewDust(source + dustVel, 0, 0, 107, dustVel.X * 1.75f, dustVel.Y * 1.75f, 100, default, 1.1f);
				Main.dust[terra].noGravity = true;
				Main.dust[terra].velocity = dustVel;
			}
		}

		private void SpawnTentacles()
		{
			if (projectile.owner == Main.myPlayer)
			{
				int tentacleAmt = 6;
				for (int tentacleIndex = 0; tentacleIndex < tentacleAmt; tentacleIndex++)
				{
					Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<PlantTentacle>(), projectile.damage, projectile.knockBack, projectile.owner, tentacleIndex, Projectile.GetByUUID(projectile.owner, projectile.whoAmI));
				}
			}
		}

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 180);
			target.AddBuff(BuffID.Venom, 90);
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 180);
			target.AddBuff(BuffID.Venom, 90);
        }

        public override bool CanDamage() => enraged;

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int height = texture.Height / Main.projFrames[projectile.type];
            int frameHeight = height * projectile.frame;
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;

            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, frameHeight, texture.Width, height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture.Width / 2f, (float)height / 2f), projectile.scale, spriteEffects, 0f);
			return false;
        }
    }
}
