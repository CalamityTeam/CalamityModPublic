using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.World;
using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Dusts;
using Terraria.GameContent.ItemDropRules;
using CalamityMod.Events;
using Terraria.Audio;
using ReLogic.Utilities;

namespace CalamityMod.NPCs.OldDuke
{
    [AutoloadBossHead]
    public class OldDuke : ModNPC
    {
        public static readonly SoundStyle HuffSound = new("CalamityMod/Sounds/Custom/OldDukeHuff");
        public static readonly SoundStyle RoarSound = new("CalamityMod/Sounds/Custom/OldDukeRoar");
        public static readonly SoundStyle VomitSound = new("CalamityMod/Sounds/Custom/OldDukeVomit");

        public SlotId RoarSoundSlot; 

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                SpriteDirection = 1,
                Scale = 0.45f
            };
            value.Position.X += 14f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.width = 150;
            NPC.height = 100;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.GetNPCDamage();
            NPC.defense = 90;
            NPC.DR_NERD(0.5f, null, null, null, true);
            NPC.LifeMaxNERB(500000, 600000, 400000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.knockBackResist = 0f;
            NPC.noTileCollide = true;
            NPC.noGravity = true;
            NPC.npcSlots = 15f;
            NPC.HitSound = SoundID.NPCHit14;
            NPC.DeathSound = SoundID.NPCDeath20;
            NPC.value = Item.buyPrice(4, 0, 0, 0);
            NPC.boss = true;
            NPC.netAlways = true;
            NPC.timeLeft = NPC.activeTime * 30;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SulphurousSeaBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.OldDuke")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.rotation);
            writer.Write(NPC.spriteDirection);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.rotation = reader.ReadSingle();
            NPC.spriteDirection = reader.ReadInt32();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityAI.OldDukeAI(NPC, Mod);
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
                typeName = CalamityUtils.GetTextValue("NPCs.BoomerDuke");
            }
        }

        public override void FindFrame(int frameHeight)
        {
            bool tired = NPC.Calamity().newAI[1] == 1f;
            if (NPC.ai[0] == 0f || NPC.ai[0] == 5f || NPC.ai[0] == 10f || NPC.ai[0] == 12f)
            {
                int num114 = tired ? 14 : 7;
                if (NPC.ai[0] == 5f || NPC.ai[0] == 12f)
                {
                    num114 = tired ? 12 : 6;
                }
                NPC.frameCounter += 1D;
                if (NPC.frameCounter > num114)
                {
                    NPC.frameCounter = 0D;
                    NPC.frame.Y += frameHeight;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 0;
                }
            }
            if (NPC.ai[0] == 1f || NPC.ai[0] == 6f || NPC.ai[0] == 11f)
            {
                NPC.frame.Y = frameHeight * 2;
            }
            if (NPC.ai[0] == 2f || NPC.ai[0] == 7f || NPC.ai[0] == 14f)
            {
                NPC.frame.Y = frameHeight * 6;
            }
            if (NPC.ai[0] == 3f || NPC.ai[0] == 8f || NPC.ai[0] == 13f || NPC.ai[0] == -1f)
            {
                int num115 = 120;
                if (NPC.ai[2] < (num115 - 50) || NPC.ai[2] > (num115 - 10))
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 7D)
                    {
                        NPC.frameCounter = 0D;
                        NPC.frame.Y += frameHeight;
                    }
                    if (NPC.frame.Y >= frameHeight * 6)
                    {
                        NPC.frame.Y = 0;
                    }
                }
                else
                {
                    NPC.frame.Y = frameHeight * 5;
                    if (NPC.ai[2] > (num115 - 40) && NPC.ai[2] < (num115 - 15))
                    {
                        NPC.frame.Y = frameHeight * 6;
                    }
                }
            }
            if (NPC.ai[0] == 4f || NPC.ai[0] == 9f)
            {
                int num116 = 180;
                if (NPC.ai[2] < (num116 - 60) || NPC.ai[2] > (num116 - 20))
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 7D)
                    {
                        NPC.frameCounter = 0D;
                        NPC.frame.Y += frameHeight;
                    }
                    if (NPC.frame.Y >= frameHeight * 6)
                    {
                        NPC.frame.Y = 0;
                    }
                }
                else
                {
                    NPC.frame.Y = frameHeight * 5;
                    if (NPC.ai[2] > (num116 - 50) && NPC.ai[2] < (num116 - 25))
                    {
                        NPC.frame.Y = frameHeight * 6;
                    }
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2(texture2D15.Width / 2, texture2D15.Height / Main.npcFrameCount[NPC.type] / 2);
            Color color = drawColor;
            Color color36 = Color.White;
            float amount9 = 0f;
            bool flag8 = NPC.ai[0] > 4f;
            bool flag9 = NPC.ai[0] > 9f && NPC.ai[0] <= 12f;
            int num150 = 120;
            int num151 = 60;
            if (flag9)
            {
                color = CalamityGlobalNPC.buffColor(color, 0.4f, 0.8f, 0.4f, 1f);
            }
            else if (flag8)
            {
                color = CalamityGlobalNPC.buffColor(color, 0.5f, 0.7f, 0.5f, 1f);
            }
            else if (NPC.ai[0] == 4f && NPC.ai[2] > num150)
            {
                float num152 = NPC.ai[2] - num150;
                num152 /= num151;
                color = CalamityGlobalNPC.buffColor(color, 1f - 0.5f * num152, 1f - 0.3f * num152, 1f - 0.5f * num152, 1f);
            }

            int num153 = 10;
            int num154 = 2;
            if (NPC.ai[0] == -1f)
            {
                num153 = 0;
            }
            if (NPC.ai[0] == 0f || NPC.ai[0] == 5f || NPC.ai[0] == 10f || NPC.ai[0] == 12f)
            {
                num153 = 7;
            }
            if (NPC.ai[0] == 1f || NPC.ai[0] == 6f || NPC.ai[0] > 9f)
            {
                color36 = Color.Lime;
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
                    color38 *= (num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    vector41 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            int num156 = 0;
            float num157 = 0f;
            float scaleFactor9 = 0f;

            if (NPC.ai[0] == -1f)
            {
                num156 = 0;
            }

            if (NPC.ai[0] == 3f || NPC.ai[0] == 8f || NPC.ai[0] == 13f)
            {
                int num158 = 60;
                int num159 = 30;
                if (NPC.ai[2] > num158)
                {
                    num156 = 6;
                    num157 = 1f - (float)Math.Cos((NPC.ai[2] - num158) / num159 * MathHelper.TwoPi);
                    num157 /= 3f;
                    scaleFactor9 = 40f;
                }
            }

            if ((NPC.ai[0] == 4f || NPC.ai[0] == 9f) && NPC.ai[2] > num150)
            {
                num156 = 6;
                num157 = 1f - (float)Math.Cos((NPC.ai[2] - num150) / num151 * MathHelper.TwoPi);
                num157 /= 3f;
                scaleFactor9 = 60f;
            }

            if (NPC.ai[0] == 12f)
            {
                num156 = 6;
                num157 = 1f - (float)Math.Cos(NPC.ai[2] / 30f * MathHelper.TwoPi);
                num157 /= 3f;
                scaleFactor9 = 20f;
            }

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num160 = 0; num160 < num156; num160++)
                {
                    Color color39 = drawColor;
                    color39 = Color.Lerp(color39, color36, amount9);
                    color39 = NPC.GetAlpha(color39);
                    color39 *= 1f - num157;
                    Vector2 vector42 = NPC.Center + (num160 / (float)num156 * MathHelper.TwoPi + NPC.rotation).ToRotationVector2() * scaleFactor9 * num157 - screenPos;
                    vector42 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    vector42 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector42, NPC.frame, color39, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            Color color2 = drawColor;
            color2 = Color.Lerp(color2, color36, amount9);
            color2 = NPC.GetAlpha(color2);
            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, (NPC.ai[0] > 9f ? color2 : NPC.GetAlpha(drawColor)), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            if (NPC.ai[0] >= 4f && NPC.Calamity().newAI[1] != 1f)
            {
                texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/OldDuke/OldDukeGlow").Value;
                Color color40 = Color.Lerp(Color.White, Color.Yellow, 0.5f);
                color36 = Color.Yellow;

                amount9 = 1f;
                num157 = 0.5f;
                scaleFactor9 = 10f;
                num154 = 1;

                if (NPC.ai[0] == 4f || NPC.ai[0] == 9f)
                {
                    float num161 = NPC.ai[2] - num150;
                    num161 /= num151;
                    color36 *= num161;
                    color40 *= num161;
                }

                if (NPC.ai[0] == 12f)
                {
                    float num162 = NPC.ai[2];
                    num162 /= 30f;
                    if (num162 > 0.5f)
                    {
                        num162 = 1f - num162;
                    }
                    num162 *= 2f;
                    num162 = 1f - num162;
                    color36 *= num162;
                    color40 *= num162;
                }

                if (CalamityConfig.Instance.Afterimages)
                {
                    for (int num163 = 1; num163 < num153; num163 += num154)
                    {
                        Color color41 = color40;
                        color41 = Color.Lerp(color41, color36, amount9);
                        color41 *= (num153 - num163) / 15f;
                        Vector2 vector44 = NPC.oldPos[num163] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                        vector44 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                        vector44 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                        spriteBatch.Draw(texture2D15, vector44, NPC.frame, color41, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                    }

                    for (int num164 = 1; num164 < num156; num164++)
                    {
                        Color color42 = color40;
                        color42 = Color.Lerp(color42, color36, amount9);
                        color42 = NPC.GetAlpha(color42);
                        color42 *= 1f - num157;
                        Vector2 vector45 = NPC.Center + (num164 / (float)num156 * MathHelper.TwoPi + NPC.rotation).ToRotationVector2() * scaleFactor9 * num157 - screenPos;
                        vector45 -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                        vector45 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                        spriteBatch.Draw(texture2D15, vector45, NPC.frame, color42, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                    }
                }

                spriteBatch.Draw(texture2D15, vector43, NPC.frame, color40, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
            }

            return false;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<SupremeHealingPotion>();
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            CalamityGlobalNPC.SetNewShopVariable(new int[] { ModContent.NPCType<SEAHOE>() }, DownedBossSystem.downedBoomerDuke);

            // Mark Old Duke as dead
            DownedBossSystem.downedBoomerDuke = true;

            // Mark first acid rain encounter as true even if he wasn't fought in the acid rain, because it makes sense
            AcidRainEvent.OldDukeHasBeenEncountered = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<OldDukeBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons and such
                int[] items = new int[]
                {
                    ModContent.ItemType<InsidiousImpaler>(),
                    ModContent.ItemType<FetidEmesis>(),
                    ModContent.ItemType<SepticSkewer>(),
                    ModContent.ItemType<VitriolicViper>(),
                    ModContent.ItemType<CadaverousCarrion>(),
                    ModContent.ItemType<ToxicantTwister>()
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, items));
                normalOnly.Add(ModContent.ItemType<TheOldReaper>(), 10);

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<MutatedTruffle>()));
                normalOnly.Add(ModContent.ItemType<OldDukeScales>(), DropHelper.NormalWeaponDropRateFraction);

                // Vanity
                normalOnly.Add(ModContent.ItemType<OldDukeMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            }

            npcLoot.Add(ModContent.ItemType<OldDukeTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<OldDukeRelic>());

            // GFB Shattered Community drop
            npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(ModContent.ItemType<ShatteredCommunity>());

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedBoomerDuke, ModContent.ItemType<LoreOldDuke>(), desc: DropHelper.FirstKillText);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                target.AddBuff(ModContent.BuffType<Irradiated>(), 480);
                if (Main.zenithWorld)
                {
                    target.AddBuff(BuffID.Rabies, Main.rand.Next(180, 601));
                }
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life > 0)
            {
                int num211 = 0;
                while (num211 < hit.Damage / NPC.lifeMax * 100.0)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);
                    num211++;
                }
            }
            else
            {
                for (int num212 = 0; num212 < 150; num212++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, 2 * hit.HitDirection, -2f, 0, default, 1f);
                }

                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center + Vector2.UnitX * 20f * NPC.direction, NPC.velocity, Mod.Find<ModGore>("OldDukeGore").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center + Vector2.UnitX * 20f * NPC.direction, NPC.velocity, Mod.Find<ModGore>("OldDukeGore2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center - Vector2.UnitX * 20f * NPC.direction, NPC.velocity, Mod.Find<ModGore>("OldDukeGore3").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.Center - Vector2.UnitX * 20f * NPC.direction, NPC.velocity, Mod.Find<ModGore>("OldDukeGore4").Type, NPC.scale);
                }
            }
        }
    }
}
