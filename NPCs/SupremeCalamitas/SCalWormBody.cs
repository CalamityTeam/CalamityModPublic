using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.SupremeCalamitas
{
	public class SCalWormBody : ModNPC
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sepulcher");
		}
		
		public override void SetDefaults()
		{
			npc.damage = 0; //70
			npc.npcSlots = 5f;
			npc.width = 26; //28
			npc.height = 18; //28
			npc.defense = 0;
            npc.lifeMax = CalamityWorld.revenge ? 1200000 : 1000000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 2000000;
            }
            npc.aiStyle = 6; //new
            aiType = -1; //new
            animationType = 10; //new
			npc.knockBackResist = 0f;
			npc.scale = 1.2f;
			if (Main.expertMode)
			{
				npc.scale = 1.35f;
			}
			npc.alpha = 255;
            npc.chaseable = false;
            npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.canGhostHeal = false;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			npc.netAlways = true;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
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
            if (Main.npc[(int)npc.ai[1]].alpha < 128)
			{
				npc.alpha -= 42;
				if (npc.alpha < 0)
				{
					npc.alpha = 0;
				}
			}
		}

		public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if ((projectile.penetrate == -1 || projectile.penetrate > 1) && !projectile.minion)
			{
				projectile.penetrate = 1;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            Texture2D texture = mod.GetTexture("NPCs/SupremeCalamitas/SCalWormBodyAlt");
            CalamityMod.DrawTexture(spriteBatch, (npc.localAI[3] == 1f ? texture : Main.npcTexture[npc.type]), 0, npc, drawColor);
            return false;
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
            if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 50;
				npc.height = 50;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 5; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 10; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 235, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}
	}
}