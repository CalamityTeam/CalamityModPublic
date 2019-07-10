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
	public class PlaguedFlyingFox : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Melter");
			Main.npcFrameCount[npc.type] = 4;
		}
		
		public override void SetDefaults()
		{
            npc.npcSlots = 0.5f;
            npc.aiStyle = 14;
            aiType = 152;
            animationType = 152;
			npc.damage = 55;
			npc.width = 38; //324
			npc.height = 34; //216
			npc.defense = 35;
			npc.lifeMax = 500;
			npc.knockBackResist = 0f;
			npc.value = Item.buyPrice(0, 0, 10, 0);
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath4;
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
			banner = npc.type;
			bannerItem = mod.ItemType("MelterBanner");
		}

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
            if (spawnInfo.playerSafe || !NPC.downedGolemBoss || spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.HardmodeJungle.Chance * 0.09f;
        }
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
            player.AddBuff(mod.BuffType("Plague"), 300, true);
        }
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 46, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 46, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
		
		public override void NPCLoot()
		{
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PlagueCellCluster"), Main.rand.Next(1, 3));
        }
	}
}
