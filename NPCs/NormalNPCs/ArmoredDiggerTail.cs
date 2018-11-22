using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.NormalNPCs
{
	public class ArmoredDiggerTail : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Armored Digger");
		}
		
		public override void SetDefaults()
		{
			npc.damage = 20;
			npc.width = 38; //324
			npc.height = 38; //216
			npc.defense = 55;
			npc.lifeMax = 2000;
			npc.knockBackResist = 0f;
			npc.aiStyle = 6;
            aiType = -1;
            animationType = 10;
			npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.Item4;
			npc.DeathSound = SoundID.Item14;
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
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (Main.expertMode)
			{
				player.AddBuff(BuffID.Chilled, 200, true);
				player.AddBuff(BuffID.Electrified, 120, true);
			}
			else
			{
				player.AddBuff(BuffID.Chilled, 100, true);
				player.AddBuff(BuffID.Electrified, 60, true);
			}
		}
		
		public override bool CheckActive()
		{
			return false;
		}
		
		public override bool PreNPCLoot()
		{
			return false;
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 234, hitDirection, -1f, 0, default(Color), 1f);
			}
		}
	}
}