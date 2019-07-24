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

namespace CalamityMod.NPCs.SlimeGod
{
	public class SlimeSpawnCorrupt : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Corrupt Slime Spawn");
			Main.npcFrameCount[npc.type] = 4;
		}
		
		public override void SetDefaults()
		{
			npc.aiStyle = 14;
			npc.damage = 30;
			npc.width = 40; //324
			npc.height = 30; //216
			npc.defense = 10;
			npc.lifeMax = 160;
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 100000;
            }
            npc.knockBackResist = 0f;
			animationType = 121;
			npc.alpha = 60;
			npc.lavaImmune = false;
			npc.noGravity = false;
			npc.noTileCollide = false;
			npc.canGhostHeal = false;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.buffImmune[24] = true;
		}

        public override void AI()
        {
            float speedMult = 1.002f;
            npc.velocity.X *= speedMult;
            npc.velocity.Y *= speedMult;
        }

        public override bool PreNPCLoot()
		{
			return false;
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			if (Main.netMode != 1 && npc.life <= 0)
			{
				Vector2 spawnAt = npc.Center + new Vector2(0f, (float)npc.height / 2f);
				NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, mod.NPCType("SlimeSpawnCorrupt2"));
			}
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default(Color), 1f);
			}
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(BuffID.Weak, 90, true);
		}
	}
}