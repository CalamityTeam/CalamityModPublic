using System;
using System.Collections.Generic;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Systems.Collections;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using CalamityMod.ChatTags;
using CalamityMod.CustomRecipes;
using CalamityMod.Items.Fishing;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Potions.Food;
using CalamityMod.Items.SummonItems.TownPets;
using CalamityMod.Items.Tools;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Typeless;
using Terraria.Audio;
using Terraria.Chat;
using Terraria.GameContent.Bestiary;
using Terraria.UI.Chat;
using Terraria.Utilities;

namespace CalamityMod.NPCs.TownNPCs
{
    [AutoloadHead]
    public class ShadySalesman : ModNPC
    {
        public bool hasFiredShotThisAttack = false;
        public int attackFrameTimer = 0;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 10;
            NPCID.Sets.AttackFrameCount[Type] = 1;
            NPCID.Sets.DangerDetectRange[Type] = 700;
            NPCID.Sets.AttackType[Type] = 1;
            NPCID.Sets.AttackTime[Type] = 30;
            NPCID.Sets.AttackAverageChance[Type] = 10;
            NPCID.Sets.HatOffsetY[Type] = 18;

            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPCID.Sets.NoTownNPCHappiness[Type] = true;
            NPCID.Sets.IsTownChild[Type] = true;
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
            };
            ContentSamples.NpcBestiaryRarityStars[Type] = 3; //Same as the Traveling Merchant
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.lavaImmune = false;
            NPC.width = 48;
            NPC.height = 64;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.99f;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.ShadySalesman")
            });
        }

        public override List<string> SetNPCNameList() => ModContent.GetInstance<TownPiggy>().SetNPCNameList();

        public override void PartyHatPosition(ref Vector2 position, ref SpriteEffects spriteEffects) => position.X -= 4f * NPC.direction;

        public override void SendExtraAI(System.IO.BinaryWriter writer)
        {
            writer.Write(hasFiredShotThisAttack);
            writer.Write(attackFrameTimer);
        }
        public override void ReceiveExtraAI(System.IO.BinaryReader reader)
        {
            hasFiredShotThisAttack = reader.ReadBoolean();
            attackFrameTimer = reader.ReadInt32();
        }

        public override bool PreAI()
        {
            if (Main.dayTime && !IsNpcOnscreen(NPC.Center))
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (CalamityWorld.unlockedTownPig)
                    {
                        string name = NPC.GivenName;
                        NPC.Transform(ModContent.NPCType<TownPiggy>());
                        NPC.GivenName = name;
                    }
                    else
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer)
                            Main.NewText(Language.GetTextValue("LegacyMisc.35", NPC.FullName), 50, 125, 255);
                        else
                            ChatHelper.BroadcastChatMessage(NetworkText.FromKey("LegacyMisc.35", NPC.GetFullNetName()), new Color(50, 125, 255));
                        NPC.active = false;
                        NPC.netSkip = -1;
                    }
                    return false;
                }
            }
            if (!CalamityWorld.unlockedTownPig)
                NPC.UpdateHomeTileState(false, -1, -1);
            return true;
        }

        public override void AI()
        {
            if (NPC.ai[0] == 12f) // Attacking
            {
                attackFrameTimer++;

                if (attackFrameTimer >= 3 && !hasFiredShotThisAttack)
                {
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Texture2D gunTex = TextureAssets.Item[ModContent.ItemType<ElephantKiller>()].Value;
                        int holdoutOffset = -21;

                        Vector2 gunDrawPos = NPC.Center + new Vector2(0f, 8f);
                        float rotation = NPC.ai[2] * ((float)Math.PI / 2f) * NPC.spriteDirection;

                        Vector2 origin = NPC.spriteDirection == -1 ? new Vector2(gunTex.Width + holdoutOffset, gunTex.Height / 2f) : new Vector2(-holdoutOffset, gunTex.Height / 2f);
                        Vector2 muzzleLocal = NPC.spriteDirection == -1 ? new Vector2(0f, 6f) : new Vector2(gunTex.Width, 6f);
                        Vector2 muzzlePos = gunDrawPos + (muzzleLocal - origin).RotatedBy(rotation);
                        Vector2 barrelDirection = new Vector2(NPC.spriteDirection, 0f).RotatedBy(rotation);

                        Particle bloom = new CustomSpark(muzzlePos, Vector2.Zero, "CalamityMod/Particles/BrightFlash", false, 5, 0.18f, Color.Khaki, Vector2.One, true, true, glowOpacity: 0.45f, colorFadeSpeed: 8);
                        GeneralParticleHandler.SpawnParticle(bloom, true);

                        for (int i = 0; i < 8; i++)
                        {
                            Vector2 vel = barrelDirection.RotatedByRandom(0.18f) * Main.rand.NextFloat(3f, 7f);
                            Particle spark = new CustomSpark(muzzlePos, vel, "CalamityMod/Particles/FadeLine", false, Main.rand.Next(10, 18), 0.18f, new Color(255, 230, 140), new Vector2(0.5f, 1f), true, shrinkSpeed: 0.4f, colorFadeSpeed: 10);
                            GeneralParticleHandler.SpawnParticle(spark, true);
                        }
                        SoundEngine.PlaySound(ElephantKiller.Shot with { Volume = 0.9f, PitchVariance = 0.15f, MaxInstances = 3 }, NPC.Center);
                    }
                    hasFiredShotThisAttack = true;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                if (hasFiredShotThisAttack)
                {
                    hasFiredShotThisAttack = false;
                    NPC.netUpdate = true; // Syncs state exit
                }
                attackFrameTimer = 0;
            }
        }

        private static bool IsNpcOnscreen(Vector2 center)
        {
            int w = NPC.sWidth + NPC.safeRangeX * 2;
            int h = NPC.sHeight + NPC.safeRangeY * 2;
            Rectangle npcScreenRect = new Rectangle((int)center.X - w / 2, (int)center.Y - h / 2, w, h);
            foreach (Player player in Main.ActivePlayers)
                if (player.getRect().Intersects(npcScreenRect))
                    return true;
            return false;
        }

        public override string GetChat()
        {
            int piggyIndex = NPC.FindFirstNPC(ModContent.NPCType<TownPiggy>());

            WeightedRandom<string> dialogue = new WeightedRandom<string>();

            dialogue.Add(this.GetLocalizedValue("Chat.Normal1"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal2"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal3"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal4"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal5"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal6"));

            if (piggyIndex != -1)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.TownPigPresent1"));
                dialogue.Add(this.GetLocalizedValue("Chat.TownPigPresent2"));
                dialogue.Add(this.GetLocalizedValue("Chat.TownPigPresent3")); 
                dialogue.Add(this.GetLocalizedValue("Chat.TownPigPresent4"));
            }

            if (Main.raining)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.Rain"));
            }

            if (Main._shouldUseWindyDayMusic)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.WindyDay"));
            }

            if (Main.LocalPlayer.ZoneDesert)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.Desert"));
            }

            if (Main.LocalPlayer.ZoneSnow)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.Tundra"));
            }

            if (Main.LocalPlayer.ZoneGraveyard)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.Graveyard"));
            }

            if (Main.bloodMoon)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.BloodMoon1"));
                dialogue.Add(this.GetLocalizedValue("Chat.BloodMoon2"));
                dialogue.Add(this.GetLocalizedValue("Chat.BloodMoon3"));
            }

            if (Main.hardMode)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.Hardmode"));
            }

            if (NPC.downedMoonlord)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.MoonLordDefeated"));
            }

            return dialogue;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                shopName = "Shop";
            }
        }

        public override void AddShops()
        {
            NPCShop shop = new(Type);
            shop.Add<TheConcoction>(Condition.MoonPhasesEven)
                .Add<TheElixir>(Condition.MoonPhasesOdd)
                .Add<BaconOil>(Condition.MoonPhasesHalf0)
                .Add<TheSandwich>(Condition.MoonPhasesHalf1)
                .Add<RageBait>()
                .Add<CombatVoucher>(Condition.DownedGoblinArmy)
                .Add<AggressiveVoucher>(Condition.DownedGoblinArmy, Condition.MoonPhases04)
                .Add<OddVoucher>(Condition.DownedGoblinArmy, Condition.MoonPhases15)
                .Add<UnbreakableVoucher>(Condition.DownedGoblinArmy, Condition.MoonPhases26)
                .Add<HurriedVoucher>(Condition.DownedGoblinArmy, Condition.MoonPhases37)
                .Add<TrustyOldRod>()
                .Add<GluttonyBlender>()
                .Add<GreedPot>()
                .Add<FishStocks>()
                .Add<TheGift>()
                .Add<TheMonument>(Condition.Hardmode)
                .Add<TheHousingContract>(Condition.Hardmode)
                .Add<OmniGun>(Condition.DownedGolem)
                .Add<GladiatorsLocket>(new Condition(CalamityUtils.GetText("Condition.HasFoundGladiatorsLocket"), () => RecipeUnlockHandler.HasFoundGladiatorsLocket && Main.moonPhase == 0))
                .Add<LuxorsGift>(new Condition(CalamityUtils.GetText("Condition.HasFoundLuxorsGift"), () => RecipeUnlockHandler.HasFoundLuxorsGift && Main.moonPhase == 1))
                .Add<CrimsonEffigy>(new Condition(CalamityUtils.GetText("Condition.HasFoundCrimsonEffigy"), () => RecipeUnlockHandler.HasFoundCrimsonEffigy && Main.moonPhase == 2))
                .Add<FungalSymbiote>(new Condition(CalamityUtils.GetText("Condition.HasFoundFungalSymbiote"), () => RecipeUnlockHandler.HasFoundFungalSymbiote && Main.moonPhase == 3))
                .Add<UnstableGraniteCore>(new Condition(CalamityUtils.GetText("Condition.HasFoundUnstableGraniteCore"), () => RecipeUnlockHandler.HasFoundUnstableGraniteCore && Main.moonPhase == 4))
                .Add<FrozenCube>(new Condition(CalamityUtils.GetText("Condition.HasFoundFrozenCube"), () => RecipeUnlockHandler.HasFoundFrozenCube && Main.moonPhase == 5))
                .Add<CorruptionEffigy>(new Condition(CalamityUtils.GetText("Condition.HasFoundCorruptionEffigy"), () => RecipeUnlockHandler.HasFoundCorruptionEffigy && Main.moonPhase == 6))
                .Add<TrinketofChi>(new Condition(CalamityUtils.GetText("Condition.HasFoundTrinketOfChi"), () => RecipeUnlockHandler.HasFoundTrinketOfChi && Main.moonPhase == 7))
                .Add<HeartofDarkness>(Condition.DownedSkeletron, Condition.MoonPhasesEven, CalamityConditions.InRevengeanceMode)
                .Add<StressPills>(Condition.DownedSkeletron, Condition.MoonPhasesOdd, CalamityConditions.InRevengeanceMode)

                .Register();
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.gfxOffY = -2f;

            if (NPC.ai[0] == 12f) // Attacking
            {
                NPC.spriteDirection = NPC.direction;

                NPC.frameCounter = 0;
                NPC.frame.Y = frameHeight * 9;
            }

            else if (NPC.velocity.Y == 0f)
            {
                if (!NPC.IsABestiaryIconDummy)
                {
                    if (NPC.direction == 1)
                        NPC.spriteDirection = 1;
                    else if (NPC.direction == -1)
                        NPC.spriteDirection = -1;

                    if (NPC.velocity.X == 0f)
                    {
                        NPC.frame.Y = 0;
                        NPC.frameCounter = 0.0;
                        return;
                    }
                }
                NPC.frameCounter += NPC.IsABestiaryIconDummy ? 0.6f : Math.Abs(NPC.velocity.X) * 0.25f;
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter > 12.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y / frameHeight >= Main.npcFrameCount[Type] - 1)
                    NPC.frame.Y = frameHeight;
            }

            else
            {
                NPC.frameCounter = 0.0;
                NPC.frame.Y = 0;
            }
        }

        public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // Manually draw gun frame when attacking
            if (NPC.ai[0] == 12f && !NPC.IsABestiaryIconDummy)
            {
                Texture2D gunTex = TextureAssets.Item[ModContent.ItemType<ElephantKiller>()].Value;
                int holdoutOffset = -2;
                Vector2 origin = NPC.spriteDirection == -1 ? new Vector2(gunTex.Width + holdoutOffset, gunTex.Height / 2f) : new Vector2(-holdoutOffset, gunTex.Height / 2f);
                Vector2 drawPos = NPC.Center - screenPos + new Vector2(0f, 8f);
                float rotation = NPC.ai[2] * ((float)Math.PI / 2f) * NPC.spriteDirection;

                spriteBatch.Draw(gunTex, drawPos, null, drawColor, rotation, origin, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            }
        }

        //Pulled and modified from Vanilla, originally is used to spawn the Travelling Merchant
        internal static void SpawnTravelNPC(int npcToSpawnNextTo)
        {
            int bestX = Main.npc[npcToSpawnNextTo].homeTileX;
            int bestY = Main.npc[npcToSpawnNextTo].homeTileY;
            int minValue = bestX;
            int num3 = bestX;
            int num4 = bestY;
            int num5 = bestX;
            while (num5 > bestX - 10 && (WorldGen.SolidTile(num5, num4) || Main.tileSolidTop[Main.tile[num5, num4].TileType]) && (!Main.tile[num5, num4 - 1].IsTileSolid() || !Main.tileSolid[Main.tile[num5, num4 - 1].TileType] || Main.tileSolidTop[Main.tile[num5, num4 - 1].TileType]) && (!Main.tile[num5, num4 - 2].IsTileSolid() || !Main.tileSolid[Main.tile[num5, num4 - 2].TileType] || Main.tileSolidTop[Main.tile[num5, num4 - 2].TileType]) && (!Main.tile[num5, num4 - 3].IsTileSolid() || !Main.tileSolid[Main.tile[num5, num4 - 3].TileType] || Main.tileSolidTop[Main.tile[num5, num4 - 3].TileType]))
            {
                minValue = num5;
                num5--;
            }
            for (int k = bestX; k < bestX + 10 && (WorldGen.SolidTile(k, num4) || Main.tileSolidTop[Main.tile[k, num4].TileType]) && (!Main.tile[k, num4 - 1].IsTileSolid() || !Main.tileSolid[Main.tile[k, num4 - 1].TileType] || Main.tileSolidTop[Main.tile[k, num4 - 1].TileType]) && (!Main.tile[k, num4 - 2].IsTileSolid() || !Main.tileSolid[Main.tile[k, num4 - 2].TileType] || Main.tileSolidTop[Main.tile[k, num4 - 2].TileType]) && (!Main.tile[k, num4 - 3].IsTileSolid() || !Main.tileSolid[Main.tile[k, num4 - 3].TileType] || Main.tileSolidTop[Main.tile[k, num4 - 3].TileType]); k++)
            {
                num3 = k;
            }
            for (int l = 0; l < 30; l++)
            {
                int num6 = Main.rand.Next(minValue, num3 + 1);
                if (l < 20)
                {
                    if (num6 < bestX - 1 || num6 > bestX + 1)
                    {
                        bestX = num6;
                        break;
                    }
                }
                else if (num6 != bestX)
                {
                    bestX = num6;
                    break;
                }
            }
            int num7 = bestX;
            int num8 = bestY;
            bool flag = false;
            if (!flag && !((double)num8 > Main.worldSurface))
            {
                for (int m = 20; m < 500; m++)
                {
                    for (int n = 0; n < 2; n++)
                    {
                        num7 = ((n != 0) ? (bestX - m * 2) : (bestX + m * 2));
                        if (num7 > 10 && num7 < Main.maxTilesX - 10)
                        {
                            int num9 = bestY - m;
                            double num10 = bestY + m;
                            if (num9 < 10)
                            {
                                num9 = 10;
                            }
                            if (num10 > Main.worldSurface)
                            {
                                num10 = Main.worldSurface;
                            }
                            for (int num11 = num9; (double)num11 < num10; num11++)
                            {
                                num8 = num11;
                                if (!Main.tile[num7, num8].IsTileSolid() || !Main.tileSolid[Main.tile[num7, num8].TileType])
                                {
                                    continue;
                                }
                                if (Main.tile[num7, num8 - 3].LiquidType != LiquidID.Water || Main.tile[num7, num8 - 2].LiquidType != LiquidID.Water || Main.tile[num7, num8 - 1].LiquidType != LiquidID.Water || Collision.SolidTiles(num7 - 1, num7 + 1, num8 - 3, num8 - 1))
                                {
                                    break;
                                }
                                flag = true;
                                Rectangle value = new Rectangle(num7 * 16 + 8 - NPC.sWidth / 2 - NPC.safeRangeX, num8 * 16 + 8 - NPC.sHeight / 2 - NPC.safeRangeY, NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
                                for (int num12 = 0; num12 < 255; num12++)
                                {
                                    if (Main.player[num12].active && new Rectangle((int)Main.player[num12].position.X, (int)Main.player[num12].position.Y, Main.player[num12].width, Main.player[num12].height).Intersects(value))
                                    {
                                        flag = false;
                                        break;
                                    }
                                }
                                break;
                            }
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (flag)
                    {
                        break;
                    }
                }
            }
            int myIndex = NPC.NewNPC(NPC.GetSpawnSourceForTownSpawn(), num7 * 16, num8 * 16, ModContent.NPCType<ShadySalesman>(), 1);
            Main.npc[myIndex].homeTileX = bestX;
            Main.npc[myIndex].homeTileY = bestY;
            Main.npc[myIndex].homeless = true;
            if (num7 < bestX)
            {
                Main.npc[myIndex].direction = 1;
            }
            else if (num7 > bestX)
            {
                Main.npc[myIndex].direction = -1;
            }
            Main.npc[myIndex].netUpdate = true;
            string fullName = Main.npc[myIndex].FullName;
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue("Announcement.HasArrived", fullName), 50, 125);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasArrived", Main.npc[myIndex].GetFullNetName()), new Color(50, 125, 255));
            }
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 10;
            knockback = 1f;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<ShadySalesmanGunshot>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 8f;
            gravityCorrection = 0f;
            randomOffset = 0f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 75;
            randExtraCooldown = 10;
        }

        // Spawn the effects used for Tinkerer's Voucher failing when below 1 life
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                for (int d = 0; d < 10; d++)
                {
                    for (int smokeCount = 0; smokeCount < 3; smokeCount++)
                    {
                        Vector2 velocity = Main.rand.NextVector2CircularEdge(1f, 1f) * Main.rand.NextFloat(3f, 12f);
                        Color smokeStart = Main.rand.NextBool() ? Color.Gray : Color.LightGray;
                        Color smokeEnd = Color.DimGray;
                        float smokeSize = Main.rand.NextFloat(0.9f, 2f);

                        Particle smoke = new SmallSmokeParticle(NPC.Center, velocity, smokeStart, smokeEnd, smokeSize, Main.rand.Next(90, 140));
                        GeneralParticleHandler.SpawnParticle(smoke);
                    }

                    Particle skull = new DesertProwlerSkullParticle(NPC.Center, new Vector2(2.5f, 2.5f).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1f), Main.rand.NextBool() ? Color.LightGray : Color.Silver, Color.Gray, Main.rand.NextFloat(0.15f, 0.5f), Main.rand.Next(100, 190));
                    GeneralParticleHandler.SpawnParticle(skull);
                }
                SoundEngine.PlaySound(SoundID.NPCDeath6, NPC.Center);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            var hardmode = npcLoot.DefineConditionalDropSet(DropHelper.Hardmode());
            hardmode.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, ModContent.ItemType<ElephantKiller>()));

        }

        public override void ModifyActiveShop(string shopName, Item[] items)
        {
            ShadySalesmanShopTooltips.addSalesPitch = true;
        }

        public override void ResetEffects()
        {
            if (Main.npcShop < 1) //ModifyActiveShop only runs once when shop opens, so we only reset this if no shop is open
                ShadySalesmanShopTooltips.addSalesPitch = false;
        }
    }

    public class ShadySalesmanSpawnSystem : ModSystem
    {
        private static bool CanSpawnTonight = true;

        public override void PreUpdateWorld()
        {
            if (!DownedBossSystem.downedHorribleHog) // Cannot spawn before Horrible Hog has been killed.
                return;

            if (Main.dayTime)
            {
                CanSpawnTonight = true;
                return;
            }

            if (!CanSpawnTonight || Main.eclipse || (Main.invasionType > 0 && Main.invasionDelay == 0 && Main.invasionSize > 0))
                return;

            for (int i = 0; i < 200; i++)
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<ShadySalesman>())
                    return;

            List<int> townNPCs = [];
            if (!NPC.AnyNPCs(ModContent.NPCType<TownPiggy>()))
            {
                for (int j = 0; j < 200; j++)
                    if (Main.npc[j].active && Main.npc[j].townNPC && Main.npc[j].type != NPCID.OldMan && !Main.npc[j].homeless)
                        townNPCs.Add(j);

                if (townNPCs.Count <= 1)
                    return;
            }

            int petPig = NPC.FindFirstNPC(ModContent.NPCType<TownPiggy>());
            if (petPig == -1)
            {
                if (!Main.rand.NextBool(4))
                {
                    CanSpawnTonight = false;
                    return;
                }
                ShadySalesman.SpawnTravelNPC(townNPCs[Main.rand.Next(townNPCs.Count)]);
            }
            else
            {
                string name = Main.npc[petPig].GivenName;
                Main.npc[petPig].Transform(ModContent.NPCType<ShadySalesman>());
                Main.npc[petPig].GivenName = name;

                string fullName = Main.npc[petPig].FullName;
            }
            CanSpawnTonight = false;
        }
    }

    public class ShadySalesmanShopTooltips : GlobalItem
    {
        /// <summary>
        /// Scale for the "fine print" of the shop.
        /// </summary>
        public static float SmallTextSize => 0.8f;
        /// <summary>
        /// Tooltip Line names that should be left alone instead of becoming fine print
        /// All exempted tooltip lines are moved above the fine print section
        /// </summary>
        static List<string> ExemptFromSmall => [
            "ItemName",
            "SalesPitch",
            "Price",
            "SpecialPrice",
            "JourneyResearch",
            ];
        public override void Load()
        {
            On_ChatManager.DrawColorCodedStringShadow_SpriteBatch_DynamicSpriteFont_TextSnippetArray_Vector2_Color_float_Vector2_Vector2_float_float += HideDefaultShadowForSmallText;
        }

        // Doze May 30 2026
        // Terraria forces drawing the shadow effects for all tooltip texts.
        // This system forces the shadows to be transparent for SmallTextSnippets as it draws the shadows manually
        private void HideDefaultShadowForSmallText(On_ChatManager.orig_DrawColorCodedStringShadow_SpriteBatch_DynamicSpriteFont_TextSnippetArray_Vector2_Color_float_Vector2_Vector2_float_float orig, SpriteBatch spriteBatch, ReLogic.Graphics.DynamicSpriteFont font, TextSnippet[] snippets, Vector2 position, Color baseColor, float rotation, Vector2 origin, Vector2 baseScale, float maxWidth, float spread)
        {
            List<Color> colorsToRestore = [];
            foreach (var item in snippets)
            {
                if (item is SmallTextSnippet s)
                {
                    colorsToRestore.Add(s.Color);
                    s.Color = Color.Transparent;
                }
            }
            orig(spriteBatch, font, snippets, position, baseColor, rotation, origin, baseScale, maxWidth, spread);
            if (colorsToRestore.Count > 0)
                foreach (var item in snippets)
                {
                    if (item is SmallTextSnippet s)
                    {
                        s.Color = colorsToRestore[0];
                        colorsToRestore.RemoveAt(0);
                    }
                }
        }

        public static bool addSalesPitch = false; //Used to track if the shop should show the pitches

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!addSalesPitch || !CalamityItemSets.HasSalesmanText[item.type])
                return;
            tooltips.Insert(1, new TooltipLine(Mod, "SalesPitch", item.ModItem?.GetLocalizedValue("SalesPitch") ?? "Error: Sales Pitches not supported for vanilla items") // While vanilla item support COULD be added, salesman won't need sales pitches for vanilla items ever and so this is fine.
            {
            });

            int placetoMove = -1;

            for (var i = 0; i < tooltips.Count; i++)
            {
                var line = tooltips[i];
                if (!ExemptFromSmall.Contains(line.Name))
                {
                    if (placetoMove < 0)
                        placetoMove = i;
                    TextSnippet[] snippets = ChatManager.ParseMessage(line.Text, Color.White).ToArray();
                    ChatManager.ConvertNormalSnippets(snippets);

                    string lineText = "";
                    for (int i2 = 0; i2 < snippets.Length; i2++)
                    {
                        lineText += snippets[i2].Text;
                    }

                    if (string.IsNullOrWhiteSpace(lineText))
                        //Sometimes the game tries to draw entirely empty lines with no size.
                        //This would cause no issue except for it still applies the Y offset those lines
                        //So, we just force hide those lines entirely here
                        line.Hide(); 
                    else
                    {
                        string[] lines = lineText.Split('\n');
                        tooltips.RemoveAt(i);

                        for (int k = 0; k < lines.Length; k++)
                        {
                            if (string.IsNullOrWhiteSpace(lines[k]))
                                continue;

                            // Create a distinct TooltipLine for each line of text
                            TooltipLine newLine = new TooltipLine(Mod, $"{line.Name}_{k}", $"[scale/{SmallTextSize}:{lines[k]}]")
                            {
                                OverrideColor = line.OverrideColor
                            };

                            // Insert it back into the list at the current position
                            tooltips.Insert(i, newLine);
                            i++;
                        }

                        i--;
                    }

                }
                else if (placetoMove > 0)
                {
                    tooltips.RemoveAt(i);
                    tooltips.Insert(placetoMove, line);
                    placetoMove++;
                }

            }
        }

        public override bool PreDrawTooltipLine(Item item, DrawableTooltipLine line, ref int yOffset)
        {
            //Applies the Y offset to lines that are small to ensure they line up properly visually.
            //Without this, they'd stay spaced the same as normal text...
            //although the tooltip background would still size as if they're stacked closer like this does. Thanks vanilla code jank.
            if (line.Text.StartsWith($"[scale/{SmallTextSize}:"))
            {
                yOffset = -(int)(FontAssets.MouseText.Value.MeasureString(" ") * (1 - SmallTextSize)).Y - 1;
            }
            return base.PreDrawTooltipLine(item, line, ref yOffset);
        }
    }
}
