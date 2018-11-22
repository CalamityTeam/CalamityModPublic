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
	public class HeatSpirit : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Heat Spirit");
			Main.npcFrameCount[npc.type] = 4;
		}
		
		public override void SetDefaults()
		{
			npc.npcSlots = 0.5f;
			npc.aiStyle = 86;
			npc.damage = 33;
			npc.width = 40; //324
			npc.height = 24; //216
			npc.defense = 20;
			npc.lifeMax = 50;
			npc.knockBackResist = 0f;
			npc.value = Item.buyPrice(0, 0, 5, 0);
			npc.HitSound = SoundID.NPCHit52;
			npc.DeathSound = SoundID.NPCDeath55;
		}
		
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || !Main.hardMode)
			{
				return 0f;
			}
			return SpawnCondition.Underworld.Chance * 0.065f;
		}
		
		public override void FindFrame(int frameHeight)
        {
			if (npc.velocity.X < 0f)
			{
				npc.direction = -1;
			}
			else
			{
				npc.direction = 1;
			}
			if (npc.direction == 1)
			{
				npc.spriteDirection = 1;
			}
			if (npc.direction == -1)
			{
				npc.spriteDirection = -1;
			}
			npc.rotation = (float)Math.Atan2((double)(npc.velocity.Y * (float)npc.direction), (double)(npc.velocity.X * (float)npc.direction));
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }
		
		public override void AI()
		{
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.5f, 0f, 0.05f);
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (Main.expertMode)
			{
				player.AddBuff(BuffID.OnFire, 180, true);
				player.AddBuff(mod.BuffType("BrimstoneFlames"), 80, true);
			}
			else
			{
				player.AddBuff(BuffID.OnFire, 120, true);
				player.AddBuff(mod.BuffType("BrimstoneFlames"), 40, true);
			}
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 235, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 235, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
		
		public override void NPCLoot()
		{
			if (Main.rand.Next(2) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EssenceofChaos"));
			}
		}
	}
}