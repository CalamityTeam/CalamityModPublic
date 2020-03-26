using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Summon
{
    public class MagicHat : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Magic Hat");
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
            projectile.minionSlots = 5f;
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
            CalamityGlobalProjectile modProj = projectile.Calamity();

			//set up minion buffs and bools
            bool hatExists = projectile.type == ModContent.ProjectileType<MagicHat>();
            player.AddBuff(ModContent.BuffType<MagicHatBuff>(), 3600);
            if (hatExists)
            {
                if (player.dead)
                {
                    modPlayer.magicHat = false;
                }
                if (modPlayer.magicHat)
                {
                    projectile.timeLeft = 2;
                }
            }

			//projectile movement
            projectile.position.X = player.Center.X - (float)(projectile.width / 2);
            projectile.position.Y = player.Center.Y - (float)(projectile.height / 2) + player.gfxOffY - 60f;
            if (player.gravDir == -1f)
            {
                projectile.position.Y = projectile.position.Y + 150f;
                projectile.rotation = MathHelper.Pi;
            }
            else
            {
                projectile.rotation = 0f;
            }
            projectile.position.X = (float)(int)projectile.position.X;
            projectile.position.Y = (float)(int)projectile.position.Y;

			//I don't know what this is
            float num395 = (float)Main.mouseTextColor / 200f - 0.35f;
            num395 *= 0.2f;
            projectile.scale = num395 + 0.95f;

			//on summon dust and flexible damage
            if (projectile.localAI[0] == 0f)
            {
                int dustAmt = 50;
                for (int dustIndex = 0; dustIndex < dustAmt; dustIndex++)
                {
                    int dustEffects = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, 234, 0f, 0f, 0, default, 1f);
                    Main.dust[dustEffects].velocity *= 2f;
                    Main.dust[dustEffects].scale *= 1.15f;
                }
                modProj.spawnedPlayerMinionDamageValue = player.MinionDamage();
                modProj.spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != modProj.spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)modProj.spawnedPlayerMinionProjectileDamageValue /
                    modProj.spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }

			//finding an enemy, then shooting projectiles at it
            if (projectile.owner == Main.myPlayer)
            {
                float projPosX = projectile.position.X;
                float projPosY = projectile.position.Y;
                float detectionRange = 1500f;
                bool enemyDetected = false;
                if (player.HasMinionAttackTargetNPC)
                {
                    NPC npc = Main.npc[player.MinionAttackTargetNPC];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
                        float xDist = npc.position.X + (float)(npc.width / 2);
                        float yDist = npc.position.Y + (float)(npc.height / 2);
                        float enemyDist = Math.Abs(projPosX + (float)(projectile.width / 2) - xDist) + Math.Abs(projPosY + (float)(projectile.height / 2) - yDist);
                        if (enemyDist < detectionRange)
                        {
                            detectionRange = enemyDist;
                            projPosX = xDist;
                            projPosY = yDist;
                            enemyDetected = true;
                        }
                    }
                }
                else
                {
                    for (int index = 0; index < Main.npc.Length; index++)
                    {
						NPC target = Main.npc[index];
                        if (target.CanBeChasedBy(projectile, true))
                        {
                            float xDist = target.position.X + (float)(target.width / 2);
                            float yDist = target.position.Y + (float)(target.height / 2);
                            float enemyDist = Math.Abs(projPosX + (float)(projectile.width / 2) - xDist) + Math.Abs(projPosY + (float)(projectile.height / 2) - yDist);
                            if (enemyDist < detectionRange)
                            {
                                detectionRange = enemyDist;
                                projPosX = xDist;
                                projPosY = yDist;
                                enemyDetected = true;
                            }
                        }
                    }
                }
                if (enemyDetected)
                {
					projectile.ai[1] += 1f;
					if ((projectile.ai[1] % 5f) == 0f)
					{
						int amount = Main.rand.Next(1, 2);
						for (int i = 0; i < amount; i++)
						{
							int projType = Utils.SelectRandom(Main.rand, new int[]
							{
								ModContent.ProjectileType<MagicUmbrella>(),
								ModContent.ProjectileType<MagicRifle>(),
								ModContent.ProjectileType<MagicHammer>(),
								ModContent.ProjectileType<MagicAxe>(),
								ModContent.ProjectileType<MagicBird>()
							});
							float velocityX = Main.rand.NextFloat(-10f, 10f);
							float velocityY = Main.rand.NextFloat(-15f, -8f);
							Projectile.NewProjectile(projectile.oldPosition.X + (float)(projectile.width / 2), projectile.oldPosition.Y + (float)(projectile.height / 2), velocityX, velocityY, projType, projectile.damage, 0f, projectile.owner, 0f, 0f);
						}
					}
                }
            }
        }

		//glowmask effect
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(200, 200, 200, 200);
        }

		//no contact damage
        public override bool CanDamage()
        {
            return false;
        }
    }
}
