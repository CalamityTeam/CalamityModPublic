using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.Bumblefuck
{
	public class Bumblefuck3 : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bumblebirb");
			Main.npcFrameCount[npc.type] = 10;
		}
		
		public override void SetDefaults()
		{
			npc.npcSlots = 1f;
			npc.aiStyle = -1;
			aiType = -1;
			npc.damage = 190;
			npc.width = 80; //324
			npc.height = 80; //216
			npc.scale = 0.85f;
			npc.defense = 40;
			npc.lifeMax = 60000;
			npc.knockBackResist = 0f;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
				npc.buffImmune[BuffID.Ichor] = false;
                npc.buffImmune[BuffID.CursedInferno] = false;
                npc.buffImmune[mod.BuffType("ExoFreeze")] = false;
            }
			npc.lavaImmune = true;
			npc.noGravity = true;
			npc.canGhostHeal = false;
			npc.HitSound = SoundID.NPCHit51;
			npc.DeathSound = SoundID.NPCDeath46;
		}

        public override void AI()
        {
            if (!NPC.AnyNPCs(mod.NPCType("Yharon")))
            {
                npc.active = false;
                npc.netUpdate = true;
                return;
            }
            Player player = Main.player[npc.target];
            bool revenge = CalamityWorld.revenge;
            Vector2 vector = npc.Center;
            npc.noTileCollide = false;
            npc.noGravity = true;
            npc.damage = npc.defDamage;
            float generalSpeedValue = 19f;
            float generalSpeedValue2 = 25f;
            float generalSpeedValue3 = 9f;
            float generalSpeedValue4 = 17f;
            float generalSpeedValue5 = 8f;
            if (Vector2.Distance(player.Center, vector) > 5600f)
            {
                if (npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                }
            }
            if (npc.target < 0 || player.dead || !player.active)
            {
                npc.TargetClosest(true);
                if (player.dead)
                {
                    npc.ai[0] = -1f;
                }
            }
            else
            {
                Vector2 vector205 = player.Center - npc.Center;
                if (npc.ai[0] > 1f && vector205.Length() > 3600f)
                {
                    npc.ai[0] = 1f;
                }
            }
            if (npc.ai[0] == -1f)
            {
                Vector2 value50 = new Vector2(0f, -8f);
                npc.velocity = (npc.velocity * generalSpeedValue + value50) / 10f;
                npc.noTileCollide = true;
                npc.dontTakeDamage = true;
                return;
            }
            if (npc.ai[0] == 0f)
            {
                npc.TargetClosest(true);
                if (npc.Center.X < player.Center.X - 2f)
                {
                    npc.direction = 1;
                }
                if (npc.Center.X > player.Center.X + 2f)
                {
                    npc.direction = -1;
                }
                npc.spriteDirection = npc.direction;
                npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.05f) / 10f;
                if (npc.collideX)
                {
                    npc.velocity.X = npc.velocity.X * (-npc.oldVelocity.X * 0.5f);
                    if (npc.velocity.X > generalSpeedValue2)
                    {
                        npc.velocity.X = generalSpeedValue2;
                    }
                    if (npc.velocity.X < -generalSpeedValue2)
                    {
                        npc.velocity.X = -generalSpeedValue2;
                    }
                }
                if (npc.collideY)
                {
                    npc.velocity.Y = npc.velocity.Y * (-npc.oldVelocity.Y * 0.5f);
                    if (npc.velocity.Y > generalSpeedValue2)
                    {
                        npc.velocity.Y = generalSpeedValue2;
                    }
                    if (npc.velocity.Y < -generalSpeedValue2)
                    {
                        npc.velocity.Y = -generalSpeedValue2;
                    }
                }
                Vector2 value51 = player.Center - npc.Center;
                value51.Y -= 200f;
                if (value51.Length() > 1600f) //800
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                }
                else if (value51.Length() > 80f)
                {
                    float scaleFactor15 = generalSpeedValue3; //6
                    float num1306 = 30f;
                    value51.Normalize();
                    value51 *= scaleFactor15;
                    npc.velocity = (npc.velocity * (num1306 - 1f) + value51) / num1306;
                }
                else if (npc.velocity.Length() > generalSpeedValue4) //8
                {
                    npc.velocity *= 0.95f;
                }
                else if (npc.velocity.Length() < generalSpeedValue5) //4
                {
                    npc.velocity *= 1.05f;
                }
                npc.ai[1] += 1f;
                if (npc.justHit)
                {
                    npc.ai[1] += (float)Main.rand.Next(10, 30);
                }
                if (npc.ai[1] >= 180f && Main.netMode != 1)
                {
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                    while (npc.ai[0] == 0f)
                    {
                        int num1307 = Main.rand.Next(3);
                        if (num1307 == 0 && Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1))
                        {
                            npc.ai[0] = 2f;
                        }
                        else if (num1307 >= 1)
                        {
                            npc.ai[0] = 3f;
                        }
                    }
                    return;
                }
            }
            else
            {
                if (npc.ai[0] == 1f)
                {
                    npc.collideX = false;
                    npc.collideY = false;
                    npc.noTileCollide = true;
                    if (npc.target < 0 || !player.active || player.dead)
                    {
                        npc.TargetClosest(true);
                    }
                    if (npc.velocity.X < 0f)
                    {
                        npc.direction = -1;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.direction = 1;
                    }
                    npc.spriteDirection = npc.direction;
                    npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.04f) / 10f;
                    Vector2 value52 = player.Center - npc.Center;
                    if (value52.Length() < 500f && !Collision.SolidCollision(npc.position, npc.width, npc.height)) //500
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                    }
                    float scaleFactor16 = generalSpeedValue3 + value52.Length() / 100f; //7
                    float num1308 = 25f;
                    value52.Normalize();
                    value52 *= scaleFactor16;
                    npc.velocity = (npc.velocity * (num1308 - 1f) + value52) / num1308;
                    return;
                }
                if (npc.ai[0] == 2f)
                {
                    npc.damage = (int)((double)npc.defDamage * 0.85);
                    if (npc.target < 0 || !player.active || player.dead)
                    {
                        npc.TargetClosest(true);
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                    }
                    if (player.Center.X - 10f < npc.Center.X)
                    {
                        npc.direction = -1;
                    }
                    else if (player.Center.X + 10f > npc.Center.X)
                    {
                        npc.direction = 1;
                    }
                    npc.spriteDirection = npc.direction;
                    npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.05f) / 5f;
                    if (npc.collideX)
                    {
                        npc.velocity.X = npc.velocity.X * (-npc.oldVelocity.X * 0.5f);
                        if (npc.velocity.X > generalSpeedValue2) //4
                        {
                            npc.velocity.X = generalSpeedValue2; //4
                        }
                        if (npc.velocity.X < -generalSpeedValue2) //4
                        {
                            npc.velocity.X = -generalSpeedValue2; //4
                        }
                    }
                    if (npc.collideY)
                    {
                        npc.velocity.Y = npc.velocity.Y * (-npc.oldVelocity.Y * 0.5f);
                        if (npc.velocity.Y > generalSpeedValue2) //4
                        {
                            npc.velocity.Y = generalSpeedValue2; //4
                        }
                        if (npc.velocity.Y < -generalSpeedValue2) //4
                        {
                            npc.velocity.Y = -generalSpeedValue2; //4
                        }
                    }
                    Vector2 value53 = player.Center - npc.Center;
                    value53.Y -= 20f;
                    npc.ai[2] += 0.0222222228f;
                    if (Main.expertMode)
                    {
                        npc.ai[2] += 0.0166666675f;
                    }
                    float scaleFactor17 = generalSpeedValue3 + npc.ai[2] + value53.Length() / 120f; //4
                    float num1309 = 20f;
                    value53.Normalize();
                    value53 *= scaleFactor17;
                    npc.velocity = (npc.velocity * (num1309 - 1f) + value53) / num1309;
                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 240f || !Collision.CanHit(npc.Center, 1, 1, player.Center, 1, 1))
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        return;
                    }
                }
                else
                {
                    if (npc.ai[0] == 3f)
                    {
                        npc.noTileCollide = true;
                        if (npc.velocity.X < 0f)
                        {
                            npc.direction = -1;
                        }
                        else
                        {
                            npc.direction = 1;
                        }
                        npc.spriteDirection = npc.direction;
                        npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.035f) / 5f;
                        Vector2 value54 = player.Center - npc.Center;
                        value54.Y -= 12f;
                        if (npc.Center.X > player.Center.X)
                        {
                            value54.X += 400f;
                        }
                        else
                        {
                            value54.X -= 400f;
                        }
                        if (Math.Abs(npc.Center.X - player.Center.X) > 350f && Math.Abs(npc.Center.Y - player.Center.Y) < 20f)
                        {
                            npc.ai[0] = 3.1f;
                            npc.ai[1] = 0f;
                        }
                        npc.ai[1] += 0.0333333351f;
                        float scaleFactor18 = generalSpeedValue4 + npc.ai[1]; //8
                        float num1310 = 4f;
                        value54.Normalize();
                        value54 *= scaleFactor18;
                        npc.velocity = (npc.velocity * (num1310 - 1f) + value54) / num1310;
                        return;
                    }
                    if (npc.ai[0] == 3.1f)
                    {
                        npc.noTileCollide = true;
                        npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.035f) / 5f;
                        Vector2 vector206 = player.Center - npc.Center;
                        vector206.Y -= 12f;
                        float scaleFactor19 = generalSpeedValue2; //16
                        float num1311 = 8f;
                        vector206.Normalize();
                        vector206 *= scaleFactor19;
                        npc.velocity = (npc.velocity * (num1311 - 1f) + vector206) / num1311;
                        if (npc.velocity.X < 0f)
                        {
                            npc.direction = -1;
                        }
                        else
                        {
                            npc.direction = 1;
                        }
                        npc.spriteDirection = npc.direction;
                        npc.ai[1] += 1f;
                        if (npc.ai[1] > 10f)
                        {
                            npc.velocity = vector206;
                            if (npc.velocity.X < 0f)
                            {
                                npc.direction = -1;
                            }
                            else
                            {
                                npc.direction = 1;
                            }
                            npc.ai[0] = 3.2f;
                            npc.ai[1] = 0f;
                            npc.ai[1] = (float)npc.direction;
                            return;
                        }
                    }
                    else
                    {
                        if (npc.ai[0] == 3.2f)
                        {
                            npc.damage = (int)((double)npc.defDamage * 1.5);
                            npc.collideX = false;
                            npc.collideY = false;
                            npc.noTileCollide = true;
                            npc.ai[2] += 0.0333333351f;
                            npc.velocity.X = (generalSpeedValue4 + npc.ai[2]) * npc.ai[1]; //16
                            if ((npc.ai[1] > 0f && npc.Center.X > player.Center.X + 260f) || (npc.ai[1] < 0f && npc.Center.X < player.Center.X - 260f))
                            {
                                if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
                                {
                                    npc.ai[0] = 0f;
                                    npc.ai[1] = 0f;
                                    npc.ai[2] = 0f;
                                    npc.ai[3] = 0f;
                                }
                                else if (Math.Abs(npc.Center.X - player.Center.X) > 1200f) //800
                                {
                                    npc.ai[0] = 1f;
                                    npc.ai[1] = 0f;
                                    npc.ai[2] = 0f;
                                    npc.ai[3] = 0f;
                                }
                            }
                            npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.035f) / 5f;
                            return;
                        }
                    }
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override bool PreNPCLoot()
        {
            return false;
        }
		
		public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += (double)(npc.velocity.Length() / 4f);
			npc.frameCounter += 1.0;
			if (npc.ai[0] < 4f)
			{
				if (npc.frameCounter >= 6.0)
				{
					npc.frameCounter = 0.0;
					npc.frame.Y = npc.frame.Y + frameHeight;
				}
				if (npc.frame.Y / frameHeight > 4)
				{
					npc.frame.Y = 0;
				}
			}
			else
			{
				if (npc.frameCounter >= 6.0)
				{
					npc.frameCounter = 0.0;
					npc.frame.Y = npc.frame.Y + frameHeight;
				}
				if (npc.frame.Y / frameHeight > 9)
				{
					npc.frame.Y = frameHeight * 5;
				}
			}
        }
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 50; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 244, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}