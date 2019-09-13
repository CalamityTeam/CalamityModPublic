using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.Scavenger
{
    public class FlamePillar : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flame Pillar");
            Main.npcFrameCount[npc.type] = 4;
        }

		public override void SetDefaults()
		{
			npc.damage = 0;
			npc.npcSlots = 1f;
			npc.width = 40; //324
			npc.height = 150; //216
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

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            bool provy = CalamityWorld.downedProvidence;
            if (CalamityGlobalNPC.scavenger < 0 || !Main.npc[CalamityGlobalNPC.scavenger].active)
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
                npc.alpha -= 3;
                if (npc.alpha < 0)
                {
                    if (CalamityWorld.downedProvidence && !CalamityWorld.bossRushActive)
                    {
                        npc.damage = 400;
                    }
                    else
                    {
                        npc.damage = Main.expertMode ? 250 : 180;
                    }
                    npc.alpha = 0;
                }
            }
            if (npc.ai[0] == 0f)
            {
                npc.noTileCollide = false;
                npc.ai[1] += 1f;
                if (npc.ai[1] >= 180f)
                {
                    npc.ai[0] = 1f;
                }
            }
            else if (npc.ai[0] == 1f)
            {
                if (npc.ai[1] >= 0f)
                {
                    npc.ai[1] -= 1f;
                    npc.localAI[0] += 1f;
                    float SpeedX = (float)Main.rand.Next(-6, 7);
                    float SpeedY = (float)Main.rand.Next(-12, -8);
                    if (npc.localAI[0] >= 60f)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 shootFromVector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            int damage = Main.expertMode ? 32 : 45;
                            Projectile.NewProjectile(shootFromVector.X, shootFromVector.Y, SpeedX, SpeedY, mod.ProjectileType("RavagerFlame"), damage + (provy ? 30 : 0), 0f, Main.myPlayer, 0f, 0f);
                        }
                        npc.localAI[0] = 0f;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    npc.dontTakeDamage = false;
                    npc.life = 0;
                    HitEffect(npc.direction, 9999);
                    npc.netUpdate = true;
                    return;
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
                }
            }
        }

		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 50;
				npc.height = 180;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 30; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 135, 0f, 0f, 100, default, 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.NextBool(2))
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 30; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 1, 0f, 0f, 100, default, 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 8, 0f, 0f, 100, default, 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}
	}
}
