using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.AbyssNPCs
{
	public class BobbitWormHead : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bobbit Worm");
            Main.npcFrameCount[npc.type] = 4;
        }
		
		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			npc.damage = 150;
			npc.width = 80; //324
			npc.height = 40; //216
			npc.defense = 50;
			npc.lifeMax = 10000;
			npc.knockBackResist = 0f;
			aiType = -1;
            npc.noGravity = true;
            npc.value = Item.buyPrice(0, 0, 50, 0);
            npc.buffImmune[mod.BuffType("CrushDepth")] = true;
            npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
			banner = npc.type;
			bannerItem = mod.ItemType("BobbitWormBanner");
		}
		
		public override void AI()
		{
			if (npc.ai[0] == 0f) 
			{
				npc.noTileCollide = true;
                float num659 = 14f;
				Vector2 vector79 = new Vector2(npc.Center.X, npc.Center.Y);
				float num660 = Main.npc[CalamityGlobalNPC.bobbitWormBottom].Center.X - vector79.X;
				float num661 = Main.npc[CalamityGlobalNPC.bobbitWormBottom].Center.Y - vector79.Y;
				float num662 = (float)Math.Sqrt((double)(num660 * num660 + num661 * num661));
				if (num662 < 11f + num659) 
				{
                    npc.rotation = 0f;
					npc.velocity.X = num660;
					npc.velocity.Y = num661;
					npc.ai[1] += 1f;
					if (npc.ai[1] >= 60f) 
					{
						npc.TargetClosest(true);
						if ((npc.Center.Y + ((Main.player[npc.target].GetModPlayer<CalamityPlayer>(mod).anechoicPlating || 
                            Main.player[npc.target].GetModPlayer<CalamityPlayer>(mod).anechoicCoating) ? 50f : 100f) > Main.player[npc.target].Center.Y)) 
						{
							npc.ai[1] = 0f;
							npc.ai[0] = 1f;
							return;
						}
						npc.ai[1] = 0f;
						return;
					}
				} 
				else
				{
                    num662 = num659 / num662;
					npc.velocity.X = num660 * num662;
					npc.velocity.Y = num661 * num662;
					npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - 1.57f;
				}
			} 
			else if (npc.ai[0] == 1f)
			{
				npc.noTileCollide = true;
                npc.collideX = false;
				npc.collideY = false;
				float num663 = 11f;
				Vector2 vector80 = new Vector2(npc.Center.X, npc.Center.Y);
				float num664 = Main.player[npc.target].Center.X - vector80.X;
				float num665 = Main.player[npc.target].Center.Y - vector80.Y;
				float num666 = (float)Math.Sqrt((double)(num664 * num664 + num665 * num665));
				num666 = num663 / num666;
				npc.velocity.X = num664 * num666;
				npc.velocity.Y = num665 * num666;
				npc.ai[0] = 2f;
				npc.rotation = (float)Math.Atan2((double)(-(double)npc.velocity.Y), (double)(-(double)npc.velocity.X)) - 1.57f;
			} 
			else if (npc.ai[0] == 2f)
			{
				if (Math.Abs(npc.velocity.X) > Math.Abs(npc.velocity.Y)) 
				{
					if (npc.velocity.X > 0f && npc.Center.X > Main.player[npc.target].Center.X) 
					{
						npc.noTileCollide = false;
					}
					if (npc.velocity.X < 0f && npc.Center.X < Main.player[npc.target].Center.X) 
					{
						npc.noTileCollide = false;
					}
				} 
				else
				{
					if (npc.velocity.Y > 0f && npc.Center.Y > Main.player[npc.target].Center.Y) 
					{
						npc.noTileCollide = false;
					}
					if (npc.velocity.Y < 0f && npc.Center.Y < Main.player[npc.target].Center.Y) 
					{
						npc.noTileCollide = false;
					}
				}
				Vector2 vector81 = new Vector2(npc.Center.X, npc.Center.Y);
				float num667 = Main.npc[CalamityGlobalNPC.bobbitWormBottom].Center.X - vector81.X;
				float num668 = Main.npc[CalamityGlobalNPC.bobbitWormBottom].Center.Y - vector81.Y;
				float num669 = (float)Math.Sqrt((double)(num667 * num667 + num668 * num668));
				if (num669 > 700f || npc.collideX || npc.collideY) 
				{
					npc.noTileCollide = true;
                    npc.ai[0] = 0f;
					return;
				}
			} 
			else if (npc.ai[0] == 3f)
			{
				npc.noTileCollide = true;
                float num671 = 11f;
				float num672 = 0.25f;
				Vector2 vector82 = new Vector2(npc.Center.X, npc.Center.Y);
				float num673 = Main.player[npc.target].Center.X - vector82.X;
				float num674 = Main.player[npc.target].Center.Y - vector82.Y;
				float num675 = (float)Math.Sqrt((double)(num673 * num673 + num674 * num674));
				num675 = num671 / num675;
				num673 *= num675;
				num674 *= num675;
				if (npc.velocity.X < num673) 
				{
					npc.velocity.X = npc.velocity.X + num672;
					if (npc.velocity.X < 0f && num673 > 0f) 
					{
						npc.velocity.X = npc.velocity.X + num672 * 2f;
					}
				} 
				else if (npc.velocity.X > num673)
				{
					npc.velocity.X = npc.velocity.X - num672;
					if (npc.velocity.X > 0f && num673 < 0f) 
					{
						npc.velocity.X = npc.velocity.X - num672 * 2f;
					}
				}
				if (npc.velocity.Y < num674) 
				{
					npc.velocity.Y = npc.velocity.Y + num672;
					if (npc.velocity.Y < 0f && num674 > 0f) 
					{
						npc.velocity.Y = npc.velocity.Y + num672 * 2f;
					}
				} 
				else if (npc.velocity.Y > num674)
				{
					npc.velocity.Y = npc.velocity.Y - num672;
					if (npc.velocity.Y > 0f && num674 < 0f) 
					{
						npc.velocity.Y = npc.velocity.Y - num672 * 2f;
					}
				}
				npc.rotation = (float)Math.Atan2((double)(-(double)npc.velocity.Y), (double)(-(double)npc.velocity.X));
			}
		}

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.1f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
			float drawPositionX = Main.npc[CalamityGlobalNPC.bobbitWormBottom].Center.X - center.X;
			float drawPositionY = Main.npc[CalamityGlobalNPC.bobbitWormBottom].Center.Y - center.Y;
			float rotation = (float)Math.Atan2((double)drawPositionY, (double)drawPositionX) - 1.57f;
			bool draw = true;
			while (draw)
			{
				float totalDrawDistance = (float)Math.Sqrt((double)(drawPositionX * drawPositionX + drawPositionY * drawPositionY));
				if (totalDrawDistance < 16f)
				{
					draw = false;
				}
				else
				{
					totalDrawDistance = 16f / totalDrawDistance;
					drawPositionX *= totalDrawDistance;
					drawPositionY *= totalDrawDistance;
					center.X += drawPositionX;
					center.Y += drawPositionY;
					drawPositionX = Main.npc[CalamityGlobalNPC.bobbitWormBottom].Center.X - center.X;
					drawPositionY = Main.npc[CalamityGlobalNPC.bobbitWormBottom].Center.Y - center.Y;
					drawPositionY += 4f;
					Microsoft.Xna.Framework.Color color = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16f));
					Main.spriteBatch.Draw(mod.GetTexture("NPCs/AbyssNPCs/BobbitWormSegment"), new Vector2(center.X - Main.screenPosition.X, center.Y - Main.screenPosition.Y), 
						new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, 0, mod.GetTexture("NPCs/AbyssNPCs/BobbitWormSegment").Width, mod.GetTexture("NPCs/AbyssNPCs/BobbitWormSegment").Height)), color, rotation, 
						new Vector2((float)mod.GetTexture("NPCs/AbyssNPCs/BobbitWormSegment").Width * 0.5f, (float)mod.GetTexture("NPCs/AbyssNPCs/BobbitWormSegment").Height * 0.5f), 1f, SpriteEffects.None, 0f);
				}
			}
			return true;
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
            player.AddBuff(BuffID.Bleeding, 300, true);
            player.AddBuff(mod.BuffType("CrushDepth"), 300, true);
            if (CalamityWorld.revenge)
            {
                player.AddBuff(mod.BuffType("MarkedforDeath"), 300);
                player.AddBuff(mod.BuffType("Horror"), 120, true);
            }
        }

        public override void NPCLoot()
        {
            if (Main.rand.Next(1000) == 0 && CalamityWorld.revenge)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("HalibutCannon"));
            }
            if (NPC.downedPlantBoss || CalamityWorld.downedCalamitas)
            {
                if (Main.rand.Next(2) == 0)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DepthCells"), Main.rand.Next(5, 8));
                }
                if (Main.expertMode && Main.rand.Next(2) == 0)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DepthCells"), Main.rand.Next(2, 4));
                }
            }
        }

        public override void HitEffect(int hitDirection, double damage)
		{
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 20; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
                }
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/BobbitWorm"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/BobbitWorm2"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/BobbitWorm3"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/BobbitWorm4"), 1f);
            }
        }
	}
}