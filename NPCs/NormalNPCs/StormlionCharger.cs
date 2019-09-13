using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class StormlionCharger : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stormlion");
			Main.npcFrameCount[npc.type] = 6;
		}

		public override void SetDefaults()
		{
			npc.damage = 20;
			npc.aiStyle = 3;
			npc.width = 33; //324
			npc.height = 31; //216
			npc.defense = 9;
			npc.lifeMax = 80;
			npc.knockBackResist = 0.2f;
			animationType = 508;
			npc.value = Item.buyPrice(0, 0, 2, 0);
			npc.HitSound = SoundID.NPCHit31;
			npc.DeathSound = SoundID.NPCDeath34;
			banner = npc.type;
			bannerItem = mod.ItemType("StormlionBanner");
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
				}
			}
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSunkenSea)
			{
				return 0f;
			}
			return SpawnCondition.DesertCave.Chance * 0.2f;
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(BuffID.Electrified, 90, true);
		}

		public override void NPCLoot()
		{
			if (Main.rand.NextBool(2))
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("StormlionMandible"));
			}
		}
	}
}
