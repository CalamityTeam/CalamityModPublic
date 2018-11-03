using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.NormalNPCs
{
	public class PlagueBee : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Plague Charger");
			Main.npcFrameCount[npc.type] = 4;
		}
		
		public override void SetDefaults()
		{
			npc.damage = 37;
			npc.width = 36; //324
			npc.height = 30; //216
			npc.defense = 20;
			npc.scale = 0.5f;
			npc.lifeMax = 200;
			npc.aiStyle = 5;
			aiType = 210;
			npc.knockBackResist = 0f;
			animationType = 210;
			npc.value = Item.buyPrice(0, 0, 8, 0);
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			npc.noGravity = true;
			npc.noTileCollide = false;
			npc.buffImmune[189] = true;
			npc.buffImmune[153] = true;
			npc.buffImmune[70] = true;
			npc.buffImmune[69] = true;
			npc.buffImmune[44] = true;
			npc.buffImmune[39] = true;
			npc.buffImmune[24] = true;
			npc.buffImmune[20] = true;
			npc.buffImmune[mod.BuffType("BrimstoneFlames")] = true;
			npc.buffImmune[mod.BuffType("HolyLight")] = true;
			npc.buffImmune[mod.BuffType("Plague")] = true;
		}
		
		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PlagueCellCluster"), Main.rand.Next(1, 3));
		}
		
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || !NPC.downedGolemBoss)
			{
				return 0f;
			}
			return SpawnCondition.HardmodeJungle.Chance * 0.12f;
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (Main.expertMode)
			{
				player.AddBuff(mod.BuffType("Plague"), 280, true);
			}
			else
			{
				player.AddBuff(mod.BuffType("Plague"), 260, true);
			}
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 46, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 10; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 46, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}