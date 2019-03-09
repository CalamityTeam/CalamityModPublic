using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.DesertScourge
{
	public class DriedSeekerBody : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dried Seeker");
		}
		
		public override void SetDefaults()
		{
			npc.damage = 9; //70
			npc.npcSlots = 5f;
			npc.width = 18; //324
			npc.height = 18; //216
			npc.defense = 5;
			npc.lifeMax = 100; //250000
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 50000;
            }
            npc.knockBackResist = 0f;
			npc.aiStyle = 6;
            aiType = -1;
            animationType = 10;
			npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.canGhostHeal = false;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			npc.netAlways = true;
			npc.dontCountMe = true;
		}
		
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
		
		public override void AI()
		{
			if (!Main.npc[(int)npc.ai[1]].active)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.active = false;
            }
		}
		
		public override bool CheckActive()
		{
			return false;
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 3; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 10; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}