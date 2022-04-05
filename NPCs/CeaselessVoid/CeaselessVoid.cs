using CalamityMod.Dusts;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.IO;
using Terraria.Audio;

namespace CalamityMod.NPCs.CeaselessVoid
{
    [AutoloadBossHead]
    public class CeaselessVoid : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ceaseless Void");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.npcSlots = 36f;
            NPC.width = 100;
            NPC.height = 100;
            NPC.defense = 80;
            CalamityGlobalNPC global = NPC.Calamity();
            global.DR = 0.5f;

            bool notDoGFight = CalamityWorld.DoGSecondStageCountdown <= 0 || !DownedBossSystem.downedSentinel1;
            NPC.LifeMaxNERB(notDoGFight ? 64400 : 16100, notDoGFight ? 77280 : 19320, 72000);

            // If fought alone, Ceaseless Void plays its own theme
            if (notDoGFight)
            {
                NPC.value = Item.buyPrice(2, 0, 0, 0);
                music = CalamityMod.Instance.GetMusicFromMusicMod("Void") ?? MusicID.Boss3;
            }
            // If fought as a DoG interlude, keep the DoG music playing
            else
                music = CalamityMod.Instance.GetMusicFromMusicMod("ScourgeofTheUniverse") ?? MusicID.Boss3;

            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.DeathSound = SoundID.NPCDeath14;
            bossBag = ModContent.ItemType<CeaselessVoidBag>();
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.dontTakeDamage);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.dontTakeDamage = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            CalamityAI.CeaselessVoidAI(NPC, Mod);
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;

            Rectangle targetHitbox = target.Hitbox;

            float dist1 = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float dist2 = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float dist3 = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float dist4 = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = dist1;
            if (dist2 < minDist)
                minDist = dist2;
            if (dist3 < minDist)
                minDist = dist3;
            if (dist4 < minDist)
                minDist = dist4;

            return minDist <= 50f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = Main.npcTexture[NPC.type];
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[NPC.type].Width / 2), (float)(Main.npcTexture[NPC.type].Height / Main.npcFrameCount[NPC.type] / 2));
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 7;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = lightColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (float)(num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - Main.screenPosition;
                    vector41 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
                    vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = NPC.Center - Main.screenPosition;
            vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(lightColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/CeaselessVoid/CeaselessVoidGlow");
            Color color37 = Color.Lerp(Color.White, Color.Cyan, 0.5f);

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num163 = 1; num163 < num153; num163++)
                {
                    Color color41 = color37;
                    color41 = Color.Lerp(color41, color36, amount9);
                    color41 *= (float)(num153 - num163) / 15f;
                    Vector2 vector44 = NPC.oldPos[num163] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - Main.screenPosition;
                    vector44 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
                    vector44 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector44, NPC.frame, color41, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void NPCLoot()
        {
            // Only drop items if fought at full strength
            bool fullStrength = !DownedBossSystem.downedSentinel1 || CalamityWorld.DoGSecondStageCountdown <= 0;
            if (fullStrength)
            {
                CalamityGlobalNPC.SetNewBossJustDowned(NPC);

                DropHelper.DropBags(NPC);

                DropHelper.DropItemChance(NPC, ModContent.ItemType<CeaselessVoidTrophy>(), 10);
                bool lastSentinelKilled = !DownedBossSystem.downedSentinel1 && DownedBossSystem.downedSentinel2 && DownedBossSystem.downedSentinel3;
                DropHelper.DropItemCondition(NPC, ModContent.ItemType<KnowledgeSentinels>(), true, lastSentinelKilled);

                if (!Main.expertMode)
                {
                    // Materials
                    DropHelper.DropItem(NPC, ModContent.ItemType<DarkPlasma>(), true, 2, 3);

                    // Weapons
                    float dropChance = DropHelper.NormalWeaponDropRateFloat;
                    DropHelper.DropItemChance(NPC, ModContent.ItemType<MirrorBlade>(), dropChance);
                    DropHelper.DropItemChance(NPC, ModContent.ItemType<VoidConcentrationStaff>(), dropChance);

                    // Equipment
                    DropHelper.DropItem(NPC, ModContent.ItemType<TheEvolution>(), true);

                    // Vanity
                    DropHelper.DropItemChance(NPC, ModContent.ItemType<CeaselessVoidMask>(), 7);
                    if (Main.rand.NextBool(20))
                    {
                        DropHelper.DropItem(NPC, ModContent.ItemType<AncientGodSlayerHelm>());
                        DropHelper.DropItem(NPC, ModContent.ItemType<AncientGodSlayerChestplate>());
                        DropHelper.DropItem(NPC, ModContent.ItemType<AncientGodSlayerLeggings>());
                    }
                }
            }

            // If DoG's fight is active, set the timer for the remaining two sentinels
            if (CalamityWorld.DoGSecondStageCountdown > 14460)
            {
                CalamityWorld.DoGSecondStageCountdown = 14460;
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = Mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                    netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                    netMessage.Send();
                }
            }

            // Mark Ceaseless Void as dead
            if (fullStrength)
            {
                DownedBossSystem.downedSentinel1 = true;
                CalamityNetcode.SyncWorld();
            }
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<SupremeHealingPotion>();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 8;
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/OtherworldlyHit"), NPC.Center);
            }

            for (int k = 0; k < 5; k++)
            {
                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, hitDirection, -1f, 0, default, 1f);
                Main.dust[dust].noGravity = true;
            }

            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                NPC.width = 100;
                NPC.height = 100;
                NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    Main.dust[num622].noGravity = true;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 2f;
                }

                float randomSpread = (float)(Main.rand.Next(-200, 200) / 100);
                Gore.NewGore(NPC.position, NPC.velocity * randomSpread, Mod.GetGoreSlot("Gores/CeaselessVoid"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity * randomSpread, Mod.GetGoreSlot("Gores/CeaselessVoid2"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity * randomSpread, Mod.GetGoreSlot("Gores/CeaselessVoid2"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity * randomSpread, Mod.GetGoreSlot("Gores/CeaselessVoid3"), 1f);
                Gore.NewGore(NPC.position, NPC.velocity * randomSpread, Mod.GetGoreSlot("Gores/CeaselessVoid3"), 1f);
            }
        }
    }
}
