using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
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
			npc.width = 30;
			npc.height = 22;
			npc.defense = 2;
			npc.lifeMax = 12;
			npc.knockBackResist = 0f;
			animationType = NPCID.CorruptSlime;
			npc.value = Item.buyPrice(0, 0, 0, 30);
			npc.alpha = 60;
			npc.lavaImmune = false;
			npc.noGravity = false;
			npc.noTileCollide = false;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			banner = npc.type;
			bannerItem = ModContent.ItemType<WulfrumSlimeBanner>();
		}

		/*public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			float pylonMult = NPC.AnyNPCs(ModContent.NPCType<WulfrumPylon>()) ? 2f : 1f;
			if (spawnInfo.playerSafe || spawnInfo.player.Calamity().ZoneSulphur)
			{
				return 0f;
			}
			return SpawnCondition.OverworldDaySlime.Chance * (Main.hardMode ? 0.08f : 0.33f) * pylonMult;
		}*/

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 3, hitDirection, -1f, 0, default, 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 15; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 3, hitDirection, -1f, 0, default, 1f);
				}
			}
		}

		public override void NPCLoot()
		{
			DropHelper.DropItem(npc, ModContent.ItemType<WulfrumShard>(), 1, 1);
			DropHelper.DropItemCondition(npc, ModContent.ItemType<WulfrumShard>(), Main.expertMode, 0.5f, 1, 1);
		}
	}
}
