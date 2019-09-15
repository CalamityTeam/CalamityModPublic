using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class CrimulanBlightSlime : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Crimulan Blight Slime");
			Main.npcFrameCount[npc.type] = 4;
		}

		public override void SetDefaults()
		{
			npc.aiStyle = 1;
			npc.damage = 30;
			npc.width = 60; //324
			npc.height = 42; //216
			npc.defense = 12;
			npc.lifeMax = 130;
			npc.knockBackResist = 0.3f;
			animationType = 244;
			npc.value = Item.buyPrice(0, 0, 2, 0);
			npc.alpha = 105;
			npc.lavaImmune = false;
			npc.noGravity = false;
			npc.noTileCollide = false;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.buffImmune[24] = true;
			banner = npc.type;
			bannerItem = mod.ItemType("CrimulanBlightSlimeBanner");
		}

		public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || spawnInfo.player.GetCalamityPlayer().ZoneAbyss)
			{
				return 0f;
			}
			return SpawnCondition.Crimson.Chance * 0.15f;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 40; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
				}
			}
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(BuffID.ManaSickness, 120, true);
			player.AddBuff(BuffID.Confused, 120, true);
		}

		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EbonianGel"), Main.rand.Next(15, 17));
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Gel, Main.rand.Next(10, 15));
			if (Main.rand.NextBool(100) && NPC.downedBoss3)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Carnage"));
			}
		}
	}
}
