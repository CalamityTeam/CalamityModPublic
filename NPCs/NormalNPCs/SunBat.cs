using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class SunBat : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sun Bat");
			Main.npcFrameCount[npc.type] = 6;
		}

		public override void SetDefaults()
		{
			npc.lavaImmune = true;
			npc.aiStyle = 14;
            aiType = 151;
			npc.damage = 35;
			npc.width = 26; //324
			npc.height = 20; //216
			npc.defense = 20;
			npc.lifeMax = 120;
			npc.knockBackResist = 0.65f;
			npc.value = Item.buyPrice(0, 0, 5, 0);
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath4;
			banner = npc.type;
			bannerItem = mod.ItemType("SunBatBanner");
		}

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
		{
			if (spawnInfo.playerSafe || !Main.hardMode || spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyss ||
				spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSunkenSea)
			{
				return 0f;
			}
			return SpawnCondition.Underground.Chance * 0.12f;
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(BuffID.OnFire, 120, true);
			player.AddBuff(mod.BuffType("HolyLight"), 120, true);
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 64, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 64, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}

		public override void NPCLoot()
		{
			if (Main.rand.Next(3) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EssenceofCinder"));
			}
		}
	}
}
