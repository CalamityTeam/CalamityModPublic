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
	public class Bloatfish : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bloatfish");
            Main.npcFrameCount[npc.type] = 4;
        }
		
		public override void SetDefaults()
		{
            npc.noGravity = true;
            npc.damage = 5;
			npc.width = 74;
			npc.height = 94;
			npc.defense = 100;
			npc.lifeMax = 12000;
            npc.aiStyle = -1;
			aiType = -1;
			npc.value = Item.buyPrice(0, 0, 30, 0);
            npc.buffImmune[mod.BuffType("CrushDepth")] = true;
            npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.9f;
			banner = npc.type;
			bannerItem = mod.ItemType("BloatfishBanner");
		}

        public override void AI()
        {
            npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
            npc.noGravity = true;
            if (npc.direction == 0)
            {
                npc.TargetClosest(true);
            }
            if (npc.wet)
            {
                npc.TargetClosest(false);
                if (npc.collideX)
                {
                    npc.velocity.X = npc.velocity.X * -1f;
                    npc.direction *= -1;
                    npc.netUpdate = true;
                }
                if (npc.collideY)
                {
                    npc.netUpdate = true;
                    if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y = Math.Abs(npc.velocity.Y) * -1f;
                        npc.directionY = -1;
                        npc.ai[0] = -1f;
                    }
                    else if (npc.velocity.Y < 0f)
                    {
                        npc.velocity.Y = Math.Abs(npc.velocity.Y);
                        npc.directionY = 1;
                        npc.ai[0] = 1f;
                    }
                }
                npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.1f;
                if (npc.velocity.X < -0.2f || npc.velocity.X > 0.2f)
                {
                    npc.velocity.X = npc.velocity.X * 0.95f;
                }
                if (npc.ai[0] == -1f)
                {
                    npc.velocity.Y = npc.velocity.Y - 0.01f;
                    if ((double)npc.velocity.Y < -0.3)
                    {
                        npc.ai[0] = 1f;
                    }
                }
                else
                {
                    npc.velocity.Y = npc.velocity.Y + 0.01f;
                    if ((double)npc.velocity.Y > 0.3)
                    {
                        npc.ai[0] = -1f;
                    }
                }
                int num258 = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
                int num259 = (int)(npc.position.Y + (float)(npc.height / 2)) / 16;
                if (Main.tile[num258, num259 - 1] == null)
                {
                    Main.tile[num258, num259 - 1] = new Tile();
                }
                if (Main.tile[num258, num259 + 1] == null)
                {
                    Main.tile[num258, num259 + 1] = new Tile();
                }
                if (Main.tile[num258, num259 + 2] == null)
                {
                    Main.tile[num258, num259 + 2] = new Tile();
                }
                if (Main.tile[num258, num259 - 1].liquid > 128)
                {
                    if (Main.tile[num258, num259 + 1].active())
                    {
                        npc.ai[0] = -1f;
                    }
                    else if (Main.tile[num258, num259 + 2].active())
                    {
                        npc.ai[0] = -1f;
                    }
                }
                if ((double)npc.velocity.Y > 0.4 || (double)npc.velocity.Y < -0.4)
                {
                    npc.velocity.Y = npc.velocity.Y * 0.95f;
                }
            }
            else
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity.X = npc.velocity.X * 0.94f;
                    if ((double)npc.velocity.X > -0.2 && (double)npc.velocity.X < 0.2)
                    {
                        npc.velocity.X = 0f;
                    }
                }
                npc.velocity.Y = npc.velocity.Y + 0.3f;
                if (npc.velocity.Y > 3f)
                {
                    npc.velocity.Y = 3f;
                }
                npc.ai[0] = 1f;
            }
            npc.rotation = npc.velocity.Y * (float)npc.direction * 0.1f;
            if ((double)npc.rotation < -0.2)
            {
                npc.rotation = -0.2f;
            }
            if ((double)npc.rotation > 0.2)
            {
                npc.rotation = 0.2f;
                return;
            }
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if ((projectile.penetrate == -1 || projectile.penetrate > 1) && !projectile.minion)
            {
                projectile.penetrate = 1;
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(mod.BuffType("CrushDepth"), 900, true);
        }

        public override void FindFrame(int frameHeight)
        {
            if (!npc.wet)
            {
                npc.frameCounter = 0.0;
                return;
            }
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyssLayer4 && spawnInfo.water && NPC.CountNPCS(mod.NPCType("Bloatfish")) < 3)
            {
				return SpawnCondition.CaveJellyfish.Chance * 0.3f;
			}
            return 0f;
        }

        public override void NPCLoot()
        {
            Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Voidstone"), Main.rand.Next(10, 21));
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
				for (int k = 0; k < 50; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
			if (npc.scale < 2f)
			{
				npc.scale += 0.05f;
			}
		}
	}
}