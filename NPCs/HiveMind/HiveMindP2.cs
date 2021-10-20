using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
/* states:
 * 0 = slow drift
 * 1 = reelback and teleport after spawn enemy
 * 2 = reelback for spin lunge + death legacy
 * 3 = spin lunge
 * 4 = semicircle spawn arc
 * 5 = raindash
 * 6 = deceleration
 */

namespace CalamityMod.NPCs.HiveMind
{
	[AutoloadBossHead]
    public class HiveMindP2 : ModNPC
    {
        //this block of values can be modified in SetDefaults() based on difficulty mode or something
        int minimumDriftTime = 300;
        int teleportRadius = 300;
        int decelerationTime = 30;
        int reelbackFade = 2;       //divide 255 by this for duration of reelback in ticks
        float arcTime = 45f;        //ticks needed to complete movement for spawn and rain attacks (DEATH ONLY)
        float driftSpeed = 1f;      //default speed when slowly floating at player
        float driftBoost = 1f;      //max speed added as health decreases
        int lungeDelay = 90;        //# of ticks long hive mind spends sliding to a stop before lunging
        int lungeTime = 33;
        int lungeFade = 15;         //divide 255 by this for duration of hive mind spin before slowing for lunge
        double lungeRots = 0.2;     //number of revolutions made while spinning/fading in for lunge
        bool dashStarted = false;
        int phase2timer = 360;
        int rotationDirection;
        double rotation;
        double rotationIncrement;
        int state = 0;
        int previousState = 0;
        int nextState = 0;
        int reelCount = 0;
        Vector2 deceleration;
        int counter = 0;
        bool initialised = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Hive Mind");
            Main.npcFrameCount[npc.type] = 8;
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.npcSlots = 5f;
			npc.GetNPCDamage();
			npc.width = 177;
            npc.height = 142;
            npc.defense = 5;
            npc.LifeMaxNERB(5800, 7560, 300000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 6, 0, 0);
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            music = CalamityMod.Instance.GetMusicFromMusicMod("HiveMind") ?? MusicID.Boss2;
            bossBag = ModContent.ItemType<HiveMindBag>();
            NPCID.Sets.TrailCacheLength[npc.type] = 8;
            NPCID.Sets.TrailingMode[npc.type] = 1;
            if (Main.expertMode)
            {
                minimumDriftTime = 120;
                reelbackFade = 4;
            }
            if (CalamityWorld.revenge)
            {
                lungeRots = 0.3;
                minimumDriftTime = 90;
                reelbackFade = 5;
                lungeTime = 28;
                driftSpeed = 2f;
                driftBoost = 2f;
            }
            if (CalamityWorld.death)
            {
                lungeRots = 0.4;
                minimumDriftTime = 60;
                reelbackFade = 6;
                lungeTime = 23;
                driftSpeed = 3f;
                driftBoost = 1f;
            }
			if (CalamityWorld.malice)
			{
				lungeRots = 0.4;
				minimumDriftTime = 40;
				reelbackFade = 10;
				lungeTime = 16;
				driftSpeed = 6f;
				driftBoost = 1f;
			}
            phase2timer = minimumDriftTime;
            rotationIncrement = 0.0246399424 * lungeRots * lungeFade;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(state);
            writer.Write(nextState);
            writer.Write(phase2timer);
            writer.Write(dashStarted);
            writer.Write(rotationDirection);
            writer.Write(rotation);
            writer.Write(previousState);
            writer.Write(reelCount);
            writer.Write(counter);
            writer.Write(initialised);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            state = reader.ReadInt32();
            nextState = reader.ReadInt32();
            phase2timer = reader.ReadInt32();
            dashStarted = reader.ReadBoolean();
            rotationDirection = reader.ReadInt32();
            rotation = reader.ReadDouble();
            previousState = reader.ReadInt32();
            reelCount = reader.ReadInt32();
            counter = reader.ReadInt32();
            initialised = reader.ReadBoolean();
        }

        public override void FindFrame(int frameHeight)
        {
            int width = npc.width;
            int height = npc.height;

            if (!initialised)
            {
                counter = 8;
                npc.frameCounter = 6;
                initialised = true;
            }

            //ensure width and height are set.
            npc.frame.Width = width;
            npc.frame.Height = height;
            npc.frameCounter++;
            if (npc.frameCounter >= 6)
            {
                npc.frame.X = counter >= 8 ? width + 3 : 0;
                if (counter == 8)
                    npc.frame.Y = 0;
                else
                    npc.frame.Y += height;
                npc.frameCounter = 0;
                counter++;
            }
            if (counter == 16)
            {
                counter = 1;
                npc.frame.Y = 0;
                npc.frame.X = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            Color color24 = lightColor;
            color24 = npc.GetAlpha(color24);
            Color color25 = Lighting.GetColor((int)((double)npc.position.X + (double)npc.width * 0.5) / 16, (int)(((double)npc.position.Y + (double)npc.height * 0.5) / 16.0));
            Texture2D texture2D3 = ModContent.GetTexture("CalamityMod/NPCs/HiveMind/HiveMindP2");
            int num156 = Main.npcTexture[npc.type].Height / 8;
            Rectangle rectangle = new Rectangle(npc.frame.X, npc.frame.Y, npc.frame.X, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            int num157 = 8;
            int num158 = 2;
            int num159 = 1;
            float num160 = 0f;
            int num161 = num159;
            while (state != 0 && CalamityConfig.Instance.Afterimages && ((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)))
            {
                Color color26 = color25;
                color26 = npc.GetAlpha(color26);
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
                Vector2 value4 = npc.oldPos[num161];
                float num165 = npc.rotation;
                SpriteEffects effects = spriteEffects;
                Main.spriteBatch.Draw(texture2D3, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + npc.rotation * num160 * (float)(num161 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, npc.scale, effects, 0f);
                goto IL_6881;
            }

            var something = npc.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(Main.npcTexture[npc.type], npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), npc.frame, color24, npc.rotation, npc.frame.Size() / 2, npc.scale, something, 0);
            return false;
        }

        private void SpawnStuff()
        {
			int maxSpawns = (CalamityWorld.death || BossRushEvent.BossRushActive || CalamityWorld.malice) ? 5 : CalamityWorld.revenge ? 4 : Main.expertMode ? Main.rand.Next(3, 5) : Main.rand.Next(2, 4);
			for (int i = 0; i < maxSpawns; i++)
			{
				int type = NPCID.EaterofSouls;
				int choice = -1;
				do
				{
					choice++;
					switch (choice)
					{
						case 0:
						case 1:
							type = NPCID.EaterofSouls;
							break;
						case 2:
							type = NPCID.DevourerHead;
							break;
						case 3:
						case 4:
							type = ModContent.NPCType<DankCreeper>();
							break;
						default:
							break;
					}
				}
				while (NPC.AnyNPCs(type) && choice < 5);

				if (choice < 5)
					NPC.NewNPC((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), type);
			}
        }

        private void ReelBack()
        {
            npc.alpha = 0;
            phase2timer = 0;
            deceleration = npc.velocity / 255f * reelbackFade;
            if (CalamityWorld.revenge || BossRushEvent.BossRushActive || CalamityWorld.malice)
            {
                state = 2;
                Main.PlaySound(SoundID.ForceRoar, (int)npc.Center.X, (int)npc.Center.Y, -1, 1f, 0f);
            }
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    SpawnStuff();
                state = nextState;
                nextState = 0;
                if (state == 2)
                {
                    Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                }
                else
                {
                    Main.PlaySound(SoundID.ForceRoar, (int)npc.Center.X, (int)npc.Center.Y, -1, 1f, 0f);
                }
            }
        }

        public override void AI()
        {
			CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

			// Get a target
			if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
				npc.TargetClosest();

			// Despawn safety, make sure to target another player if the current player target is too far away
			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
				npc.TargetClosest();

			Player player = Main.player[npc.target];

			// Percent life remaining
			float lifeRatio = npc.life / (float)npc.lifeMax;

			if (CalamityWorld.malice || CalamityWorld.revenge || BossRushEvent.BossRushActive)
			{
				// Increase aggression if player is taking a long time to kill the boss
				if (lifeRatio > calamityGlobalNPC.killTimeRatio_IncreasedAggression)
					lifeRatio = calamityGlobalNPC.killTimeRatio_IncreasedAggression;
			}

			bool enraged = calamityGlobalNPC.enraged > 0;
			bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;

			float enrageScale = 0f;
            if ((npc.position.Y / 16f) < Main.worldSurface || malice)
            {
                npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
                enrageScale += 1f;
            }
            if (!player.ZoneCorrupt || malice)
            {
                npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
                enrageScale += 1f;
            }
			if (BossRushEvent.BossRushActive)
				enrageScale += 1f;
            if (enraged)
            {
                npc.Calamity().CurrentlyEnraged = true;
                enrageScale += 1f;
            }

			if (npc.alpha != 0)
            {
                if (npc.damage != 0)
                    npc.damage = 0;
            }
            else
                npc.damage = npc.defDamage;

            switch (state)
            {
                case 0: //slowdrift
                    if (npc.alpha > 0)
                        npc.alpha -= 3;
                    if (nextState == 0)
                    {
						npc.TargetClosest();
						if ((CalamityWorld.revenge || malice) && lifeRatio < 0.66f)
                        {
							if (CalamityWorld.death || malice)
							{
								do
									nextState = Main.rand.Next(3, 6);
								while (nextState == previousState);
								previousState = nextState;
							}
							else if (lifeRatio < 0.33f)
							{
								do
									nextState = Main.rand.Next(3, 6);
								while (nextState == previousState);
								previousState = nextState;
							}
							else
							{
								do
									nextState = Main.rand.Next(3, 5);
								while (nextState == previousState);
								previousState = nextState;
							}
                        }
                        else
                        {
                            if ((CalamityWorld.revenge || malice) && (Main.rand.NextBool(3) || reelCount == 2))
                            {
                                reelCount = 0;
                                nextState = 2;
                            }
                            else
                            {
                                reelCount++;
								if (Main.expertMode && reelCount == 2)
								{
									reelCount = 0;
									nextState = 2;
								}
								else
									nextState = 1;

                                npc.ai[1] = 0f;
                                npc.ai[2] = 0f;
                            }
                        }
                        if (nextState == 3)
                            rotation = MathHelper.ToRadians(Main.rand.Next(360));
                        npc.netUpdate = true;
                    }

                    if (!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 5000f)
                    {
                        npc.TargetClosest(false);
						player = Main.player[npc.target];
						if (!player.active || player.dead || Vector2.Distance(npc.Center, player.Center) > 5000f)
						{
							if (npc.timeLeft > 60)
								npc.timeLeft = 60;
							if (npc.localAI[3] < 120f)
							{
								float[] aiArray = npc.localAI;
								int number = 3;
								float num244 = aiArray[number];
								aiArray[number] = num244 + 1f;
							}
							if (npc.localAI[3] > 60f)
							{
								npc.velocity.Y += (npc.localAI[3] - 60f) * 0.5f;
							}
							return;
						}
                    }
					else if (npc.timeLeft < 1800)
						npc.timeLeft = 1800;

					if (npc.localAI[3] > 0f)
                    {
                        float[] aiArray = npc.localAI;
                        int number = 3;
                        float num244 = aiArray[number];
                        aiArray[number] = num244 - 1f;
                        return;
                    }

                    npc.velocity = player.Center - npc.Center;
                    phase2timer--;
                    if (phase2timer <= -180) //no stalling drift mode forever
                    {
                        npc.velocity *= 2f / 255f * (reelbackFade + 2 * (int)enrageScale);
                        ReelBack();
                        npc.netUpdate = true;
                    }
                    else
                    {
                        npc.velocity.Normalize();
                        if (Main.expertMode || malice) //variable velocity in expert and up
                        {
                            npc.velocity *= driftSpeed + enrageScale + driftBoost * lifeRatio;
                        }
                        else
                        {
                            npc.velocity *= driftSpeed + enrageScale;
                        }
                    }
                    break;
                case 1: //reelback and teleport
                    npc.alpha += reelbackFade + 2 * (int)enrageScale;
                    npc.velocity -= deceleration;
                    if (npc.alpha >= 255)
                    {
                        npc.alpha = 255;
                        npc.velocity = Vector2.Zero;
                        state = 0;
                        if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] != 0f && npc.ai[2] != 0f)
                        {
                            npc.position.X = npc.ai[1] * 16 - npc.width / 2;
                            npc.position.Y = npc.ai[2] * 16 - npc.height / 2;
                        }
                        phase2timer = minimumDriftTime + Main.rand.Next(121);
                        npc.netUpdate = true;
                    }
                    else if (npc.ai[1] == 0f && npc.ai[2] == 0f)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            int posX = (int)player.Center.X / 16 + Main.rand.Next(15, 46) * (Main.rand.NextBool(2) ? -1 : 1);
                            int posY = (int)player.Center.Y / 16 + Main.rand.Next(15, 46) * (Main.rand.NextBool(2) ? -1 : 1);
                            if (!WorldGen.SolidTile(posX, posY) && Collision.CanHit(new Vector2(posX * 16, posY * 16), 1, 1, player.position, player.width, player.height))
                            {
                                npc.ai[1] = posX;
                                npc.ai[2] = posY;
                                npc.netUpdate = true;
                                break;
                            }
                        }
                    }
                    break;
                case 2: //reelback for lunge + death legacy
                    npc.alpha += reelbackFade + 2 * (int)enrageScale;
                    npc.velocity -= deceleration;
                    if (npc.alpha >= 255)
                    {
                        npc.alpha = 255;
                        npc.velocity = Vector2.Zero;
                        dashStarted = false;
                        if ((CalamityWorld.revenge || malice) && lifeRatio < 0.66f)
                        {
							state = nextState;
                            nextState = 0;
                            previousState = state;
                        }
                        else
                        {
                            state = 3;
                        }
                        if (player.velocity.X > 0)
                            rotationDirection = 1;
                        else if (player.velocity.X < 0)
                            rotationDirection = -1;
                        else
                            rotationDirection = player.direction;
                    }
                    break;
                case 3: //lunge
                    npc.netUpdate = true;
                    if (npc.alpha > 0)
                    {
                        npc.alpha -= lungeFade;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.Center = player.Center + new Vector2(teleportRadius, 0).RotatedBy(rotation);
                        }
                        rotation += rotationIncrement * rotationDirection;
                        phase2timer = lungeDelay;
                    }
                    else
                    {
                        phase2timer--;
                        if (!dashStarted)
                        {
                            if (phase2timer <= 0)
                            {
                                phase2timer = lungeTime - 4 * (int)enrageScale;
                                npc.velocity = player.Center + (malice ? player.velocity * 20f : Vector2.Zero) - npc.Center;
                                npc.velocity.Normalize();
                                npc.velocity *= teleportRadius / (lungeTime - (int)enrageScale);
                                dashStarted = true;
                                Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                            }
                            else
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    npc.Center = player.Center + new Vector2(teleportRadius, 0).RotatedBy(rotation);
                                }
                                rotation += rotationIncrement * rotationDirection * phase2timer / lungeDelay;
                            }
                        }
                        else
                        {
                            if (phase2timer <= 0)
                            {
                                state = 6;
                                phase2timer = 0;
                                deceleration = npc.velocity / decelerationTime;
                            }
                        }
                    }
                    break;
                case 4: //enemy spawn arc
                    if (npc.alpha > 0)
                    {
                        npc.alpha -= 5;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.Center = player.Center;
                            npc.position.Y += teleportRadius;
                        }
                        npc.netUpdate = true;
                    }
                    else
                    {
                        if (!dashStarted)
                        {
                            dashStarted = true;
                            Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                            npc.velocity.X = MathHelper.Pi * teleportRadius / arcTime;
                            npc.velocity *= rotationDirection;
                            npc.netUpdate = true;
                        }
                        else
                        {
                            npc.velocity = npc.velocity.RotatedBy(MathHelper.Pi / arcTime * -rotationDirection);
                            phase2timer++;
                            if (phase2timer == (int)arcTime / 6)
                            {
                                phase2timer = 0;
                                npc.ai[0]++;
                                if (Main.netMode != NetmodeID.MultiplayerClient && Collision.CanHit(npc.Center, 1, 1, player.position, player.width, player.height)) //draw line of sight
                                {
                                    if (npc.ai[0] == 2 || npc.ai[0] == 4)
                                    {
                                        if ((Main.expertMode || malice) && !NPC.AnyNPCs(ModContent.NPCType<DarkHeart>()))
                                        {
                                            NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, ModContent.NPCType<DarkHeart>());
                                        }
									}
                                    else if (!NPC.AnyNPCs(NPCID.EaterofSouls))
                                    {
                                        NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.EaterofSouls);
                                    }
                                }
                                if (npc.ai[0] == 6)
                                {
                                    npc.velocity = npc.velocity.RotatedBy(MathHelper.Pi / arcTime * -rotationDirection);
                                    SpawnStuff();
                                    state = 6;
                                    npc.ai[0] = 0;
                                    deceleration = npc.velocity / decelerationTime;
                                }
                            }
                        }
                    }
                    break;
                case 5: //raindash
                    if (npc.alpha > 0)
                    {
                        npc.alpha -= 5;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            npc.Center = player.Center;
                            npc.position.Y -= teleportRadius;
                            npc.position.X += teleportRadius * rotationDirection;
                        }
                        npc.netUpdate = true;
                    }
                    else
                    {
                        if (!dashStarted)
                        {
                            dashStarted = true;
                            Main.PlaySound(SoundID.Roar, (int)npc.Center.X, (int)npc.Center.Y, 0);
                            npc.velocity.X = teleportRadius / arcTime * 3;
                            npc.velocity *= -rotationDirection;
                            npc.netUpdate = true;
                        }
                        else
                        {
                            phase2timer++;
                            if (phase2timer == (int)arcTime / 20)
                            {
                                phase2timer = 0;
                                npc.ai[0]++;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
									int type = ModContent.ProjectileType<ShadeNimbusHostile>();
									int damage = npc.GetProjectileDamage(type);
									Projectile.NewProjectile(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height), 0, 0, type, damage, 0, Main.myPlayer, 11, 0);
                                }
                                if (npc.ai[0] == 10)
                                {
                                    state = 6;
                                    npc.ai[0] = 0;
                                    deceleration = npc.velocity / decelerationTime;
                                }
                            }
                        }
                    }
                    break;
                case 6: //deceleration
                    npc.velocity -= deceleration;
                    phase2timer++;
                    if (phase2timer == decelerationTime)
                    {
                        phase2timer = minimumDriftTime + Main.rand.Next(121);
                        state = 0;
                        npc.netUpdate = true;
                    }
                    break;
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            if (npc.alpha > 0)
                return false;
            return null;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return npc.alpha <= 0; //no damage when not fully visible
        }

        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (phase2timer < 0 && damage > 1)
            {
                npc.velocity *= -4f;
                ReelBack();
                npc.netUpdate = true;
            }
            return true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * npc.GetExpertDamageMultiplier());
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < damage / npc.lifeMax * 100.0; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 14, hitDirection, -1f, 0, default, 1f);
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(15) && NPC.CountNPCS(ModContent.NPCType<HiveBlob2>()) < 2)
            {
                Vector2 spawnAt = npc.Center + new Vector2(0f, npc.height / 2f);
                NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, ModContent.NPCType<HiveBlob2>());
            }
            if (npc.life <= 0)
            {
                int goreAmount = 10;
                for (int i = 1; i <= goreAmount; i++)
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/HiveMindGores/HiveMindP2Gore" + i), 1f);
                npc.position.X = npc.position.X + (npc.width / 2);
                npc.position.Y = npc.position.Y + (npc.height / 2);
                npc.width = 200;
                npc.height = 150;
                npc.position.X = npc.position.X - (npc.width / 2);
                npc.position.Y = npc.position.Y - (npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 14, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 14, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 14, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void NPCLoot()
        {
			CalamityGlobalNPC.SetNewBossJustDowned(npc);

			DropHelper.DropBags(npc);

			// Legendary drop for Evil boss tier 2
			DropHelper.DropItemCondition(npc, ModContent.ItemType<Carnage>(), true, CalamityWorld.malice);

			DropHelper.DropItemChance(npc, ModContent.ItemType<HiveMindTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeHiveMind>(), true, !CalamityWorld.downedHiveMind);

			CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.Dryad }, CalamityWorld.downedHiveMind);

			// All other drops are contained in the bag, so they only drop directly on Normal
			if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItemSpray(npc, ModContent.ItemType<TrueShadowScale>(), 25, 30, 5);
                DropHelper.DropItemSpray(npc, ItemID.DemoniteBar, 8, 12, 2);
                DropHelper.DropItemSpray(npc, ItemID.RottenChunk, 9, 15, 3);
                if (Main.hardMode)
                    DropHelper.DropItemSpray(npc, ItemID.CursedFlame, 10, 20, 2);

                // Weapons
                float w = DropHelper.NormalWeaponDropRateFloat;
                DropHelper.DropEntireWeightedSet(npc,
                    DropHelper.WeightStack<PerfectDark>(w),
                    DropHelper.WeightStack<LeechingDagger>(w),
                    DropHelper.WeightStack<Shadethrower>(w),
                    DropHelper.WeightStack<ShadowdropStaff>(w),
                    DropHelper.WeightStack<ShaderainStaff>(w),
                    DropHelper.WeightStack<DankStaff>(w),
                    DropHelper.WeightStack<RotBall>(w, 30, 50)
                );

                //Equipment
                DropHelper.DropItemChance(npc, ModContent.ItemType<FilthyGlove>(), 4);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<HiveMindMask>(), 7);
                DropHelper.DropItemChance(npc, ModContent.ItemType<RottingEyeball>(), 10);
            }

            // If neither The Hive Mind nor The Perforator Hive have been killed yet, notify players of Aerialite Ore
            if (!CalamityWorld.downedHiveMind && !CalamityWorld.downedPerforator)
            {
                string key = "Mods.CalamityMod.SkyOreText";
                Color messageColor = Color.Cyan;
                CalamityUtils.SpawnOre(ModContent.TileType<AerialiteOre>(), 12E-05, 0.4f, 0.6f, 3, 8);

                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }

            // Mark The Hive Mind as dead
            CalamityWorld.downedHiveMind = true;
            CalamityNetcode.SyncWorld();
        }
    }
}
