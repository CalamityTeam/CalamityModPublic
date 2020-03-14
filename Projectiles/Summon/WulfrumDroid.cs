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
        public float dust = 0f;

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
            if (dust == 0f)
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
                dust += 1f;
            }
            if ((player.allDamage + player.minionDamage - 1f) != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    (player.allDamage + player.minionDamage - 1f));
                projectile.damage = damage2;
            }
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
			float num6 = 0.05f;
			float width = (float) projectile.width;
			for (int index = 0; index < 1000; ++index)
			{
				if (index != projectile.whoAmI && Main.projectile[index].active && (Main.projectile[index].owner == projectile.owner && Main.projectile[index].type == projectile.type) && (double) Math.Abs(projectile.position.X - Main.projectile[index].position.X) + (double) Math.Abs(projectile.position.Y - Main.projectile[index].position.Y) < (double) width)
				{
					if ((double) projectile.position.X < (double) Main.projectile[index].position.X)
						projectile.velocity.X -= num6;
					else
						projectile.velocity.X += num6;
					if ((double) projectile.position.Y < (double) Main.projectile[index].position.Y)
						projectile.velocity.Y -= num6;
					else
						projectile.velocity.Y += num6;
				}
			}
			Vector2 vector2_3 = projectile.position;
			float num7 = 400f;
			bool flag = false;
			int num8 = -1;
			if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
				if (npc.CanBeChasedBy((object) this, false))
				{
					float num1 = Vector2.Distance(npc.Center, projectile.Center);
					if (((double) Vector2.Distance(projectile.Center, vector2_3) > (double) num1 && (double) num1 < (double) num7 || !flag) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
					{
						num7 = num1;
						vector2_3 = npc.Center;
						flag = true;
						num8 = npc.whoAmI;
					}
				}
			}
			if (!flag)
			{
				for (int index2 = 0; index2 < Main.npc.Length; ++index2)
				{
					NPC npc = Main.npc[index2];
					if (npc.CanBeChasedBy((object) this, false))
					{
						float num1 = Vector2.Distance(npc.Center, projectile.Center);
						if (((double) Vector2.Distance(projectile.Center, vector2_3) > (double) num1 && (double) num1 < (double) num7 || !flag) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
						{
							num7 = num1;
							vector2_3 = npc.Center;
							flag = true;
							num8 = index2;
						}
					}
				}
			}
			int num9 = 500;
			if (flag)
				num9 = 1000;
			if ((double) Vector2.Distance(player.Center, projectile.Center) > (double) num9)
			{
				projectile.ai[0] = 1f;
				projectile.netUpdate = true;
			}
			if ((double) projectile.ai[0] == 1.0)
				projectile.tileCollide = false;
			if (flag && (double) projectile.ai[0] == 0.0)
			{
				Vector2 vector2_1 = vector2_3 - projectile.Center;
				float num1 = vector2_1.Length();
				vector2_1.Normalize();
				if ((double) num1 > 200.0)
				{
					float num2 = 6f;
					Vector2 vector2_2 = vector2_1 * num2;
					projectile.velocity.X = (float) (((double) projectile.velocity.X * 40.0 + (double) vector2_2.X) / 41.0);
					projectile.velocity.Y = (float) (((double) projectile.velocity.Y * 40.0 + (double) vector2_2.Y) / 41.0);
				}
				else if ((double) projectile.velocity.Y > -1.0)
					projectile.velocity.Y -= 0.1f;
			}
			else
			{
				if (!Collision.CanHitLine(projectile.Center, 1, 1, Main.player[projectile.owner].Center, 1, 1))
					projectile.ai[0] = 1f;
				float num1 = 6f;
				if ((double) projectile.ai[0] == 1.0)
					num1 = 15f;
				Vector2 center = projectile.Center;
				Vector2 vector2_1 = player.Center - center + new Vector2(0.0f, -60f);
				float num3 = vector2_1.Length();
				if ((double) num3 > 200.0 && (double) num1 < 9.0)
					num1 = 9f;
				if ((double) num3 < 100.0 && (double) projectile.ai[0] == 1.0 && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
				{
					projectile.ai[0] = 0.0f;
					projectile.netUpdate = true;
				}
				if ((double) num3 > 2000.0)
				{
					projectile.position.X = Main.player[projectile.owner].Center.X - (float) (projectile.width / 2);
					projectile.position.Y = Main.player[projectile.owner].Center.Y - (float) (projectile.width / 2);
				}
				else if ((double) num3 > 70.0)
				{
					vector2_1.Normalize();
					projectile.velocity = (projectile.velocity * 20f + vector2_1 * num1) / 21f;
				}
				else
				{
					if ((double) projectile.velocity.X == 0.0 && (double) projectile.velocity.Y == 0.0)
					{
						projectile.velocity.X = -0.15f;
						projectile.velocity.Y = -0.05f;
					}
					projectile.velocity = projectile.velocity * 1.01f;
				}
			}
			projectile.rotation = projectile.velocity.X * 0.05f;
			if ((double) projectile.velocity.X > 0.0)
				projectile.spriteDirection = projectile.direction = 1;
			else if ((double) projectile.velocity.X < 0.0)
				projectile.spriteDirection = projectile.direction = -1;
			if ((double) projectile.ai[1] > 0.0)
				projectile.ai[1] += (float) Main.rand.Next(1, 4);
			if ((double) projectile.ai[1] > 90.0)
			{
				projectile.ai[1] = 0.0f;
				projectile.netUpdate = true;
			}
			if ((double) projectile.ai[1] != 0.0)
				return;
			++projectile.ai[1];
			if (Main.myPlayer != projectile.owner)
				return;
			if (!flag)
				return;
			Vector2 vec = vector2_3 - projectile.Center;
			vec.Normalize();
			Vector2 vec2 = vec * 10f;
			int bolt = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, vec2.X, vec2.Y, ModContent.ProjectileType<WulfrumBolt>(), projectile.damage, 0.0f, Main.myPlayer, 0.0f, 0.0f);
			Main.projectile[bolt].Calamity().forceMinion = true;
			Main.projectile[bolt].netUpdate = true;
			Main.projectile[bolt].penetrate = 1;
			Main.projectile[bolt].extraUpdates = 1;
			ProjectileID.Sets.MinionShot[Main.projectile[bolt].type] = true;
			projectile.netUpdate = true;
        }

        public override bool CanDamage()
        {
            return false;
        }
    }
}
