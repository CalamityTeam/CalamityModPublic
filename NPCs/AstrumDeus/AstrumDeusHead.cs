using System.IO;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.CalamityAIs.CalamityBossAIs;
using CalamityMod.UI.VanillaBossBars;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AstrumDeus
{
    [AutoloadBossHead]
    [LongDistanceNetSync]
    public class AstrumDeusHead : ModNPC
    {
        public static readonly SoundStyle SpawnSound = new("CalamityMod/Sounds/Custom/AstrumDeus/AstrumDeusSpawn");
        public static readonly SoundStyle LaserSound = new("CalamityMod/Sounds/Custom/AstrumDeus/AstrumDeusLaser") { Volume = 0.35f };
        public static readonly SoundStyle GodRaySound = new("CalamityMod/Sounds/Custom/AstrumDeus/AstrumDeusGodRay") { Volume = 0.4f };
        public static readonly SoundStyle MineSound = new("CalamityMod/Sounds/Custom/AstrumDeus/AstrumDeusMine") { Volume = 0.4f };
        public static readonly SoundStyle SplitSound = new("CalamityMod/Sounds/Custom/AstrumDeus/AstrumDeusSplit");
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/AstrumDeusHit", 2) { Volume = 0.7f };
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/AstrumDeusDeath");

        public static Asset<Texture2D> TextureGlow1;
        public static Asset<Texture2D> TextureGlow2;
        public static Asset<Texture2D> TextureGlow3;
        public static Asset<Texture2D> TextureGlow4;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.70f,
                PortraitScale = 0.75f,
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/AstrumDeus_Bestiary"
            };
            value.Position.X += 55f;
            value.Position.Y += 23f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            if (!Main.dedServ)
            {
                TextureGlow1 = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
                TextureGlow2 = ModContent.Request<Texture2D>(Texture + "Glow2", AssetRequestMode.AsyncLoad);
                TextureGlow3 = ModContent.Request<Texture2D>(Texture + "Glow3", AssetRequestMode.AsyncLoad);
                TextureGlow4 = ModContent.Request<Texture2D>(Texture + "Glow4", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.npcSlots = 5f;
            NPC.width = 56;
            NPC.height = 56;
            NPC.defense = 20;
            NPC.DR_NERD(0.1f);
            NPC.LifeMaxNERB(200000, 240000, 650000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;

            if (BossRushEvent.BossRushActive)
                NPC.scale *= 1.5f;
            else if (CalamityWorld.death)
                NPC.scale *= 1.4f;
            else if (CalamityWorld.revenge)
                NPC.scale *= 1.35f;
            else if (Main.expertMode)
                NPC.scale *= 1.2f;

            NPC.boss = true;
            NPC.BossBar = ModContent.GetInstance<AstrumDeusBossBar>();
            NPC.value = Item.buyPrice(1, 0, 0, 0);
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = HitSound;
            NPC.DeathSound = DeathSound;
            NPC.netAlways = true;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<BiomeManagers.AstralInfectionBiome>().Type };

            if (Main.zenithWorld)
            {
                if (CalamityWorld.death) // killing 10 worms with half of the og's health is ridiculous
                    NPC.lifeMax /= 3;

                NPC.value /= 5;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.AstrumDeus")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.dontTakeDamage);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.dontTakeDamage = reader.ReadBoolean();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            AstrumDeusAI.VanillaAstrumDeusAI(NPC, Mod, true);
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            bool drawCyan = NPC.Calamity().newAI[3] >= (Main.getGoodWorld ? 300f : 600f);
            bool deathModeEnragePhase = NPC.Calamity().newAI[0] == 3f;
            bool doubleWormPhase = NPC.Calamity().newAI[0] != 0f && !deathModeEnragePhase;

            Texture2D mainWormTex = TextureAssets.Npc[NPC.type].Value;
            Texture2D secondWormTex = TextureGlow2.Value;
            Vector2 halfSizeTex = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / 2);

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(mainWormTex.Width, mainWormTex.Height) * NPC.scale / 2f;
            drawLocation += halfSizeTex * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(mainWormTex, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTex, NPC.scale, spriteEffects, 0f);

            mainWormTex = TextureGlow1.Value;
            Color phaseColor = drawCyan ? Color.Cyan : Color.Orange;
            if (doubleWormPhase)
            {
                mainWormTex = drawCyan ? mainWormTex : TextureGlow3.Value;
                secondWormTex = drawCyan ? TextureGlow4.Value : secondWormTex;
            }
            Color mainWormColorLerp = Color.Lerp(Color.White, doubleWormPhase ? phaseColor : Color.Cyan, 0.5f) * (deathModeEnragePhase ? 1f : NPC.Opacity);
            Color secondWormColorLerp = Color.Lerp(Color.White, doubleWormPhase ? phaseColor : Color.Orange, 0.5f) * (deathModeEnragePhase ? 1f : NPC.Opacity);

            int timesToDraw = deathModeEnragePhase ? 3 : drawCyan ? 1 : 2;
            for (int i = 0; i < timesToDraw; i++)
                spriteBatch.Draw(mainWormTex, drawLocation, NPC.frame, mainWormColorLerp, NPC.rotation, halfSizeTex, NPC.scale, spriteEffects, 0f);

            timesToDraw = deathModeEnragePhase ? 3 : drawCyan ? 2 : 1;
            for (int i = 0; i < timesToDraw; i++)
                spriteBatch.Draw(secondWormTex, drawLocation, NPC.frame, secondWormColorLerp, NPC.rotation, halfSizeTex, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (!Main.zenithWorld && Main.rand.NextBool(5)) // I value people's computers
                {
                    NPC.position.X = NPC.position.X + (NPC.width / 2);
                    NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                    NPC.width = 50;
                    NPC.height = 50;
                    NPC.position.X = NPC.position.X - (NPC.width / 2);
                    NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                    for (int i = 0; i < 5; i++)
                    {
                        int purpleDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                        Main.dust[purpleDust].velocity *= 3f;
                        if (Main.rand.NextBool())
                        {
                            Main.dust[purpleDust].scale = 0.5f;
                            Main.dust[purpleDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int j = 0; j < 10; j++)
                    {
                        int astralDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 3f);
                        Main.dust[astralDust].noGravity = true;
                        Main.dust[astralDust].velocity *= 5f;
                        astralDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 2f);
                        Main.dust[astralDust].velocity *= 2f;
                    }
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType) => potionType = ModContent.ItemType<StarblightSoot>();

        public static bool ShouldNotDropThings(NPC npc) => npc.Calamity().newAI[0] == 0f || ((CalamityWorld.death || BossRushEvent.BossRushActive) && npc.Calamity().newAI[0] != 3f);

        public override bool SpecialOnKill()
        {
            if (ShouldNotDropThings(NPC))
                return false;

            int closestSegmentID = DropHelper.FindClosestWormSegment(NPC,
                ModContent.NPCType<AstrumDeusHead>(),
                ModContent.NPCType<AstrumDeusBody>(),
                ModContent.NPCType<AstrumDeusTail>());
            NPC.position = Main.npc[closestSegmentID].position;

            return false;
        }

        public override void OnKill()
        {
            if (ShouldNotDropThings(NPC))
                return;

            // Killing ANY split Deus makes all other Deus heads die immediately.
            foreach (NPC otherWormHead in Main.ActiveNPCs)
            {
                if (otherWormHead.type == NPC.type)
                {
                    // Kill the other worm head after setting it to not drop loot.
                    otherWormHead.Calamity().newAI[0] = 0f;
                    otherWormHead.life = 0;
                    otherWormHead.checkDead();
                    otherWormHead.netUpdate = true;
                }
            }

            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // Notify players that Astral Ore can be mined if Deus has never been killed yet
            if (!DownedBossSystem.downedAstrumDeus)
            {
                string key = "Mods.CalamityMod.Status.Progression.AstralBossText";
                Color messageColor = Color.Gold;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }

            // Mark Astrum Deus as dead
            DownedBossSystem.downedAstrumDeus = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            var lastWorm = npcLoot.DefineConditionalDropSet(info => !ShouldNotDropThings(info.npc));
            lastWorm.Add(ItemDropRule.BossBag(ModContent.ItemType<AstrumDeusBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = new LeadingConditionRule(new Conditions.NotExpert());
            lastWorm.Add(normalOnly);
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<TheMicrowave>(),
                    ModContent.ItemType<StarSputter>(),
                    ModContent.ItemType<StarShower>(),
                    ModContent.ItemType<StarspawnHelixStaff>(),
                    ModContent.ItemType<RegulusRiot>(),
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));

                // Vanity
                normalOnly.Add(ModContent.ItemType<AstrumDeusMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<HideofAstrumDeus>()));
                normalOnly.Add(ModContent.ItemType<ChromaticOrb>(), 5);

                // Materials
                normalOnly.Add(ItemID.FallenStar, 1, 25, 40);
                normalOnly.Add(ModContent.ItemType<StarblightSoot>(), 1, 50, 80);
            }

            npcLoot.DefineConditionalDropSet(() => true).Add(DropHelper.PerPlayer(ItemID.SuperHealingPotion, 1, 5, 15), hideLootReport: true); // Healing Potions don't show up in the Bestiary
            lastWorm.Add(ModContent.ItemType<AstrumDeusTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).AddIf((info) => !ShouldNotDropThings(info.npc), ModContent.ItemType<AstrumDeusRelic>());

            // Fragments
            lastWorm.Add(DropHelper.NormalVsExpertQuantity(ItemID.FragmentSolar, 1, 16, 24, 20, 32));
            lastWorm.Add(DropHelper.NormalVsExpertQuantity(ItemID.FragmentVortex, 1, 16, 24, 20, 32));
            lastWorm.Add(DropHelper.NormalVsExpertQuantity(ItemID.FragmentNebula, 1, 16, 24, 20, 32));
            lastWorm.Add(DropHelper.NormalVsExpertQuantity(ItemID.FragmentStardust, 1, 16, 24, 20, 32));
            lastWorm.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<MeldBlob>(), 1, 16, 24, 20, 32));

            // GFB Worm and Spaghetti drop
            var GFBOnly = npcLoot.DefineConditionalDropSet(DropHelper.GFB);
            {
                GFBOnly.Add(ItemID.Worm, 1, 1, 9999, true);
                GFBOnly.Add(ItemID.CanOfWorms, 1, 1, 9999, true);
                GFBOnly.Add(ItemID.GummyWorm, 1, 1, 9999, true);
                GFBOnly.Add(ItemID.TruffleWorm, 1, 1, 9999, true);
                GFBOnly.Add(ItemID.EnchantedNightcrawler, 1, 1, 9999, true);
                GFBOnly.Add(ItemID.Spaghetti, 1, 1, 9999, true);
            }

            // Lore
            bool firstDeusKill(DropAttemptInfo info) => !DownedBossSystem.downedAstrumDeus && !ShouldNotDropThings(info.npc);
            npcLoot.AddConditionalPerPlayer(firstDeusKill, ModContent.ItemType<LoreAstrumDeus>(), desc: DropHelper.FirstKillText);
            npcLoot.AddConditionalPerPlayer(firstDeusKill, ModContent.ItemType<LoreAstralInfection>(), desc: DropHelper.FirstKillText);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 200, true);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }
    }
}
