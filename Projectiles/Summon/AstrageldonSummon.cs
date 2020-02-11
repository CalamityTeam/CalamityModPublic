using CalamityMod.Buffs.Summon;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles.Boss;

namespace CalamityMod.Projectiles.Summon
{
    public class AstrageldonSummon : ModProjectile
    {
        public bool dust = false;
		private int attackCounter = 1;
		private int teleportCounter = 400;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astrageldon");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 64;
            projectile.height = 64;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.minionSlots = 1;
            projectile.alpha = 75;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.tileCollide = false;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.aiStyle = 26;
            aiType = 266;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
			//for platform collision?
            fallThrough = false;
            return true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];;
            CalamityPlayer modPlayer = player.Calamity();
			CalamityGlobalProjectile modProj = projectile.Calamity();

            projectile.minionSlots = modProj.lineColor;

			//hitbox size scaling
			projectile.width = (int)(64f * projectile.scale);
			projectile.height = (int)(64f * projectile.scale);

			//on spawn effects and flexible minions
            if (!dust)
            {
                modProj.spawnedPlayerMinionDamageValue = (player.allDamage + player.minionDamage - 1f);
                modProj.spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int dustAmt = 16;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(dustIndex - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int dusty = Dust.NewDust(vector6 + vector7, 0, 0, ModContent.DustType<AstralOrange>(), vector7.X * 1f, vector7.Y * 1f, 100, default, 1.1f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].noLight = true;
                    Main.dust[dusty].velocity = vector7;
                }
                dust = true;
            }
            if ((player.allDamage + player.minionDamage - 1f) != modProj.spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)modProj.spawnedPlayerMinionProjectileDamageValue /
                    modProj.spawnedPlayerMinionDamageValue *
                    (player.allDamage + player.minionDamage - 1f));
                projectile.damage = damage2;
            }

			//Bool setup
            bool flag64 = projectile.type == ModContent.ProjectileType<AstrageldonSummon>();
            player.AddBuff(ModContent.BuffType<AstrageldonBuff>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.aSlime = false;
                }
                if (modPlayer.aSlime)
                {
                    projectile.timeLeft = 2;
                }
            }

			if (projectile.frame == 0 || projectile.frame == 1)
			{
				float mindistance = 1000f;
				float longdistance = 2000f;
				float longestdistance = 3000f;
				Vector2 objectivepos = projectile.position;
				bool gotoenemy = false;
				if (player.HasMinionAttackTargetNPC)
				{
					NPC npc = Main.npc[player.MinionAttackTargetNPC];
					if (npc.CanBeChasedBy(projectile, false))
					{
						bool lineOfSight = Collision.CanHit(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
						float disttoobjective = Vector2.Distance(npc.Center, projectile.Center);
						if (((Vector2.Distance(projectile.Center, objectivepos) > disttoobjective && disttoobjective < mindistance) || !gotoenemy) && lineOfSight)
						{
							
							mindistance = disttoobjective;
							objectivepos = npc.Center;
							gotoenemy = true;
						}
					}
				}
				else
				{
					for (int num645 = 0; num645 < Main.npc.Length; num645++)
					{
						NPC npc = Main.npc[num645];
						if (npc.CanBeChasedBy(projectile, false))
						{
							bool lineOfSight = Collision.CanHit(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
							float disttoobjective = Vector2.Distance(npc.Center, projectile.Center);
							if (((Vector2.Distance(projectile.Center, objectivepos) > disttoobjective && disttoobjective < mindistance) || !gotoenemy) && lineOfSight)
							{
								
								mindistance = disttoobjective;
								objectivepos = npc.Center;
								gotoenemy = true;
							}
						}
					}
				}
				float maxdisttoenemy = longdistance;
				if (gotoenemy)
				{
					maxdisttoenemy = longestdistance;
				}
				if (gotoenemy)
				{
					float teleportRange = objectivepos.Length();
					float scaleAddition = projectile.scale * 5f;
					if (teleportCounter <= 0 && teleportRange >= 800f)
					{
						projectile.position.X = objectivepos.X - (float)(projectile.width / 2) + Main.rand.NextFloat(-100f, 100f);
						projectile.position.Y = objectivepos.Y - (float)(projectile.height / 2) - Main.rand.NextFloat(0f + scaleAddition, 200f + scaleAddition);
						projectile.netUpdate = true;
						teleportCounter = 600;
					}
					if (teleportCounter > 0)
						teleportCounter -= Main.rand.Next(1, 4);
				}

				if (attackCounter > 0)
				{
					attackCounter += Main.rand.Next(1, 4);
				}
				if (attackCounter > 300)
				{
					attackCounter = 0;
					projectile.netUpdate = true;
				}
				float scaleFactor3 = 6f;
				int projType = ModContent.ProjectileType<AstrageldonLaser>();
				if (gotoenemy && attackCounter == 0)
				{
					attackCounter += 2;
					if (Main.myPlayer == projectile.owner)
					{
						Vector2 laserVel = objectivepos - projectile.Center;
						laserVel.Normalize();
						laserVel *= scaleFactor3;
						int laser = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, laserVel.X, laserVel.Y, projType, projectile.damage, 0f, projectile.owner, 0f, 0f);
						projectile.netUpdate = true;
					}
				}
			}
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (projectile.penetrate == 0)
            {
                projectile.Kill();
            }
            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D texture = Main.projectileTexture[projectile.type];
            int height = texture.Height / Main.projFrames[projectile.type];
            int y6 = height * projectile.frame;
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y6, texture.Width, height)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture.Width / 2f, (float)height / 2f), projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
