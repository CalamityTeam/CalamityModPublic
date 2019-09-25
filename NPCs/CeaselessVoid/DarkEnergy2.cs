using System;
using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;

namespace CalamityMod.NPCs.CeaselessVoid
{
    public class DarkEnergy2 : ModNPC
	{
        public int invinceTime = 120;

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dark Energy");
			Main.npcFrameCount[npc.type] = 6;
		}

		public override void SetDefaults()
		{
			npc.aiStyle = -1;
			npc.damage = 0;
			npc.dontTakeDamage = true;
			npc.width = 80; //324
			npc.height = 80; //216
			npc.defense = 68;
            npc.lifeMax = 6000;
            if (CalamityWorld.DoGSecondStageCountdown <= 0)
            {
                npc.lifeMax = 24000;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = 44000;
            }
			double HPBoost = (double)Config.BossHealthPercentageBoost * 0.01;
			npc.lifeMax += (int)((double)npc.lifeMax * HPBoost);
			npc.knockBackResist = 0.1f;
            npc.noGravity = true;
			npc.noTileCollide = true;
			npc.canGhostHeal = false;
			aiType = -1;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.HitSound = SoundID.NPCHit53;
			npc.DeathSound = SoundID.NPCDeath44;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(invinceTime);
			writer.Write(npc.dontTakeDamage);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			invinceTime = reader.ReadInt32();
			npc.dontTakeDamage = reader.ReadBoolean();
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
			bool expertMode = Main.expertMode;
            if (invinceTime > 0)
            {
                invinceTime--;
            }
            else
            {
                npc.damage = expertMode ? 240 : 120;
                npc.dontTakeDamage = false;
            }

			double mult = 0.5 +
				(CalamityWorld.revenge ? 0.2 : 0.0) +
				(CalamityWorld.death ? 0.2 : 0.0);
			if ((double)npc.life < (double)npc.lifeMax * mult)
			{
				npc.knockBackResist = 0f;
			}

			if (npc.ai[1] == 0f)
			{
				npc.scale -= 0.01f;
				npc.alpha += 15;
				if (npc.alpha >= 125)
				{
					npc.alpha = 130;
					npc.ai[1] = 1f;
				}
			}
			else if (npc.ai[1] == 1f)
			{
				npc.scale += 0.01f;
				npc.alpha -= 15;
				if (npc.alpha <= 0)
				{
					npc.alpha = 0;
					npc.ai[1] = 0f;
				}
			}
			npc.TargetClosest(true);
			Player player = Main.player[npc.target];
			if (!player.active || player.dead || CalamityGlobalNPC.voidBoss < 0 || !Main.npc[CalamityGlobalNPC.voidBoss].active)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
                    npc.velocity = new Vector2(0f, -10f);
                    if (npc.timeLeft > 150)
                    {
                        npc.timeLeft = 150;
                    }
                    return;
                }
            }
            else if (npc.timeLeft < 2400)
            {
                npc.timeLeft = 2400;
            }
            Vector2 vector145 = new Vector2(npc.Center.X, npc.Center.Y);
			float num1258 = Main.player[npc.target].Center.X - vector145.X;
			float num1259 = Main.player[npc.target].Center.Y - vector145.Y;
			float num1260 = (float)Math.Sqrt((double)(num1258 * num1258 + num1259 * num1259));

			float num1261 = expertMode ? 15f : 12f;
			if (CalamityWorld.revenge)
				num1261 += 3f;
			if (CalamityWorld.death)
				num1261 += 3f;

			num1260 = num1261 / num1260;
			num1258 *= num1260;
			num1259 *= num1260;
			npc.velocity.X = (npc.velocity.X * 100f + num1258) / 101f;
			npc.velocity.Y = (npc.velocity.Y * 100f + num1259) / 101f;
		}

		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (CalamityWorld.revenge)
			{
				player.AddBuff(mod.BuffType("Horror"), 300, true);
			}
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;
			return true;
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				for (int k = 0; k < 20; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 173, hitDirection, -1f, 0, default, 1f);
				}
			}
		}
	}
}
