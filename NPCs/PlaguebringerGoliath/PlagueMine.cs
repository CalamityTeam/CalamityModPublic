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

namespace CalamityMod.NPCs.PlaguebringerGoliath
{
	public class PlagueMine : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Plague Mine");
		}
		
		public override void SetDefaults()
		{
			npc.damage = 0;
			npc.npcSlots = 1f;
			npc.width = 42; //324
			npc.height = 42; //216
			npc.defense = 0;
			npc.lifeMax = 100;
			npc.aiStyle = -1; //new
            aiType = -1; //new
			npc.knockBackResist = 0f;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.canGhostHeal = false;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(npc.dontTakeDamage);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			npc.dontTakeDamage = reader.ReadBoolean();
		}

		public override void AI()
		{
			Player player = Main.player[npc.target];
			if (!player.active || player.dead)
			{
				npc.TargetClosest(false);
				player = Main.player[npc.target];
				if (!player.active || player.dead)
				{
					if (npc.timeLeft > 10)
					{
						npc.timeLeft = 10;
					}
					return;
				}
			}
			else if (npc.timeLeft > 600)
			{
				npc.timeLeft = 600;
			}
            Vector2 vector = Main.player[npc.target].Center - npc.Center;
            if (vector.Length() < 90f || npc.ai[3] >= 900f)
            {
                npc.dontTakeDamage = false;
                CheckDead();
                npc.life = 0;
                return;
            }
            npc.ai[3] += 1f;
            npc.dontTakeDamage = (npc.ai[3] >= 180f ? false : true);
            if (npc.ai[3] >= 480f)
            {
                npc.velocity.Y *= 0.985f;
                npc.velocity.X *= 0.985f;
                return;
            }
			npc.TargetClosest(true);
			float num1372 = 7f;
			Vector2 vector167 = new Vector2(npc.Center.X + (float)(npc.direction * 20), npc.Center.Y + 6f);
			float num1373 = player.position.X + (float)player.width * 0.5f - vector167.X;
			float num1374 = player.Center.Y - vector167.Y;
			float num1375 = (float)Math.Sqrt((double)(num1373 * num1373 + num1374 * num1374));
			float num1376 = num1372 / num1375;
			num1373 *= num1376;
			num1374 *= num1376;
			npc.ai[0] -= 1f;
			if (num1375 < 200f || npc.ai[0] > 0f)
			{
				if (num1375 < 200f)
				{
					npc.ai[0] = 20f;
				}
				if (npc.velocity.X < 0f)
				{
					npc.direction = -1;
				}
				else
				{
					npc.direction = 1;
				}
				return;
			}
			npc.velocity.X = (npc.velocity.X * 50f + num1373) / 51f;
			npc.velocity.Y = (npc.velocity.Y * 50f + num1374) / 51f;
			if (num1375 < 350f)
			{
				npc.velocity.X = (npc.velocity.X * 10f + num1373) / 11f;
				npc.velocity.Y = (npc.velocity.Y * 10f + num1374) / 11f;
			}
			if (num1375 < 300f)
			{
				npc.velocity.X = (npc.velocity.X * 7f + num1373) / 8f;
				npc.velocity.Y = (npc.velocity.Y * 7f + num1374) / 8f;
			}
		}

        public override bool CheckDead()
        {
            Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 14);
            npc.position.X = npc.position.X + (float)(npc.width / 2);
            npc.position.Y = npc.position.Y + (float)(npc.height / 2);
            npc.damage = CalamityWorld.death ? 300 : 150;
            npc.width = (npc.height = 216);
            npc.position.X = npc.position.X - (float)(npc.width / 2);
            npc.position.Y = npc.position.Y - (float)(npc.height / 2);
            for (int num621 = 0; num621 < 15; num621++)
            {
                int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 89, 0f, 0f, 100, default(Color), 2f);
                Main.dust[num622].velocity *= 3f;
                if (Main.rand.Next(2) == 0)
                {
                    Main.dust[num622].scale = 0.5f;
                    Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
                Main.dust[num622].noGravity = true;
            }
            for (int num623 = 0; num623 < 30; num623++)
            {
                int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 89, 0f, 0f, 100, default(Color), 3f);
                Main.dust[num624].noGravity = true;
                Main.dust[num624].velocity *= 5f;
                num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 89, 0f, 0f, 100, default(Color), 2f);
                Main.dust[num624].velocity *= 2f;
                Main.dust[num624].noGravity = true;
            }
            return true;
        }
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = 100;
			npc.damage = 0;
		}
	}
}