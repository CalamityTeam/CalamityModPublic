using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;
using Terraria.World.Generation;
using Terraria.GameContent.Generation;
using CalamityMod.Tiles;
using CalamityMod;

namespace CalamityMod.NPCs.AstrumDeus
{
	[AutoloadBossHead]
	public class AstrumDeusHeadSpectral : ModNPC
	{
        private bool flies = true;
        private const int minLength = 12;
        private const int maxLength = 13;
        private float speed = 10f;
        private float turnSpeed = 0.2f;
        private bool TailSpawned = false;
		
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Astrum Deus");
		}
		
		public override void SetDefaults()
		{
			npc.damage = 110; //150
			npc.npcSlots = 5f;
			npc.width = 56; //324
			npc.height = 56; //216
			npc.defense = 40;
            npc.lifeMax = CalamityWorld.revenge ? 33750 : 25000; //250000
            if (CalamityWorld.death)
            {
                npc.lifeMax = 51250;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 2300000 : 1700000;
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
			npc.boss = true;
			npc.value = Item.buyPrice(0, 30, 0, 0);
			npc.alpha = 255;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
			npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit4;
			npc.DeathSound = SoundID.NPCDeath14;
			npc.netAlways = true;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/AstrumDeus");
            bossBag = mod.ItemType("AstrumDeusBag");
        }
		
		public override void AI()
		{
            npc.dontTakeDamage = NPC.AnyNPCs(mod.NPCType("AstrumDeusHead"));
            npc.chaseable = !NPC.AnyNPCs(mod.NPCType("AstrumDeusHead"));
            bool expertMode = (Main.expertMode || CalamityWorld.bossRushActive);
            float speedLimit = CalamityWorld.revenge ? 9f : 7f;
            float turnSpeedLimit = CalamityWorld.revenge ? 0.14f : 0.12f;
            if (CalamityWorld.death || CalamityWorld.bossRushActive)
            {
                speedLimit = (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged ? 15f : 12f);
                turnSpeedLimit = (npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged ? 0.2f : 0.16f);
            }
            float speedBoost = speedLimit * (1f - (float)((double)npc.life / (double)npc.lifeMax));
            float turnSpeedBoost = turnSpeedLimit * (1f - (float)((double)npc.life / (double)npc.lifeMax));
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);
			if (npc.ai[3] > 0f)
			{
				npc.realLife = (int)npc.ai[3];
			}
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
			{
				npc.TargetClosest(true);
			}
			npc.velocity.Length();
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
			if (Main.netMode != 1)
			{
				if (!TailSpawned && npc.ai[0] == 0f)
	            {
	                int Previous = npc.whoAmI;
	                for (int num36 = 0; num36 < maxLength; num36++)
	                {
	                    int lol = 0;
	                    if (num36 >= 0 && num36 < minLength)
	                    {
	                        lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("AstrumDeusBodySpectral"), npc.whoAmI);
	                    }
	                    else
	                    {
	                        lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("AstrumDeusTailSpectral"), npc.whoAmI);
	                    }
	                    Main.npc[lol].realLife = npc.whoAmI;
	                    Main.npc[lol].ai[2] = (float)npc.whoAmI;
	                    Main.npc[lol].ai[1] = (float)Previous;
	                    Main.npc[Previous].ai[0] = (float)lol;
	                    Previous = lol;
	                }
	                TailSpawned = true;
	            }
				if (!npc.active && Main.netMode == 2)
				{
					NetMessage.SendData(28, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
				}
			}
			int num180 = (int)(npc.position.X / 16f) - 1;
			int num181 = (int)((npc.position.X + (float)npc.width) / 16f) + 2;
			int num182 = (int)(npc.position.Y / 16f) - 1;
			int num183 = (int)((npc.position.Y + (float)npc.height) / 16f) + 2;
			if (num180 < 0)
			{
				num180 = 0;
			}
			if (num181 > Main.maxTilesX)
			{
				num181 = Main.maxTilesX;
			}
			if (num182 < 0)
			{
				num182 = 0;
			}
			if (num183 > Main.maxTilesY)
			{
				num183 = Main.maxTilesY;
			}
            npc.localAI[1] = 0f;
            bool canFly = flies;
            if (Main.player[npc.target].dead || Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 10000f)
            {
                npc.TargetClosest(false);
                canFly = false;
                npc.velocity.Y = npc.velocity.Y + 10f;
                if ((double)npc.position.Y > Main.worldSurface * 16.0)
                {
                    npc.velocity.Y = npc.velocity.Y + 10f;
                }
                if ((double)npc.position.Y > Main.rockLayer * 16.0)
                {
                    for (int num957 = 0; num957 < 200; num957++)
                    {
                        if (Main.npc[num957].aiStyle == npc.aiStyle)
                        {
                            Main.npc[num957].active = false;
                        }
                    }
                }
            }
            float num188 = speed + speedBoost;
            float num189 = turnSpeed + turnSpeedBoost;
            Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num191 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
            float num192 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
            int num42 = -1;
            int num43 = (int)(Main.player[npc.target].Center.X / 16f);
            int num44 = (int)(Main.player[npc.target].Center.Y / 16f);
            for (int num45 = num43 - 2; num45 <= num43 + 2; num45++)
            {
                for (int num46 = num44; num46 <= num44 + 15; num46++)
                {
                    if (WorldGen.SolidTile2(num45, num46))
                    {
                        num42 = num46;
                        break;
                    }
                }
                if (num42 > 0)
                {
                    break;
                }
            }
            float num48 = num188 * 1.3f;
            float num49 = num188 * 0.7f;
            float num50 = npc.velocity.Length();
            if (num50 > 0f)
            {
                if (num50 > num48)
                {
                    npc.velocity.Normalize();
                    npc.velocity *= num48;
                }
                else if (num50 < num49)
                {
                    npc.velocity.Normalize();
                    npc.velocity *= num49;
                }
            }
            for (int num52 = 0; num52 < 200; num52++)
            {
                if (Main.npc[num52].active && Main.npc[num52].type == npc.type && num52 != npc.whoAmI)
                {
                    Vector2 vector4 = Main.npc[num52].Center - npc.Center;
                    if (vector4.Length() < 60f)
                    {
                        vector4.Normalize();
                        vector4 *= 200f;
                        num191 -= vector4.X;
                        num192 -= vector4.Y;
                    }
                }
            }
            num191 = (float)((int)(num191 / 16f) * 16);
            num192 = (float)((int)(num192 / 16f) * 16);
            vector18.X = (float)((int)(vector18.X / 16f) * 16);
            vector18.Y = (float)((int)(vector18.Y / 16f) * 16);
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
            if (npc.ai[1] > 0f && npc.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    num191 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - vector18.X;
                    num192 = Main.npc[(int)npc.ai[1]].position.Y + (float)(Main.npc[(int)npc.ai[1]].height / 2) - vector18.Y;
                }
                catch
                {
                }
                npc.rotation = (float)System.Math.Atan2((double)num192, (double)num191) + 1.57f;
                num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
                int num194 = npc.width;
                num193 = (num193 - (float)num194) / num193;
                num191 *= num193;
                num192 *= num193;
                npc.velocity = Vector2.Zero;
                npc.position.X = npc.position.X + num191;
                npc.position.Y = npc.position.Y + num192;
            }
            else
            {
                num193 = (float)System.Math.Sqrt((double)(num191 * num191 + num192 * num192));
                float num196 = System.Math.Abs(num191);
                float num197 = System.Math.Abs(num192);
                float num198 = num188 / num193;
                num191 *= num198;
                num192 *= num198;
                if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
                {
                    if (npc.velocity.X < num191)
                    {
                        npc.velocity.X = npc.velocity.X + num189;
                    }
                    else
                    {
                        if (npc.velocity.X > num191)
                        {
                            npc.velocity.X = npc.velocity.X - num189;
                        }
                    }
                    if (npc.velocity.Y < num192)
                    {
                        npc.velocity.Y = npc.velocity.Y + num189;
                    }
                    else
                    {
                        if (npc.velocity.Y > num192)
                        {
                            npc.velocity.Y = npc.velocity.Y - num189;
                        }
                    }
                    if ((double)System.Math.Abs(num192) < (double)num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
                    {
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num189 * 2f;
                        }
                        else
                        {
                            npc.velocity.Y = npc.velocity.Y - num189 * 2f;
                        }
                    }
                    if ((double)System.Math.Abs(num191) < (double)num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
                    {
                        if (npc.velocity.X > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num189 * 2f; //changed from 2
                        }
                        else
                        {
                            npc.velocity.X = npc.velocity.X - num189 * 2f; //changed from 2
                        }
                    }
                }
                else
                {
                    if (num196 > num197)
                    {
                        if (npc.velocity.X < num191)
                        {
                            npc.velocity.X = npc.velocity.X + num189 * 1.1f; //changed from 1.1
                        }
                        else if (npc.velocity.X > num191)
                        {
                            npc.velocity.X = npc.velocity.X - num189 * 1.1f; //changed from 1.1
                        }
                        if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
                        {
                            if (npc.velocity.Y > 0f)
                            {
                                npc.velocity.Y = npc.velocity.Y + num189;
                            }
                            else
                            {
                                npc.velocity.Y = npc.velocity.Y - num189;
                            }
                        }
                    }
                    else
                    {
                        if (npc.velocity.Y < num192)
                        {
                            npc.velocity.Y = npc.velocity.Y + num189 * 1.1f;
                        }
                        else if (npc.velocity.Y > num192)
                        {
                            npc.velocity.Y = npc.velocity.Y - num189 * 1.1f;
                        }
                        if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
                        {
                            if (npc.velocity.X > 0f)
                            {
                                npc.velocity.X = npc.velocity.X + num189;
                            }
                            else
                            {
                                npc.velocity.X = npc.velocity.X - num189;
                            }
                        }
                    }
                }
            }
            npc.rotation = (float)System.Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
        }
		
		public override bool CheckActive()
		{
			return false;
		}

        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.AnyNPCs(mod.NPCType("AstrumDeusHead")))
            {
                return new Color(250, 150, Main.DiscoB, npc.alpha);
            }
            return null;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (NPC.AnyNPCs(mod.NPCType("AstrumDeusHead")))
            {
                return false;
            }
            return true;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
            if (((projectile.type == ProjectileID.HallowStar || projectile.type == ProjectileID.CrystalShard) && projectile.ranged) ||
                projectile.type == mod.ProjectileType("TerraBulletSplit") || projectile.type == mod.ProjectileType("TerraArrow2"))
            {
                damage /= 2;
            }
            if (projectile.penetrate == -1 && !projectile.minion && !projectile.thrown)
			{
				damage /= 5;
			}
			else if (projectile.penetrate > 1)
			{
				damage /= projectile.penetrate;
			}
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
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
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}
		
		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = ItemID.None;
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (CalamityWorld.downedStarGod)
			{
				player.AddBuff(mod.BuffType("GodSlayerInferno"), 150, true);
			}
		}
		
		public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
		{
			npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
			npc.damage = (int)(npc.damage * 0.8f);
		}
	}
}