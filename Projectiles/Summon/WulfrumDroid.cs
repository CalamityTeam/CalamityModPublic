using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class WulfrumDroid : ModProjectile
    {
        private bool onSpawn = true;
		private float attackCounter = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Droid");
            Main.projFrames[projectile.type] = 3;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            //projectile.aiStyle = 62;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

			//on spawn effects and flexible dmg
            if (onSpawn)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = (player.allDamage + player.minionDamage - 1f);
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 61, vector7.X * 1.1f, vector7.Y * 1.1f, 100, default, 1.4f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].noLight = true;
                    Main.dust[num228].velocity = vector7;
                }
                onSpawn = false;
            }
            if ((player.allDamage + player.minionDamage - 1f) != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    (player.allDamage + player.minionDamage - 1f));
                projectile.damage = damage2;
            }

			//framing
            projectile.frameCounter++;
            if (projectile.frameCounter > 8)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= 3)
            {
                projectile.frame = 0;
            }
			//rotation and sprite direction
			projectile.rotation = projectile.velocity.X * 0.05f;
			if (projectile.velocity.X > 0f)
				projectile.spriteDirection = projectile.direction = 1;
			else if (projectile.velocity.X < 0f)
				projectile.spriteDirection = projectile.direction = -1;

			//CalPlayer bools and buff
            bool flag64 = projectile.type == ModContent.ProjectileType<WulfrumDroid>();
            player.AddBuff(ModContent.BuffType<WulfrumDroidBuff>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.wDroid = false;
                }
                if (modPlayer.wDroid)
                {
                    projectile.timeLeft = 2;
                }
            }

			//anti sticking movement
			float num6 = 0.05f;
			float width = (float) projectile.width;
			for (int index = 0; index < Main.projectile.Length; ++index)
			{
				Projectile proj = Main.projectile[index];
                bool typeCheck = proj.type == ModContent.ProjectileType<WulfrumDroid>();
				if (index != projectile.whoAmI && proj.active && (proj.owner == projectile.owner && typeCheck) && (double) Math.Abs(projectile.position.X - proj.position.X) + (double) Math.Abs(projectile.position.Y - proj.position.Y) < (double) width)
				{
					if (projectile.position.X < proj.position.X)
						projectile.velocity.X -= num6;
					else
						projectile.velocity.X += num6;
					if (projectile.position.Y < proj.position.Y)
						projectile.velocity.Y -= num6;
					else
						projectile.velocity.Y += num6;
				}
			}

			//find nearby enemies
			Vector2 targetLocation = projectile.position;
			float range = 400f;
			bool foundEnemy = false;
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
				bool lineOfSight = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
                if (npc.CanBeChasedBy(projectile, false))
                {
                    float enemyDist = Vector2.Distance(npc.Center, projectile.Center);
                    if ((Vector2.Distance(projectile.Center, targetLocation) > enemyDist && enemyDist < range) && lineOfSight)
                    {
						range = enemyDist;
                        targetLocation = npc.Center;
                        foundEnemy = true;
                    }
                }
            }
			if (!foundEnemy)
			{
				for (int index2 = 0; index2 < Main.npc.Length; ++index2)
				{
					NPC npc = Main.npc[index2];
					bool lineOfSight = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
					if (npc.CanBeChasedBy(projectile, false))
					{
						float enemyDist = Vector2.Distance(npc.Center, projectile.Center);
						if ((Vector2.Distance(projectile.Center, targetLocation) > enemyDist && enemyDist < range) && lineOfSight)
						{
							range = enemyDist;
							targetLocation = npc.Center;
							foundEnemy = true;
						}
					}
				}
			}

			//bigger range if an enemy is found
			int distBeforeReturningtoPlayer = 500;
			if (foundEnemy)
				distBeforeReturningtoPlayer = 1000;
			//return to player if too far
			if (Vector2.Distance(player.Center, projectile.Center) > distBeforeReturningtoPlayer)
			{
				projectile.ai[0] = 1f;
				projectile.netUpdate = true;
			}

			//if returning to player, ignore tiles
			if (projectile.ai[0] == 1f)
				projectile.tileCollide = false;

			//go to enemy if enemy found and not returning to player
			if (foundEnemy && projectile.ai[0] == 0f)
			{
				Vector2 howFarIsTarget = targetLocation - projectile.Center;
				float targetDist = howFarIsTarget.Length();
				howFarIsTarget.Normalize();
				if (targetDist > 200f)
				{
					float velocityMult = 6f;
					Vector2 vector2_2 = howFarIsTarget * velocityMult;
					projectile.velocity.X = ((projectile.velocity.X * 40f + vector2_2.X) / 41f);
					projectile.velocity.Y = ((projectile.velocity.Y * 40f + vector2_2.Y) / 41f);
				}
				else if (projectile.velocity.Y > -1f)
					projectile.velocity.Y -= 0.1f;
			}
			//ai when not tracking an enemy
			else
			{
				//return to player if not in line of sight
				if (!Collision.CanHitLine(projectile.Center, 1, 1, player.Center, 1, 1))
					projectile.ai[0] = 1f;

				float velocityMult = 6f;
				//if returning to player, move faster
				if (projectile.ai[0] == 1f)
					velocityMult = 15f;
				Vector2 center = projectile.Center;
				Vector2 howFarIsPlayer = player.Center - center + new Vector2(0.0f, -60f);
				float playerDist = howFarIsPlayer.Length();
				if (playerDist > 200f && velocityMult < 9f)
					velocityMult = 9f;
				//if returning to player, nearby, and has line of sight, return to normal
				if (playerDist < 100f && projectile.ai[0] == 1f && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
				{
					projectile.ai[0] = 0f;
					projectile.netUpdate = true;
				}
				//teleport to player if too far
				if (playerDist > 2000f)
				{
					projectile.position.X = player.Center.X - (float) (projectile.width / 2);
					projectile.position.Y = player.Center.Y - (float) (projectile.width / 2);
				}
				//fly back to the player if reasonably far
				else if (playerDist > 70f)
				{
					howFarIsPlayer.Normalize();
					projectile.velocity = (projectile.velocity * 20f + howFarIsPlayer * velocityMult) / 21f;
				}
				//normal, idle movement
				else
				{
					if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
					{
						projectile.velocity.X = -0.15f;
						projectile.velocity.Y = -0.05f;
					}
					projectile.velocity = projectile.velocity * 1.01f;
				}
			}

			//increment attack counter
			if (attackCounter > 0f)
				attackCounter += (float) Main.rand.Next(1, 4);
			if (attackCounter > 90f)
			{
				attackCounter = 0f;
				projectile.netUpdate = true;
			}


			//shoot at target
			if (foundEnemy && Main.myPlayer == projectile.owner && attackCounter == 0f)
			{
				Vector2 targetSite = targetLocation - projectile.Center;
				targetSite.Normalize();
				Vector2 boltVelocity = targetSite * 10f;
				int bolt = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, boltVelocity.X, boltVelocity.Y, ModContent.ProjectileType<WulfrumBolt>(), projectile.damage, 0f, Main.myPlayer, 0f, 0f);
				Main.projectile[bolt].Calamity().forceMinion = true;
				Main.projectile[bolt].netUpdate = true;
				Main.projectile[bolt].penetrate = 1;
				Main.projectile[bolt].extraUpdates = 1;
				projectile.netUpdate = true;

				++attackCounter;
			}
        }

        public override bool CanDamage()
        {
            return false;
        }
    }
}
