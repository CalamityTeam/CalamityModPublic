using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;

namespace CalamityMod.NPCs.TheDevourerofGods
{
	public class DevourerofGodsBody : ModNPC
	{
        public int invinceTime = 360;

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Devourer of Gods");
		}
		
		public override void SetDefaults()
		{
			npc.damage = 180; //70
			npc.npcSlots = 5f;
			npc.width = 34; //38
			npc.height = 34; //30
			npc.defense = 0;
            npc.lifeMax = CalamityWorld.revenge ? 500000 : 450000; //1000000 960000
            if (CalamityWorld.death)
            {
                npc.lifeMax = 850000;
            }
            npc.aiStyle = 6; //new
            aiType = -1; //new
            animationType = 10; //new
			npc.knockBackResist = 0f;
			npc.scale = 1.4f;
			npc.alpha = 255;
			npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
            npc.chaseable = false;
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
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/ScourgeofTheUniverse");
            else
                music = MusicID.Boss3;
			npc.dontCountMe = true;
			if (Main.expertMode)
			{
				npc.scale = 1.5f;
			}
		}
		
		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}
		
		public override void AI()
		{
            bool expertMode = Main.expertMode;
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);
            if (invinceTime > 0)
            {
                invinceTime--;
                npc.dontTakeDamage = true;
            }
            else
            {
                npc.dontTakeDamage = false;
            }
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
				float shootTimer = 2f + (expertMode ? 1f : 0f);
				npc.localAI[0] += shootTimer;
				int projectileType = mod.ProjectileType("DoGNebulaShot");
				int damage = expertMode ? 55 : 68;
				float num941 = 5f;
                if (npc.localAI[0] >= (float)Main.rand.Next(2800, 32000))
                {
                    npc.localAI[0] = 0f;
                    npc.TargetClosest(true);
                    Vector2 vector104 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)(npc.height / 2));
                    float num942 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector104.X + (float)Main.rand.Next(-20, 21);
                    float num943 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector104.Y + (float)Main.rand.Next(-20, 21);
                    float num944 = (float)Math.Sqrt((double)(num942 * num942 + num943 * num943));
                    num944 = num941 / num944;
                    num942 *= num944;
                    num943 *= num944;
                    num942 += (float)Main.rand.Next(-5, 6) * 0.05f;
                    num943 += (float)Main.rand.Next(-5, 6) * 0.05f;
                    vector104.X += num942 * 5f;
                    vector104.Y += num943 * 5f;
                    Projectile.NewProjectile(vector104.X, vector104.Y, num942, num943, projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
                    npc.netUpdate = true;
                }
			}
		}
		
		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			cooldownSlot = 1;
			return true;
		}
		
		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
            if (damage > npc.lifeMax / 2)
            {
                damage = 0;
                return false;
            }
            double protection = CalamityWorld.death ? 0.05 : 0.08;
            damage = (int)((double)damage * protection);
            return true;
		}

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (ProjectileID.Sets.StardustDragon[projectile.type])
            {
                damage /= 2;
            }
            if (projectile.type == mod.ProjectileType("SulphuricAcidMist2"))
            {
                damage /= 8;
            }
            if (projectile.minion)
            {
                return;
            }
            if (projectile.penetrate == -1) //not a minion and penetrate is infinite
            {
                damage = (int)((double)damage * 0.2);
            }
            else if (projectile.penetrate > 3) //not a minion, penetrate is not infinite, penetrate is greater than 3
            {
                damage = (int)((double)damage * 0.7);
            }
            else //not a minion, penetrate is not infinite, and penetrate is not greater than 1
            {
                projectile.penetrate = 1;
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
			if (npc.life <= 0)
			{
				float randomSpread = (float)(Main.rand.Next(-100, 100) / 100);
				Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/DoGBody"), 1f);
				Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/DoGBody2"), 1f);
				Gore.NewGore(npc.position, npc.velocity * randomSpread * Main.rand.NextFloat(), mod.GetGoreSlot("Gores/DoGBody3"), 1f);
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 50;
				npc.height = 50;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 10; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 20; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			player.AddBuff(mod.BuffType("GodSlayerInferno"), 240, true);
			player.AddBuff(BuffID.Frostburn, 240, true);
			player.AddBuff(BuffID.Darkness, 240, true);
		}
	}
}