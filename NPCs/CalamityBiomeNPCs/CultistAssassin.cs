using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;
using CalamityMod.CalPlayer;

namespace CalamityMod.NPCs.CalamityBiomeNPCs
{
    public class CultistAssassin : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Cultist Assassin");
			Main.npcFrameCount[npc.type] = 4;
		}

		public override void SetDefaults()
		{
			npc.lavaImmune = true;
			npc.aiStyle = 3;
			npc.damage = 50;
			npc.width = 18; //324
			npc.height = 40; //216
			npc.defense = 25;
			npc.lifeMax = 80;
			npc.knockBackResist = 0.5f;
			animationType = 331;
			aiType = NPCID.ChaosElemental;
			npc.value = Item.buyPrice(0, 0, 2, 0);
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath50;
			if (CalamityWorld.downedProvidence)
			{
				npc.damage = 250;
				npc.defense = 130;
				npc.lifeMax = 5000;
				npc.value = Item.buyPrice(0, 0, 50, 0);
			}
			banner = npc.type;
			bannerItem = mod.ItemType("CultistAssassinBanner");
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
			return (spawnInfo.player.Calamity().ZoneCalamity || spawnInfo.player.ZoneDungeon) && Main.hardMode ? 0.04f : 0f;
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

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (CalamityWorld.revenge)
			{
				player.AddBuff(mod.BuffType("Horror"), 180, true);
			}
		}

		public override void NPCLoot()
		{
			if (CalamityWorld.downedProvidence && Main.rand.NextBool(2))
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Bloodstone"));
			}
			if (Main.rand.NextBool(3))
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EssenceofChaos"));
			}
		}
	}
}
