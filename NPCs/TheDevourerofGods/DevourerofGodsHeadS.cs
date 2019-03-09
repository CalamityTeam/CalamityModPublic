using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Projectiles;
using Terraria.World.Generation;
using Terraria.GameContent.Generation;
using CalamityMod.Tiles;
using CalamityMod;

namespace CalamityMod.NPCs.TheDevourerofGods
{
	[AutoloadBossHead]
	public class DevourerofGodsHeadS : ModNPC
	{
        private bool tail = false;
        private const int minLength = 120;
        private const int maxLength = 121;
        private bool halfLife = false;
        private int flameTimer = 900;
        private int laserShoot = 0;
        private float phaseSwitch = 0f;
        private float[] shotSpacing = new float[4] { 1000f, 1000f, 1000f, 1000f };
        private const float spacingVar = 100;
        private const int totalShots = 20;
        private int idleCounter = (CalamityWorld.death || CalamityWorld.bossRushActive) ? 540 : 360;
        private double damageTaken = 0.0;

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Devourer of Gods");
		}
		
		public override void SetDefaults()
		{
			npc.damage = 300; //150
			npc.npcSlots = 5f;
			npc.width = 100; //130
			npc.height = 144; //150
			npc.defense = 0;
			npc.lifeMax = CalamityWorld.revenge ? 937500 : 825000; //720000 672000
			if (CalamityWorld.death)
			{
				npc.lifeMax = 1530000;
			}
			if (CalamityWorld.bossRushActive)
			{
				npc.lifeMax = CalamityWorld.death ? 5000000 : 4600000;
			}
			npc.takenDamageMultiplier = CalamityWorld.bossRushActive ? 1.5f : 1.25f;
			npc.aiStyle = -1; //new
            aiType = -1; //new
            animationType = 10; //new
			npc.knockBackResist = 0f;
			npc.scale = 1.1f;
			npc.boss = true;
			npc.value = Item.buyPrice(1, 0, 0, 0);
			npc.alpha = 255;
			npc.behindTiles = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath14;
            npc.netAlways = true;
			for (int k = 0; k < npc.buffImmune.Length; k++)
			{
				npc.buffImmune[k] = true;
			}
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/UniversalCollapse");
            else
                music = MusicID.LunarBoss;
            bossBag = mod.ItemType("DevourerofGodsBag");
		}

        public override void BossHeadRotation(ref float rotation)
        {
            rotation = npc.rotation;
        }

        public override void AI()
		{
            CalamityGlobalNPC.DoGHead = npc.whoAmI;
			Vector2 vector = npc.Center;
			bool flies = npc.ai[2] == 0f;
			bool expertMode = Main.expertMode;
			bool speedBoost2 = (double)npc.life <= (double)npc.lifeMax * 0.6 || (CalamityWorld.bossRushActive && (double)npc.life <= (double)npc.lifeMax * 0.9); //speed increase
			bool speedBoost4 = (double)npc.life <= (double)npc.lifeMax * 0.2 && !CalamityWorld.bossRushActive; //speed increase
            bool breathFireMore = (double)npc.life <= (double)npc.lifeMax * 0.1;
            if (speedBoost2 && !speedBoost4)
            {
                if (npc.localAI[3] == 0f)
                {
                    if (Main.netMode != 1)
                    {
                        npc.localAI[2] += 1f;
                        if (npc.localAI[2] >= 720f)
                        {
                            npc.localAI[2] = 0f;
                            npc.localAI[3] = 1f;
                            npc.netUpdate = true;
                        }
                    }
                }
                else if (npc.localAI[3] == 1f)
                {
                    //npc.damage = 0;
                    npc.alpha += 4;
                    if (npc.alpha == 204) //255
                    {
                        laserShoot = 0;
                    }
                    if (npc.alpha >= 204) //255
                    {
                        npc.alpha = 204; //255
                        idleCounter--;
                        if (idleCounter <= 0)
                        {
                            npc.localAI[3] = 2f;
                            idleCounter = (CalamityWorld.death || CalamityWorld.bossRushActive) ? 540 : 360;
                            npc.netUpdate = true;
                        }
                    }
                }
                else if (npc.localAI[3] == 2f)
                {
                    npc.alpha -= 1;
                    if (npc.alpha <= 0)
                    {
                        if (flameTimer < 270)
                        {
                            flameTimer = 270;
                        }
                        //npc.damage = expertMode ? 480 : 300;
                        npc.alpha = 0;
                        npc.localAI[3] = 0f;
                        npc.netUpdate = true;
                    }
                }
            }
            else
            {
                npc.alpha -= 6;
                if (npc.alpha < 0)
                {
                    npc.alpha = 0;
                }
                if (npc.localAI[3] > 0f)
                {
                    //npc.damage = expertMode ? 480 : 300;
                    npc.localAI[3] = 0f;
                    npc.netUpdate = true;
                }
            }
			if (npc.alpha <= 0)
			{
				npc.damage = expertMode ? 480 : 300;
			}
			else
			{
				npc.damage = 0;
			}
            if (speedBoost4)
			{
				if (!halfLife)
				{
					string key = "Mods.CalamityMod.EdgyBossText11";
					Color messageColor = Color.Cyan;
					if (Main.netMode == 0)
					{
						Main.NewText(Language.GetTextValue(key), messageColor);
					}
					else if (Main.netMode == 2)
					{
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
					}
					halfLife = true;
				}
			}
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
            if (Main.netMode != 1)
            {
                if (!tail && npc.ai[0] == 0f)
                {
                    int Previous = npc.whoAmI;
                    for (int segmentSpawn = 0; segmentSpawn < maxLength; segmentSpawn++)
                    {
                        int segment = 0;
                        if (segmentSpawn >= 0 && segmentSpawn < minLength)
                        {
                            segment = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("DevourerofGodsBodyS"), npc.whoAmI);
                        }
                        else
                        {
                            segment = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), mod.NPCType("DevourerofGodsTailS"), npc.whoAmI);
                        }
                        Main.npc[segment].realLife = npc.whoAmI;
                        Main.npc[segment].ai[2] = (float)npc.whoAmI;
                        Main.npc[segment].ai[1] = (float)Previous;
                        Main.npc[Previous].ai[0] = (float)segment;
                        NetMessage.SendData(23, -1, -1, null, segment, 0f, 0f, 0f, 0);
                        Previous = segment;
                    }
                    tail = true;
                }
                int projectileDamage = expertMode ? 69 : 80;
                if (npc.alpha <= 0 && (CalamityWorld.revenge || CalamityWorld.bossRushActive))
                {
                    if (flameTimer <= 0)
                    {
                        flameTimer = 900;
                    }
                    else
                    {
                        flameTimer--;
                        float num861 = 4f;
                        int num863 = 1;
                        if (npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)Main.player[npc.target].width)
                        {
                            num863 = -1;
                        }
                        Vector2 vector86 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                        float num864 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) + (float)(num863 * 180) - vector86.X;
                        float num865 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector86.Y;
                        float num866 = (float)Math.Sqrt((double)(num864 * num864 + num865 * num865));
                        num866 = num861 / num866;
                        num864 *= num866;
                        num865 *= num866;
                        if (breathFireMore)
                        {
                            if (flameTimer <= 810 && flameTimer > 630)
                            {
                                if (npc.soundDelay == 0)
                                {
                                    npc.soundDelay = 21;
                                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 109);
                                }
                                if (npc.soundDelay % 3 == 0)
                                {
                                    float num867 = 1f;
                                    int num869 = mod.ProjectileType("DoGFire");
                                    vector86 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                                    num864 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector86.X;
                                    num865 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector86.Y;
                                    num866 = (float)Math.Sqrt((double)(num864 * num864 + num865 * num865));
                                    num866 = num867 / num866;
                                    num864 *= num866;
                                    num865 *= num866;
                                    num865 += (float)Main.rand.Next(-10, 11) * 0.01f;
                                    num864 += (float)Main.rand.Next(-10, 11) * 0.01f;
                                    num865 += npc.velocity.Y * 0.75f;
                                    num864 += npc.velocity.X * 0.75f;
                                    vector86.X -= num864 * 1f;
                                    vector86.Y -= num865 * 1f;
                                    int damage = expertMode ? 56 : 64;
                                    Projectile.NewProjectile(vector86.X, vector86.Y, num864, num865, num869, damage, 0f, Main.myPlayer, 0f, 1f);
                                }
                            }
                            else if (flameTimer <= 630)
                            {
                                if (npc.soundDelay == 0)
                                {
                                    npc.soundDelay = 21;
                                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 109);
                                }
                                if (npc.soundDelay % 3 == 0)
                                {
                                    float num867 = 1f;
                                    int num869 = mod.ProjectileType("DoGFire");
                                    vector86 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                                    num864 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector86.X;
                                    num865 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector86.Y;
                                    num866 = (float)Math.Sqrt((double)(num864 * num864 + num865 * num865));
                                    num866 = num867 / num866;
                                    num864 *= num866;
                                    num865 *= num866;
                                    num865 += (float)Main.rand.Next(-10, 11) * 0.01f;
                                    num864 += (float)Main.rand.Next(-10, 11) * 0.01f;
                                    num865 += npc.velocity.Y * 0.75f;
                                    num864 += npc.velocity.X * 0.75f;
                                    vector86.X -= num864 * 1f;
                                    vector86.Y -= num865 * 1f;
                                    int damage = expertMode ? 56 : 64;
                                    Projectile.NewProjectile(vector86.X, vector86.Y, num864, num865, num869, damage, 0f, Main.myPlayer, 0f, 2f);
                                }
                            }
                        }
                        else
                        {
                            if (flameTimer <= 270 && flameTimer > 90)
                            {
                                if (npc.soundDelay == 0)
                                {
                                    npc.soundDelay = 21;
                                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 109);
                                }
                                if (npc.soundDelay % 3 == 0)
                                {
                                    float num867 = 1f;
                                    int num869 = mod.ProjectileType("DoGFire");
                                    vector86 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                                    num864 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector86.X;
                                    num865 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector86.Y;
                                    num866 = (float)Math.Sqrt((double)(num864 * num864 + num865 * num865));
                                    num866 = num867 / num866;
                                    num864 *= num866;
                                    num865 *= num866;
                                    num865 += (float)Main.rand.Next(-10, 11) * 0.01f;
                                    num864 += (float)Main.rand.Next(-10, 11) * 0.01f;
                                    num865 += npc.velocity.Y * 0.75f;
                                    num864 += npc.velocity.X * 0.75f;
                                    vector86.X -= num864 * 1f;
                                    vector86.Y -= num865 * 1f;
                                    int damage = expertMode ? 56 : 64;
                                    Projectile.NewProjectile(vector86.X, vector86.Y, num864, num865, num869, damage, 0f, Main.myPlayer, 0f, 1f);
                                }
                            }
                            else if (flameTimer <= 90)
                            {
                                if (npc.soundDelay == 0)
                                {
                                    npc.soundDelay = 21;
                                    Main.PlaySound(2, (int)npc.position.X, (int)npc.position.Y, 109);
                                }
                                if (npc.soundDelay % 3 == 0)
                                {
                                    float num867 = 1f;
                                    int num869 = mod.ProjectileType("DoGFire");
                                    vector86 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                                    num864 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector86.X;
                                    num865 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector86.Y;
                                    num866 = (float)Math.Sqrt((double)(num864 * num864 + num865 * num865));
                                    num866 = num867 / num866;
                                    num864 *= num866;
                                    num865 *= num866;
                                    num865 += (float)Main.rand.Next(-10, 11) * 0.01f;
                                    num864 += (float)Main.rand.Next(-10, 11) * 0.01f;
                                    num865 += npc.velocity.Y * 0.75f;
                                    num864 += npc.velocity.X * 0.75f;
                                    vector86.X -= num864 * 1f;
                                    vector86.Y -= num865 * 1f;
                                    int damage = expertMode ? 56 : 64;
                                    Projectile.NewProjectile(vector86.X, vector86.Y, num864, num865, num869, damage, 0f, Main.myPlayer, 0f, 0f);
                                }
                            }
                        }
                    }
                }
                if (!speedBoost4 && (npc.localAI[3] == 1f || npc.GetGlobalNPC<CalamityGlobalNPC>(mod).enraged))
                {
                    laserShoot += 3;
                    if (laserShoot >= 2400)
                    {
                        laserShoot = 0;
                    }
                    float speed = (CalamityWorld.bossRushActive ? 4.5f : 4f);
                    if (laserShoot % (CalamityWorld.bossRushActive ? 210 : (CalamityWorld.death ? 240 : 300)) == 0) //300 600 900 1200 1500 1800 2100 2400
                    {
                        Main.PlaySound(2, (int)Main.player[npc.target].position.X, (int)Main.player[npc.target].position.Y, 12);
                        float targetPosY = Main.player[npc.target].position.Y + (Main.rand.Next(2) == 0 ? 50f : 0f);
                        int extraLasers = Main.rand.Next(2);
                        for (int x = 0; x < totalShots; x++)
                        {
                            Projectile.NewProjectile(Main.player[npc.target].position.X + 1000f, targetPosY + this.shotSpacing[0], -speed, 0f, mod.ProjectileType("DoGDeath"), projectileDamage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(Main.player[npc.target].position.X - 1000f, targetPosY + this.shotSpacing[0], speed, 0f, mod.ProjectileType("DoGDeath"), projectileDamage, 0f, Main.myPlayer, 0f, 0f);
                            this.shotSpacing[0] -= spacingVar; //100
                        }
                        if (extraLasers == 1 && (CalamityWorld.revenge || CalamityWorld.bossRushActive))
                        {
                            for (int x = 0; x < 10; x++)
                            {
                                Projectile.NewProjectile(Main.player[npc.target].position.X + 1000f, targetPosY + this.shotSpacing[3], -speed, 0f, mod.ProjectileType("DoGDeath"), projectileDamage, 0f, Main.myPlayer, 0f, 0f);
                                Projectile.NewProjectile(Main.player[npc.target].position.X - 1000f, targetPosY + this.shotSpacing[3], speed, 0f, mod.ProjectileType("DoGDeath"), projectileDamage, 0f, Main.myPlayer, 0f, 0f);
                                this.shotSpacing[3] -= (Main.rand.Next(2) == 0 ? 170f : 190f); //100
                            }
                            this.shotSpacing[3] = 1000f;
                        }
                        this.shotSpacing[0] = 1000f;
                    }
                    if (laserShoot % (CalamityWorld.bossRushActive ? 300 : (CalamityWorld.death ? 360 : 450)) == 0) //480 960 1440 1920 2400
                    {
                        for (int x = 0; x < totalShots; x++)
                        {
                            Projectile.NewProjectile(Main.player[npc.target].position.X + this.shotSpacing[1], Main.player[npc.target].position.Y + 1000f, 0f, -speed, mod.ProjectileType("DoGDeath"), projectileDamage, 0f, Main.myPlayer, 0f, 0f);
                            this.shotSpacing[1] -= spacingVar; //100
                        }
                        this.shotSpacing[1] = 1000f;
                    }
                    if (laserShoot % (CalamityWorld.bossRushActive ? 420 : (CalamityWorld.death ? 480 : 600)) == 0) //600 1200 1800 2400
                    {
                        for (int x = 0; x < totalShots; x++)
                        {
                            Projectile.NewProjectile(Main.player[npc.target].position.X + this.shotSpacing[2], Main.player[npc.target].position.Y - 1000f, 0f, speed, mod.ProjectileType("DoGDeath"), projectileDamage, 0f, Main.myPlayer, 0f, 0f);
                            this.shotSpacing[2] -= spacingVar; //100
                        }
                        this.shotSpacing[2] = 1000f;
                    }
                }
            }
            if (!NPC.AnyNPCs(mod.NPCType("DevourerofGodsTailS")))
            {
                npc.active = false;
            }
            float fallSpeed = 16f;
			if (Main.player[npc.target].dead)
			{
				flies = false;
                npc.velocity.Y = npc.velocity.Y + 2f;
                if ((double)npc.position.Y > Main.worldSurface * 16.0)
                {
                    npc.velocity.Y = npc.velocity.Y + 2f;
                    fallSpeed = 32f;
                }
                if ((double)npc.position.Y > Main.rockLayer * 16.0)
				{
                    for (int a = 0; a < 200; a++)
					{
						if (Main.npc[a].type == mod.NPCType("DevourerofGodsHeadS") || Main.npc[a].type == mod.NPCType("DevourerofGodsBodyS") ||
                            Main.npc[a].type == mod.NPCType("DevourerofGodsTailS"))
						{
							Main.npc[a].active = false;
                        }
					}
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
            if (npc.velocity.X < 0f)
            {
                npc.spriteDirection = -1;
            }
            else if (npc.velocity.X > 0f)
            {
                npc.spriteDirection = 1;
            }
            if (npc.ai[2] == 0f)
			{
				if (Main.netMode != 2)
				{
                    if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < 5600f)
                    {
                        Main.player[Main.myPlayer].AddBuff(mod.BuffType("Warped"), 2);
                    }
                }
				phaseSwitch += 1f;
				npc.localAI[1] = 0f;
                float speed = 15f;
                float turnSpeed = 0.4f;
                float homingSpeed = 24f;
                float homingTurnSpeed = 0.5f;
                if (Vector2.Distance(Main.player[npc.target].Center, vector) > 5600f) //RAGE
                {
                    phaseSwitch += 9f;
                }
                float num188 = speed;
				float num189 = turnSpeed;
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
				if (num42 > 0)
				{
					num42 *= 16;
					float num47 = (float)(num42 - 800);
					if (Main.player[npc.target].position.Y > num47)
					{
						num192 = num47;
						if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < 500f)
						{
							if (npc.velocity.X > 0f)
							{
								num191 = Main.player[npc.target].Center.X + 600f;
							}
							else
							{
								num191 = Main.player[npc.target].Center.X - 600f;
							}
						}
					}
				}
                else
                {
                    num188 = homingSpeed;
                    num189 = homingTurnSpeed;
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
				if (num42 > 0)
				{
					for (int num51 = 0; num51 < 200; num51++)
					{
						if (Main.npc[num51].active && Main.npc[num51].type == npc.type && num51 != npc.whoAmI)
						{
							Vector2 vector3 = Main.npc[num51].Center - npc.Center;
							if (vector3.Length() < 400f)
							{
								vector3.Normalize();
								vector3 *= 1000f;
								num191 -= vector3.X;
								num192 -= vector3.Y;
							}
						}
					}
				}
				else
				{
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
                    if (num191 < 0f)
                    {
                        npc.spriteDirection = -1;
                    }
                    else if (num191 > 0f)
                    {
                        npc.spriteDirection = 1;
                    }
                }
				else
				{
                    if (npc.velocity.Y > fallSpeed * 0.5f)
                    {
                        npc.velocity.Y = fallSpeed * 0.5f;
                    }
                    if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)fallSpeed * 0.3)
                    {
                        if (npc.velocity.X < 0f)
                        {
                            npc.velocity.X = npc.velocity.X - num189 * 1.1f;
                        }
                        else
                        {
                            npc.velocity.X = npc.velocity.X + num189 * 1.1f;
                        }
                    }
                    else if (npc.velocity.Y == fallSpeed)
                    {
                        if (npc.velocity.X < num191)
                        {
                            npc.velocity.X = npc.velocity.X + num189;
                        }
                        else if (npc.velocity.X > num191)
                        {
                            npc.velocity.X = npc.velocity.X - num189;
                        }
                    }
                    else if (npc.velocity.Y > 4f)
                    {
                        if (npc.velocity.X < 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num189 * 0.9f;
                        }
                        else
                        {
                            npc.velocity.X = npc.velocity.X - num189 * 0.9f;
                        }
                    }
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
				if (phaseSwitch > ((CalamityWorld.death || CalamityWorld.bossRushActive) ? 600f : 900f))
				{
					npc.ai[2] = 1f;
					phaseSwitch = 0f;
					npc.netUpdate = true;
					return;
				}
			}
			else if (npc.ai[2] == 1f)
			{
				if (Main.netMode != 2)
				{
                    if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < 5600f)
                    {
                        Main.player[Main.myPlayer].AddBuff(mod.BuffType("ExtremeGrav"), 2);
                    }
                }
				phaseSwitch += 1f;
                float turnSpeed = 0.3f;
                bool increaseSpeed = Vector2.Distance(Main.player[npc.target].Center, vector) > 3200f;
                if (Vector2.Distance(Main.player[npc.target].Center, vector) > 5600f) //RAGE
                {
                    turnSpeed = 1.5f;
                }
                else if (increaseSpeed)
                {
                    turnSpeed = 1f;
                }
                if (!flies)
				{
					for (int num952 = num180; num952 < num181; num952++)
					{
						for (int num953 = num182; num953 < num183; num953++)
						{
							if (Main.tile[num952, num953] != null && ((Main.tile[num952, num953].nactive() && (Main.tileSolid[(int)Main.tile[num952, num953].type] || (Main.tileSolidTop[(int)Main.tile[num952, num953].type] && Main.tile[num952, num953].frameY == 0))) || Main.tile[num952, num953].liquid > 64))
							{
								Vector2 vector105;
								vector105.X = (float)(num952 * 16);
								vector105.Y = (float)(num953 * 16);
								if (npc.position.X + (float)npc.width > vector105.X && npc.position.X < vector105.X + 16f && npc.position.Y + (float)npc.height > vector105.Y && npc.position.Y < vector105.Y + 16f)
								{
									flies = true;
									break;
								}
							}
						}
					}
				}
				if (!flies)
				{
					npc.localAI[1] = 1f;
					Rectangle rectangle12 = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
					int num954 = 1200;
                    if ((double)npc.life <= (double)npc.lifeMax * 0.8 && (double)npc.life > (double)npc.lifeMax * 0.2)
                    {
                        num954 = 1400;
                    }
					bool flag95 = true;
					if (npc.position.Y > Main.player[npc.target].position.Y)
					{
						for (int num955 = 0; num955 < 255; num955++)
						{
							if (Main.player[num955].active)
							{
								Rectangle rectangle13 = new Rectangle((int)Main.player[num955].position.X - 1000, (int)Main.player[num955].position.Y - 1000, 2000, num954);
								if (rectangle12.Intersects(rectangle13))
								{
									flag95 = false;
									break;
								}
							}
						}
						if (flag95)
						{
							flies = true;
						}
					}
				}
                else
                {
                    npc.localAI[1] = 0f;
                }
                float num189 = turnSpeed;
				Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
				float num191 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
				float num192 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
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
                    if (num191 < 0f)
                    {
                        npc.spriteDirection = -1;
                    }
                    else if (num191 > 0f)
                    {
                        npc.spriteDirection = 1;
                    }
                }
				else
				{
					if (!flies)
					{
						npc.TargetClosest(true);
                        npc.velocity.Y = npc.velocity.Y + turnSpeed; //turnspeed * 0.5f
						if (npc.velocity.Y > fallSpeed)
						{
							npc.velocity.Y = fallSpeed;
						}
						if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)fallSpeed * 2.2) //max speed
						{
							if (npc.velocity.X < 0f)
							{
								npc.velocity.X = npc.velocity.X - num189 * 1.1f;
							}
							else
							{
								npc.velocity.X = npc.velocity.X + num189 * 1.1f;
							}
						}
						else if (npc.velocity.Y == fallSpeed)
						{
							if (npc.velocity.X < num191)
							{
								npc.velocity.X = npc.velocity.X + num189;
							}
							else if (npc.velocity.X > num191)
							{
								npc.velocity.X = npc.velocity.X - num189;
							}
						}
						else if (npc.velocity.Y > 4f)
						{
							if (npc.velocity.X < 0f)
							{
								npc.velocity.X = npc.velocity.X + num189 * 0.9f;
							}
							else
							{
								npc.velocity.X = npc.velocity.X - num189 * 0.9f;
							}
						}
					}
					else
					{
                        double maximumSpeed1 = increaseSpeed ? 1.2 : 0.4;
                        double maximumSpeed2 = increaseSpeed ? 3.0 : 1.0;
                        num193 = (float)Math.Sqrt((double)(num191 * num191 + num192 * num192));
                        float num25 = Math.Abs(num191);
                        float num26 = Math.Abs(num192);
                        float num27 = fallSpeed / num193;
                        num191 *= num27;
                        num192 *= num27;
                        if (((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f)) && ((npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f)))
                        {
                            if (npc.velocity.X < num191)
                            {
                                npc.velocity.X = npc.velocity.X + turnSpeed * 1.5f;
                            }
                            else if (npc.velocity.X > num191)
                            {
                                npc.velocity.X = npc.velocity.X - turnSpeed * 1.5f;
                            }
                            if (npc.velocity.Y < num192)
                            {
                                npc.velocity.Y = npc.velocity.Y + turnSpeed * 1.5f;
                            }
                            else if (npc.velocity.Y > num192)
                            {
                                npc.velocity.Y = npc.velocity.Y - turnSpeed * 1.5f;
                            }
                        }
                        if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
                        {
                            if (npc.velocity.X < num191)
                            {
                                npc.velocity.X = npc.velocity.X + turnSpeed;
                            }
                            else if (npc.velocity.X > num191)
                            {
                                npc.velocity.X = npc.velocity.X - turnSpeed;
                            }
                            if (npc.velocity.Y < num192)
                            {
                                npc.velocity.Y = npc.velocity.Y + turnSpeed;
                            }
                            else if (npc.velocity.Y > num192)
                            {
                                npc.velocity.Y = npc.velocity.Y - turnSpeed;
                            }
                            if ((double)Math.Abs(num192) < (double)fallSpeed * maximumSpeed1 /*0.2*/ && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
                            {
                                if (npc.velocity.Y > 0f)
                                {
                                    npc.velocity.Y = npc.velocity.Y + turnSpeed * 2f;
                                }
                                else
                                {
                                    npc.velocity.Y = npc.velocity.Y - turnSpeed * 2f;
                                }
                            }
                            if ((double)Math.Abs(num191) < (double)fallSpeed * maximumSpeed1 /*0.2*/ && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
                            {
                                if (npc.velocity.X > 0f)
                                {
                                    npc.velocity.X = npc.velocity.X + turnSpeed * 2f;
                                }
                                else
                                {
                                    npc.velocity.X = npc.velocity.X - turnSpeed * 2f;
                                }
                            }
                        }
                        else if (num25 > num26)
                        {
                            if (npc.velocity.X < num191)
                            {
                                npc.velocity.X = npc.velocity.X + turnSpeed * 1.1f;
                            }
                            else if (npc.velocity.X > num191)
                            {
                                npc.velocity.X = npc.velocity.X - turnSpeed * 1.1f;
                            }
                            if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)fallSpeed * maximumSpeed2) //0.5
                            {
                                if (npc.velocity.Y > 0f)
                                {
                                    npc.velocity.Y = npc.velocity.Y + turnSpeed;
                                }
                                else
                                {
                                    npc.velocity.Y = npc.velocity.Y - turnSpeed;
                                }
                            }
                        }
                        else
                        {
                            if (npc.velocity.Y < num192)
                            {
                                npc.velocity.Y = npc.velocity.Y + turnSpeed * 1.1f;
                            }
                            else if (npc.velocity.Y > num192)
                            {
                                npc.velocity.Y = npc.velocity.Y - turnSpeed * 1.1f;
                            }
                            if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)fallSpeed * maximumSpeed2) //0.5
                            {
                                if (npc.velocity.X > 0f)
                                {
                                    npc.velocity.X = npc.velocity.X + turnSpeed;
                                }
                                else
                                {
                                    npc.velocity.X = npc.velocity.X - turnSpeed;
                                }
                            }
                        }
                    }
				}
				npc.rotation = (float)System.Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
				if (flies)
				{
					if (npc.localAI[0] != 1f)
					{
						npc.netUpdate = true;
					}
					npc.localAI[0] = 1f;
				}
				else
				{
					if (npc.localAI[0] != 0f)
					{
						npc.netUpdate = true;
					}
					npc.localAI[0] = 0f;
				}
				if (phaseSwitch > ((CalamityWorld.death || CalamityWorld.bossRushActive) ? 600f : 900f))
				{
					npc.ai[2] = 0f;
					phaseSwitch = 0f;
					npc.netUpdate = true;
					return;
				}
				if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
				{
					npc.netUpdate = true;
					return;
				}
			}
		}
		
		public override void BossLoot(ref string name, ref int potionType)
		{
			potionType = mod.ItemType("CosmiliteBrick");
		}
		
		public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
            if (projectile.type == mod.ProjectileType("SulphuricAcidMist2"))
            {
                damage /= 8;
            }
        }
		
		public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
		{
			if (damage > npc.lifeMax / 2)
			{
				string key = "Mods.CalamityMod.EdgyBossText2";
				Color messageColor = Color.Cyan;
				if (Main.netMode == 0)
				{
					Main.NewText(Language.GetTextValue(key), messageColor);
				}
				else if (Main.netMode == 2)
				{
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}
				damage = 0;
				return false;
			}
            double multiplier = 1.0;
            damageTaken += (crit ? (damage * 2) : damage);
            damage = (int)((double)damage * multiplier);
            if (damageTaken >= 50000.0)
            {
                damage = (int)((double)damage * 0.5);
            }
            return true;
		}

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			scale = 2f;
			return null;
		}
		
		public override bool CheckActive()
		{
			return false;
		}
		
		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life <= 0)
			{
				Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DoGS"), 1f);
                Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/DoGS2"), 1f);
                npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 50;
				npc.height = 50;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 15; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 30; num623++)
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
			player.AddBuff(mod.BuffType("GodSlayerInferno"), 300, true);
			if ((CalamityWorld.death || CalamityWorld.bossRushActive) && npc.alpha <= 0)
			{
                player.KillMe(PlayerDeathReason.ByOther(10), 1000.0, 0, false);
            }
			int num = Main.rand.Next(5);
			string key = "Mods.CalamityMod.EdgyBossText3";
			if (num == 0)
			{
				key = "Mods.CalamityMod.EdgyBossText3";
			}
			else if (num == 1)
			{
				key = "Mods.CalamityMod.EdgyBossText4";
			}
			else if (num == 2)
			{
				key = "Mods.CalamityMod.EdgyBossText5";
			}
			else if (num == 3)
			{
				key = "Mods.CalamityMod.EdgyBossText6";
			}
			else if (num == 4)
			{
				key = "Mods.CalamityMod.EdgyBossText7";
			}
			Color messageColor = Color.Cyan;
			if (Main.netMode == 0)
			{
				Main.NewText(Language.GetTextValue(key), messageColor);
			}
			else if (Main.netMode == 2)
			{
				NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
			}
			player.AddBuff(BuffID.Frostburn, 300, true);
			player.AddBuff(BuffID.Darkness, 300, true);
		}
	}
}