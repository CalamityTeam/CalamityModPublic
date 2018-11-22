using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.PlaguebringerGoliath
{
	public class PlagueHomingMissile : ModNPC
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Plague Homing Missile");
			Main.npcFrameCount[npc.type] = 4;
		}
		
		public override void SetDefaults()
		{
			npc.damage = 90;
			npc.width = 22; //324
			npc.height = 22; //216
			npc.defense = 10;
			npc.lifeMax = 1000;
			npc.aiStyle = -1;
			aiType = -1;
			npc.knockBackResist = 0f;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			npc.noGravity = true;
			npc.buffImmune[189] = true;
			npc.buffImmune[153] = true;
			npc.buffImmune[70] = true;
			npc.buffImmune[69] = true;
			npc.buffImmune[44] = true;
			npc.buffImmune[39] = true;
			npc.buffImmune[24] = true;
			npc.buffImmune[20] = true;
			npc.buffImmune[mod.BuffType("BrimstoneFlames")] = true;
			npc.buffImmune[mod.BuffType("HolyLight")] = true;
			npc.buffImmune[mod.BuffType("Plague")] = true;
		}
		
		public override void AI()
		{
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.015f, 0.1f, 0f);
            if (Math.Abs(npc.velocity.X) >= 3f || Math.Abs(npc.velocity.Y) >= 3f)
            {
                float num247 = 0f;
                float num248 = 0f;
                if (Main.rand.Next(2) == 1)
                {
                    num247 = npc.velocity.X * 0.5f;
                    num248 = npc.velocity.Y * 0.5f;
                }
                int num249 = Dust.NewDust(new Vector2(npc.position.X + 3f + num247, npc.position.Y + 3f + num248) - npc.velocity * 0.5f, npc.width - 8, npc.height - 8, 6, 0f, 0f, 100, default(Color), 0.5f);
                Main.dust[num249].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
                Main.dust[num249].velocity *= 0.2f;
                Main.dust[num249].noGravity = true;
                num249 = Dust.NewDust(new Vector2(npc.position.X + 3f + num247, npc.position.Y + 3f + num248) - npc.velocity * 0.5f, npc.width - 8, npc.height - 8, 31, 0f, 0f, 100, default(Color), 0.25f);
                Main.dust[num249].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num249].velocity *= 0.05f;
            }
            else if (Main.rand.Next(4) == 0)
            {
                int num252 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 31, 0f, 0f, 100, default(Color), 0.5f);
                Main.dust[num252].scale = 0.1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num252].fadeIn = 1.5f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num252].noGravity = true;
                Main.dust[num252].position = npc.Center + new Vector2(0f, (float)(-(float)npc.height / 2)).RotatedBy((double)npc.rotation, default(Vector2)) * 1.1f;
                Main.rand.Next(2);
                num252 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 6, 0f, 0f, 100, default(Color), 1f);
                Main.dust[num252].scale = 1f + (float)Main.rand.Next(5) * 0.1f;
                Main.dust[num252].noGravity = true;
                Main.dust[num252].position = npc.Center + new Vector2(0f, (float)(-(float)npc.height / 2 - 6)).RotatedBy((double)npc.rotation, default(Vector2)) * 1.1f;
            }
            npc.rotation = npc.velocity.ToRotation() + 1.57079637f;
            if (npc.ai[2] < 90f)
            {
                npc.ai[2] += 1f;
                return;
            }
            int num3;
            if (npc.ai[0] == 0f && npc.ai[1] > 0f)
            {
                float[] var_2_2065C_cp_0 = npc.ai;
                int var_2_2065C_cp_1 = 1;
                float num73 = var_2_2065C_cp_0[var_2_2065C_cp_1];
                var_2_2065C_cp_0[var_2_2065C_cp_1] = num73 - 1f;
            }
            else if (npc.ai[0] == 0f && npc.ai[1] == 0f)
            {
                npc.ai[0] = 1f;
                npc.ai[1] = (float)Player.FindClosest(npc.position, npc.width, npc.height);
                npc.netUpdate = true;
                float num754 = npc.velocity.Length();
                npc.velocity = Vector2.Normalize(npc.velocity) * (num754 + 4.2f); //4f
            }
            else if (npc.ai[0] == 1f)
            {
                float[] var_2_2087A_cp_0 = npc.localAI;
                int var_2_2087A_cp_1 = 1;
                float num73 = var_2_2087A_cp_0[var_2_2087A_cp_1];
                var_2_2087A_cp_0[var_2_2087A_cp_1] = num73 + 1f;
                float num757 = 600f;
                float num758 = 0f;
                float num759 = 300f;
                if (npc.localAI[1] == num757)
                {
                    CheckDead();
                    npc.life = 0;
                    return;
                }
                if (npc.localAI[1] >= num758 && npc.localAI[1] < num758 + num759)
                {
                    npc.noTileCollide = true;
                    Vector2 v3 = Main.player[(int)npc.ai[1]].Center - npc.Center;
                    float num760 = npc.velocity.ToRotation();
                    float num761 = v3.ToRotation();
                    double num762 = (double)(num761 - num760);
                    if (num762 > 3.1415926535897931)
                    {
                        num762 -= 6.2831853071795862;
                    }
                    if (num762 < -3.1415926535897931)
                    {
                        num762 += 6.2831853071795862;
                    }
                    npc.velocity = npc.velocity.RotatedBy(num762 * 0.20000000298023224, default(Vector2));
                }
                else
                {
                    npc.noTileCollide = false;
                }
            }
            for (int num767 = 0; num767 < 255; num767 = num3 + 1)
            {
                Player player5 = Main.player[num767];
                if (player5.active && !player5.dead && Vector2.Distance(player5.Center, npc.Center) <= 42f)
                {
                    CheckDead();
                    npc.life = 0;
                    return;
                }
                num3 = num767;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override bool CheckDead()
        {
            Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 14);
            npc.position.X = npc.position.X + (float)(npc.width / 2);
            npc.position.Y = npc.position.Y + (float)(npc.height / 2);
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

        public override bool PreNPCLoot()
		{
			return false;
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(mod.BuffType("Plague"), 120, true);
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < 5; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 46, hitDirection, -1f, 0, default(Color), 1f);
			}
			if (npc.life <= 0)
			{
				for (int k = 0; k < 10; k++)
				{
					Dust.NewDust(npc.position, npc.width, npc.height, 46, hitDirection, -1f, 0, default(Color), 1f);
				}
			}
		}
	}
}