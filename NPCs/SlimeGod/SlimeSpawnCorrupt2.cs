using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.SlimeGod
{
	public class SlimeSpawnCorrupt2 : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Corrupt Slime Spawn");
			Main.npcFrameCount[npc.type] = 2;
		}
		
		public override void SetDefaults()
		{
			npc.aiStyle = 1;
			npc.damage = 30;
			npc.width = 40; //324
			npc.height = 30; //216
			npc.defense = 10;
			npc.lifeMax = 90;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 50000;
            }
            npc.knockBackResist = 0f;
			animationType = 81;
			npc.alpha = 60;
			npc.lavaImmune = false;
			npc.noGravity = false;
			npc.noTileCollide = false;
			npc.canGhostHeal = false;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.buffImmune[24] = true;
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
		
		public override bool PreNPCLoot()
		{
			return false;
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(BuffID.ManaSickness, 60, true);
		}
	}
}