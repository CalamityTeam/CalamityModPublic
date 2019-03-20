using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.AstrumDeus
{
	public class AstrumDeusBodySpectral : ModNPC
	{
        private int spawn = 0;

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astrum Deus");
		}
		
		public override void SetDefaults()
		{
			npc.damage = 80; //70
			npc.npcSlots = 5f;
			npc.width = 38; //324
			npc.height = 44; //216
			npc.defense = 60;
            npc.lifeMax = CalamityWorld.revenge ? 35850 : 25000; //250000
            if (CalamityWorld.death)
            {
                npc.lifeMax = 53750;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 1500000 : 1300000;
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
			NPCID.Sets.TrailCacheLength[npc.type] = 8;
			NPCID.Sets.TrailingMode[npc.type] = 1;
			npc.alpha = 255;
			npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.canGhostHeal = false;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			npc.netAlways = true;
			npc.boss = true;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/AstrumDeus");
            else
                music = MusicID.Boss3;
            npc.dontCountMe = true;
		}
		
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
		
		public override void AI()
		{
            npc.dontTakeDamage = AstrumDeusHeadSpectral.colorChange;
            npc.chaseable = !AstrumDeusHeadSpectral.colorChange;
            bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
			bool revenge = (CalamityWorld.revenge || CalamityWorld.bossRushActive);
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);
			if (!Main.npc[(int)npc.ai[1]].active)
            {
                npc.life = 0;
                npc.HitEffect(0, 10.0);
                npc.active = false;
            }
			if (Main.npc[(int)npc.ai[1]].alpha < 128)
			{
				if (npc.alpha != 0)
				{
					for (int num934 = 0; num934 < 2; num934++)
					{
						int num935 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 182, 0f, 0f, 100, default(Color), 2f);
						Main.dust[num935].noGravity = true;
						Main.dust[num935].noLight = true;
					}
				}
				npc.alpha -= 42;
				if (npc.alpha < 0)
				{
					npc.alpha = 0;
				}
			}
			if (Main.netMode != 1)
			{
				int shootTime = 4;
				if ((double)npc.life <= (double)npc.lifeMax * 0.65 || Main.player[npc.target].gravDir == -1f)
				{
					shootTime += CalamityWorld.death ? 4 : 2;
                }
				if ((double)npc.life <= (double)npc.lifeMax * 0.3 || CalamityWorld.bossRushActive || Main.player[npc.target].gravDir == -1f)
				{
					shootTime += CalamityWorld.death ? 6 : 2;
				}
				npc.localAI[0] += (float)Main.rand.Next(shootTime);
				if (npc.localAI[0] >= (float)Main.rand.Next(1400, 20000))
				{
					npc.localAI[0] = 0f;
					npc.TargetClosest(true);
					if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
					{
						float num941 = revenge ? 14f : 12f; //speed
                        if (CalamityWorld.death || CalamityWorld.bossRushActive)
                        {
                            num941 = 16f;
                        }
						if (Main.player[npc.target].gravDir == -1f)
						{
							num941 *= 2f;
						}
						Vector2 vector104 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
						float num942 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector104.X + (float)Main.rand.Next(-20, 21);
						float num943 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector104.Y + (float)Main.rand.Next(-20, 21);
						float num944 = (float)Math.Sqrt((double)(num942 * num942 + num943 * num943));
						num944 = num941 / num944;
						num942 *= num944;
						num943 *= num944;
						num942 += (float)Main.rand.Next(-5, 6) * 0.05f;
						num943 += (float)Main.rand.Next(-5, 6) * 0.05f;
						int num945 = expertMode ? 31 : 37;
						int num946 = mod.ProjectileType("AstralShot2");
						vector104.X += num942 * 5f;
						vector104.Y += num943 * 5f;
						int num947 = Projectile.NewProjectile(vector104.X, vector104.Y, num942, num943, num946, num945, 0f, Main.myPlayer, 0f, 0f);
						npc.netUpdate = true;
					}
				}
			}
		}

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Color lightColor = new Color(250, 150, Main.DiscoB, npc.alpha);
            Mod mod = ModLoader.GetMod("CalamityMod");
            Texture2D texture = mod.GetTexture("NPCs/AstrumDeus/AstrumDeusBodyAltSpectral");
			SpriteEffects spriteEffects = SpriteEffects.None;
			Color newColor = (AstrumDeusHeadSpectral.colorChange ? lightColor : drawColor);
			Microsoft.Xna.Framework.Color color24 = npc.GetAlpha(newColor);
			Microsoft.Xna.Framework.Color color25 = Lighting.GetColor((int)((double)npc.position.X + (double)npc.width * 0.5) / 16, (int)(((double)npc.position.Y + (double)npc.height * 0.5) / 16.0));
			Texture2D texture2D3 = (npc.localAI[3] == 1f ? texture : Main.npcTexture[npc.type]);
			int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
			int y3 = num156 * (int)npc.frameCounter;
			Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, y3, texture2D3.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;
			int num157 = 8;
			int num158 = 2;
			int num159 = 1;
			float num160 = 0f;
			int num161 = num159;
			while (((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)) && Lighting.NotRetro)
			{
				Microsoft.Xna.Framework.Color color26 = npc.GetAlpha(color25);
				{
					goto IL_6899;
				}
				IL_6881:
				num161 += num158;
				continue;
				IL_6899:
				float num164 = (float)(num157 - num161);
				if (num158 < 0)
				{
					num164 = (float)(num159 - num161);
				}
				color26 *= num164 / ((float)NPCID.Sets.TrailCacheLength[npc.type] * 1.5f);
				Vector2 value4 = (npc.oldPos[num161]);
				float num165 = npc.rotation;
				Main.spriteBatch.Draw(texture2D3, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + npc.rotation * num160 * (float)(num161 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, npc.scale, spriteEffects, 0f);
				goto IL_6881;
			}
			CalamityMod.DrawTexture(spriteBatch, texture2D3, 0, npc, newColor);
            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (AstrumDeusHeadSpectral.colorChange)
            {
                return false;
            }
            return true;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
            if ((double)npc.life <= (double)npc.lifeMax * 0.33)
            {
                damage = (int)((double)damage * 0.33f);
                if (projectile.minion)
                {
                    return;
                }
            }
            else
            {
                if (projectile.penetrate == -1 && !projectile.minion && !projectile.thrown)
                {
                    damage /= 2;
                }
                else if (projectile.penetrate > 1)
                {
                    damage /= projectile.penetrate;
                }
            }
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
            if (NPC.CountNPCS(mod.NPCType("AstrumDeusProbe3")) < 6 && CalamityWorld.revenge)
            {
                if (npc.life > 0 && Main.netMode != 1 && spawn == 0 && Main.rand.Next(25) == 0)
                {
                    spawn = 1;
                    int num660 = NPC.NewNPC((int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)npc.height), mod.NPCType("AstrumDeusProbe3"), 0, 0f, 0f, 0f, 0f, 255);
                    if (Main.netMode == 2 && num660 < 200)
                    {
                        NetMessage.SendData(23, -1, -1, null, num660, 0f, 0f, 0f, 0, 0, 0);
                    }
                    npc.netUpdate = true;
                }
            }
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
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 10; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, mod.DustType("AstralOrange"), 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(mod.BuffType("GodSlayerInferno"), 120, true);
		}
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}
	}
}