using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

//reminder to remove unnecessary "CalamityMod." before CalamityWorld
//reminder to replace "ModLoader.GetMod("CalamityMod")." with "mod."
//enable the //CalamityGlobalNPC.hiveMind2 bit

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
		int reelbackFade = 3;		//divide 255 by this for duration of reelback in ticks
        float arcTime = 45f;        //ticks needed to complete movement for spawn and rain attacks (DEATH ONLY)
        float driftSpeed = 2f;	    //default speed when slowly floating at player
        float driftBoost = 1f;      //max speed added as health decreases
		int lungeDelay = 90;		//# of ticks long hive mind spends sliding to a stop before lunging
		int lungeTime = 30;
		int lungeFade = 15;			//divide 255 by this for duration of hive mind spin before slowing for lunge
		double lungeRots = 0.2;	    //number of revolutions made while spinning/fading in for lunge
		bool dashStarted = false;
		int phase2timer = 360;
		int rotationDirection;
		double rotation;
		double rotationIncrement;
		int state = 0;
        int previousState = 0;
        int nextState = 0;
        int reelCount = 0;
        int oldDamage = 30;
        Vector2 deceleration;

        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Hive Mind");
			Main.npcFrameCount[npc.type] = 4;
		}
		
		public override void SetDefaults()
		{
			npc.npcSlots = 5f;
			npc.damage = 30;
			npc.width = 150; //324
			npc.height = 120; //216
			npc.defense = 5;
			npc.lifeMax = CalamityWorld.revenge ? 4320 : 3000;
            if (CalamityWorld.death)
            {
                npc.lifeMax = 7200;
            }
            if (CalamityWorld.bossRushActive)
            {
                npc.lifeMax = CalamityWorld.death ? 880000 : 740000;
            }
            npc.aiStyle = -1; //new
            aiType = -1; //new
			npc.knockBackResist = 0f;
			npc.value = Item.buyPrice(0, 5, 0, 0);
			npc.boss = true;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.HitSound = SoundID.NPCHit1;
			npc.DeathSound = SoundID.NPCDeath1;
            music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/HiveMind");
            bossBag = mod.ItemType("HiveMindBag");
			NPCID.Sets.TrailCacheLength[npc.type] = 8;
			NPCID.Sets.TrailingMode[npc.type] = 1;
			if (Main.expertMode)
            {
                minimumDriftTime = 240;
            }
            if (CalamityWorld.revenge)
			{
				minimumDriftTime = 120;
                reelbackFade = 5;
                driftBoost = 3f;
			}
			if (CalamityWorld.death || CalamityWorld.bossRushActive)
			{
                lungeRots = 0.4;
                minimumDriftTime = 60;
				reelbackFade = 7;
				//decelerationTime = 20;
                //lungeFade = 6;
				//lungeDelay = 120;
				lungeTime = 20;
				driftSpeed = 4f;
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
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += 0.15f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
			SpriteEffects spriteEffects = SpriteEffects.None;
			Microsoft.Xna.Framework.Color color24 = lightColor;
			color24 = npc.GetAlpha(color24);
			Microsoft.Xna.Framework.Color color25 = Lighting.GetColor((int)((double)npc.position.X + (double)npc.width * 0.5) / 16, (int)(((double)npc.position.Y + (double)npc.height * 0.5) / 16.0));
			Texture2D texture2D3 = mod.GetTexture("NPCs/HiveMind/HiveMindP2");
			int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
			int y3 = num156 * (int)npc.frameCounter;
			Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle(0, y3, texture2D3.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;
			int arg_5ADA_0 = npc.type;
			int arg_5AE7_0 = npc.type;
			int arg_5AF4_0 = npc.type;
			int num157 = 8;
			int num158 = 2;
			int num159 = 1;
			float num160 = 0f;
			int num161 = num159;
			while (state != 0 && ((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)))
			{
				Microsoft.Xna.Framework.Color color26 = color25;
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
				Vector2 value4 = (npc.oldPos[num161]);
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
            Player player = Main.player[npc.target];
			for (int i = 0; i < 5; i++)
            {
                bool spawnedSomething = false;
                int type = NPCID.EaterofSouls;
                int maxAmount = 0;
                int random = !CalamityWorld.death && Collision.CanHit(npc.Center, 1, 1, player.position, player.width, player.height) ? 5 : 3;
                switch (Main.rand.Next(random))
				{
					case 0:
						type = NPCID.DevourerHead;
						maxAmount = 1;
						break;
					case 1:
						type = mod.NPCType("DankCreeper");
						maxAmount = 1;
						break;
					case 2:
						type = mod.NPCType("DankCreeper");
						maxAmount = 2;
						break;
					case 3:
                        type = mod.NPCType("HiveBlob2");
                        maxAmount = 2;
                        break;
                    case 4:
						type = NPCID.EaterofSouls;
                        maxAmount = 2;
                        break;
                    case 5:
						type = mod.NPCType("DarkHeart");
						maxAmount = 2;
						break;
				}
                int numToSpawn = maxAmount - NPC.CountNPCS(type);
                while (numToSpawn > 0)
                {
                    numToSpawn--;
                    spawnedSomething = true;
                    int spawn = NPC.NewNPC((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), type);
                    Main.npc[spawn].velocity.X = (float)Main.rand.Next(-15, 16) * 0.1f;
                    Main.npc[spawn].velocity.Y = (float)Main.rand.Next(-30, 1) * 0.1f;
                }
                if (spawnedSomething)
					return;
            }
        }

        private void ReelBack()
        {
            npc.alpha = 0;
            phase2timer = 0;
            deceleration = npc.velocity / 255f * reelbackFade;
            if (CalamityWorld.death)
            {
                state = 2;
                Main.PlaySound(36, (int)npc.Center.X, (int)npc.Center.Y, -1, 1f, 0f);
            }
            else
            {
                if (Main.netMode != 1)
                    SpawnStuff();
                state = nextState;
                nextState = 0;
                if (state == 2)
                {
                    Main.PlaySound(15, (int)npc.Center.X, (int)npc.Center.Y, 0);
                }
                else
                {
                    Main.PlaySound(36, (int)npc.Center.X, (int)npc.Center.Y, -1, 1f, 0f);
                }
            }
        }

		public override void AI()
		{
			Player player = Main.player[npc.target];
			npc.defense = (player.ZoneCorrupt || CalamityWorld.bossRushActive) ? 5 : 9999;
			CalamityGlobalNPC.hiveMind2 = npc.whoAmI;
            if (npc.alpha != 0)
            {
                if (npc.damage != 0)
                {
                    oldDamage = npc.damage;
                    npc.damage = 0;
                }
            }
            else
            {
                npc.damage = oldDamage;
            }
            switch (state)
            {
                case 0: //slowdrift
                    if (npc.alpha > 0)
                        npc.alpha -= 3;
                    if (nextState == 0)
                    {
                        npc.TargetClosest(true);
                        if (CalamityWorld.death || CalamityWorld.bossRushActive)
                        {
                            do nextState = Main.rand.Next(3, 6);
                            while (nextState == previousState);
                            previousState = nextState;
                        }
                        else
                        {
                            if (CalamityWorld.revenge && (Main.rand.Next(4) == 0 || reelCount == 3))
                            {
                                reelCount = 0;
                                nextState = 2;
                            }
                            else
                            {
                                reelCount++;
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
                        npc.TargetClosest(true);
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
                            npc.velocity.Y = npc.velocity.Y + (npc.localAI[3] - 60f) * 0.5f;
                        }
                        return;
                    }
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
                        npc.velocity *= 2f / 255f * reelbackFade;
                        ReelBack();
                        npc.netUpdate = true;
                    }
                    else
                    {
                        npc.velocity.Normalize();
                        if (Main.expertMode || CalamityWorld.bossRushActive) //variable velocity in expert and up
                        {
                            npc.velocity *= driftSpeed + driftBoost * (npc.lifeMax - npc.life) / npc.lifeMax;
                        }
                        else
                        {
                            npc.velocity *= driftSpeed;
                        }
                    }
                    break;
                case 1: //reelback and teleport
                    npc.alpha += reelbackFade;
                    npc.velocity -= deceleration;
                    if (npc.alpha >= 255)
                    {
                        npc.alpha = 255;
                        npc.velocity = Vector2.Zero;
                        state = 0;
                        if (Main.netMode != 1 && npc.ai[1] != 0f && npc.ai[2] != 0f)
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
                            int posX = (int)player.Center.X / 16 + Main.rand.Next(15, 46) * (Main.rand.Next(2) == 0 ? -1 : 1);
                            int posY = (int)player.Center.Y / 16 + Main.rand.Next(15, 46) * (Main.rand.Next(2) == 0 ? -1 : 1);
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
                    npc.alpha += reelbackFade;
                    npc.velocity -= deceleration;
                    if (npc.alpha >= 255)
                    {
                        npc.alpha = 255;
                        npc.velocity = Vector2.Zero;
                        dashStarted = false;
                        if (CalamityWorld.death || CalamityWorld.bossRushActive)
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
                case 3: //rev lunge
                    npc.netUpdate = true;
                    if (npc.alpha > 0)
                    {
                        npc.alpha -= lungeFade;
                        if (Main.netMode != 1)
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
                                phase2timer = lungeTime;
                                npc.velocity = player.Center - npc.Center;
                                npc.velocity.Normalize();
                                npc.velocity *= teleportRadius / lungeTime;
                                dashStarted = true;
                                Main.PlaySound(15, (int)npc.Center.X, (int)npc.Center.Y, 0);
                            }
                            else
                            {
                                if (Main.netMode != 1)
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
                case 4: //enemy spawn arc (death mode)
                    if (npc.alpha > 0)
                    {
                        npc.alpha -= 5;
                        if (Main.netMode != 1)
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
                            Main.PlaySound(15, (int)npc.Center.X, (int)npc.Center.Y, 0);
                            npc.velocity.X = 3.14159265f * teleportRadius / arcTime;
                            npc.velocity *= rotationDirection;
                            npc.netUpdate = true;
                        }
                        else
                        {
                            npc.velocity = npc.velocity.RotatedBy(3.14159265 / arcTime * -rotationDirection);
                            phase2timer++;
                            if (phase2timer == (int)arcTime / 6)
                            {
                                phase2timer = 0;
                                npc.ai[0]++;
                                if (Main.netMode != 1 && Collision.CanHit(npc.Center, 1, 1, player.position, player.width, player.height)) //draw line of sight
                                {
                                    if (npc.ai[0] == 2 || npc.ai[0] == 4)
                                    {
                                        if ((Main.expertMode || CalamityWorld.bossRushActive) && NPC.CountNPCS(mod.NPCType("DarkHeart")) < 2)
                                        {
                                            NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, mod.NPCType("DarkHeart"));
                                        }
                                    }
                                    else if (NPC.CountNPCS(NPCID.EaterofSouls) < 2)
                                    {
                                        NPC.NewNPC((int)npc.Center.X, (int)npc.Center.Y, NPCID.EaterofSouls);
                                    }
                                }
                                if (npc.ai[0] == 6)
                                {
                                    npc.velocity = npc.velocity.RotatedBy(3.14159265 / arcTime * -rotationDirection);
                                    SpawnStuff();
                                    state = 6;
                                    npc.ai[0] = 0;
                                    deceleration = npc.velocity / decelerationTime;
                                }
                            }
                        }
                    }
                    break;
                case 5: //raindash (death mode)
                    if (npc.alpha > 0)
                    {
                        npc.alpha -= 5;
                        if (Main.netMode != 1)
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
                            Main.PlaySound(15, (int)npc.Center.X, (int)npc.Center.Y, 0);
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
                                if (Main.netMode != 1)
                                {
                                    int damage = Main.expertMode ? 11 : 14;
                                    Projectile.NewProjectile(npc.position.X + Main.rand.Next(npc.width), npc.position.Y + Main.rand.Next(npc.height), 0, 0, mod.ProjectileType("ShadeNimbusHostile"), damage, 0, Main.myPlayer, 11, 0);
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

		public override bool? CanHitNPC (NPC target)
		{
			if (npc.alpha > 0)
				return false;
			return null;
		}

		public override bool CanHitPlayer (Player target, ref int cooldownSlot)
		{
			return npc.alpha <= 0; //no damage when not fully visible
		}

		public override bool StrikeNPC (ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
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
			npc.damage = (int)(npc.damage * 0.8f);
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			for (int k = 0; k < damage / npc.lifeMax * 100.0; k++)
			{
				Dust.NewDust(npc.position, npc.width, npc.height, 14, hitDirection, -1f, 0, default(Color), 1f);
			}
            if (Main.netMode != 1 && Main.rand.Next(15) == 0 && NPC.CountNPCS(mod.NPCType("HiveBlob2")) < 2)
            {
                Vector2 spawnAt = npc.Center + new Vector2(0f, (float)npc.height / 2f);
                NPC.NewNPC((int)spawnAt.X, (int)spawnAt.Y, mod.NPCType("HiveBlob2"));
            }
            if (npc.life <= 0)
			{
				npc.position.X = npc.position.X + (float)(npc.width / 2);
				npc.position.Y = npc.position.Y + (float)(npc.height / 2);
				npc.width = 200;
				npc.height = 150;
				npc.position.X = npc.position.X - (float)(npc.width / 2);
				npc.position.Y = npc.position.Y - (float)(npc.height / 2);
				for (int num621 = 0; num621 < 40; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 14, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 70; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 14, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 14, 0f, 0f, 100, default(Color), 2f);
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
			if (Main.rand.Next(10) == 0)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("HiveMindTrophy"));
			}
            if (CalamityWorld.armageddon)
            {
                for (int i = 0; i < 10; i++)
                {
                    npc.DropBossBags();
                }
            }
            if (Main.expertMode)
			{
				npc.DropBossBags();
			}
			else
			{
				if (Main.rand.Next(7) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("HiveMindMask"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ShaderainStaff"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("LeechingDagger"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ShadowdropStaff"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PerfectDark"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Shadethrower"));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("RotBall"), Main.rand.Next(25, 51));
				}
				if (Main.rand.Next(4) == 0)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DankStaff"));
				}
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("TrueShadowScale"), Main.rand.Next(25, 31));
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.DemoniteBar, Main.rand.Next(7, 11));
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.RottenChunk, Main.rand.Next(9, 16));
                if (Main.hardMode)
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.CursedFlame, Main.rand.Next(10, 21));
                }
            }
		}
		
		public override void OnHitPlayer(Player player, int damage, bool crit)
		{
			if (CalamityWorld.revenge)
			{
				player.AddBuff(mod.BuffType("Horror"), 90, true);
			}
		}
	}
}
