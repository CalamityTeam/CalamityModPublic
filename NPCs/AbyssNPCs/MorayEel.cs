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
	public class MorayEel : ModNPC
	{
        public bool hasBeenHit = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Moray Eel");
            Main.npcFrameCount[npc.type] = 6;
        }
		
		public override void SetDefaults()
		{
            npc.noGravity = true;
            npc.damage = 35;
			npc.width = 136;
			npc.height = 40;
			npc.defense = 8;
			npc.lifeMax = 110;
            npc.aiStyle = -1;
			aiType = -1;
            npc.buffImmune[mod.BuffType("CrushDepth")] = true;
            npc.value = Item.buyPrice(0, 0, 10, 0);
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
            npc.knockBackResist = 0.75f;
        }

        public override void AI()
        {
            npc.spriteDirection = ((npc.direction > 0) ? 1 : -1);
            npc.noGravity = true;
            if (npc.direction == 0)
            {
                npc.TargetClosest(true);
            }
            if (npc.justHit)
            {
                hasBeenHit = true;
            }
            npc.chaseable = hasBeenHit;
            if (npc.wet)
            {
                bool flag14 = hasBeenHit;
                npc.TargetClosest(false);
                if (Main.player[npc.target].wet && !Main.player[npc.target].dead && 
                    (Main.player[npc.target].Center - npc.Center).Length() < ((Main.player[npc.target].GetModPlayer<CalamityPlayer>(mod).anechoicPlating ||
                    Main.player[npc.target].GetModPlayer<CalamityPlayer>(mod).anechoicCoating) ? 250f : 300f) *
                    (Main.player[npc.target].GetModPlayer<CalamityPlayer>(mod).fishAlert ? 3f : 1f))
                {
                    flag14 = true;
                }
                if ((!Main.player[npc.target].wet || Main.player[npc.target].dead) && flag14)
                {
                    flag14 = false;
                }
                if (!flag14)
                {
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
                }
                if (flag14)
                {
                    npc.TargetClosest(true);
                    npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.15f;
                    npc.velocity.Y = npc.velocity.Y + (float)npc.directionY * 0.1f;
                    if (npc.velocity.X > 6f)
                    {
                        npc.velocity.X = 6f;
                    }
                    if (npc.velocity.X < -6f)
                    {
                        npc.velocity.X = -6f;
                    }
                    if (npc.velocity.Y > 4f)
                    {
                        npc.velocity.Y = 4f;
                    }
                    if (npc.velocity.Y < -4f)
                    {
                        npc.velocity.Y = -4f;
                    }
                }
                else
                {
                    npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.1f;
                    if (npc.velocity.X < -2f || npc.velocity.X > 2f)
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
                npc.velocity.Y = npc.velocity.Y + 0.4f;
                if (npc.velocity.Y > 12f)
                {
                    npc.velocity.Y = 12f;
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

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion)
            {
                return hasBeenHit;
            }
            return null;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += (hasBeenHit ? 0.15f : 0.075f);
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2((float)mod.GetTexture("NPCs/AbyssNPCs/MorayEelGlow").Width, (float)(mod.GetTexture("NPCs/AbyssNPCs/MorayEelGlow").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 0f + 4f + npc.gfxOffY);
            Microsoft.Xna.Framework.Color color = new Microsoft.Xna.Framework.Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.Gold);
            Main.spriteBatch.Draw(mod.GetTexture("NPCs/AbyssNPCs/MorayEelGlow"), vector,
                new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Bleeding, 180, true);
            player.AddBuff(BuffID.Venom, 90, true);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneSulphur)
            {
                return 0f;
            }
            if (spawnInfo.player.GetModPlayer<CalamityPlayer>(mod).ZoneAbyssLayer1 && spawnInfo.water)
            {
                return 0.2f;
            }
            return SpawnCondition.OceanMonster.Chance * 0.1f;
        }

        public override void NPCLoot()
        {
            if (Main.rand.Next(25) == 0)
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Flipper);
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
				for (int k = 0; k < 25; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}