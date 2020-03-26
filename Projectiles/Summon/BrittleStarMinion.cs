using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class BrittleStarMinion : ModProjectile
    {
        public float dust = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brittle Star");
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
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 33, vector7.X * 1.1f, vector7.Y * 1.1f, 100, default, 1.4f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].noLight = true;
                    Main.dust[num228].velocity = vector7;
                }
                dust += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }
            projectile.rotation += projectile.velocity.X * 0.04f;
            bool flag64 = projectile.type == ModContent.ProjectileType<BrittleStarMinion>();
            player.AddBuff(ModContent.BuffType<BrittleStar>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.bStar = false;
                }
                if (modPlayer.bStar)
                {
                    projectile.timeLeft = 2;
                }
            }
            float SAImovement = 0.05f;
            for (int num638 = 0; num638 < Main.projectile.Length; num638++)
            {
                bool flag23 = Main.projectile[num638].type == ModContent.ProjectileType<BrittleStarMinion>();
                if (num638 != projectile.whoAmI && Main.projectile[num638].active && Main.projectile[num638].owner == projectile.owner && flag23 && Math.Abs(projectile.position.X - Main.projectile[num638].position.X) + Math.Abs(projectile.position.Y - Main.projectile[num638].position.Y) < (float)projectile.width)
                {
                    if (projectile.position.X < Main.projectile[num638].position.X)
                    {
                        projectile.velocity.X -= SAImovement;
                    }
                    else
                    {
                        projectile.velocity.X += SAImovement;
                    }
                    if (projectile.position.Y < Main.projectile[num638].position.Y)
                    {
                        projectile.velocity.Y -= SAImovement;
                    }
                    else
                    {
                        projectile.velocity.Y += SAImovement;
                    }
                }
            }

			float num1 = 700f;
			float num2 = 800f;
			float num3 = 1200f;
			float num4 = 150f;
			bool flag1 = false;
			if (projectile.ai[0] == 2f)
			{
				projectile.ai[1] += 1f;
				projectile.extraUpdates = 2;
				projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi;
				if (projectile.ai[1] > 40f)
				{
					projectile.ai[1] = 1f;
					projectile.ai[0] = 0.0f;
					projectile.extraUpdates = 0;
					projectile.numUpdates = 0;
					projectile.netUpdate = true;
				}
				else
					flag1 = true;
			}
			if (flag1)
				return;
			Vector2 Position2 = projectile.position;
			bool flag2 = false;
			if (projectile.tileCollide && WorldGen.SolidTile(Framing.GetTileSafely((int) projectile.Center.X / 16, (int) projectile.Center.Y / 16)))
				projectile.tileCollide = false;
			NPC targetedNPC = projectile.OwnerMinionAttackTargetNPC;
			if (targetedNPC != null && targetedNPC.CanBeChasedBy((object) projectile, false))
			{
				float num6 = Vector2.Distance(targetedNPC.Center, projectile.Center);
				if ((!flag2 && num6 < num1) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, targetedNPC.position, targetedNPC.width, targetedNPC.height))
				{
					num1 = num6;
					Position2 = targetedNPC.Center;
					flag2 = true;
				}
			}
			if (!flag2)
			{
				for (int index = 0; index < Main.maxNPCs; ++index)
				{
					NPC npc = Main.npc[index];
					if (npc.CanBeChasedBy((object) projectile, false))
					{
						float num6 = Vector2.Distance(npc.Center, projectile.Center);
						if ((!flag2 && num6 < num1) && Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
						{
							num1 = num6;
							Position2 = npc.Center;
							flag2 = true;
						}
					}
				}
			}
			if (flag2)
			{
				projectile.tileCollide = true;
			}
			else
			{
				projectile.tileCollide = false;
			}
			float num8 = num2;
			if (flag2)
				num8 = num3;
			if (Vector2.Distance(player.Center, projectile.Center) > num8)
			{
				projectile.ai[0] = 1f;
				projectile.tileCollide = false;
				projectile.netUpdate = true;
			}
			if (((1) & (flag2 ? 1 : 0)) != 0 && projectile.ai[0] == 0f)
			{
				Vector2 vector2 = Position2 - projectile.Center;
				float num6 = vector2.Length();
				vector2.Normalize();
				if (num6 > 200f)
				{
					float num7 = 8f;
					projectile.velocity = (projectile.velocity * 40f + vector2 * num7) / 41f;
				}
				else
				{
					float num7 = 4f;
					projectile.velocity = (projectile.velocity * 40f + vector2 * -num7) / 41f;
				}
			}
			else
			{
				bool flag3 = false;
				if (!flag3)
					flag3 = projectile.ai[0] == 1f;
				float num6 = 6f;
				if (flag3)
					num6 = 15f;
				Vector2 center = projectile.Center;
				Vector2 vector2 = player.Center - center + new Vector2(0.0f, -60f);
				float num7 = vector2.Length();
				if (num7 > 200f && num6 < 8f)
					num6 = 8f;
				if (num7 < num4 & flag3 && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
				{
					projectile.ai[0] = 0.0f;
					projectile.netUpdate = true;
				}
				if (num7 > 2000f)
				{
					projectile.position.X = player.Center.X - (float) (projectile.width / 2);
					projectile.position.Y = player.Center.Y - (float) (projectile.height / 2);
					projectile.netUpdate = true;
				}
				if (num7 > 70f)
				{
					vector2.Normalize();
					vector2 *= num6;
					projectile.velocity = (projectile.velocity * 40f + vector2) / 41f;
				}
				else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
				{
					projectile.velocity.X = -0.15f;
					projectile.velocity.Y = -0.05f;
				}
			}
			if (projectile.ai[1] > 0f)
			{
				projectile.ai[1] += (float) Main.rand.Next(1, 4);
			}
			if (projectile.ai[1] > 40f)
			{
				projectile.ai[1] = 0.0f;
				projectile.netUpdate = true;
			}
			if (projectile.ai[0] == 0f)
			{
				if (projectile.ai[1] != 0f || (!flag2 || num1 >= 500f))
					return;
				projectile.ai[1] += 1f;
				if (Main.myPlayer != projectile.owner)
					return;
				projectile.ai[0] = 2f;
				Vector2 vector2 = Position2 - projectile.Center;
				vector2.Normalize();
				projectile.velocity = vector2 * 8f;
				projectile.netUpdate = true;
			}
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
