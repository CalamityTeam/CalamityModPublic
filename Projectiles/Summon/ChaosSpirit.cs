using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class ChaosSpirit : ModProjectile
    {
        public int dust = 3;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hydrothermic Vent");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 34;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minion = true;
            projectile.minionSlots = 0f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
        }

        public override void AI()
        {
            bool flag64 = projectile.type == ModContent.ProjectileType<ChaosSpirit>();
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
            if (!modPlayer.chaosSpirit)
            {
                projectile.active = false;
                return;
            }
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.cSpirit = false;
                }
                if (modPlayer.cSpirit)
                {
                    projectile.timeLeft = 2;
                }
            }
            dust--;
            if (dust >= 0)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num501 = 50;
                for (int num502 = 0; num502 < num501; num502++)
                {
                    int num503 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + 16f), projectile.width, projectile.height - 16, Main.rand.NextBool(3) ? 16 : 127, 0f, 0f, 0, default, 1f);
                    Main.dust[num503].velocity *= 2f;
                    Main.dust[num503].scale *= 1.15f;
                }
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 1f / 255f, (255 - projectile.alpha) * 0.35f / 255f, (255 - projectile.alpha) * 0f / 255f);
            projectile.frameCounter++;
            if (projectile.frameCounter > 9)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
            if (projectile.frame >= Main.projFrames[projectile.type])
            {
                projectile.frame = 0;
            }
			bool reversedGravity = player.gravDir == -1f;
            projectile.position.X = player.Center.X - (float)(projectile.width / 2);
            projectile.position.Y = player.Center.Y - (float)(projectile.height / 2) + player.gfxOffY - 60f;
            if (reversedGravity)
            {
                projectile.position.Y += 120f;
                projectile.rotation = MathHelper.Pi;
            }
            else
            {
                projectile.rotation = 0f;
            }
            projectile.position.X = (float)(int)projectile.position.X;
            projectile.position.Y = (float)(int)projectile.position.Y;

            if (projectile.owner == Main.myPlayer)
            {
                if (projectile.ai[0] != 0f)
                {
                    projectile.ai[0] -= 1f;
                    return;
                }
                bool foundTarget = false;
				Vector2 targetVec = projectile.Center;
				Vector2 half = new Vector2(0.5f);
                float range = 1000f;
				int targetIndex = -1;
				if (player.HasMinionAttackTargetNPC)
				{
					NPC npc = Main.npc[player.MinionAttackTargetNPC];
					if (npc.CanBeChasedBy(projectile, false))
					{
						Vector2 sizeCheck = npc.position + npc.Size * half;
						float npcDist = Vector2.Distance(sizeCheck, targetVec);
						if (npcDist < range && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
						{
							range = npcDist;
							targetVec = sizeCheck;
							foundTarget = true;
							targetIndex = npc.whoAmI;
						}
					}
				}
				if (!foundTarget)
				{
					for (int k = 0; k < Main.maxNPCs; k++)
					{
						NPC npc = Main.npc[k];
						if (npc.CanBeChasedBy(projectile, false))
						{
							Vector2 sizeCheck = npc.position + npc.Size * half;
							float npcDist = Vector2.Distance(sizeCheck, targetVec);
							if (npcDist < range && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
							{
								range = npcDist;
								targetVec = sizeCheck;
								foundTarget = true;
								targetIndex = k;
							}
						}
					}
				}
				float yAdjust = player.gravDir == -1f ? 0f : 10f;
                if (foundTarget && targetIndex != -1)
                {
					int projectileType = ModContent.ProjectileType<ChaosFlame>();
					//If the target is above the minion, fire directly at it at double speed
					if (reversedGravity ? (Main.npc[targetIndex].Bottom.Y > projectile.Top.Y) : (Main.npc[targetIndex].Bottom.Y < projectile.Top.Y))
					{
						Vector2 source = new Vector2(projectile.Center.X - 4f, projectile.Center.Y - yAdjust);
						float speed = Main.rand.Next(14, 19); //modify the speed the projectile are shot.  Lower number = slower projectile.
						Vector2 velocity = targetVec - projectile.Center;
						float targetDist = velocity.Length();
						targetDist = speed / targetDist;
						velocity.X *= targetDist;
						velocity.Y *= targetDist;
						Projectile.NewProjectile(source, velocity, projectileType, projectile.damage, 5f, projectile.owner, 0f, 0f);
						Main.PlaySound(SoundID.Item20, projectile.position);
						projectile.ai[0] = 10f;
					}
					//Otherwise, fire away like a volcano
					else
					{
						int amount = Main.rand.Next(2, 4); //2 to 3
						for (int i = 0; i < amount; i++)
						{
							float velocityX = Main.rand.NextFloat(-10f, 10f);
							float velocityY = Main.rand.NextFloat(-10f, -7f);
							if (reversedGravity)
								velocityY *= -1f;
							int flame = Projectile.NewProjectile(projectile.oldPosition.X + (projectile.width / 2), projectile.oldPosition.Y + (projectile.height / 2), velocityX, velocityY, projectileType, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
							Main.projectile[flame].aiStyle = 1;
						}
						Main.PlaySound(SoundID.Item20, projectile.position);
						projectile.ai[0] = 20f;
					}
                }

				//doesn't look good
				/*Vector2 goreVec = new Vector2(projectile.Center.X, projectile.Center.Y - yAdjust - 10f);
				Vector2 goreVec = new Vector2(projectile.Center.X + projectile.velocity.X, projectile.Center.Y + projectile.velocity.Y);
				if (Main.rand.NextBool(8))
				{
					int smoke = Gore.NewGore(goreVec, default, Main.rand.Next(375, 378), 0.5f);
					Main.gore[smoke].behindTiles = true;
				}*/
            }
        }

        public override Color? GetAlpha(Color lightColor) => new Color(200, 200, 200, 200);

        public override bool CanDamage() => false;
    }
}
