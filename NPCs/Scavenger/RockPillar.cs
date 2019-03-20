using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.Scavenger
{
	public class RockPillar : ModNPC
	{
        int tileCollideCounter = 0;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rock Pillar");
		}
		
		public override void SetDefaults()
		{
			npc.damage = 0;
			npc.npcSlots = 1f;
			npc.width = 60; //324
			npc.height = 300; //216
			npc.defense = 0;
			npc.lifeMax = 100;
            npc.alpha = 255;
			npc.aiStyle = -1; //new
            aiType = -1; //new
			npc.knockBackResist = 0f;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
				npc.buffImmune[BuffID.Ichor] = false;
			}
            npc.dontTakeDamage = true;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
		}

        public override void AI()
        {
            if (!Main.npc[CalamityGlobalNPC.scavenger].active)
            {
                npc.dontTakeDamage = false;
                npc.life = 0;
                HitEffect(npc.direction, 9999);
                npc.netUpdate = true;
                return;
            }
            if (npc.timeLeft < 3000)
            {
                npc.timeLeft = 3000;
            }
            if (npc.alpha > 0)
            {
                npc.alpha -= 10;
                if (npc.alpha < 0)
                {
                    npc.alpha = 0;
                }
            }
            if (Math.Abs(npc.velocity.X) > 0.5f)
            {
                if (tileCollideCounter < 210)
                {
                    tileCollideCounter++;
                    npc.noTileCollide = true;
                }
                else
                {
                    npc.noTileCollide = false;
                }
                if (CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive)
                {
                    npc.damage = 400;
                }
                else
                {
                    npc.damage = Main.expertMode ? 250 : 180;
                }
            }
            else
            {
                npc.noTileCollide = false;
                npc.damage = 0;
            }
            if (npc.ai[0] == 0f)
            {
                npc.noTileCollide = false;
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity.X = npc.velocity.X * 0.8f;
                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 0f)
                    {
                        npc.ai[1] += 1f;
                    }
                    if (npc.ai[1] >= 300f)
                    {
                        npc.ai[1] = -20f;
                    }
                    else if (npc.ai[1] == -1f)
                    {
                        npc.TargetClosest(true);
                        npc.velocity.X = (float)(4 * npc.direction);
                        npc.velocity.Y = -24.5f;
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                    }
                }
            }
            else if (npc.ai[0] == 1f)
            {
                if (npc.velocity.Y == 0f)
                {
                    Main.PlaySound(SoundID.Item14, npc.position);
                    npc.ai[0] = 0f;
                    npc.dontTakeDamage = false;
                    npc.life = 0;
                    HitEffect(npc.direction, 9999);
                    npc.netUpdate = true;
                    return;
                }
                else
                {
                    npc.TargetClosest(true);
                    if (npc.position.X < Main.player[npc.target].position.X && npc.position.X + (float)npc.width > Main.player[npc.target].position.X + (float)Main.player[npc.target].width)
                    {
                        npc.velocity.X = npc.velocity.X * 0.9f;
                        npc.velocity.Y = npc.velocity.Y + 0.2f;
                    }
                    else
                    {
                        if (npc.direction < 0)
                        {
                            npc.velocity.X = npc.velocity.X - 0.2f;
                        }
                        else if (npc.direction > 0)
                        {
                            npc.velocity.X = npc.velocity.X + 0.2f;
                        }
                        float random = (float)Main.rand.Next(7);
                        float velocityX = 6f + random;
                        if (npc.velocity.X < -velocityX)
                        {
                            npc.velocity.X = -velocityX;
                        }
                        if (npc.velocity.X > velocityX)
                        {
                            npc.velocity.X = velocityX;
                        }
                    }
                }
            }
            if (npc.target <= 0 || npc.target == 255 || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }
            int distanceFromTarget = 8000;
            if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) + Math.Abs(npc.Center.Y - Main.player[npc.target].Center.Y) > (float)distanceFromTarget)
            {
                npc.TargetClosest(true);
                if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) + Math.Abs(npc.Center.Y - Main.player[npc.target].Center.Y) > (float)distanceFromTarget)
                {
                    npc.active = false;
                    npc.netUpdate = true;
                    return;
                }
            }
        }
		
		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 80;
				npc.height = 360;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 30; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 8, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 30; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 1, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 8, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}
	}
}