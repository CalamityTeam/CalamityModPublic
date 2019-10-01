using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;
using CalamityMod.CalPlayer;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class AeroSlime : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aero Slime");
			Main.npcFrameCount[npc.type] = 4;
		}

		public override void SetDefaults()
		{
			npc.aiStyle = 14;
			npc.damage = 18;
			npc.width = 40;
			npc.height = 30;
			npc.defense = 6;
			npc.lifeMax = 50;
			npc.knockBackResist = 0f;
			animationType = 121;
			npc.value = Item.buyPrice(0, 0, 1, 0);
			npc.alpha = 50;
			npc.lavaImmune = false;
			npc.noGravity = false;
			npc.noTileCollide = false;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			banner = npc.type;
			bannerItem = mod.ItemType("AeroSlimeBanner");
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || (!CalamityWorld.downedHiveMind && !CalamityWorld.downedPerforator) || spawnInfo.player.GetCalamityPlayer().ZoneAbyss ||
				spawnInfo.player.GetCalamityPlayer().ZoneSunkenSea)
			{
				return 0f;
			}
			return SpawnCondition.Cavern.Chance * 0.05f;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 59, hitDirection, -1f, 0, default, 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 59, hitDirection, -1f, 0, default, 1f);
				}
			}
		}

		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("AerialiteOre"), Main.rand.Next(10, 27));
		}
	}
}
