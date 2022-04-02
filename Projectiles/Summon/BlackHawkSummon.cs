using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
	public class BlackHawkSummon : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Black Hawk");
			ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
			Main.projFrames[projectile.type] = 3;
		}

		public override void SetDefaults()
		{
			projectile.width = 36;
			projectile.height = 36;
			projectile.netImportant = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.aiStyle = 66;
			projectile.minionSlots = 1f;
			projectile.timeLeft = 18000;
			projectile.penetrate = -1;
			projectile.tileCollide = true;
			projectile.timeLeft *= 5;
			projectile.minion = true;
		}

		public override void AI()
		{
			//Set namespaces
			Player player = Main.player[projectile.owner];
			CalamityPlayer modPlayer = player.Calamity();
			CalamityGlobalProjectile modProj = projectile.Calamity();

			//On spawn effects
			if (projectile.localAI[0] == 0f)
			{
				//Set constants
				modProj.spawnedPlayerMinionDamageValue = player.MinionDamage();
				modProj.spawnedPlayerMinionProjectileDamageValue = projectile.damage;
				//Spawn dust
				int dustAmt = 36;
				for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
				{
					Vector2 direction = Vector2.Normalize(projectile.velocity) * new Vector2(projectile.width / 2f, projectile.height) * 0.75f;
					direction = direction.RotatedBy((double)((dustIndex - (dustAmt / 2f - 1f)) * MathHelper.TwoPi / dustAmt), default) + projectile.Center;
					Vector2 dustVel = direction - projectile.Center;
					int fire = Dust.NewDust(direction + dustVel, 0, 0, 258, dustVel.X * 1.75f, dustVel.Y * 1.75f, 100, default, 1.1f);
					Main.dust[fire].noGravity = true;
					Main.dust[fire].velocity = dustVel;
				}
				projectile.localAI[0] += 1f;
			}

			//Flexible minion damage update
			if (player.MinionDamage() != modProj.spawnedPlayerMinionDamageValue)
			{
				int damage2 = (int)((float)modProj.spawnedPlayerMinionProjectileDamageValue /
					modProj.spawnedPlayerMinionDamageValue * player.MinionDamage());
				projectile.damage = damage2;
			}

			//Update frames
			projectile.frameCounter++;
			if (projectile.frameCounter >= 4)
			{
				projectile.frame++;
				projectile.frameCounter = 0;
			}
			if (projectile.frame >= Main.projFrames[projectile.type])
			{
				projectile.frame = 0;
			}

			//Set up buff and timeLeft
			bool correctMinion = projectile.type == ModContent.ProjectileType<BlackHawkSummon>();
			player.AddBuff(ModContent.BuffType<BlackHawkBuff>(), 3600);
			if (correctMinion)
			{
				if (player.dead)
				{
					modPlayer.blackhawk = false;
				}
				if (modPlayer.blackhawk)
				{
					projectile.timeLeft = 2;
				}
			}

			//Anti sticky movement to prevent overlapping minions
			projectile.MinionAntiClump();

			float maxDistance = 700f;
			Vector2 targetVec = projectile.position;
			bool foundTarget = false;
			//If targeting something, prioritize that enemy
			if (player.HasMinionAttackTargetNPC)
			{
				NPC npc = Main.npc[player.MinionAttackTargetNPC];
				if (npc.CanBeChasedBy(projectile, false))
				{
					float extraDist = (npc.width / 2) + (npc.height / 2);
					//Calculate distance between target and the projectile to know if it's too far or not
					float targetDist = Vector2.Distance(npc.Center, projectile.Center);
					if (!foundTarget && targetDist < (maxDistance + extraDist))
					{
						maxDistance = targetDist;
						targetVec = npc.Center;
						foundTarget = true;
					}
				}
			}
			if (!foundTarget)
			{
				for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
				{
					NPC npc = Main.npc[npcIndex];
					if (npc.CanBeChasedBy(projectile, false))
					{
						float extraDist = (npc.width / 2) + (npc.height / 2);
						//Calculate distance between target and the projectile to know if it's too far or not
						float targetDist = Vector2.Distance(npc.Center, projectile.Center);
						if (!foundTarget && targetDist < (maxDistance + extraDist))
						{
							maxDistance = targetDist;
							targetVec = npc.Center;
							foundTarget = true;
						}
					}
				}
			}

			//If too far, make the minion start returning to the player.
			float separationAnxietyDist = 1300f;
			if (foundTarget)
			{
				//Max travel distance increases if targeting something
				separationAnxietyDist = 2600f;
			}
			if (Vector2.Distance(player.Center, projectile.Center) > separationAnxietyDist)
			{
				projectile.ai[0] = 1f;
				projectile.netUpdate = true;
			}

			//If a target is found, move toward it
			if (foundTarget && projectile.ai[0] == 0f)
			{
				Vector2 vecToTarget = targetVec - projectile.Center;
				float targetDist = vecToTarget.Length();
				vecToTarget.Normalize();
				//If farther than 200 pixels, move toward it
				if (targetDist > 200f)
				{
					float speedMult = 18f;
					vecToTarget *= speedMult;
					projectile.velocity = (projectile.velocity * 40f + vecToTarget) / 41f;
				}
				//Otherwise, back it up slowly
				else
				{
					float speedMult = -9f;
					vecToTarget *= speedMult;
					projectile.velocity = (projectile.velocity * 40f + vecToTarget) / 41f;
				}
			}

			//If not targeting something, act passively
			else
			{
				bool returningToPlayer = false;
				if (!returningToPlayer)
				{
					returningToPlayer = projectile.ai[0] == 1f;
				}
				//Move faster if actively returning to the player
				float speedMult = 12f;
				if (returningToPlayer)
				{
					speedMult = 30f;
				}
				Vector2 vecToPlayer = player.Center - projectile.Center + new Vector2(0f, -120f);
				float playerDist = vecToPlayer.Length();
				//Speed up if near the player
				if (playerDist < 200f && speedMult < 16f)
				{
					speedMult = 16f;
				}
				//If close enough to the player, return to normal
				if (playerDist < 600f && returningToPlayer)
				{
					projectile.ai[0] = 0f;
					projectile.netUpdate = true;
				}
				//If abnormally far, teleport to the player
				if (playerDist > 2000f)
				{
					projectile.position.X = player.Center.X - (float)(projectile.width / 2);
					projectile.position.Y = player.Center.Y - (float)(projectile.height / 2);
					projectile.netUpdate = true;
				}
				//Move toward player if more than 70 pixels away
				if (playerDist > 70f)
				{
					vecToPlayer.Normalize();
					vecToPlayer *= speedMult;
					projectile.velocity = (projectile.velocity * 40f + vecToPlayer) / 41f;
				}
				//Move if still
				else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
				{
					projectile.velocity.X = -0.15f;
					projectile.velocity.Y = -0.05f;
				}
			}

			//Update rotation
			if (foundTarget)
			{
				projectile.rotation = projectile.rotation.AngleTowards(projectile.AngleTo(targetVec) + MathHelper.Pi, 0.1f);
			}
			else
			{
				projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi;
			}

			//Increment attack cooldown
			if (projectile.ai[1] > 0f)
			{
				projectile.ai[1] += Main.rand.Next(1, 4);
			}
			//Set the minion to be ready for attack
			if (projectile.ai[1] > 130f)
			{
				projectile.ai[1] = 0f;
				projectile.netUpdate = true;
			}

			//Return if on attack cooldown, has no target, or returning to the player
			if (projectile.ai[0] != 0f || !foundTarget || projectile.ai[1] != 0f)
				return;

			//Shoot a bullet
			if (Main.myPlayer == projectile.owner)
			{
				float projSpeed = 6f;
				int projType = ModContent.ProjectileType<BlackHawkBullet>();
				Main.PlaySound(SoundID.Item20, projectile.Center);
				projectile.ai[1] += 2f;
				Vector2 velocity = targetVec - projectile.Center;
				velocity.Normalize();
				velocity *= projSpeed;
				Projectile.NewProjectile(projectile.Center, velocity, projType, projectile.damage, projectile.knockBack, projectile.owner);
				projectile.netUpdate = true;
			}
		}

		//Does no contact damage
		public override bool CanDamage() => false;

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = Main.projectileTexture[projectile.type];
			int frameHeight = texture.Height / Main.projFrames[projectile.type];
			int y6 = frameHeight * projectile.frame;
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, frameHeight)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), projectile.scale, spriteEffects, 0f);
			return false;
		}

		//Pretty glowmask
		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture = ModContent.GetTexture("CalamityMod/Projectiles/Summon/BlackHawkGlow");
			int frameHeight = texture.Height / Main.projFrames[projectile.type];
			int y6 = frameHeight * projectile.frame;
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
				spriteEffects = SpriteEffects.FlipHorizontally;

			spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, frameHeight)), Color.White, projectile.rotation, new Vector2(texture.Width / 2f, frameHeight / 2f), projectile.scale, spriteEffects, 0f);
		}
	}
}
