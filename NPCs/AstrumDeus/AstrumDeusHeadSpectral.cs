using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AstrumDeus
{
    [AutoloadBossHead]
    public class AstrumDeusHeadSpectral : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astrum Deus");
            NPCID.Sets.TrailingMode[NPC.type] = 1;
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

            if (CalamityWorld.malice || BossRushEvent.BossRushActive)
                NPC.scale = 1.5f;
            else if (CalamityWorld.death)
                NPC.scale = 1.4f;
            else if (CalamityWorld.revenge)
                NPC.scale = 1.35f;
            else if (Main.expertMode)
                NPC.scale = 1.2f;

            NPC.boss = true;
            NPC.value = Item.buyPrice(1, 0, 0, 0);
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = Mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/AstrumDeusDeath");
            NPC.netAlways = true;
            music = CalamityMod.Instance.GetMusicFromMusicMod("AstrumDeus") ?? MusicID.Boss3;
            bossBag = ModContent.ItemType<AstrumDeusBag>();
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
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
            CalamityAI.AstrumDeusAI(NPC, Mod, true);
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            bool drawCyan = NPC.Calamity().newAI[3] >= 600f;
            bool deathModeEnragePhase = NPC.Calamity().newAI[0] == 3f;
            bool doubleWormPhase = NPC.Calamity().newAI[0] != 0f && !deathModeEnragePhase;

            Texture2D texture2D15 = Main.npcTexture[NPC.type];
            Texture2D texture2D16 = ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumDeus/AstrumDeusHeadGlow2");
            Vector2 vector11 = new Vector2(Main.npcTexture[NPC.type].Width / 2, Main.npcTexture[NPC.type].Height / 2);
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = deathModeEnragePhase ? 10 : 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = lightColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2(NPC.width, NPC.height) / 2f - Main.screenPosition;
                    vector41 -= new Vector2(texture2D15.Width, texture2D15.Height) * NPC.scale / 2f;
                    vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = NPC.Center - Main.screenPosition;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(lightColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumDeus/AstrumDeusHeadGlow");
            Color phaseColor = drawCyan ? Color.Cyan : Color.Orange;
            if (doubleWormPhase)
            {
                texture2D15 = drawCyan ? texture2D15 : ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumDeus/AstrumDeusHeadGlow3");
                texture2D16 = drawCyan ? ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumDeus/AstrumDeusHeadGlow4") : texture2D16;
            }
            Color color37 = Color.Lerp(Color.White, doubleWormPhase ? phaseColor : Color.Cyan, 0.5f) * (deathModeEnragePhase ? 1f : NPC.Opacity);
            Color color42 = Color.Lerp(Color.White, doubleWormPhase ? phaseColor : Color.Orange, 0.5f) * (deathModeEnragePhase ? 1f : NPC.Opacity);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num163 = 1; num163 < num153; num163++)
                {
                    Color color41 = color37;
                    color41 = Color.Lerp(color41, color36, amount9);
                    color41 *= (num153 - num163) / 15f;
                    Vector2 vector44 = NPC.oldPos[num163] + new Vector2(NPC.width, NPC.height) / 2f - Main.screenPosition;
                    vector44 -= new Vector2(texture2D15.Width, texture2D15.Height) * NPC.scale / 2f;
                    vector44 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector44, NPC.frame, color41, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

                    Color color43 = color42;
                    color43 = Color.Lerp(color43, color36, amount9);
                    color43 *= (num153 - num163) / 15f;
                    spriteBatch.Draw(texture2D16, vector44, NPC.frame, color43, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            int timesToDraw = deathModeEnragePhase ? 3 : drawCyan ? 1 : 2;
            for (int i = 0; i < timesToDraw; i++)
                spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            timesToDraw = deathModeEnragePhase ? 3 : drawCyan ? 2 : 1;
            for (int i = 0; i < timesToDraw; i++)
                spriteBatch.Draw(texture2D16, vector43, NPC.frame, color42, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 50;
                NPC.height = 50;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int num621 = 0; num621 < 5; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 10; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType) => potionType = ModContent.ItemType<Stardust>();

        public override bool SpecialNPCLoot()
        {
            if (NPC.Calamity().newAI[0] == 0f || ((CalamityWorld.death || BossRushEvent.BossRushActive) && NPC.Calamity().newAI[0] != 3f))
                return false;

            int closestSegmentID = DropHelper.FindClosestWormSegment(NPC,
                ModContent.NPCType<AstrumDeusHeadSpectral>(),
                ModContent.NPCType<AstrumDeusBodySpectral>(),
                ModContent.NPCType<AstrumDeusTailSpectral>());
            NPC.position = Main.npc[closestSegmentID].position;

            return false;
        }

        public override bool PreNPCLoot()
        {
            // Unsplit Deus does not drop anything when killed/despawned.
            if (NPC.Calamity().newAI[0] == 0f || ((CalamityWorld.death || BossRushEvent.BossRushActive) && NPC.Calamity().newAI[0] != 3f))
                return false;

            // Killing ANY split Deus makes all other Deus heads die immediately.
            for (int i = 0; i < Main.maxNPCs; ++i)
            {
                NPC otherWormHead = Main.npc[i];
                if (otherWormHead.active && otherWormHead.type == NPC.type)
                {
                    // Kill the other worm head after setting it to not drop loot.
                    otherWormHead.Calamity().newAI[0] = 0f;
                    otherWormHead.life = 0;
                    otherWormHead.checkDead();
                    otherWormHead.netUpdate = true;
                }
            }

            return true;
        }

        public override void NPCLoot()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            DropHelper.DropBags(NPC);

            DropHelper.DropItem(NPC, ItemID.GreaterHealingPotion, 8, 14);
            DropHelper.DropItemChance(NPC, ModContent.ItemType<AstrumDeusTrophy>(), 10);
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<KnowledgeAstrumDeus>(), !DownedBossSystem.downedStarGod);
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<KnowledgeAstralInfection>(), !DownedBossSystem.downedStarGod);

            // Drop a large spray of all 4 lunar fragments
            int minFragments = Main.expertMode ? 20 : 16;
            int maxFragments = Main.expertMode ? 32 : 24;
            DropHelper.DropItemSpray(NPC, ItemID.FragmentSolar, minFragments, maxFragments, 4);
            DropHelper.DropItemSpray(NPC, ItemID.FragmentVortex, minFragments, maxFragments, 4);
            DropHelper.DropItemSpray(NPC, ItemID.FragmentNebula, minFragments, maxFragments, 4);
            DropHelper.DropItemSpray(NPC, ItemID.FragmentStardust, minFragments, maxFragments, 4);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                DropHelper.DropItemSpray(NPC, ModContent.ItemType<Stardust>(), 50, 80, 5);
                DropHelper.DropItemSpray(NPC, ItemID.FallenStar, 25, 40, 5);

                // Weapons
                float w = DropHelper.NormalWeaponDropRateFloat;
                DropHelper.DropEntireWeightedSet(NPC,
                    DropHelper.WeightStack<TheMicrowave>(w),
                    DropHelper.WeightStack<StarSputter>(w),
                    DropHelper.WeightStack<Starfall>(w),
                    DropHelper.WeightStack<GodspawnHelixStaff>(w),
                    DropHelper.WeightStack<RegulusRiot>(w)
                );

                // Equipment
                DropHelper.DropItem(NPC, ModContent.ItemType<HideofAstrumDeus>(), true);
                DropHelper.DropItemChance(NPC, ModContent.ItemType<ChromaticOrb>(), 5);

                // Vanity
                DropHelper.DropItemChance(NPC, ModContent.ItemType<AstrumDeusMask>(), 7);
            }

            // Notify players that Astral Ore can be mined if Deus has never been killed yet
            if (!DownedBossSystem.downedStarGod)
            {
                string key = "Mods.CalamityMod.AstralBossText";
                Color messageColor = Color.Gold;
                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }

            // Mark Astrum Deus as dead
            DownedBossSystem.downedStarGod = true;
            CalamityNetcode.SyncWorld();
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240, true);
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
        }
    }
}
