using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.CraftingStations;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using CalamityMod;
namespace CalamityMod.NPCs.SlimeGod
{
    [AutoloadBossHead]
    public class SlimeGodCore : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Slime God");
        }

        public override void SetDefaults()
        {
            npc.damage = 40;
            npc.npcSlots = 10f;
            npc.width = 44;
            npc.height = 44;
            npc.defense = 6;
            npc.LifeMaxNERB(3000, 3750, 2500000);
            double HPBoost = CalamityMod.CalamityConfig.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            NPCID.Sets.TrailCacheLength[npc.type] = 8;
            NPCID.Sets.TrailingMode[npc.type] = 1;
            npc.aiStyle = -1;
            aiType = -1;
            npc.buffImmune[ModContent.BuffType<GlacialState>()] = true;
            npc.buffImmune[ModContent.BuffType<TemporalSadness>()] = true;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 8, 0, 0);
            npc.alpha = 80;
            animationType = 10;
            npc.boss = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath1;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/SlimeGod");
            else
                music = MusicID.Boss1;
            bossBag = ModContent.ItemType<SlimeGodBag>();
        }

        public override void AI()
        {
            CalamityGlobalNPC.slimeGod = npc.whoAmI;
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
			bool death = CalamityWorld.death || CalamityWorld.bossRushActive;
			Player player = Main.player[npc.target];
            int randomDust = Main.rand.Next(2);
            if (randomDust == 0)
            {
                randomDust = 173;
            }
            else
            {
                randomDust = 260;
            }
            int num658 = Dust.NewDust(npc.position, npc.width, npc.height, randomDust, npc.velocity.X, npc.velocity.Y, 255, new Color(0, 80, 255, 80), npc.scale * 1.5f);
            Main.dust[num658].noGravity = true;
            Main.dust[num658].velocity *= 0.5f;
            bool flag100 = false;
            if (!death)
            {
                if (CalamityGlobalNPC.slimeGodRed != -1)
                {
                    if (Main.npc[CalamityGlobalNPC.slimeGodRed].active)
                    {
                        flag100 = true;
                    }
                }
                if (CalamityGlobalNPC.slimeGodPurple != -1)
                {
                    if (Main.npc[CalamityGlobalNPC.slimeGodPurple].active)
                    {
                        flag100 = true;
                    }
                }
            }
            if (!player.active || player.dead)
            {
                npc.TargetClosest(false);
                player = Main.player[npc.target];
                if (!player.active || player.dead)
                {
                    npc.velocity = new Vector2(0f, 30f);
                    if ((double)npc.position.Y > Main.rockLayer * 16.0)
                    {
                        for (int x = 0; x < 200; x++)
                        {
                            if (Main.npc[x].type == ModContent.NPCType<SlimeGod>() || Main.npc[x].type == ModContent.NPCType<SlimeGodSplit>() ||
                                Main.npc[x].type == ModContent.NPCType<SlimeGodRun>() || Main.npc[x].type == ModContent.NPCType<SlimeGodRunSplit>())
                            {
                                Main.npc[x].active = false;
                                Main.npc[x].netUpdate = true;
                            }
                        }
                        npc.active = false;
                        npc.netUpdate = true;
                    }
                    return;
                }
            }
            else if (npc.timeLeft < 2400)
            {
                npc.timeLeft = 2400;
            }
            if (!flag100)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[1] += (npc.Calamity().enraged > 0 || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive)) ? 2f : 1f;
                    if (expertMode && Main.rand.NextBool(2))
                    {
                        if (npc.localAI[0] >= 75f)
                        {
                            npc.localAI[0] = 0f;
                            npc.TargetClosest(true);
                            if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                            {
                                float num179 = revenge ? 2f : 3f;
                                if (CalamityWorld.bossRushActive)
                                    num179 = 12f;

                                Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                                float num180 = player.position.X + (float)player.width * 0.5f - value9.X;
                                float num181 = Math.Abs(num180) * 0.1f;
                                float num182 = player.position.Y + (float)player.height * 0.5f - value9.Y - num181;
                                float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                                npc.netUpdate = true;
                                num183 = num179 / num183;
                                num180 *= num183;
                                num182 *= num183;
                                int num184 = 24;
                                int num185 = Main.rand.Next(2);
                                if (num185 == 0)
                                {
                                    num185 = ModContent.ProjectileType<AbyssMine>();
                                }
                                else
                                {
                                    num185 = ModContent.ProjectileType<AbyssMine2>();
                                    num184 = 22;
                                }
                                value9.X += num180;
                                value9.Y += num182;
                                num180 = player.position.X + (float)player.width * 0.5f - value9.X;
                                num182 = player.position.Y + (float)player.height * 0.5f - value9.Y;
                                num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                                num183 = num179 / num183;
                                num180 *= num183;
                                num182 *= num183;
                                Projectile.NewProjectile(value9.X, value9.Y, num180, num182, num185, num184, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                    }
                    else if (npc.localAI[1] >= 75f)
                    {
                        npc.localAI[1] = 0f;
                        npc.TargetClosest(true);
                        if (Collision.CanHit(npc.position, npc.width, npc.height, player.position, player.width, player.height))
                        {
                            float num179 = revenge ? 6f : 5f;
                            if (CalamityWorld.bossRushActive)
                                num179 = 12f;

                            Vector2 value9 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            float num180 = player.position.X + (float)player.width * 0.5f - value9.X;
                            float num181 = Math.Abs(num180) * 0.1f;
                            float num182 = player.position.Y + (float)player.height * 0.5f - value9.Y - num181;
                            float num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                            npc.netUpdate = true;
                            num183 = num179 / num183;
                            num180 *= num183;
                            num182 *= num183;
                            int num184 = expertMode ? 17 : 21;
                            int num185 = Main.rand.Next(2);
                            if (num185 == 0)
                            {
                                num185 = ModContent.ProjectileType<AbyssBallVolley>();
                            }
                            else
                            {
                                num185 = ModContent.ProjectileType<AbyssBallVolley2>();
                                num184 = expertMode ? 16 : 19;
                            }
                            value9.X += num180;
                            value9.Y += num182;
                            for (int num186 = 0; num186 < 2; num186++)
                            {
                                num180 = player.position.X + (float)player.width * 0.5f - value9.X;
                                num182 = player.position.Y + (float)player.height * 0.5f - value9.Y;
                                num183 = (float)Math.Sqrt((double)(num180 * num180 + num182 * num182));
                                num183 = num179 / num183;
                                num180 += (float)Main.rand.Next(-30, 31);
                                num182 += (float)Main.rand.Next(-30, 31);
                                num180 *= num183;
                                num182 *= num183;
                                Projectile.NewProjectile(value9.X, value9.Y, num180, num182, num185, num184, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                    }
                }
            }
            npc.TargetClosest(true);
            float num1372 = 6f;
            if (!flag100)
            {
                num1372 = 14f;
            }
            else if (revenge)
            {
                num1372 = 10f;
            }
            if (CalamityWorld.bossRushActive || player.gravDir == -1f)
            {
                num1372 = 22f;
            }
            if (npc.Calamity().enraged > 0 || player.gravDir == -1f || (CalamityMod.CalamityConfig.BossRushXerocCurse && CalamityWorld.bossRushActive))
            {
                num1372 += 8f;
            }
            Vector2 vector167 = new Vector2(npc.Center.X + (float)(npc.direction * 20), npc.Center.Y + 6f);
            float num1373 = player.position.X + (float)player.width * 0.5f - vector167.X;
            float num1374 = player.Center.Y - vector167.Y;
            float num1375 = (float)Math.Sqrt((double)(num1373 * num1373 + num1374 * num1374));
            float num1376 = num1372 / num1375;
            num1373 *= num1376;
            num1374 *= num1376;
            npc.ai[0] -= 1f;
            if (num1375 < 200f || npc.ai[0] > 0f)
            {
                if (num1375 < 200f)
                {
                    npc.ai[0] = 20f;
                }
                if (npc.velocity.X < 0f)
                {
                    npc.direction = -1;
                }
                else
                {
                    npc.direction = 1;
                }
                return;
            }
            npc.velocity.X = (npc.velocity.X * 50f + num1373) / 51f;
            npc.velocity.Y = (npc.velocity.Y * 50f + num1374) / 51f;
            if (num1375 < 350f)
            {
                npc.velocity.X = (npc.velocity.X * 10f + num1373) / 11f;
                npc.velocity.Y = (npc.velocity.Y * 10f + num1374) / 11f;
            }
            if (num1375 < 300f)
            {
                npc.velocity.X = (npc.velocity.X * 7f + num1373) / 8f;
                npc.velocity.Y = (npc.velocity.Y * 7f + num1374) / 8f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Color color24 = npc.GetAlpha(drawColor);
            Color color25 = Lighting.GetColor((int)((double)npc.position.X + (double)npc.width * 0.5) / 16, (int)(((double)npc.position.Y + (double)npc.height * 0.5) / 16.0));
            Texture2D texture2D3 = Main.npcTexture[npc.type];
            int num156 = Main.npcTexture[npc.type].Height / Main.npcFrameCount[npc.type];
            int y3 = num156 * (int)npc.frameCounter;
            Rectangle rectangle = new Rectangle(0, y3, texture2D3.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            int num157 = 8;
            int num158 = 2;
            int num159 = 1;
            float num160 = 0f;
            int num161 = num159;
            spriteBatch.Draw(texture2D3, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), npc.frame, color24, npc.rotation, npc.frame.Size() / 2, npc.scale, spriteEffects, 0);
            while (((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)) && Lighting.NotRetro)
            {
                Color color26 = npc.GetAlpha(color25);
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
                Main.spriteBatch.Draw(texture2D3, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color26, num165 + npc.rotation * num160 * (float)(num161 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, npc.scale, spriteEffects, 0f);
                goto IL_6881;
            }
            return false;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void NPCLoot()
        {
            bool otherSlimeGodsAlive =
                NPC.AnyNPCs(ModContent.NPCType<SlimeGod>()) ||
                NPC.AnyNPCs(ModContent.NPCType<SlimeGodSplit>()) ||
                NPC.AnyNPCs(ModContent.NPCType<SlimeGodRun>()) ||
                NPC.AnyNPCs(ModContent.NPCType<SlimeGodRunSplit>());
            if (!otherSlimeGodsAlive)
                DropSlimeGodLoot(npc);
        }

        // This loot code is shared with every other Slime God component.
        public static void DropSlimeGodLoot(NPC npc)
        {
            CalamityMod mod = ModContent.GetInstance<CalamityMod>();
            DropHelper.DropBags(npc);

            DropHelper.DropItemChance(npc, ModContent.ItemType<SlimeGodTrophy>(), 10);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeSlimeGod>(), true, !CalamityWorld.downedSlimeGod);
            DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedSlimeGod, 3, 1, 0);

            // Purified Jam is once per player, but drops for all players.
            CalamityPlayer mp = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)].Calamity();
            if (!mp.revJamDrop)
            {
                DropHelper.DropItemCondition(npc, ModContent.ItemType<PurifiedJam>(), true, CalamityWorld.revenge && !CalamityWorld.downedSlimeGod, 6, 8);
                mp.revJamDrop = true;
            }

            // Gel always drops directly, even on Expert
            DropHelper.DropItemSpray(npc, ItemID.Gel, 180, 250);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItemSpray(npc, ModContent.ItemType<PurifiedGel>(), 25, 40);

                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<OverloadedBlaster>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<AbyssalTome>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<EldritchTome>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<CorroslimeStaff>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<CrimslimeStaff>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<GelDart>(), 4, 80, 100);

                // Vanity
                DropHelper.DropItemFromSetChance(npc, 7, ModContent.ItemType<SlimeGodMask>(), ModContent.ItemType<SlimeGodMask2>());

                // Other
                DropHelper.DropItem(npc, ModContent.ItemType<StaticRefiner>());
            }

            // Mark the Slime God as dead
            CalamityWorld.downedSlimeGod = true;
            CalamityMod.UpdateServerBoolean();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 4, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 40;
                npc.height = 40;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 4, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 4, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 4, 0f, 0f, 100, default, 2f);
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
            player.AddBuff(BuffID.VortexDebuff, 240, true);
            if (CalamityWorld.revenge)
            {
                player.AddBuff(ModContent.BuffType<Horror>(), 120, true);
                player.AddBuff(ModContent.BuffType<MarkedforDeath>(), 120);
            }
        }
    }
}
