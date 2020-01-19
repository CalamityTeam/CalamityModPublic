using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Placeables.FurnitureCosmilite;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DevourerofGods
{
    [AutoloadBossHead]
    public class DevourerofGodsHeadS : ModNPC
    {
        private bool tail = false;
        private const int minLength = 100;
        private const int maxLength = 101;
        private bool halfLife = false;
        private int laserShoot = 0;
        private int phaseSwitch = 0;
        private int[] shotSpacing = new int[4] { 1050, 1050, 1050, 1050 };
        private const int spacingVar = 105;
        private const int totalShots = 20;
        private int idleCounter = (CalamityWorld.death || CalamityWorld.bossRushActive) ? 540 : 360;
        public int laserWallPhase = 0;
        private int laserWallCounter = 0;
		private int postTeleportTimer = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Devourer of Gods");
        }

        public override void SetDefaults()
        {
            npc.damage = 300;
            npc.npcSlots = 5f;
            npc.width = 186;
            npc.height = 186;
            npc.defense = 50;
            npc.LifeMaxNERB(1150000, 1350000, 9200000);
            double HPBoost = Config.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.takenDamageMultiplier = 1.25f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.boss = true;
            npc.value = Item.buyPrice(1, 0, 0, 0);
            npc.alpha = 255;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
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
            bossBag = ModContent.ItemType<DevourerofGodsBag>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(npc.dontTakeDamage);
            writer.Write(halfLife);
            writer.Write(laserShoot);
            writer.Write(phaseSwitch);
            writer.Write(shotSpacing[0]);
            writer.Write(shotSpacing[1]);
            writer.Write(shotSpacing[2]);
            writer.Write(shotSpacing[3]);
            writer.Write(idleCounter);
            writer.Write(laserWallPhase);
            writer.Write(laserWallCounter);
			writer.Write(postTeleportTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			npc.dontTakeDamage = reader.ReadBoolean();
            halfLife = reader.ReadBoolean();
            laserShoot = reader.ReadInt32();
            phaseSwitch = reader.ReadInt32();
            shotSpacing[0] = reader.ReadInt32();
            shotSpacing[1] = reader.ReadInt32();
            shotSpacing[2] = reader.ReadInt32();
            shotSpacing[3] = reader.ReadInt32();
            idleCounter = reader.ReadInt32();
            laserWallPhase = reader.ReadInt32();
            laserWallCounter = reader.ReadInt32();
			postTeleportTimer = reader.ReadInt32();
        }

        public override void BossHeadRotation(ref float rotation)
        {
            rotation = npc.rotation;
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            // whoAmI variable
            CalamityGlobalNPC.DoGHead = npc.whoAmI;

            // Percent life remaining
            float lifeRatio = (float)npc.life / (float)npc.lifeMax;

            // Variables
            Vector2 vector = npc.Center;
            bool flies = npc.ai[2] == 0f;
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
			bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;
            bool speedBoost = lifeRatio < 0.6 || (death && lifeRatio < 0.9);
            bool speedBoost2 = lifeRatio < 0.2;
            bool breathFireMore = lifeRatio < 0.15 || death;

			// Light
			Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);

			// Worm shit again
			if (npc.ai[3] > 0f)
				npc.realLife = (int)npc.ai[3];

			// Target
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
				npc.TargetClosest(true);

			// Velocity
			npc.velocity.Length();

			float distanceFromTarget = Vector2.Distance(Main.player[npc.target].Center, vector);
			bool tooFarAway = distanceFromTarget > 5600f;

			// Immunity after teleport
			npc.dontTakeDamage = postTeleportTimer > 0;

			// Laser walls
			if (speedBoost && !speedBoost2 && postTeleportTimer <= 0)
            {
                if (laserWallPhase == 0) // Start laser wall phase
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        laserWallCounter += 1;
                        if (laserWallCounter >= 720)
                        {
                            laserWallCounter = 0;
                            laserWallPhase = 1;
                        }
                    }
                }
                else if (laserWallPhase == 1) // Turn invisible and fire laser walls
                {
                    npc.alpha += 5;
                    if (npc.alpha == 255)
                        laserShoot = 0;

                    if (npc.alpha >= 255)
                    {
                        npc.alpha = 255;
                        idleCounter--;
                        if (idleCounter <= 0)
                        {
                            laserWallPhase = 2;
                            idleCounter = death ? 540 : 360;
                        }
                    }
                }
                else if (laserWallPhase == 2) // Turn visible
                {
					if (distanceFromTarget > 500f && revenge)
						Teleport();
					else
					{
						npc.alpha -= 1;
						if (npc.alpha <= 0)
						{
							npc.alpha = 0;
							laserWallPhase = 0;
						}
					}
                }
            }
            else
            {
				if (postTeleportTimer > 0)
				{
					postTeleportTimer -= 4;
					if (postTeleportTimer < 0)
						postTeleportTimer = 0;

					npc.alpha = postTeleportTimer;
				}
				else
				{
					npc.alpha -= 6;
					if (npc.alpha < 0)
						npc.alpha = 0;
				}

                if (laserWallPhase > 0)
                    laserWallPhase = 0;
            }

			// Anger message
			if (speedBoost2)
            {
                if (!halfLife)
                {
                    string key = "Mods.CalamityMod.EdgyBossText11";
                    Color messageColor = Color.Cyan;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    else if (Main.netMode == NetmodeID.Server)
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);

                    halfLife = true;
                }
            }

            // Spawn segments and fire projectiles
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // Segments
                if (!tail && npc.ai[0] == 0f)
                {
                    int Previous = npc.whoAmI;
                    for (int segmentSpawn = 0; segmentSpawn < maxLength; segmentSpawn++)
                    {
                        int segment;
                        if (segmentSpawn >= 0 && segmentSpawn < minLength)
                            segment = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<DevourerofGodsBodyS>(), npc.whoAmI);
                        else
                            segment = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<DevourerofGodsTailS>(), npc.whoAmI);

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

                // Fireballs
                if (npc.alpha <= 0 && distanceFromTarget > 500f && expertMode)
                {
                    calamityGlobalNPC.newAI[0] += 1f;
                    if (calamityGlobalNPC.newAI[0] >= 150f && calamityGlobalNPC.newAI[0] % (breathFireMore ? 60f : 120f) == 0f)
                    {
                        Vector2 vector44 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                        float num427 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector44.X;
                        float num428 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector44.Y;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float num430 = 16f;
                            int num432 = ModContent.ProjectileType<DoGFire>();

                            float num429 = (float)Math.Sqrt((double)(num427 * num427 + num428 * num428));
                            num429 = num430 / num429;
                            num427 *= num429;
                            num428 *= num429;
                            num428 += npc.velocity.Y * 0.5f;
                            num427 += npc.velocity.X * 0.5f;
                            vector44.X -= num427 * 1f;
                            vector44.Y -= num428 * 1f;

                            Projectile.NewProjectile(vector44.X, vector44.Y, num427, num428, num432, projectileDamage, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
                else if (distanceFromTarget < 250f)
                    calamityGlobalNPC.newAI[0] = 0f;

                // Laser walls
                if (!speedBoost2 && (laserWallPhase == 1 || calamityGlobalNPC.enraged > 0 || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive)))
                {
                    laserShoot += 1;

                    float speed = 4f;
                    int divisor = CalamityWorld.bossRushActive ? 90 : 120;

					if (laserShoot % divisor == 0)
					{
						Main.PlaySound(2, (int)Main.player[npc.target].position.X, (int)Main.player[npc.target].position.Y, 12);

						float targetPosY = Main.player[npc.target].position.Y + (Main.rand.NextBool(2) ? 50f : 0f);

						// Side walls
						for (int x = 0; x < totalShots; x++)
						{
							Projectile.NewProjectile(Main.player[npc.target].position.X + 1000f, targetPosY + (float)shotSpacing[0], -speed, 0f, ModContent.ProjectileType<DoGDeath>(), projectileDamage, 0f, Main.myPlayer, 0f, 0f);
							Projectile.NewProjectile(Main.player[npc.target].position.X - 1000f, targetPosY + (float)shotSpacing[0], speed, 0f, ModContent.ProjectileType<DoGDeath>(), projectileDamage, 0f, Main.myPlayer, 0f, 0f);
							shotSpacing[0] -= spacingVar;
						}

						if (Main.rand.NextBool(2) && revenge)
						{
							for (int x = 0; x < 10; x++)
							{
								Projectile.NewProjectile(Main.player[npc.target].position.X + 1000f, targetPosY + (float)shotSpacing[3], -speed, 0f, ModContent.ProjectileType<DoGDeath>(), projectileDamage, 0f, Main.myPlayer, 0f, 0f);
								Projectile.NewProjectile(Main.player[npc.target].position.X - 1000f, targetPosY + (float)shotSpacing[3], speed, 0f, ModContent.ProjectileType<DoGDeath>(), projectileDamage, 0f, Main.myPlayer, 0f, 0f);
								shotSpacing[3] -= Main.rand.NextBool(2) ? 180 : 200;
							}
							shotSpacing[3] = 1050;
						}
						shotSpacing[0] = 1050;

						// Lower wall
						for (int x = 0; x < totalShots; x++)
						{
							Projectile.NewProjectile(Main.player[npc.target].position.X + (float)shotSpacing[1], Main.player[npc.target].position.Y + 1000f, 0f, -speed, ModContent.ProjectileType<DoGDeath>(), projectileDamage, 0f, Main.myPlayer, 0f, 0f);
							shotSpacing[1] -= spacingVar;
						}
						shotSpacing[1] = 1050;

						// Upper wall
						for (int x = 0; x < totalShots; x++)
						{
							Projectile.NewProjectile(Main.player[npc.target].position.X + (float)shotSpacing[2], Main.player[npc.target].position.Y - 1000f, 0f, speed, ModContent.ProjectileType<DoGDeath>(), projectileDamage, 0f, Main.myPlayer, 0f, 0f);
							shotSpacing[2] -= spacingVar;
						}
						shotSpacing[2] = 1050;
					}
                }
            }

            // Despawn
            if (!NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsTailS>()))
                npc.active = false;

            float fallSpeed = 16f;
            if (Main.player[npc.target].dead)
            {
                flies = true;
                npc.velocity.Y = npc.velocity.Y - 3f;
                if ((double)npc.position.Y < Main.topWorld + 16f)
                {
                    npc.velocity.Y = npc.velocity.Y - 3f;
                    fallSpeed = 32f;
                }
                if ((double)npc.position.Y < Main.topWorld + 16f)
                {
                    for (int a = 0; a < 200; a++)
                    {
                        if (Main.npc[a].type == ModContent.NPCType<DevourerofGodsHeadS>() || Main.npc[a].type == ModContent.NPCType<DevourerofGodsBodyS>() || Main.npc[a].type == ModContent.NPCType<DevourerofGodsTailS>())
                            Main.npc[a].active = false;
                    }
                }
            }
            fallSpeed += death ? 3.7f : 3.5f * (1f - lifeRatio);

            // Movement
            int num180 = (int)(npc.position.X / 16f) - 1;
            int num181 = (int)((npc.position.X + (float)npc.width) / 16f) + 2;
            int num182 = (int)(npc.position.Y / 16f) - 1;
            int num183 = (int)((npc.position.Y + (float)npc.height) / 16f) + 2;

            if (num180 < 0)
                num180 = 0;
            if (num181 > Main.maxTilesX)
                num181 = Main.maxTilesX;
            if (num182 < 0)
                num182 = 0;
            if (num183 > Main.maxTilesY)
                num183 = Main.maxTilesY;

            if (npc.velocity.X < 0f)
                npc.spriteDirection = -1;
            else if (npc.velocity.X > 0f)
                npc.spriteDirection = 1;

            if (Main.player[npc.target].dead)
                npc.TargetClosest(false);

            // Flight
            if (npc.ai[2] == 0f)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < 5600f)
                        Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<Warped>(), 2);
                }

                phaseSwitch += 1;

                int phaseLimit = death ? 180 : 900 / (1 + (int)(5f * (1f - lifeRatio)));

                npc.localAI[1] = 0f;

                float speed = death ? 20f : 15f + (3f * (1f - lifeRatio));
                float turnSpeed = death ? 0.38f : 0.3f + (0.06f * (1f - lifeRatio));
                float homingSpeed = death ? 38f : 24f + (12f * (1f - lifeRatio));
                float homingTurnSpeed = death ? 0.5f : 0.33f + (0.15f * (1f - lifeRatio));

				// Go to ground phase sooner
				if (tooFarAway)
				{
					if (revenge && laserWallPhase == 0)
						Teleport();
					else
						phaseSwitch += 10;
				}

				float num188 = speed;
                float num189 = turnSpeed;
                Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num191 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
                float num192 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
                int num42 = -1;
                int num43 = (int)(Main.player[npc.target].Center.X / 16f);
                int num44 = (int)(Main.player[npc.target].Center.Y / 16f);

                // Charge at target for 1.5 seconds
                bool flyAtTarget = (!speedBoost || speedBoost2) && phaseSwitch > phaseLimit - 90 && revenge;

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
                        break;
                }

                if (!flyAtTarget)
                {
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
                                    num191 = Main.player[npc.target].Center.X + 600f;
                                else
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

                if (!flyAtTarget)
                {
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
                float num196 = System.Math.Abs(num191);
                float num197 = System.Math.Abs(num192);
                float num198 = num188 / num193;
                num191 *= num198;
                num192 *= num198;

                if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
                {
                    if (npc.velocity.X < num191)
                        npc.velocity.X = npc.velocity.X + num189;
                    else
                    {
                        if (npc.velocity.X > num191)
                            npc.velocity.X = npc.velocity.X - num189;
                    }

                    if (npc.velocity.Y < num192)
                        npc.velocity.Y = npc.velocity.Y + num189;
                    else
                    {
                        if (npc.velocity.Y > num192)
                            npc.velocity.Y = npc.velocity.Y - num189;
                    }

                    if ((double)System.Math.Abs(num192) < (double)num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
                    {
                        if (npc.velocity.Y > 0f)
                            npc.velocity.Y = npc.velocity.Y + num189 * 2f;
                        else
                            npc.velocity.Y = npc.velocity.Y - num189 * 2f;
                    }

                    if ((double)System.Math.Abs(num191) < (double)num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
                    {
                        if (npc.velocity.X > 0f)
                            npc.velocity.X = npc.velocity.X + num189 * 2f;
                        else
                            npc.velocity.X = npc.velocity.X - num189 * 2f;
                    }
                }
                else
                {
                    if (num196 > num197)
                    {
                        if (npc.velocity.X < num191)
                            npc.velocity.X = npc.velocity.X + num189 * 1.1f;
                        else if (npc.velocity.X > num191)
                            npc.velocity.X = npc.velocity.X - num189 * 1.1f;

                        if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y = npc.velocity.Y + num189;
                            else
                                npc.velocity.Y = npc.velocity.Y - num189;
                        }
                    }
                    else
                    {
                        if (npc.velocity.Y < num192)
                            npc.velocity.Y = npc.velocity.Y + num189 * 1.1f;
                        else if (npc.velocity.Y > num192)
                            npc.velocity.Y = npc.velocity.Y - num189 * 1.1f;

                        if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)num188 * 0.5)
                        {
                            if (npc.velocity.X > 0f)
                                npc.velocity.X = npc.velocity.X + num189;
                            else
                                npc.velocity.X = npc.velocity.X - num189;
                        }
                    }
                }

                npc.rotation = (float)System.Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;

                if (phaseSwitch > phaseLimit)
                {
                    npc.ai[2] = 1f;
                    phaseSwitch = 0;
                    npc.netUpdate = true;
                }
            }

            // Ground
            else
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active && Vector2.Distance(Main.player[Main.myPlayer].Center, vector) < 5600f)
                        Main.player[Main.myPlayer].AddBuff(ModContent.BuffType<ExtremeGrav>(), 2);
                }

                phaseSwitch += 1;

                float turnSpeed = 0.18f + (death ? 0.14f : 0.12f * (1f - lifeRatio));
                bool increaseSpeed = distanceFromTarget > 3200f;

				// Enrage
				if (tooFarAway)
				{
					if (revenge && laserWallPhase == 0)
						Teleport();
					else
						turnSpeed *= 6f;
				}
				else if (increaseSpeed)
					turnSpeed *= 3f;

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
                    if (lifeRatio < 0.8f && lifeRatio > 0.2f && !death)
                        num954 = 1400;

                    num954 -= death ? 150 : (int)(150f * (1f - lifeRatio));

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
                            flies = true;
                    }
                }
                else
                    npc.localAI[1] = 0f;

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

                if (!flies)
                {
                    npc.TargetClosest(true);

                    npc.velocity.Y = npc.velocity.Y + turnSpeed;
                    if (npc.velocity.Y > fallSpeed)
                        npc.velocity.Y = fallSpeed;

                    if ((double)(System.Math.Abs(npc.velocity.X) + System.Math.Abs(npc.velocity.Y)) < (double)fallSpeed * 2.2)
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X = npc.velocity.X - num189 * 1.1f;
                        else
                            npc.velocity.X = npc.velocity.X + num189 * 1.1f;
                    }
                    else if (npc.velocity.Y == fallSpeed)
                    {
                        if (npc.velocity.X < num191)
                            npc.velocity.X = npc.velocity.X + num189;
                        else if (npc.velocity.X > num191)
                            npc.velocity.X = npc.velocity.X - num189;
                    }
                    else if (npc.velocity.Y > 4f)
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X = npc.velocity.X + num189 * 0.9f;
                        else
                            npc.velocity.X = npc.velocity.X - num189 * 0.9f;
                    }
                }
                else
                {
                    double maximumSpeed1 = (increaseSpeed ? 1.2 : 0.4) + (double)(death ? 0.14f : 0.12f * (1f - lifeRatio));
                    double maximumSpeed2 = (increaseSpeed ? 3.0 : 1.0) + (double)(death ? 0.27f : 0.25f * (1f - lifeRatio));

                    num193 = (float)Math.Sqrt((double)(num191 * num191 + num192 * num192));
                    float num25 = Math.Abs(num191);
                    float num26 = Math.Abs(num192);
                    float num27 = fallSpeed / num193;
                    num191 *= num27;
                    num192 *= num27;

                    if (((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f)) && ((npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f)))
                    {
                        if (npc.velocity.X < num191)
                            npc.velocity.X = npc.velocity.X + turnSpeed * 1.5f;
                        else if (npc.velocity.X > num191)
                            npc.velocity.X = npc.velocity.X - turnSpeed * 1.5f;

                        if (npc.velocity.Y < num192)
                            npc.velocity.Y = npc.velocity.Y + turnSpeed * 1.5f;
                        else if (npc.velocity.Y > num192)
                            npc.velocity.Y = npc.velocity.Y - turnSpeed * 1.5f;
                    }

                    if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
                    {
                        if (npc.velocity.X < num191)
                            npc.velocity.X = npc.velocity.X + turnSpeed;
                        else if (npc.velocity.X > num191)
                            npc.velocity.X = npc.velocity.X - turnSpeed;

                        if (npc.velocity.Y < num192)
                            npc.velocity.Y = npc.velocity.Y + turnSpeed;
                        else if (npc.velocity.Y > num192)
                            npc.velocity.Y = npc.velocity.Y - turnSpeed;

                        if ((double)Math.Abs(num192) < (double)fallSpeed * maximumSpeed1 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y = npc.velocity.Y + turnSpeed * 2f;
                            else
                                npc.velocity.Y = npc.velocity.Y - turnSpeed * 2f;
                        }

                        if ((double)Math.Abs(num191) < (double)fallSpeed * maximumSpeed1 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
                        {
                            if (npc.velocity.X > 0f)
                                npc.velocity.X = npc.velocity.X + turnSpeed * 2f;
                            else
                                npc.velocity.X = npc.velocity.X - turnSpeed * 2f;
                        }
                    }
                    else if (num25 > num26)
                    {
                        if (npc.velocity.X < num191)
                            npc.velocity.X = npc.velocity.X + turnSpeed * 1.1f;
                        else if (npc.velocity.X > num191)
                            npc.velocity.X = npc.velocity.X - turnSpeed * 1.1f;

                        if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)fallSpeed * maximumSpeed2)
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y = npc.velocity.Y + turnSpeed;
                            else
                                npc.velocity.Y = npc.velocity.Y - turnSpeed;
                        }
                    }
                    else
                    {
                        if (npc.velocity.Y < num192)
                            npc.velocity.Y = npc.velocity.Y + turnSpeed * 1.1f;
                        else if (npc.velocity.Y > num192)
                            npc.velocity.Y = npc.velocity.Y - turnSpeed * 1.1f;

                        if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)fallSpeed * maximumSpeed2)
                        {
                            if (npc.velocity.X > 0f)
                                npc.velocity.X = npc.velocity.X + turnSpeed;
                            else
                                npc.velocity.X = npc.velocity.X - turnSpeed;
                        }
                    }
                }

                npc.rotation = (float)System.Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;

                if (flies)
                {
                    if (npc.localAI[0] != 1f)
                        npc.netUpdate = true;

                    npc.localAI[0] = 1f;
                }
                else
                {
                    if (npc.localAI[0] != 0f)
                        npc.netUpdate = true;

                    npc.localAI[0] = 0f;
                }

                if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
                    npc.netUpdate = true;

                if (phaseSwitch > (death ? 600 : 900))
                {
                    npc.ai[2] = 0f;
                    phaseSwitch = 0;
                    npc.netUpdate = true;
                }
            }
        }

		private void Teleport()
		{
			postTeleportTimer = 255;
			npc.alpha = postTeleportTimer;

			int playerWidth = Main.player[npc.target].width / 2;
			int playerHeight = Main.player[npc.target].height / 2;

			float playerVelocityX = Main.player[npc.target].velocity.X;
			float playerVelocityY = Main.player[npc.target].velocity.Y;

			int x = (int)Main.player[npc.target].position.X + playerWidth - 25;
			int y = (int)Main.player[npc.target].position.Y + playerHeight - 25;

			float velocityThreshold = 0.05f;
			int vectorAdjustment = 500;
			if (playerVelocityX >= velocityThreshold)
			{
				x += vectorAdjustment;
				yAdjustment();
			}
			else if (playerVelocityX <= -velocityThreshold)
			{
				x -= vectorAdjustment;
				yAdjustment();
			}

			void yAdjustment()
			{
				if (playerVelocityY >= velocityThreshold)
					y += vectorAdjustment;
				else if (playerVelocityY <= -velocityThreshold)
					y -= vectorAdjustment;
			}

			int x2 = x + 50;
			int y2 = y + 50;

			float locationX = (float)Main.rand.Next(x, x2);
			float locationY = (float)Main.rand.Next(y, y2);
			Vector2 teleportLocation = new Vector2(locationX, locationY);

			npc.position = teleportLocation;
			npc.netUpdate = true;

			for (int i = 0; i < 200; i++)
			{
				if (Main.npc[i].active && (Main.npc[i].type == ModContent.NPCType<DevourerofGodsBodyS>() || Main.npc[i].type == ModContent.NPCType<DevourerofGodsTailS>()))
				{
					Main.npc[i].position = teleportLocation;
					Main.npc[i].netUpdate = true;
				}
			}

			Vector2 npcCenter = new Vector2(npc.position.X + (float)(npc.width / 2), npc.position.Y + (float)(npc.height / 2));
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LargeMechGaussRifle"), (int)npcCenter.X, (int)npcCenter.Y);

			int dustAmt = 50;
			int random = 5;

			for (int j = 0; j < 10; j++)
			{
				random += j * 2;
				int dustAmtSpawned = 0;
				int scale = random * 13;
				float dustPositionX = npcCenter.X - (float)(scale / 2);
				float dustPositionY = npcCenter.Y - (float)(scale / 2);
				while (dustAmtSpawned < dustAmt)
				{
					float dustVelocityX = (float)Main.rand.Next(-random, random);
					float dustVelocityY = (float)Main.rand.Next(-random, random);
					float dustVelocityScalar = (float)random * 2f;
					float dustVelocity = (float)Math.Sqrt((double)(dustVelocityX * dustVelocityX + dustVelocityY * dustVelocityY));
					dustVelocity = dustVelocityScalar / dustVelocity;
					dustVelocityX *= dustVelocity;
					dustVelocityY *= dustVelocity;
					int dust = Dust.NewDust(new Vector2(dustPositionX, dustPositionY), scale, scale, 173, 0f, 0f, 100, default, 5f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].position.X = teleportLocation.X;
					Main.dust[dust].position.Y = teleportLocation.Y;
					Main.dust[dust].position.X += (float)Main.rand.Next(-10, 11);
					Main.dust[dust].position.Y += (float)Main.rand.Next(-10, 11);
					Main.dust[dust].velocity.X = dustVelocityX;
					Main.dust[dust].velocity.Y = dustVelocityY;
					dustAmtSpawned++;
				}
			}
		}

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<CosmiliteBrick>();
        }

        public override bool SpecialNPCLoot()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(npc,
                ModContent.NPCType<DevourerofGodsHeadS>(),
                ModContent.NPCType<DevourerofGodsBodyS>(),
                ModContent.NPCType<DevourerofGodsTailS>());
            npc.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void NPCLoot()
        {
            DropHelper.DropBags(npc);

            DropHelper.DropItem(npc, ModContent.ItemType<SupremeHealingPotion>(), 8, 14);
            DropHelper.DropItemChance(npc, ModContent.ItemType<DevourerofGodsTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeDevourerofGods>(), true, !CalamityWorld.downedDoG);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedDoG, 6, 3, 2);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItem(npc, ModContent.ItemType<CosmiliteBar>(), 25, 35);
                DropHelper.DropItem(npc, ModContent.ItemType<CosmiliteBrick>(), 150, 250);

                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<Excelsus>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<EradicatorMelee>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<TheObliterator>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Deathwind>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Skullmasher>(), DropHelper.RareVariantDropRateInt);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Norfleet>(), DropHelper.RareVariantDropRateInt);
                DropHelper.DropItemChance(npc, ModContent.ItemType<DeathhailStaff>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<StaffoftheMechworm>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<Eradicator>(), 4);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<DevourerofGodsMask>(), 7);
            }

            // If DoG has not been killed yet, notify players that the holiday moons are buffed
            if (!CalamityWorld.downedDoG)
            {
                string key = "Mods.CalamityMod.DoGBossText";
                Color messageColor = Color.Cyan;
                string key2 = "Mods.CalamityMod.DoGBossText2";
                Color messageColor2 = Color.Orange;

                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                    Main.NewText(Language.GetTextValue(key2), messageColor2);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor2);
                }
            }

            // Mark DoG as dead
            CalamityWorld.downedDoG = true;
            CalamityMod.UpdateServerBoolean();
        }

        // Can only hit the target if within certain distance
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;

            Rectangle targetHitbox = target.Hitbox;

            float dist1 = Vector2.Distance(npc.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(npc.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(npc.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(npc.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= 80f && npc.alpha <= 0;
        }

        // Projectiles can only hit within certain distance
        /*public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            Rectangle projectileHitbox = projectile.Hitbox;

            float dist1 = Vector2.Distance(npc.Center, projectileHitbox.TopLeft());
            float dist2 = Vector2.Distance(npc.Center, projectileHitbox.TopRight());
            float dist3 = Vector2.Distance(npc.Center, projectileHitbox.BottomLeft());
            float dist4 = Vector2.Distance(npc.Center, projectileHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist) minDist = dist2;
            if (dist3 < minDist) minDist = dist3;
            if (dist4 < minDist) minDist = dist4;

            return minDist <= 80f;
        }*/

        // Melee hitboxes are fucked so I have no clue what to do here
        /*public override bool? CanBeHitByItem(Player player, Item item)
        {
            Rectangle targetHitbox = target.Hitbox;

            float dist1 = Vector2.Distance(npc.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(npc.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(npc.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(npc.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist) minDist = dist2;
            if (dist3 < minDist) minDist = dist3;
            if (dist4 < minDist) minDist = dist4;

            return minDist <= 80f && npc.alpha == 0;
        }*/

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (CalamityUtils.AntiButcher(npc, ref damage, 0.5f))
            {
                string key = "Mods.CalamityMod.EdgyBossText2";
                Color messageColor = Color.Cyan;
                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(key), messageColor);
                else if (Main.netMode == NetmodeID.Server)
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                return false;
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
            if (npc.soundDelay == 0)
            {
                npc.soundDelay = 8;
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCHit, "Sounds/NPCHit/OtherworldlyHit"), npc.Center);
            }
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
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 30; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 173, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<GodSlayerInferno>(), 300, true);
            player.AddBuff(ModContent.BuffType<WhisperingDeath>(), 420, true);
            player.AddBuff(BuffID.Frostburn, 300, true);
            player.AddBuff(BuffID.Darkness, 300, true);
            if ((CalamityWorld.death || CalamityWorld.bossRushActive) && npc.alpha <= 0)
            {
                player.KillMe(PlayerDeathReason.ByCustomReason(player.name + "'s essence was consumed by the devourer."), 1000.0, 0, false);
            }

            // TODO: don't talk if the player has iframes
            if (player.immune || player.immuneTime > 0)
                return;

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
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue(key), messageColor);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
            }
        }
    }
}
