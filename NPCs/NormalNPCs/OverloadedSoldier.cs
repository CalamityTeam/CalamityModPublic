using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;
using CalamityMod.World;

namespace CalamityMod.NPCs.NormalNPCs
{
	public class OverloadedSoldier : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Overloaded Soldier");
			Main.npcFrameCount[npc.type] = 15;
		}
		
		public override void SetDefaults()
		{
			npc.aiStyle = 3;
			npc.damage = 42;
			npc.width = 18; //324
			npc.height = 40; //216
			npc.defense = 28;
			npc.lifeMax = 90;
			npc.knockBackResist = 0.5f;
			animationType = 77;
			npc.value = Item.buyPrice(0, 0, 2, 0);
			npc.HitSound = SoundID.NPCHit2;
			npc.DeathSound = SoundID.NPCDeath2;
			banner = npc.type;
			bannerItem = mod.ItemType("OverloadedSoldierBanner");
		}

        public override void AI()
        {
            npc.velocity.X *= 1.05f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !Main.hardMode || spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyss ||
				spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.Underground.Chance * 0.02f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
            {
                player.AddBuff(mod.BuffType("MarkedforDeath"), 120);
            }
        }

        public override void HitEffect(int hitDirection, double damage)
		{
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 60, hitDirection, -1f, 0, default(Color), 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 60, hitDirection, -1f, 0, default(Color), 1f);
                }
            }
        }
		
		public override void NPCLoot()
		{
			Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("AncientBoneDust"));
            if (NPC.downedMoonlord)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Phantoplasm"));
            }
        }
	}
}
