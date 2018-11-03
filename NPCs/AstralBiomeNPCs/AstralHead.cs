using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.AstralBiomeNPCs
{
	public class AstralHead : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astral Head");
		}
		
		public override void SetDefaults()
		{
			npc.npcSlots = 0.5f;
			npc.damage = 50;
			npc.width = 22; //324
			npc.height = 22; //216
			npc.defense = 5;
			npc.lifeMax = 60;
			npc.aiStyle = -1;
			aiType = -1;
			npc.knockBackResist = 0.4f;
			npc.value = Item.buyPrice(0, 0, 5, 0);
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit3;
			npc.DeathSound = SoundID.NPCDeath3;
			npc.buffImmune[20] = true;
			npc.buffImmune[24] = true;
			npc.buffImmune[39] = true;
		}
		
		public override void AI()
		{
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
			{
				npc.TargetClosest(true);
			}
			float num = 2f;
			float num2 = 0.04f;
			Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
			float num4 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
			float num5 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
			num4 = (float)((int)(num4 / 8f) * 8);
			num5 = (float)((int)(num5 / 8f) * 8);
			vector.X = (float)((int)(vector.X / 8f) * 8);
			vector.Y = (float)((int)(vector.Y / 8f) * 8);
			num4 -= vector.X;
			num5 -= vector.Y;
			float num6 = (float)Math.Sqrt((double)(num4 * num4 + num5 * num5));
			float num7 = num6;
			if (num6 == 0f)
			{
				num4 = npc.velocity.X;
				num5 = npc.velocity.Y;
			}
			else
			{
				num6 = num / num6;
				num4 *= num6;
				num5 *= num6;
			}
			if (Main.player[npc.target].dead)
			{
				num4 = (float)npc.direction * num / 2f;
				num5 = -num / 2f;
			}
			if (npc.velocity.X < num4)
			{
				npc.velocity.X = npc.velocity.X + num2;
			}
			else if (npc.velocity.X > num4)
			{
				npc.velocity.X = npc.velocity.X - num2;
			}
			if (npc.velocity.Y < num5)
			{
				npc.velocity.Y = npc.velocity.Y + num2;
			}
			else if (npc.velocity.Y > num5)
			{
				npc.velocity.Y = npc.velocity.Y - num2;
			}
			if (num4 > 0f)
			{
				npc.spriteDirection = 1;
				npc.rotation = (float)Math.Atan2((double)num5, (double)num4);
			}
			else if (num4 < 0f)
			{
				npc.spriteDirection = -1;
				npc.rotation = (float)Math.Atan2((double)num5, (double)num4) + 3.14f;
			}
			float num12 = 0.7f;
			if (npc.collideX)
			{
				npc.netUpdate = true;
				npc.velocity.X = npc.oldVelocity.X * -num12;
				if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
				{
					npc.velocity.X = 2f;
				}
				if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
				{
					npc.velocity.X = -2f;
				}
			}
			if (npc.collideY)
			{
				npc.netUpdate = true;
				npc.velocity.Y = npc.oldVelocity.Y * -num12;
				if (npc.velocity.Y > 0f && (double)npc.velocity.Y < 1.5)
				{
					npc.velocity.Y = 2f;
				}
				if (npc.velocity.Y < 0f && (double)npc.velocity.Y > -1.5)
				{
					npc.velocity.Y = -2f;
				}
			}
			if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
			{
				npc.netUpdate = true;
			}
			int num13 = Dust.NewDust(new Vector2(npc.position.X - npc.velocity.X, npc.position.Y - npc.velocity.Y), npc.width, npc.height, 173, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 100, default(Color), 2f);
			Main.dust[num13].noGravity = true;
			Dust expr_F26_cp_0 = Main.dust[num13];
			expr_F26_cp_0.velocity.X = expr_F26_cp_0.velocity.X * 0.3f;
			Dust expr_F44_cp_0 = Main.dust[num13];
			expr_F44_cp_0.velocity.Y = expr_F44_cp_0.velocity.Y * 0.3f;
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 30;
				npc.height = 30;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 5; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 10; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}
		
		public override void NPCLoot()
		{
			if (Main.rand.Next(2) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Stardust"));
			}
			if (Main.expertMode && Main.rand.Next(2) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Stardust"));
			}
		}
		
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
			return (spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneAstral && !spawnInfo.player.ZoneTowerStardust && !spawnInfo.player.ZoneTowerSolar && !spawnInfo.player.ZoneTowerVortex && !spawnInfo.player.ZoneTowerNebula) ? 1f : 0f;
        }
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (CalamityWorld.downedStarGod)
			{
				player.AddBuff(mod.BuffType("GodSlayerInferno"), 150, true);
			}
		}
	}
}