using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Mounts;
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
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.AstrumAureus
{
    [AutoloadBossHead]
    public class AstrumAureus : ModNPC
    {
        private bool stomping = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astrum Aureus");
            Main.npcFrameCount[NPC.type] = 6;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
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
            NPC.LifeMaxNERB(NPC.downedMoonlord ? 196000 : 98000, NPC.downedMoonlord ? 235200 : 117600, 740000); // 30 seconds in boss rush
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 60, 0, 0);
            NPC.boss = true;
            NPC.DeathSound = SoundID.NPCDeath14;
            music = CalamityMod.Instance.GetMusicFromMusicMod("AstrumAureus") ?? MusicID.Boss3;
            bossBag = ModContent.ItemType<AstrageldonBag>();
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(stomping);
            writer.Write(NPC.alpha);
            writer.Write(NPC.localAI[2]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            stomping = reader.ReadBoolean();
            NPC.alpha = reader.ReadInt32();
            NPC.localAI[2] = reader.ReadSingle();
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
                    {
                        stomping = false;
                    }
                    NPC.frameCounter += 1.0;
                    if (NPC.frameCounter > 12.0)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0.0;
                    }
                    if (NPC.frame.Y >= frameHeight * 6)
                    {
                        NPC.frame.Y = 0;
                    }
                }
                else if (NPC.velocity.Y <= 0f || NPC.ai[1] < 0f) //prepare to jump and then jump
                {
                    NPC.frameCounter += 1.0;
                    if (NPC.frameCounter > 12.0)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0.0;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                    {
                        NPC.frame.Y = frameHeight * 5;
                    }
                }
                else //stomping
                {
                    if (!stomping)
                    {
                        stomping = true;
                        NPC.frameCounter = 0.0;
                        NPC.frame.Y = 0;
                    }
                    NPC.frameCounter += 1.0;
                    if (NPC.frameCounter > 12.0)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0.0;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                    {
                        NPC.frame.Y = frameHeight * 5;
                    }
                }
            }
            else if (NPC.ai[0] >= 5f)
            {
                if (stomping)
                {
                    stomping = false;
                }
                if (NPC.velocity.Y == 0f) //idle before teleport
                {
                    NPC.frameCounter += 1.0;
                    if (NPC.frameCounter > 12.0)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0.0;
                    }
                    if (NPC.frame.Y >= frameHeight * 6)
                    {
                        NPC.frame.Y = 0;
                    }
                }
                else //in-air
                {
                    NPC.frameCounter += 1.0;
                    if (NPC.frameCounter > 12.0)
                    {
                        NPC.frame.Y = NPC.frame.Y + frameHeight;
                        NPC.frameCounter = 0.0;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                    {
                        NPC.frame.Y = frameHeight * 5;
                    }
                }
            }
            else
            {
                if (stomping)
                {
                    stomping = false;
                }
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter > 8.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                {
                    NPC.frame.Y = 0;
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Texture2D NPCTexture = TextureAssets.Npc[NPC.type].Value;
            Texture2D GlowMaskTexture = TextureAssets.Npc[NPC.type].Value;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            if (NPC.ai[0] == 0f)
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
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2(NPC.width, NPC.height) / 2f - Main.screenPosition;
                    vector41 -= new Vector2(NPCTexture.Width, NPCTexture.Height / frameCount) * scale / 2f;
                    vector41 += vector11 * scale + new Vector2(0f, 4f + offsetY);
                    spriteBatch.Draw(NPCTexture, vector41, frame, color38, rotation, vector11, scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = NPC.Center - Main.screenPosition;
            vector43 -= new Vector2(NPCTexture.Width, NPCTexture.Height / frameCount) * scale / 2f;
            vector43 += vector11 * scale + new Vector2(0f, 4f + offsetY);
            spriteBatch.Draw(NPCTexture, vector43, frame, NPC.GetAlpha(drawColor), rotation, vector11, scale, spriteEffects, 0f);

            if (NPC.ai[0] != 1) //draw only if not recharging
            {
                Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Gold);
                Color color40 = Color.Lerp(Color.White, color, 0.5f);

                if (CalamityConfig.Instance.Afterimages)
                {
                    for (int num163 = 1; num163 < num153; num163++)
                    {
                        Color color41 = color40;
                        color41 = Color.Lerp(color41, color36, amount9);
                        color41 = NPC.GetAlpha(color41);
                        color41 *= (num153 - num163) / 15f;
                        Vector2 vector44 = NPC.oldPos[num163] + new Vector2(NPC.width, NPC.height) / 2f - Main.screenPosition;
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

        public override void NPCLoot()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            DropHelper.DropBags(NPC);

            DropHelper.DropItemChance(NPC, ModContent.ItemType<AstrageldonTrophy>(), 10);
            DropHelper.DropItemCondition(NPC, ModContent.ItemType<KnowledgeAstrumAureus>(), true, !DownedBossSystem.downedAstrageldon);

            CalamityGlobalNPC.SetNewShopVariable(new int[] { NPCID.Wizard, ModContent.NPCType<FAP>() }, DownedBossSystem.downedAstrageldon);

            // All other drops are contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                // Materials
                DropHelper.DropItemSpray(NPC, ModContent.ItemType<Stardust>(), 20, 30, 2);
                DropHelper.DropItemSpray(NPC, ItemID.FallenStar, 18, 24, 2);

                // Weapons
                float w = DropHelper.NormalWeaponDropRateFloat;
                DropHelper.DropEntireWeightedSet(NPC,
                    DropHelper.WeightStack<Nebulash>(w),
                    DropHelper.WeightStack<AuroraBlazer>(w),
                    DropHelper.WeightStack<AlulaAustralis>(w),
                    DropHelper.WeightStack<BorealisBomber>(w),
                    DropHelper.WeightStack<AuroradicalThrow>(w)
                );

                // Equipment
                DropHelper.DropItem(NPC, ModContent.ItemType<GravistarSabaton>(), true);
                DropHelper.DropItemCondition(NPC, ModContent.ItemType<SquishyBeanMount>(), NPC.downedMoonlord);

                // Vanity
                DropHelper.DropItemChance(NPC, ModContent.ItemType<AureusMask>(), 7);

                // Other
                DropHelper.DropItem(NPC, ModContent.ItemType<AstralJelly>(), 9, 12);
            }

            DropHelper.DropItemCondition(NPC, ModContent.ItemType<LeonidProgenitor>(), !Main.expertMode, 0.1f);

            // Other
            DropHelper.DropItemChance(NPC, ItemID.HallowedKey, 3);

            // Drop an Astral Meteor if applicable
            ThreadPool.QueueUserWorkItem(_ => AstralBiome.PlaceAstralMeteor());

            // If Astrum Aureus has not yet been killed, notify players of new Astral enemy drops
            if (!DownedBossSystem.downedAstrageldon)
            {
                string key = "Mods.CalamityMod.AureusBossText";
                string key2 = "Mods.CalamityMod.AureusBossText2";
                Color messageColor = Color.Gold;

                CalamityUtils.DisplayLocalizedText(key, messageColor);
                CalamityUtils.DisplayLocalizedText(key2, messageColor);
            }

            // Mark Astrum Aureus as dead
            DownedBossSystem.downedAstrageldon = true;
            CalamityNetcode.SyncWorld();
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 20;
                SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/NPCHit/AstrumAureusHit"), NPC.Center);
            }

            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 150;
                NPC.height = 100;
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

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        // Can only hit the target if within certain distance
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Vector2 npcCenter = NPC.Center;

            // NOTE: Right and left hitboxes are interchangeable, each hitbox is the same size and is located to the right or left of the center hitbox.
            Rectangle leftHitbox = new Rectangle((int)(npcCenter.X - 93f - 5f + 4f), (int)(npcCenter.Y + 33f - 5f), 10, 10);
            Rectangle bodyHitbox = new Rectangle((int)(npcCenter.X - (NPC.width / 4f)), (int)(npcCenter.Y - (NPC.height / 2f) + 24f), NPC.width / 2, NPC.height);
            Rectangle rightHitbox = new Rectangle((int)(npcCenter.X + 93f - 5f - 4f), (int)(npcCenter.Y + 33f - 5f), 10, 10);

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

            bool insideLeftHitbox = minLeftDist <= 120f;

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

            bool insideBodyHitbox = minBodyDist <= 160f;

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

            bool insideRightHitbox = minRightDist <= 120f;

            return (insideLeftHitbox || insideBodyHitbox || insideRightHitbox) && NPC.alpha == 0 && NPC.ai[0] > 1f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 240, true);
        }
    }
}
