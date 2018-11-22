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
	public class ArmoredDiggerHead : ModNPC
	{
		bool TailSpawned = false;
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Armored Digger");
		}
		
		public override void SetDefaults()
		{
			npc.damage = 50;
			npc.npcSlots = 5f;
			npc.width = 54; //324
			npc.height = 54; //216
			npc.defense = 15;
			npc.lifeMax = 2000;
			npc.knockBackResist = 0f;
			npc.aiStyle = 6;
            aiType = -1;
            animationType = 10;
			npc.value = Item.buyPrice(0, 0, 10, 0);
			npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			npc.netAlways = true;
		}
		
		public override void AI()
		{
			if (!TailSpawned)
            {
                int Previous = npc.whoAmI;
                for (int num36 = 0; num36 < 9; num36++)
                {
                    int lol = 0;
                    if (num36 >= 0 && num36 < 8)
                    {
                        lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("ArmoredDiggerBody"), npc.whoAmI);
                    }
                    else
                    {
                        lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("ArmoredDiggerTail"), npc.whoAmI);
                    }
                    Main.npc[lol].realLife = npc.whoAmI;
                    Main.npc[lol].ai[2] = (float)npc.whoAmI;
                    Main.npc[lol].ai[1] = (float)Previous;
                    Main.npc[Previous].ai[0] = (float)lol;
                    NetMessage.SendData(23, -1, -1, null, lol, 0f, 0f, 0f, 0);
                    Previous = lol;
                }
                TailSpawned = true;
            }
		}
		
		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || !NPC.downedPlantBoss || spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyss)
			{
				return 0f;
			}
			return SpawnCondition.Cavern.Chance * 0.02f;
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (Main.expertMode)
			{
				player.AddBuff(BuffID.Chilled, 200, true);
				player.AddBuff(BuffID.Electrified, 120, true);
			}
			else
			{
				player.AddBuff(BuffID.Chilled, 100, true);
				player.AddBuff(BuffID.Electrified, 60, true);
			}
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 234, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 234, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
		
		public override void NPCLoot()
		{
			if (Main.rand.Next(2) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Ectoblood"), Main.rand.Next(3, 6));
			}
		}
	}
}