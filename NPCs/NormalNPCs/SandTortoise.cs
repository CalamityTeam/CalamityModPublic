using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class SandTortoise : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sand Tortoise");
			Main.npcFrameCount[npc.type] = 8;
		}

		public override void SetDefaults()
		{
			npc.npcSlots = 2f;
			npc.damage = 110;
			npc.aiStyle = 39;
			npc.width = 46;
			npc.height = 32;
			npc.defense = 34;
			npc.scale = 1.5f;
			npc.lifeMax = 580;
			npc.knockBackResist = 0.2f;
			animationType = 153;
			npc.value = Item.buyPrice(0, 0, 15, 0);
			npc.HitSound = SoundID.NPCHit24;
			npc.DeathSound = SoundID.NPCDeath27;
			npc.noGravity = false;
			banner = npc.type;
			bannerItem = mod.ItemType("SandTortoiseBanner");
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

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || !Main.hardMode || spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSunkenSea)
			{
				return 0f;
			}
			return SpawnCondition.DesertCave.Chance * 0.05f;
		}

		public override void NPCLoot()
		{
			if (Main.rand.Next(10) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.TurtleShell);
			}
		}
	}
}
