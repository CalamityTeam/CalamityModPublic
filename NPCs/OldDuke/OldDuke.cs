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
                int frameChangeFrequency = tired ? 14 : 7;
                if (NPC.ai[0] == 5f || NPC.ai[0] == 12f)
                {
                    frameChangeFrequency = tired ? 12 : 6;
                }
                NPC.frameCounter += 1D;
                if (NPC.frameCounter > frameChangeFrequency)
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
                int frameChangeGateValue = 120;
                if (NPC.ai[2] < (frameChangeGateValue - 50) || NPC.ai[2] > (frameChangeGateValue - 10))
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
                    if (NPC.ai[2] > (frameChangeGateValue - 40) && NPC.ai[2] < (frameChangeGateValue - 15))
                    {
                        NPC.frame.Y = frameHeight * 6;
                    }
                }
            }
            if (NPC.ai[0] == 4f || NPC.ai[0] == 9f)
            {
                int secondFrameChangeGateValue = 180;
                if (NPC.ai[2] < (secondFrameChangeGateValue - 60) || NPC.ai[2] > (secondFrameChangeGateValue - 20))
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
                    if (NPC.ai[2] > (secondFrameChangeGateValue - 50) && NPC.ai[2] < (secondFrameChangeGateValue - 25))
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
            Vector2 halfSizeTexture = new Vector2(texture2D15.Width / 2, texture2D15.Height / Main.npcFrameCount[NPC.type] / 2);
            Color color = drawColor;
            Color drawLerpColor = Color.White;
            float drawLerpValue = 0f;
            bool halfTiredBuffColor = NPC.ai[0] > 4f;
            bool tiredBuffColor = NPC.ai[0] > 9f && NPC.ai[0] <= 12f;
            int ai2Compare = 120;
            int buffColorDivisor = 60;
            if (tiredBuffColor)
            {
                color = CalamityGlobalNPC.buffColor(color, 0.4f, 0.8f, 0.4f, 1f);
            }
            else if (halfTiredBuffColor)
            {
                color = CalamityGlobalNPC.buffColor(color, 0.5f, 0.7f, 0.5f, 1f);
            }
            else if (NPC.ai[0] == 4f && NPC.ai[2] > ai2Compare)
            {
                float buffColorDampener = NPC.ai[2] - ai2Compare;
                buffColorDampener /= buffColorDivisor;
                color = CalamityGlobalNPC.buffColor(color, 1f - 0.5f * buffColorDampener, 1f - 0.3f * buffColorDampener, 1f - 0.5f * buffColorDampener, 1f);
            }

            int afterimageAmt = 10;
            int afterimageIncrement = 2;
            if (NPC.ai[0] == -1f)
            {
                afterimageAmt = 0;
            }
            if (NPC.ai[0] == 0f || NPC.ai[0] == 5f || NPC.ai[0] == 10f || NPC.ai[0] == 12f)
            {
                afterimageAmt = 7;
            }
            if (NPC.ai[0] == 1f || NPC.ai[0] == 6f || NPC.ai[0] > 9f)
            {
                drawLerpColor = Color.Lime;
                drawLerpValue = 0.5f;
            }
            else
            {
                color = drawColor;
            }

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i += afterimageIncrement)
                {
                    Color afterimageColor = color;
                    afterimageColor = Color.Lerp(afterimageColor, drawLerpColor, drawLerpValue);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (afterimageAmt - i) / 15f;
                    Vector2 afterimagePos = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    afterimagePos -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    afterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, afterimagePos, NPC.frame, afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            int secondAfterimageAmt = 0;
            float afterimageOpacity = 0f;
            float afterimageScale = 0f;

            if (NPC.ai[0] == -1f)
            {
                secondAfterimageAmt = 0;
            }

            if (NPC.ai[0] == 3f || NPC.ai[0] == 8f || NPC.ai[0] == 13f)
            {
                if (NPC.ai[2] > 60)
                {
                    secondAfterimageAmt = 6;
                    afterimageOpacity = 1f - (float)Math.Cos((NPC.ai[2] - 60) / 30 * MathHelper.TwoPi);
                    afterimageOpacity /= 3f;
                    afterimageScale = 40f;
                }
            }

            if ((NPC.ai[0] == 4f || NPC.ai[0] == 9f) && NPC.ai[2] > ai2Compare)
            {
                secondAfterimageAmt = 6;
                afterimageOpacity = 1f - (float)Math.Cos((NPC.ai[2] - ai2Compare) / buffColorDivisor * MathHelper.TwoPi);
                afterimageOpacity /= 3f;
                afterimageScale = 60f;
            }

            if (NPC.ai[0] == 12f)
            {
                secondAfterimageAmt = 6;
                afterimageOpacity = 1f - (float)Math.Cos(NPC.ai[2] / 30f * MathHelper.TwoPi);
                afterimageOpacity /= 3f;
                afterimageScale = 20f;
            }

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int j = 0; j < secondAfterimageAmt; j++)
                {
                    Color secondAfterimageColor = drawColor;
                    secondAfterimageColor = Color.Lerp(secondAfterimageColor, drawLerpColor, drawLerpValue);
                    secondAfterimageColor = NPC.GetAlpha(secondAfterimageColor);
                    secondAfterimageColor *= 1f - afterimageOpacity;
                    Vector2 secondAfterimagePos = NPC.Center + (j / (float)secondAfterimageAmt * MathHelper.TwoPi + NPC.rotation).ToRotationVector2() * afterimageScale * afterimageOpacity - screenPos;
                    secondAfterimagePos -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    secondAfterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, secondAfterimagePos, NPC.frame, secondAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            Color finalDrawColor = drawColor;
            finalDrawColor = Color.Lerp(finalDrawColor, drawLerpColor, drawLerpValue);
            finalDrawColor = NPC.GetAlpha(finalDrawColor);
            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, (NPC.ai[0] > 9f ? finalDrawColor : NPC.GetAlpha(drawColor)), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            if (NPC.ai[0] >= 4f && NPC.Calamity().newAI[1] != 1f)
            {
                texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/OldDuke/OldDukeGlow").Value;
                Color yellowLerpColor = Color.Lerp(Color.White, Color.Yellow, 0.5f);
                drawLerpColor = Color.Yellow;

                drawLerpValue = 1f;
                afterimageOpacity = 0.5f;
                afterimageScale = 10f;
                afterimageIncrement = 1;

                if (NPC.ai[0] == 4f || NPC.ai[0] == 9f)
                {
                    float otherAfterimageOpacity = NPC.ai[2] - ai2Compare;
                    otherAfterimageOpacity /= buffColorDivisor;
                    drawLerpColor *= otherAfterimageOpacity;
                    yellowLerpColor *= otherAfterimageOpacity;
                }

                if (NPC.ai[0] == 12f)
                {
                    float ai2Opacity = NPC.ai[2];
                    ai2Opacity /= 30f;
                    if (ai2Opacity > 0.5f)
                    {
                        ai2Opacity = 1f - ai2Opacity;
                    }
                    ai2Opacity *= 2f;
                    ai2Opacity = 1f - ai2Opacity;
                    drawLerpColor *= ai2Opacity;
                    yellowLerpColor *= ai2Opacity;
                }

                if (CalamityConfig.Instance.Afterimages)
                {
                    for (int k = 1; k < afterimageAmt; k += afterimageIncrement)
                    {
                        Color yellowAfterimageColor = yellowLerpColor;
                        yellowAfterimageColor = Color.Lerp(yellowAfterimageColor, drawLerpColor, drawLerpValue);
                        yellowAfterimageColor *= (afterimageAmt - k) / 15f;
                        Vector2 yellowAfterimagePos = NPC.oldPos[k] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                        yellowAfterimagePos -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                        yellowAfterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                        spriteBatch.Draw(texture2D15, yellowAfterimagePos, NPC.frame, yellowAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                    }

                    for (int l = 1; l < secondAfterimageAmt; l++)
                    {
                        Color secondYellowAfterimageColor = yellowLerpColor;
                        secondYellowAfterimageColor = Color.Lerp(secondYellowAfterimageColor, drawLerpColor, drawLerpValue);
                        secondYellowAfterimageColor = NPC.GetAlpha(secondYellowAfterimageColor);
                        secondYellowAfterimageColor *= 1f - afterimageOpacity;
                        Vector2 secondYellowAfterimagePos = NPC.Center + (l / (float)secondAfterimageAmt * MathHelper.TwoPi + NPC.rotation).ToRotationVector2() * afterimageScale * afterimageOpacity - screenPos;
                        secondYellowAfterimagePos -= new Vector2(texture2D15.Width, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                        secondYellowAfterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                        spriteBatch.Draw(texture2D15, secondYellowAfterimagePos, NPC.frame, secondYellowAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                    }
                }

                spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, yellowLerpColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
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
                    ModContent.ItemType<MutatedTruffle>(),
                    ModContent.ItemType<CadaverousCarrion>(),
                    ModContent.ItemType<ToxicantTwister>()
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, items));
                normalOnly.Add(ModContent.ItemType<TheOldReaper>(), 10);

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<OldDukeScales>()));

                // Vanity
                normalOnly.Add(ModContent.ItemType<OldDukeMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            }

            npcLoot.Add(ModContent.ItemType<OldDukeTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<OldDukeRelic>());

            // GFB Shattered Community drop
            npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(ModContent.ItemType<ShatteredCommunity>(), hideLootReport: true);

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
                int onHitDust = 0;
                while (onHitDust < hit.Damage / NPC.lifeMax * 100.0)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.SulfurousSeaAcid, hit.HitDirection, -1f, 0, default, 1f);
                    onHitDust++;
                }
            }
            else
            {
                for (int r = 0; r < 150; r++)
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
