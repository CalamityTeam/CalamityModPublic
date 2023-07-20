using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Threading;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace CalamityMod.NPCs.AstrumAureus
{
    [AutoloadBossHead]
    public class AstrumAureus : ModNPC
    {
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/AureusHit", 4);
        public static readonly SoundStyle StompSound = new("CalamityMod/Sounds/Custom/AstrumAureus/LegStomp");
        public static readonly SoundStyle JumpSound = new("CalamityMod/Sounds/Custom/AstrumAureus/AureusJump");

        private bool stomping = false;
        public int slimeProjCounter = 0;
        public int slimePhase = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.27f,
                PortraitScale = 0.45f,
                PortraitPositionYOverride = -24f
            };
            value.Position.Y -= 20f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.npcSlots = 15f;
            NPC.GetNPCDamage();
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.width = 374;
            NPC.height = 374;
            NPC.defense = 40;
            NPC.DR_NERD(0.5f);
            NPC.LifeMaxNERB(100000, 120000, 740000); // 30 seconds in boss rush
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 60, 0, 0);
            NPC.boss = true;
            NPC.DeathSound = SoundID.NPCDeath14;
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<AbovegroundAstralBiome>().Type };

            if (Main.getGoodWorld)
                NPC.scale *= 0.8f;
            if (Main.zenithWorld)
                NPC.scale *= 1.5f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.AstrumAureus")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(stomping);
            writer.Write(NPC.alpha);
            writer.Write(slimePhase);
            writer.Write(slimeProjCounter);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            stomping = reader.ReadBoolean();
            NPC.alpha = reader.ReadInt32();
            slimePhase = reader.ReadInt32();
            slimeProjCounter = reader.ReadInt32();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityAI.AstrumAureusAI(NPC, Mod);
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.ai[0] == 3f || NPC.ai[0] == 4f)
            {
                if (NPC.velocity.Y == 0f && NPC.ai[1] >= 0f && NPC.ai[0] == 3f) //idle before jump
                {
                    if (stomping)
                        stomping = false;

                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 12D)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }
                    if (NPC.frame.Y >= frameHeight * 6)
                        NPC.frame.Y = 0;
                }
                else if (NPC.velocity.Y <= 0f || NPC.ai[1] < 0f) //prepare to jump and then jump
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 12D)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                        NPC.frame.Y = frameHeight * 5;
                }
                else //stomping
                {
                    if (!stomping)
                    {
                        stomping = true;
                        NPC.frameCounter = 0D;
                        NPC.frame.Y = 0;
                    }

                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 12D)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                        NPC.frame.Y = frameHeight * 5;
                }
            }
            else if (NPC.ai[0] >= 5f)
            {
                if (stomping)
                    stomping = false;

                if (NPC.velocity.Y == 0f) //idle before teleport
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 12D)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }
                    if (NPC.frame.Y >= frameHeight * 6)
                        NPC.frame.Y = 0;
                }
                else //in-air
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 12D)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                        NPC.frame.Y = frameHeight * 5;
                }
            }
            else
            {
                if (stomping)
                    stomping = false;

                NPC.frameCounter += 1D;
                if (NPC.frameCounter > 8D)
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0D;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                    NPC.frame.Y = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float lifeRatio = NPC.life / (float)NPC.lifeMax;
            bool slimePhaseHP = lifeRatio <= 0.1f || (lifeRatio > 0.6f && lifeRatio <= 0.7f);

            Texture2D NPCTexture = TextureAssets.Npc[NPC.type].Value;
            Texture2D GlowMaskTexture = TextureAssets.Npc[NPC.type].Value;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            if (NPC.ai[0] == 0f || (slimePhaseHP && Main.zenithWorld))
            {
                NPCTexture = TextureAssets.Npc[NPC.type].Value;
                GlowMaskTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumAureus/AstrumAureusGlow").Value;
            }
            else if (NPC.ai[0] == 1f) //nothing special done here
            {
                NPCTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumAureus/AstrumAureusRecharge").Value;
            }
            else if (NPC.ai[0] == 2f) //nothing special done here
            {
                NPCTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumAureus/AstrumAureusWalk").Value;
                GlowMaskTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumAureus/AstrumAureusWalkGlow").Value;
            }
            else if (NPC.ai[0] == 3f || NPC.ai[0] == 4f) //needs to have an in-air frame
            {
                if (NPC.velocity.Y == 0f && NPC.ai[1] >= 0f && NPC.ai[0] == 3f) //idle before jump
                {
                    NPCTexture = TextureAssets.Npc[NPC.type].Value; //idle frames
                    GlowMaskTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumAureus/AstrumAureusGlow").Value;
                }
                else if (NPC.velocity.Y <= 0f || NPC.ai[1] < 0f) //jump frames if flying upward or if about to jump
                {
                    NPCTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumAureus/AstrumAureusJump").Value;
                    GlowMaskTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumAureus/AstrumAureusJumpGlow").Value;
                }
                else //stomping
                {
                    NPCTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumAureus/AstrumAureusStomp").Value;
                    GlowMaskTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumAureus/AstrumAureusStompGlow").Value;
                }
            }
            else if (NPC.ai[0] >= 5f) //needs to have an in-air frame
            {
                if (NPC.velocity.Y == 0f) //idle before teleport
                {
                    NPCTexture = TextureAssets.Npc[NPC.type].Value; //idle frames
                    GlowMaskTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumAureus/AstrumAureusGlow").Value;
                }
                else //in-air frames
                {
                    NPCTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumAureus/AstrumAureusJump").Value;
                    GlowMaskTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/AstrumAureus/AstrumAureusJumpGlow").Value;
                }
            }

            int frameCount = Main.npcFrameCount[NPC.type];
            Vector2 vector11 = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / frameCount / 2);
            Rectangle frame = NPC.frame;
            float scale = NPC.scale;
            float rotation = NPC.rotation;
            float offsetY = NPC.gfxOffY;
            Color color36 = Color.White;
            if (Main.zenithWorld && slimePhaseHP)
            {
                color36 = slimePhase == 0 ? Color.Yellow : Color.Violet;
            }
            float amount9 = 0.5f;
            int num153 = 7;
            if (NPC.ai[0] == 3f || NPC.ai[0] == 4f)
                num153 = 10;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = drawColor;
                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    vector41 -= new Vector2(NPCTexture.Width, NPCTexture.Height / frameCount) * scale / 2f;
                    vector41 += vector11 * scale + new Vector2(0f, 4f + offsetY);
                    spriteBatch.Draw(NPCTexture, vector41, frame, color38, rotation, vector11, scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2(NPCTexture.Width, NPCTexture.Height / frameCount) * scale / 2f;
            vector43 += vector11 * scale + new Vector2(0f, 4f + offsetY);
            Color toUse = Main.zenithWorld && slimePhaseHP ? color36 : drawColor;
            spriteBatch.Draw(NPCTexture, vector43, frame, NPC.GetAlpha(toUse), rotation, vector11, scale, spriteEffects, 0f);

            if (NPC.ai[0] != 1 || (slimePhaseHP && Main.zenithWorld)) //draw only if not recharging
            {
                Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Gold);
                Color color40 = Color.Lerp(Color.White, color, 0.5f);
                if (Main.zenithWorld && slimePhaseHP)
                {
                    color40 = slimePhase == 0 ? Color.Violet : Color.Yellow;
                }

                if (CalamityConfig.Instance.Afterimages)
                {
                    for (int num163 = 1; num163 < num153; num163++)
                    {
                        Color color41 = color40;
                        color41 = Color.Lerp(color41, color36, amount9);
                        color41 = NPC.GetAlpha(color41);
                        color41 *= (num153 - num163) / 15f;
                        Vector2 vector44 = NPC.oldPos[num163] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                        vector44 -= new Vector2(GlowMaskTexture.Width, GlowMaskTexture.Height / frameCount) * scale / 2f;
                        vector44 += vector11 * scale + new Vector2(0f, 4f + offsetY);
                        spriteBatch.Draw(GlowMaskTexture, vector44, frame, color41, rotation, vector11, scale, spriteEffects, 0f);
                    }
                }

                spriteBatch.Draw(GlowMaskTexture, vector43, frame, color40, rotation, vector11, scale, spriteEffects, 0f);
            }

            return false;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Boss bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<AstrumAureusBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<Nebulash>(),
                    ModContent.ItemType<AuroraBlazer>(),
                    ModContent.ItemType<AlulaAustralis>(),
                    ModContent.ItemType<BorealisBomber>(),
                    ModContent.ItemType<AuroradicalThrow>(),
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));

                // Vanity
                normalOnly.Add(ModContent.ItemType<AstrumAureusMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<GravistarSabaton>()));

                // Other
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<AureusCell>(), 1, 9, 12));
                normalOnly.Add(ModContent.ItemType<LeonidProgenitor>(), 10);
                normalOnly.Add(ModContent.ItemType<SuspiciousLookingJellyBean>());
            }

            npcLoot.Add(ModContent.ItemType<AstrumAureusTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<AstrumAureusRelic>());

            // GFB Crab Banner and Asteroid Staff drop
            var GFBOnly = npcLoot.DefineConditionalDropSet(DropHelper.GFB);
            {
                GFBOnly.Add(ItemID.CrabBanner, 1, 1, 9999);
                GFBOnly.Add(ModContent.ItemType<AsteroidStaff>());
            }

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedAstrumAureus, ModContent.ItemType<LoreAstrumAureus>(), desc: DropHelper.FirstKillText);
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            CalamityGlobalNPC.SetNewShopVariable(new int[] { NPCID.Wizard, ModContent.NPCType<FAP>() }, DownedBossSystem.downedAstrumAureus);

            // If Astrum Aureus has not yet been killed, notify players of new Astral enemy drops
            if (!DownedBossSystem.downedAstrumAureus)
            {
                string key = "Mods.CalamityMod.Status.Progression.AureusBossText";
                string key2 = "Mods.CalamityMod.Status.Progression.AureusBossText2";
                Color messageColor = Color.Gold;

                CalamityUtils.DisplayLocalizedText(key, messageColor);
                CalamityUtils.DisplayLocalizedText(key2, messageColor);
            }

            // Drop an Astral Meteor if applicable
            ThreadPool.QueueUserWorkItem(_ => AstralBiome.PlaceAstralMeteor());

            // Mark Astrum Aureus as dead
            DownedBossSystem.downedAstrumAureus = true;
            CalamityNetcode.SyncWorld();
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 16;
                SoundEngine.PlaySound(HitSound, NPC.Center);
            }

            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = (int)(150 * NPC.scale);
                NPC.height = (int)(100 * NPC.scale);
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int num621 = 0; num621 < 50; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 100; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        // Can only hit the target if within certain distance
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Vector2 npcCenter = NPC.Center;

            // NOTE: Right and left hitboxes are interchangeable, each hitbox is the same size and is located to the right or left of the center hitbox.
            Rectangle leftHitbox = new Rectangle((int)(npcCenter.X - 92f * NPC.scale), (int)(npcCenter.Y + 28f * NPC.scale), 10, 10);
            Rectangle bodyHitbox = new Rectangle((int)(npcCenter.X - (NPC.width / 4f)), (int)(npcCenter.Y - (NPC.height / 2f) + 24f * NPC.scale), NPC.width / 2, NPC.height);
            Rectangle rightHitbox = new Rectangle((int)(npcCenter.X + 92f * NPC.scale), (int)(npcCenter.Y + 28f * NPC.scale), 10, 10);

            Vector2 leftHitboxCenter = new Vector2(leftHitbox.X + (leftHitbox.Width / 2), leftHitbox.Y + (leftHitbox.Height / 2));
            Vector2 bodyHitboxCenter = new Vector2(bodyHitbox.X + (bodyHitbox.Width / 2), bodyHitbox.Y + (bodyHitbox.Height / 2));
            Vector2 rightHitboxCenter = new Vector2(rightHitbox.X + (rightHitbox.Width / 2), rightHitbox.Y + (rightHitbox.Height / 2));

            Rectangle targetHitbox = target.Hitbox;

            float leftDist1 = Vector2.Distance(leftHitboxCenter, targetHitbox.TopLeft());
            float leftDist2 = Vector2.Distance(leftHitboxCenter, targetHitbox.TopRight());
            float leftDist3 = Vector2.Distance(leftHitboxCenter, targetHitbox.BottomLeft());
            float leftDist4 = Vector2.Distance(leftHitboxCenter, targetHitbox.BottomRight());

            float minLeftDist = leftDist1;
            if (leftDist2 < minLeftDist)
                minLeftDist = leftDist2;
            if (leftDist3 < minLeftDist)
                minLeftDist = leftDist3;
            if (leftDist4 < minLeftDist)
                minLeftDist = leftDist4;

            bool insideLeftHitbox = minLeftDist <= 120f * NPC.scale;

            float bodyDist1 = Vector2.Distance(bodyHitboxCenter, targetHitbox.TopLeft());
            float bodyDist2 = Vector2.Distance(bodyHitboxCenter, targetHitbox.TopRight());
            float bodyDist3 = Vector2.Distance(bodyHitboxCenter, targetHitbox.BottomLeft());
            float bodyDist4 = Vector2.Distance(bodyHitboxCenter, targetHitbox.BottomRight());

            float minBodyDist = bodyDist1;
            if (bodyDist2 < minBodyDist)
                minBodyDist = bodyDist2;
            if (bodyDist3 < minBodyDist)
                minBodyDist = bodyDist3;
            if (bodyDist4 < minBodyDist)
                minBodyDist = bodyDist4;

            bool insideBodyHitbox = minBodyDist <= 160f * NPC.scale;

            float rightDist1 = Vector2.Distance(rightHitboxCenter, targetHitbox.TopLeft());
            float rightDist2 = Vector2.Distance(rightHitboxCenter, targetHitbox.TopRight());
            float rightDist3 = Vector2.Distance(rightHitboxCenter, targetHitbox.BottomLeft());
            float rightDist4 = Vector2.Distance(rightHitboxCenter, targetHitbox.BottomRight());

            float minRightDist = rightDist1;
            if (rightDist2 < minRightDist)
                minRightDist = rightDist2;
            if (rightDist3 < minRightDist)
                minRightDist = rightDist3;
            if (rightDist4 < minRightDist)
                minRightDist = rightDist4;

            bool insideRightHitbox = minRightDist <= 120f * NPC.scale;

            return (insideLeftHitbox || insideBodyHitbox || insideRightHitbox) && NPC.alpha == 0 && NPC.ai[0] > 1f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 480, true);
        }
    }
}
