using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class PerennialSlime : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Perennial Slime");
			Main.npcFrameCount[npc.type] = 2;
		}

		public override void SetDefaults()
		{
			npc.aiStyle = 1;
			npc.damage = 35;
			npc.width = 40; //324
			npc.height = 30; //216
			npc.defense = 16;
			npc.lifeMax = 150;
			npc.knockBackResist = 0f;
			animationType = 81;
			npc.value = Item.buyPrice(0, 0, 10, 0);
			npc.alpha = 50;
			npc.lavaImmune = false;
			npc.noGravity = false;
			npc.noTileCollide = false;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			banner = npc.type;
			bannerItem = mod.ItemType("PerennialSlimeBanner");
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || !NPC.downedPlantBoss || spawnInfo.player.GetCalamityPlayer().ZoneAbyss ||
				spawnInfo.player.GetCalamityPlayer().ZoneSunkenSea)
			{
				return 0f;
			}
			return SpawnCondition.Cavern.Chance * 0.08f;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, mod.DustType("CESparkle"), hitDirection, -1f, 0, default, 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, mod.DustType("CESparkle"), hitDirection, -1f, 0, default, 1f);
				}
			}
		}

		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PerennialOre"), Main.rand.Next(10, 27));
		}
	}
}
