using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
    public class EidolonWyrmHeadHuge : ModNPC
    {
		private Vector2 patrolSpot = Vector2.Zero;
		public bool detectsPlayer = false;
        public const int minLength = 40;
        public const int maxLength = 41;
        public float speed = 7.5f; //10
        public float turnSpeed = 0.15f; //0.15
        bool TailSpawned = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eidolon Wyrm");
        }

        public override void SetDefaults()
        {
			npc.Calamity().canBreakPlayerDefense = true;
			npc.npcSlots = 24f;
            npc.damage = 1500;
            npc.width = 254; //36
            npc.height = 138; //20
            npc.defense = 700;
            CalamityGlobalNPC global = npc.Calamity();
            global.DR = 0.95f;
            global.unbreakableDR = true;
            npc.lifeMax = 1000000;
            npc.aiStyle = -1;
            aiType = -1;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(10, 0, 0, 0);
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath6;
            npc.netAlways = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
			writer.WriteVector2(patrolSpot);
			writer.Write(detectsPlayer);
            writer.Write(npc.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
			patrolSpot = reader.ReadVector2();
			detectsPlayer = reader.ReadBoolean();
            npc.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
			if (npc.target < 0 || npc.target == 255 || Main.player[npc.target].dead)
			{
				npc.TargetClosest(true);
			}
			if (npc.justHit || detectsPlayer || Main.player[npc.target].chaosState)
            {
				if (!detectsPlayer)
				{
					if (Main.netMode != NetmodeID.Server)
					{
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/Scare"), (int)Main.player[npc.target].position.X, (int)Main.player[npc.target].position.Y);
					}
					detectsPlayer = true;
				}
                npc.damage = 1500;
            }
            else
            {
                npc.damage = 0;
            }
            npc.chaseable = detectsPlayer;
            if (detectsPlayer)
            {
                if (npc.soundDelay <= 0 && Main.netMode != NetmodeID.Server)
                {
                    npc.soundDelay = 420;
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/EidolonWyrmRoarClose").WithVolume(2.5f), (int)npc.position.X, (int)npc.position.Y);
                }
            }
            else
            {
                if (Main.rand.NextBool(900) && Main.netMode != NetmodeID.Server)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/EidolonWyrmRoarClose").WithVolume(2.5f), (int)npc.position.X, (int)npc.position.Y);
                }
            }
            if (npc.ai[2] > 0f)
            {
                npc.realLife = (int)npc.ai[2];
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!TailSpawned && npc.ai[0] == 0f)
                {
                    int Previous = npc.whoAmI;
                    for (int num36 = 0; num36 < maxLength; num36++)
                    {
                        int lol;
                        if (num36 >= 0 && num36 < minLength)
                        {
                            if (num36 % 2 == 0)
                            {
                                lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<EidolonWyrmBodyHuge>(), npc.whoAmI);
                            }
                            else
                            {
                                lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<EidolonWyrmBodyAltHuge>(), npc.whoAmI);
                            }
                        }
                        else
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<EidolonWyrmTailHuge>(), npc.whoAmI);
                        }
                        Main.npc[lol].realLife = npc.whoAmI;
                        Main.npc[lol].ai[2] = (float)npc.whoAmI;
                        Main.npc[lol].ai[1] = (float)Previous;
                        Main.npc[Previous].ai[0] = (float)lol;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
                        Previous = lol;
                    }
                    TailSpawned = true;
                }
                if (detectsPlayer)
                {
                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] >= 300f)
                    {
                        npc.localAI[0] = 0f;
                        npc.TargetClosest(true);
                        npc.netUpdate = true;
                        int damage = Main.expertMode ? 300 : 400;
                        float xPos = Main.rand.NextBool(2) ? npc.position.X + 200f : npc.position.X - 200f;
                        Vector2 vector2 = new Vector2(xPos, npc.position.Y + Main.rand.Next(-200, 201));
                        int random = Main.rand.Next(3);
                        if (random == 0)
                        {
                            Projectile.NewProjectile(vector2.X, vector2.Y, 0f, 0f, ProjectileID.CultistBossLightningOrb, damage, 0f, Main.myPlayer, 0f, 0f);
                            Projectile.NewProjectile(-vector2.X, -vector2.Y, 0f, 0f, ProjectileID.CultistBossLightningOrb, damage, 0f, Main.myPlayer, 0f, 0f);
                        }
                        else if (random == 1)
                        {
                            Vector2 vec = Vector2.Normalize(Main.player[npc.target].Center - npc.Center);
                            vec = Vector2.Normalize(Main.player[npc.target].Center - npc.Center + Main.player[npc.target].velocity * 20f);
                            if (vec.HasNaNs())
                            {
                                vec = new Vector2((float)npc.direction, 0f);
                            }
                            for (int n = 0; n < 1; n++)
                            {
                                Vector2 vector4 = vec * 4f;
                                Projectile.NewProjectile(vector2.X, vector2.Y, vector4.X, vector4.Y, ProjectileID.CultistBossIceMist, damage, 0f, Main.myPlayer, 0f, 1f);
                                Projectile.NewProjectile(-vector2.X, -vector2.Y, -vector4.X, -vector4.Y, ProjectileID.CultistBossIceMist, damage, 0f, Main.myPlayer, 0f, 1f);
                            }
                        }
                        else
                        {
                            if (Math.Abs(Main.player[npc.target].velocity.X) > 0.1f || Math.Abs(Main.player[npc.target].velocity.Y) > 0.1f)
                            {
                                Main.PlaySound(SoundID.Item117, Main.player[npc.target].position);
                                for (int num621 = 0; num621 < 20; num621++)
                                {
                                    int num622 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X, Main.player[npc.target].position.Y),
                                        Main.player[npc.target].width, Main.player[npc.target].height, 185, 0f, 0f, 100, default, 2f);
                                    Main.dust[num622].velocity *= 0.6f;
                                    if (Main.rand.NextBool(2))
                                    {
                                        Main.dust[num622].scale = 0.5f;
                                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                                    }
                                }
                                for (int num623 = 0; num623 < 30; num623++)
                                {
                                    int num624 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X, Main.player[npc.target].position.Y),
                                        Main.player[npc.target].width, Main.player[npc.target].height, 185, 0f, 0f, 100, default, 3f);
                                    Main.dust[num624].noGravity = true;
                                    num624 = Dust.NewDust(new Vector2(Main.player[npc.target].position.X, Main.player[npc.target].position.Y),
                                        Main.player[npc.target].width, Main.player[npc.target].height, 185, 0f, 0f, 100, default, 2f);
                                    Main.dust[num624].velocity *= 0.2f;
                                }
                                if (Math.Abs(Main.player[npc.target].velocity.X) > 0.1f)
                                {
                                    Main.player[npc.target].velocity.X = -Main.player[npc.target].velocity.X * 2f;
                                }
                                if (Math.Abs(Main.player[npc.target].velocity.Y) > 0.1f)
                                {
                                    Main.player[npc.target].velocity.Y = -Main.player[npc.target].velocity.Y * 2f;
                                }
                            }
                        }
                    }
                }
            }

            if (npc.velocity.X < 0f)
            {
                npc.spriteDirection = -1;
            }
            else if (npc.velocity.X > 0f)
            {
                npc.spriteDirection = 1;
            }

            if (Main.player[npc.target].dead)
            {
                npc.TargetClosest(false);

				npc.velocity.Y += 3f;
				if (npc.position.Y > Main.worldSurface * 16.0)
					npc.velocity.Y += 3f;

				if (npc.position.Y > (Main.maxTilesY - 200) * 16.0)
				{
					for (int a = 0; a < Main.maxNPCs; a++)
					{
						if (Main.npc[a].type == npc.type || Main.npc[a].type == ModContent.NPCType<EidolonWyrmBodyAltHuge>() || Main.npc[a].type == ModContent.NPCType<EidolonWyrmBodyHuge>() || Main.npc[a].type == ModContent.NPCType<EidolonWyrmTailHuge>())
							Main.npc[a].active = false;
					}
				}
			}

            npc.alpha -= 42;
            if (npc.alpha < 0)
            {
                npc.alpha = 0;
            }

			if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 6400f || !NPC.AnyNPCs(ModContent.NPCType<EidolonWyrmTailHuge>()))
            {
                npc.active = false;
            }

            float num188 = speed;
            float num189 = turnSpeed;
			Vector2 vector18 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);

			if (patrolSpot == Vector2.Zero)
				patrolSpot = Main.player[npc.target].Center;

			float num191 = detectsPlayer ? Main.player[npc.target].Center.X : patrolSpot.X;
			float num192 = detectsPlayer ? Main.player[npc.target].Center.Y : patrolSpot.Y;

			if (!detectsPlayer)
			{
				num192 += 800;
				if (Math.Abs(npc.Center.X - num191) < 400f) //500
				{
					if (npc.velocity.X > 0f)
					{
						num191 += 500f;
					}
					else
					{
						num191 -= 500f;
					}
				}
			}
            else
            {
                num188 = 10f;
                num189 = 0.175f;
                if (!Main.player[npc.target].wet)
                {
                    num188 = 20f;
                    num189 = 0.25f;
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
            npc.rotation = (float)System.Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + 1.57f;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return detectsPlayer;
            }
            return null;
        }

        public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Vector2 center = new Vector2(npc.Center.X, npc.Center.Y);
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type] / 2));
            Vector2 vector = center - Main.screenPosition;
            vector -= new Vector2((float)ModContent.GetTexture("CalamityMod/NPCs/Abyss/EidolonWyrmHeadGlowHuge").Width, (float)(ModContent.GetTexture("CalamityMod/NPCs/Abyss/EidolonWyrmHeadGlowHuge").Height / Main.npcFrameCount[npc.type])) * 1f / 2f;
            vector += vector11 * 1f + new Vector2(0f, 0f + 4f + npc.gfxOffY);
            Color color = new Color(127 - npc.alpha, 127 - npc.alpha, 127 - npc.alpha, 0).MultiplyRGBA(Microsoft.Xna.Framework.Color.LightYellow);
            Main.spriteBatch.Draw(ModContent.GetTexture("CalamityMod/NPCs/Abyss/EidolonWyrmHeadGlowHuge"), vector,
                new Microsoft.Xna.Framework.Rectangle?(npc.frame), color, npc.rotation, vector11, 1f, spriteEffects, 0f);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneAbyssLayer4 && spawnInfo.water && !NPC.AnyNPCs(ModContent.NPCType<EidolonWyrmHeadHuge>()) && CalamityWorld.downedPolterghast)
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.012f;
            }
            return 0f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<Voidstone>(), 80, 100);
            DropHelper.DropItem(npc, ModContent.ItemType<EidolicWail>());
            DropHelper.DropItem(npc, ModContent.ItemType<SoulEdge>());
            DropHelper.DropItem(npc, ModContent.ItemType<HalibutCannon>());

            DropHelper.DropItemCondition(npc, ModContent.ItemType<Lumenite>(), CalamityWorld.downedCalamitas, 1, 50, 108);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Lumenite>(), CalamityWorld.downedCalamitas && Main.expertMode, 2, 15, 27);
            DropHelper.DropItemCondition(npc, ItemID.Ectoplasm, NPC.downedPlantBoss, 1, 21, 32);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (detectsPlayer)
            {
                for (int k = 0; k < 5; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default, 1f);
                }
                if (npc.life <= 0)
                {
                    for (int k = 0; k < 15; k++)
                    {
                        Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default, 1f);
                    }
                    Gore.NewGore(npc.position, npc.velocity, mod.GetGoreSlot("Gores/WyrmAdult"), 1f);
                }
            }
        }

        public override bool CheckActive()
        {
            if (detectsPlayer && !Main.player[npc.target].dead)
            {
                return false;
            }
            if (npc.timeLeft <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int k = (int)npc.ai[0]; k > 0; k = (int)Main.npc[k].ai[0])
                {
                    if (Main.npc[k].active)
                    {
                        Main.npc[k].active = false;
                        if (Main.netMode == NetmodeID.Server)
                        {
                            Main.npc[k].life = 0;
                            Main.npc[k].netSkip = -1;
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, k, 0f, 0f, 0f, 0, 0, 0);
                        }
                    }
                }
            }
            return true;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<CrushDepth>(), 1200, true);
        }
    }
}
