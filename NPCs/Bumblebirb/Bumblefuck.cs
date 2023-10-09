using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Sounds;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Potions;

namespace CalamityMod.NPCs.Bumblebirb
{
    [AutoloadBossHead]
    public class Bumblefuck : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.5f,
                PortraitScale = 0.85f,
                PortraitPositionYOverride = 14f
            };
            value.Position.X += 20f;
            value.Position.Y += 8f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override string Texture => "CalamityMod/NPCs/Bumblebirb/Birb";
        public override string BossHeadTexture => "CalamityMod/NPCs/Bumblebirb/Birb_Head_Boss";

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 32f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.GetNPCDamage();
            NPC.width = 130;
            NPC.height = 100;
            NPC.defense = 40;
            NPC.DR_NERD(0.1f);
            NPC.LifeMaxNERB(187500, 225000, 300000); // Old HP - 227500, 252500
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noTileCollide = true;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.value = Item.buyPrice(1, 25, 0, 0);
            NPC.HitSound = SoundID.NPCHit51;
            NPC.DeathSound = SoundID.NPCDeath46;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Bumblefuck")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityAI.BumblebirbAI(NPC, Mod);
        }

        public override void FindFrame(int frameHeight)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool phase2 = lifeRatio < (revenge ? 0.75f : 0.5f) || death;
            bool phase3 = lifeRatio < (death ? 0.4f : revenge ? 0.25f : 0.1f);
            bool birbSpawn = NPC.ai[0] == 4f && NPC.ai[1] > 0f;

            // Animation goes nyoom
            if (Main.zenithWorld)
            {
                NPC.frameCounter += 4;
            }

            float newPhaseTimer = 180f;
            bool phaseSwitchPhase = (phase2 && calamityGlobalNPC.newAI[0] < newPhaseTimer && calamityGlobalNPC.newAI[2] != 1f) ||
                (phase3 && calamityGlobalNPC.newAI[1] < newPhaseTimer && calamityGlobalNPC.newAI[3] != 1f);

            if (phaseSwitchPhase || birbSpawn)
            {
                float frameGateValue = birbSpawn ? NPC.ai[1] : phase3 ? calamityGlobalNPC.newAI[1] : calamityGlobalNPC.newAI[0];
                int num116 = 180;
                if (frameGateValue < (num116 - 60) || frameGateValue > (num116 - 20))
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 5D)
                    {
                        NPC.frameCounter = 0D;
                        NPC.frame.Y += frameHeight;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                    {
                        NPC.frame.Y = 0;
                    }
                }
                else
                {
                    NPC.frame.Y = frameHeight * 4;
                    if (frameGateValue > (num116 - 50) && frameGateValue < (num116 - 25))
                    {
                        NPC.frame.Y = frameHeight * 5;
                    }
                }
            }
            else if (NPC.ai[0] == 5f)
            {
                int num115 = 120;
                if (NPC.ai[1] < (num115 - 50) || NPC.ai[1] > (num115 - 10))
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 5D)
                    {
                        NPC.frameCounter = 0D;
                        NPC.frame.Y += frameHeight;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                    {
                        NPC.frame.Y = 0;
                    }
                }
                else
                {
                    NPC.frame.Y = frameHeight * 4;
                    if (NPC.ai[1] > (num115 - 40) && NPC.ai[1] < (num115 - 15))
                    {
                        NPC.frame.Y = frameHeight * 5;
                    }
                }
            }
            else
            {
                NPC.frameCounter += (NPC.ai[0] == 3.2f ? 1.5 : 1D);
                if (NPC.frameCounter > 4D) //iban said the time between frames was 5 so using that as a base
                {
                    NPC.frameCounter = 0D;
                    NPC.frame.Y += frameHeight;
                }
                if (NPC.frame.Y >= frameHeight * 5)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool phase2 = lifeRatio < (revenge ? 0.75f : 0.5f) || death;
            bool phase3 = lifeRatio < (death ? 0.4f : revenge ? 0.25f : 0.1f);

            float newPhaseTimer = 180f;
            bool phaseSwitchPhase = (phase2 && calamityGlobalNPC.newAI[0] < newPhaseTimer && calamityGlobalNPC.newAI[2] != 1f) ||
                (phase3 && calamityGlobalNPC.newAI[1] < newPhaseTimer && calamityGlobalNPC.newAI[3] != 1f);

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
            Color color = drawColor;
            Color color36 = Color.White;

            float amount9 = 0f;
            int num150 = 120;
            int num151 = 60;

            if (phase3 && calamityGlobalNPC.newAI[3] == 1f)
            {
                color = CalamityGlobalNPC.buffColor(color, 0.9f, 0.6f, 0.2f, 1f);
            }
            else if (phase2 && calamityGlobalNPC.newAI[2] == 1f)
            {
                color = CalamityGlobalNPC.buffColor(color, 0.7f, 0.7f, 0.3f, 1f);
            }
            else if (phase2 && calamityGlobalNPC.newAI[0] > (float)num150)
            {
                float num152 = calamityGlobalNPC.newAI[0] - (float)num150;
                num152 /= (float)num151;
                color = CalamityGlobalNPC.buffColor(color, 1f - 0.3f * num152, 1f - 0.3f * num152, 1f - 0.7f * num152, 1f);
            }

            int num153 = 10;
            int num154 = 2;
            if (NPC.ai[0] == 0f || NPC.ai[0] == 3.1f || NPC.ai[0] == 4f || NPC.ai[0] == 4.2f)
            {
                num153 = 4;
            }
            if (NPC.ai[0] == 1f || NPC.ai[0] == 3f || NPC.ai[0] == 4.1f)
            {
                num153 = 7;
            }
            if (NPC.ai[0] == 2f || NPC.ai[0] == 3.2f || (phase2 && calamityGlobalNPC.newAI[2] == 1f))
            {
                color36 = Color.Yellow;
                amount9 = 0.5f;
            }
            else
            {
                color = drawColor;
            }

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += num154)
                {
                    Color color38 = color;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (float)(num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    vector41 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
                    vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            int num156 = 0;
            float num157 = 0f;
            float scaleFactor9 = 0f;

            if (NPC.ai[0] == 0f || NPC.ai[0] == 3.1f || NPC.ai[0] == 4f || NPC.ai[0] == 4.2f)
            {
                num156 = 4;
            }

            if (NPC.ai[0] == 5f)
            {
                int num158 = 60;
                int num159 = 30;
                if (NPC.ai[1] > (float)num158)
                {
                    num156 = 6;
                    num157 = 1f - (float)Math.Cos((double)((NPC.ai[1] - (float)num158) / (float)num159 * MathHelper.TwoPi));
                    num157 /= 3f;
                    scaleFactor9 = 40f;
                }
            }

            if (phaseSwitchPhase)
            {
                if (phase3 && calamityGlobalNPC.newAI[1] > (float)num150)
                {
                    num156 = 6;
                    num157 = 1f - (float)Math.Cos((double)((calamityGlobalNPC.newAI[1] - (float)num150) / (float)num151 * MathHelper.TwoPi));
                    num157 /= 3f;
                    scaleFactor9 = 60f;
                }
                else if (phase2 && calamityGlobalNPC.newAI[0] > (float)num150)
                {
                    num156 = 6;
                    num157 = 1f - (float)Math.Cos((double)((calamityGlobalNPC.newAI[0] - (float)num150) / (float)num151 * MathHelper.TwoPi));
                    num157 /= 3f;
                    scaleFactor9 = 60f;
                }
            }

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num160 = 0; num160 < num156; num160++)
                {
                    Color color39 = drawColor;
                    color39 = Color.Lerp(color39, color36, amount9);
                    color39 = NPC.GetAlpha(color39);
                    color39 *= 1f - num157;
                    Vector2 vector42 = NPC.Center + ((float)num160 / (float)num156 * MathHelper.TwoPi + NPC.rotation).ToRotationVector2() * scaleFactor9 * num157 - screenPos;
                    vector42 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
                    vector42 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector42, NPC.frame, color39, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            Color color2 = drawColor;
            color2 = Color.Lerp(color2, color36, amount9);
            color2 = NPC.GetAlpha(color2);
            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, (phase3 && calamityGlobalNPC.newAI[3] == 1f ? color2 : NPC.GetAlpha(drawColor)), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            if (phase2)
            {
                texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Bumblebirb/BirbGlow").Value;
                Color color40 = Color.Lerp(Color.White, Color.Red, 0.5f);
                color36 = Color.Red;

                amount9 = 1f;
                num157 = 0.5f;
                scaleFactor9 = 10f;
                num154 = 1;

                if (phaseSwitchPhase)
                {
                    float num161 = (phase3 ? calamityGlobalNPC.newAI[1] : calamityGlobalNPC.newAI[0]) - (float)num150;
                    num161 /= (float)num151;
                    color36 *= num161;
                    color40 *= num161;
                }

                if (CalamityConfig.Instance.Afterimages)
                {
                    for (int num163 = 1; num163 < num153; num163 += num154)
                    {
                        Color color41 = color40;
                        color41 = Color.Lerp(color41, color36, amount9);
                        color41 *= (float)(num153 - num163) / 15f;
                        Vector2 vector44 = NPC.oldPos[num163] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                        vector44 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
                        vector44 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                        spriteBatch.Draw(texture2D15, vector44, NPC.frame, color41, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                    }

                    for (int num164 = 1; num164 < num156; num164++)
                    {
                        Color color42 = color40;
                        color42 = Color.Lerp(color42, color36, amount9);
                        color42 = NPC.GetAlpha(color42);
                        color42 *= 1f - num157;
                        Vector2 vector45 = NPC.Center + ((float)num164 / (float)num156 * MathHelper.TwoPi + NPC.rotation).ToRotationVector2() * scaleFactor9 * num157 - screenPos;
                        vector45 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
                        vector45 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                        spriteBatch.Draw(texture2D15, vector45, NPC.frame, color42, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                    }
                }

                spriteBatch.Draw(texture2D15, vector43, NPC.frame, color40, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
            }

            return false;
        }

        private static Color buffColor(Color newColor, float R, float G, float B, float A)
        {
            newColor.R = (byte)((float)newColor.R * R);
            newColor.G = (byte)((float)newColor.G * G);
            newColor.B = (byte)((float)newColor.B * B);
            newColor.A = (byte)((float)newColor.A * A);
            return newColor;
        }

        public override void BossLoot(ref string name, ref int potionType) => potionType = ItemID.SuperHealingPotion;

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<DragonfollyBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] items = new int[]
                {
                    ModContent.ItemType<GildedProboscis>(),
                    ModContent.ItemType<GoldenEagle>(),
                    ModContent.ItemType<RougeSlash>()
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, items));
                normalOnly.Add(ModContent.ItemType<Swordsplosion>(), 10);

                // Materials
                normalOnly.Add(ModContent.ItemType<EffulgentFeather>(), 1, 25, 30);

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<DynamoStemCells>()));
                normalOnly.Add(ModContent.ItemType<FollyFeed>(), DropHelper.NormalWeaponDropRateFraction);

                // Vanity
                normalOnly.Add(ModContent.ItemType<BumblefuckMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            }

            npcLoot.Add(ModContent.ItemType<DragonfollyTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<DragonfollyRelic>());

            // GFB Omega Healing Potion drop
            npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(ModContent.ItemType<OmegaHealingPotion>(), 1, 50, 100);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedDragonfolly, ModContent.ItemType<LoreDragonfolly>(), desc: DropHelper.FirstKillText);
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // Mark The Dragonfolly as dead
            DownedBossSystem.downedDragonfolly = true;
            CalamityNetcode.SyncWorld();

            if (Main.zenithWorld)
            {
                int spacing = 40;
                int amt = 7;
                SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, NPC.Center - Vector2.UnitY * 300f);
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int i = 0; i < amt; i++)
                    {
                        Vector2 fireFrom = new Vector2(NPC.Center.X + (spacing * i) - (spacing * amt / 2), NPC.Center.Y - 900f);
                        Vector2 ai0 = NPC.Center - fireFrom;
                        float ai = Main.rand.Next(100);
                        Vector2 velocity = Vector2.Normalize(ai0.RotatedByRandom(MathHelper.PiOver4)) * 7f;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom.X, fireFrom.Y, velocity.X, velocity.Y, ModContent.ProjectileType<RedLightning>(), NPC.damage, 0f, Main.myPlayer, ai0.ToRotation(), ai);
                    }
                }
            }

        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void ModifyTypeName(ref string typeName)
        {
            if (Main.zenithWorld)
            {
                typeName = CalamityUtils.GetTextValue("NPCs.Bumblebirb");
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 244, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 50; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 244, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 0; i < 6; i++) // 1 head, 1 wing, 4 legs = 6. one wing due to them being chonky boyes now
                    {
                        string gore = "Bumble";
                        float randomSpread = Main.rand.Next(-200, 201) / 100f;
                        gore += i == 0 ? "Head" : i > 1 ? "Leg" : "Wing";
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>(gore).Type, 1f);
                    }
                }
            }
        }
    }
}
