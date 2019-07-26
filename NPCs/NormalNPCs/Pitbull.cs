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
	public class Pitbull : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pitbull");
			Main.npcFrameCount[npc.type] = 10;
		}

		public override void SetDefaults()
		{
			npc.aiStyle = 26;
			npc.damage = 18;
			npc.width = 46; //324
			npc.height = 30; //216
			npc.defense = 12;
			npc.lifeMax = 60;
			npc.knockBackResist = 0.3f;
			animationType = 329;
			aiType = NPCID.Wolf;
			npc.value = Item.buyPrice(0, 0, 2, 0);
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath5;
			banner = npc.type;
			bannerItem = mod.ItemType("PitbullBanner");
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || !NPC.downedBoss1 || spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSulphur)
			{
				return 0f;
			}
			return SpawnCondition.OverworldNightMonster.Chance * 0.045f;
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(BuffID.Bleeding, 180, true);
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}
