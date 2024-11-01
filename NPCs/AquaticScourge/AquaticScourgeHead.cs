using System.IO;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.CalamityAIs.CalamityBossAIs;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AquaticScourge
{
    [AutoloadBossHead]
    [LongDistanceNetSync]
    public class AquaticScourgeHead : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.6f,
                PortraitScale = 0.6f,
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/AquaticScourge_Bestiary"
            };
            value.Position.X += 40f;
            value.Position.Y += 20f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.GetNPCDamage();
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.width = 90;
            NPC.height = 90;
            NPC.defense = 10;
            NPC.DR_NERD(0.05f);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.LifeMaxNERB(80000, 96000, 1000000);
            if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                NPC.lifeMax *= 2;

            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 40, 0, 0);
            NPC.behindTiles = true;
            NPC.chaseable = false;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;

            if (BossRushEvent.BossRushActive)
                NPC.scale *= 1.25f;
            else if (CalamityWorld.death)
                NPC.scale *= 1.2f;
            else if (CalamityWorld.revenge)
                NPC.scale *= 1.15f;
            else if (Main.expertMode)
                NPC.scale *= 1.1f;

            if (Main.getGoodWorld)
                NPC.scale *= 1.25f;

            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SulphurousSeaBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new BossBestiaryInfoElement(),
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.AquaticScourge")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.chaseable);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            writer.Write(NPC.npcSlots);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.chaseable = reader.ReadBoolean();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            NPC.npcSlots = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            AquaticScourgeAI.VanillaAquaticScourgeAI(NPC, Mod, true);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 scaledDraw = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / 2);

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture2D15.Width, texture2D15.Height) * NPC.scale / 2f;
            drawLocation += scaledDraw * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            Color color = NPC.GetAlpha(drawColor);

            if (CalamityWorld.revenge || BossRushEvent.BossRushActive || Main.zenithWorld)
            {
                if (NPC.Calamity().newAI[3] > 300f)
                    color = Color.Lerp(color, Color.SandyBrown, MathHelper.Clamp((NPC.Calamity().newAI[3] - 300f) / 180f, 0f, 1f));
                else if (NPC.localAI[3] > 0f)
                    color = Color.Lerp(color, Color.SandyBrown, MathHelper.Clamp(NPC.localAI[3] / 90f, 0f, 1f));
            }

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, color, NPC.rotation, scaledDraw, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Rectangle targetHitbox = target.Hitbox;

            float topLeftHitbox = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float topRightHitbox = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float bottomLeftHitbox = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float bottomRightHitbox = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = topLeftHitbox;
            if (topRightHitbox < minDist)
                minDist = topRightHitbox;
            if (bottomLeftHitbox < minDist)
                minDist = bottomLeftHitbox;
            if (bottomRightHitbox < minDist)
                minDist = bottomRightHitbox;

            return minDist <= 50f * NPC.scale;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
                return NPC.Calamity().newAI[0] == 1f;

            return null;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().disableNaturalScourgeSpawns)
                return 0f;

            if (spawnInfo.PlayerSafe)
                return 0f;

            if (spawnInfo.Player.Calamity().ZoneSulphur && spawnInfo.Water)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<AquaticScourgeHead>()))
                    return (Main.getGoodWorld ? 0.05f : 0.01f);
            }

            return 0f;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<SulphurousSand>();
        }

        public override bool SpecialOnKill()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(NPC,
                ModContent.NPCType<AquaticScourgeHead>(),
                ModContent.NPCType<AquaticScourgeBody>(),
                ModContent.NPCType<AquaticScourgeBodyAlt>(),
                ModContent.NPCType<AquaticScourgeTail>());
            NPC.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Boss bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<AquaticScourgeBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<SubmarineShocker>(),
                    ModContent.ItemType<Barinautical>(),
                    ModContent.ItemType<Downpour>(),
                    ModContent.ItemType<DeepseaStaff>(),
                    ModContent.ItemType<ScourgeoftheSeas>()
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));

                // Vanity
                normalOnly.Add(ModContent.ItemType<AquaticScourgeMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<AquaticEmblem>()));
                normalOnly.Add(ModContent.ItemType<CorrosiveSpine>(), DropHelper.NormalWeaponDropRateFraction);
                normalOnly.Add(ModContent.ItemType<SeasSearing>(), 10);

                // Fishing
                normalOnly.Add(ModContent.ItemType<BleachedAnglingKit>());
            }

            npcLoot.DefineConditionalDropSet(() => true).Add(DropHelper.PerPlayer(ItemID.GreaterHealingPotion, 1, 5, 15), hideLootReport: true); // Healing Potions don't show up in the Bestiary
            npcLoot.Add(ModContent.ItemType<AquaticScourgeTrophy>(), 10);
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<AquaticScourgeRelic>());

            // GFB troll drop
            npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(ModContent.ItemType<SupremeBaitTackleBoxFishingStation>(), hideLootReport: true);

            // Lore
            bool firstASKill() => !DownedBossSystem.downedAquaticScourge;
            npcLoot.AddConditionalPerPlayer(firstASKill, ModContent.ItemType<LoreAquaticScourge>(), desc: DropHelper.FirstKillText);
            npcLoot.AddConditionalPerPlayer(firstASKill, ModContent.ItemType<LoreSulphurSea>(), desc: DropHelper.FirstKillText);
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            CalamityGlobalNPC.SetNewShopVariable(new int[] { ModContent.NPCType<SEAHOE>() }, DownedBossSystem.downedAquaticScourge);

            // If Aquatic Scourge has not yet been killed, notify players of buffed Acid Rain
            if (!DownedBossSystem.downedAquaticScourge)
            {
                if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
                    SoundEngine.PlaySound(Mauler.RoarSound, Main.player[Main.myPlayer].Center);

                string sulfSeaBoostKey = "Mods.CalamityMod.Status.Progression.WetWormBossText";
                Color sulfSeaBoostColor = AcidRainEvent.TextColor;

                CalamityUtils.DisplayLocalizedText(sulfSeaBoostKey, sulfSeaBoostColor);

                // Set a timer for acid rain to start after 10 seconds
                AcidRainEvent.CountdownUntilForcedAcidRain = 601;
            }

            // Mark Aquatic Scourge as dead
            DownedBossSystem.downedAquaticScourge = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);

                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ASHead").Type, NPC.scale);
                }
            }
        }

        public override bool CheckActive()
        {
            if (NPC.Calamity().newAI[0] == 1f && !Main.player[NPC.target].dead && NPC.Calamity().newAI[1] != 1f)
                return false;

            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 480, true);
        }
    }
}
