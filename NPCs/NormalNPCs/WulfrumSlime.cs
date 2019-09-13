using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class WulfrumSlime : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wulfrum Slime");
			Main.npcFrameCount[npc.type] = 2;
		}

		public override void SetDefaults()
		{
			npc.aiStyle = 1;
			npc.damage = 8;
			npc.width = 30; //324
			npc.height = 22; //216
			npc.defense = 4;
			npc.lifeMax = 12;
			npc.knockBackResist = 0f;
			animationType = 81;
			npc.value = Item.buyPrice(0, 0, 0, 30);
			npc.alpha = 60;
			npc.lavaImmune = false;
			npc.noGravity = false;
			npc.noTileCollide = false;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			banner = npc.type;
			bannerItem = mod.ItemType("WulfrumSlimeBanner");
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSulphur)
			{
				return 0f;
			}
			return SpawnCondition.OverworldDaySlime.Chance * (Main.hardMode ? 0.08f : 0.33f);
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 3, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 15; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 3, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}

		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("WulfrumShard"));
			if (Main.expertMode && Main.rand.Next(2) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("WulfrumShard"));
			}
		}
	}
}
