using System.Collections.Generic;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.Personalities;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityMod.NPCs.TownNPCs
{
    [AutoloadHead]
    public class THIEF : ModNPC
    {
        public static Asset<Texture2D> AltTexture;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 23;
            NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
            NPCID.Sets.AttackFrameCount[NPC.type] = 4;
            NPCID.Sets.DangerDetectRange[NPC.type] = 500;
            NPCID.Sets.AttackType[NPC.type] = 0;
            NPCID.Sets.AttackTime[NPC.type] = 60;
            NPCID.Sets.AttackAverageChance[NPC.type] = 10;
            NPCID.Sets.ShimmerTownTransform[Type] = false;
            NPC.Happiness
                .SetBiomeAffection<DesertBiome>(AffectionLevel.Like)
                .SetBiomeAffection<JungleBiome>(AffectionLevel.Dislike)
                .SetNPCAffection(NPCID.GoblinTinkerer, AffectionLevel.Like)
                .SetNPCAffection(NPCID.Dryad, AffectionLevel.Dislike);
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifiers);
            if (!Main.dedServ)
            {
                AltTexture = ModContent.Request<Texture2D>(Texture + "Alt", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.lavaImmune = false;
            NPC.width = 18;
            NPC.height = 44;
            NPC.aiStyle = NPCAIStyleID.Passive;
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250; //Im not special :(
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.PartyGirl;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.THIEF")
            });
        }

        public override void AI()
        {
            if (!CalamityWorld.spawnedBandit)
            {
                CalamityWorld.spawnedBandit = true;
            }
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            if (CalamityWorld.spawnedBandit)
                return true;

            foreach (Player player in Main.ActivePlayers)
            {
                bool rich = player.InventoryHas(ItemID.PlatinumCoin) || player.PortableStorageHas(ItemID.PlatinumCoin);
                if (rich)
                    return NPC.downedBoss3;
            }
            return false;
        }

        public override List<string> SetNPCNameList() => new List<string>()
        {
            // Patron names
            "Xplizzy", // <@!98826096237109248> (whitegiraffe)
            "Freakish", // <@!750363283520749598> (freak5650)
            "Calder", // <@!601897959176798228> (hardlightcaster)
            "Hunter Jinx", // <@!757401399783850134> (dragonslayerornstein.)
            "Goose", // <@!591421917706321962> (dullelili)
            "Jackson", // <@!525827730646892549> (chowchow360)
            "Altarca", // <@!1140673052108128337> (altarca_27226_49175)
            "Jackie", // <@!353241811717718016> (jackalchan)

            // Original names
            this.GetLocalizedValue("Name.Laura"),
            this.GetLocalizedValue("Name.Mie"),
            this.GetLocalizedValue("Name.Bonnie"),
            this.GetLocalizedValue("Name.Sarah"),
            this.GetLocalizedValue("Name.Diane"),
            this.GetLocalizedValue("Name.Kate"),
            this.GetLocalizedValue("Name.Penelope"),
            this.GetLocalizedValue("Name.Marisa"),
            this.GetLocalizedValue("Name.Maribel"),
            this.GetLocalizedValue("Name.Valerie"),
            this.GetLocalizedValue("Name.Jessica"),
            this.GetLocalizedValue("Name.Rowan"),
            this.GetLocalizedValue("Name.Jessie"),
            this.GetLocalizedValue("Name.Jade"),
            this.GetLocalizedValue("Name.Hearn"),
            this.GetLocalizedValue("Name.Amber"),
            this.GetLocalizedValue("Name.Anne"),
            this.GetLocalizedValue("Name.Indiana")
        };

        public override string GetChat()
        {
            if (Main.bloodMoon)
                return this.GetLocalizedValue("Chat.BloodMoon" + Main.rand.Next(1, 4 + 1));

            WeightedRandom<string> dialogue = new WeightedRandom<string>();

            dialogue.Add(this.GetLocalizedValue("Chat.Normal1"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal2"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal3"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal4"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal5"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal6"));
            dialogue.Add(this.GetLocalizedValue("Chat.Normal7"));

            if (!Main.dayTime)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.Night1"));
                dialogue.Add(this.GetLocalizedValue("Chat.Night2"));
            }

            int witch = NPC.FindFirstNPC(ModContent.NPCType<WITCH>());
            if (witch != -1)
                dialogue.Add(this.GetLocalization("Chat.BrimstoneWitch").Format(Main.npc[witch].GivenName));

            //please help me I'm stuck in a children's video game - Fabsol
            int cirrusIndex = NPC.FindFirstNPC(ModContent.NPCType<FAP>());
            if (cirrusIndex != -1)
                dialogue.Add(this.GetLocalization("Chat.DrunkPrincess").Format(Main.npc[cirrusIndex].GivenName));

            int merchantIndex = NPC.FindFirstNPC(NPCID.Merchant);
            if (merchantIndex != -1)
                dialogue.Add(this.GetLocalization("Chat.Merchant").Format(Main.npc[merchantIndex].GivenName));

            int armsDealerIndex = NPC.FindFirstNPC(NPCID.ArmsDealer);
            int nurseIndex = NPC.FindFirstNPC(NPCID.Nurse);
            if (armsDealerIndex != -1 && nurseIndex != -1)
                dialogue.Add(this.GetLocalization("Chat.NurseArmsDealer").Format(Main.npc[nurseIndex].GivenName, Main.npc[armsDealerIndex].GivenName));

            if (NPC.GivenName == this.GetLocalizedValue("Name.Laura"))
                dialogue.Add(this.GetLocalizedValue("Chat.NamedLaura"));

            if (NPC.GivenName == this.GetLocalizedValue("Name.Penelope"))
                dialogue.Add(this.GetLocalizedValue("Chat.NamedPenelope"));

            if (NPC.GivenName == this.GetLocalizedValue("Name.Valerie"))
                dialogue.Add(this.GetLocalizedValue("Chat.NamedValerie"));

            if (NPC.GivenName == this.GetLocalizedValue("Name.Rowan"))
                dialogue.Add(this.GetLocalizedValue("Chat.NamedRowan"));

            if (Main.LocalPlayer.ZoneJungle)
                dialogue.Add(this.GetLocalizedValue("Chat.Jungle"));

            if (BirthdayParty.PartyIsUp)
                dialogue.Add(this.GetLocalizedValue("Chat.Party"));

            if (Main.hardMode)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.Hardmode1"));
                dialogue.Add(this.GetLocalizedValue("Chat.Hardmode2"));
                dialogue.Add(this.GetLocalizedValue("Chat.Hardmode3"));
            }
            if (NPC.downedMoonlord)
            {
                dialogue.Add(this.GetLocalizedValue("Chat.MoonLordDefeated1"));
                dialogue.Add(this.GetLocalizedValue("Chat.MoonLordDefeated2"));
                dialogue.Add(this.GetLocalizedValue("Chat.MoonLordDefeated3"));
            }

            if (Main.LocalPlayer.InventoryHas(ItemID.BoneGlove))
                dialogue.Add(this.GetLocalizedValue("Chat.HasBoneGlove"));

            if (Main.LocalPlayer.InventoryHas(ModContent.ItemType<Valediction>()))
                dialogue.Add(this.GetLocalizedValue("Chat.HasValediction"));

            return dialogue;
        }

        public string Refund()
        {
            int goblinIndex = NPC.FindFirstNPC(NPCID.GoblinTinkerer);
            if (goblinIndex != -1 && CalamityWorld.Reforges >= 1)
            {
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    DoRefund(bandit: NPC);
                }
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    // Possible Bug here: Minor text bug when two players send request this simultaneously
                    // Which result to both player to have successful message but only one request got accepted on server
                    // But since this is how base gamecode works theres no way to fix this clean way (Unless someone implement net queued response for NPC dialog)
                    // And as this does not duplicate the coin amount, It's not that bad I think...?
                    //
                    // Other way possible is to having bandit stolen inventory per player
                    // But I didn't wanted to change system too much
                    ModPacket packet = CalamityMod.Instance.GetPacket();
                    packet.Write((byte)CalamityModMessageType.WantToRefundReforges);
                    packet.Write((byte)Main.myPlayer);
                    packet.Send();
                }

                SoundEngine.PlaySound(SoundID.Coins); // Money dink sound
                switch (Main.rand.Next(2))
                {
                    case 0:
                        return this.GetLocalization("Refund1").Format(Main.npc[goblinIndex].GivenName);
                    case 1:
                        return this.GetLocalizedValue("Refund2");
                }
            }
            return this.GetLocalizedValue("NoRefund");
        }

        public static void DoRefund(NPC bandit)
        {
            if (bandit == null)
                return;

            if (CalamityWorld.Reforges <= 0)
                return;

            int[] coinCounts = Utils.CoinsSplit(CalamityWorld.MoneyStolenByBandit);
            if (coinCounts[0] > 0)
                Item.NewItem(new EntitySource_Gift(bandit), bandit.Hitbox, ItemID.CopperCoin, coinCounts[0]);
            if (coinCounts[1] > 0)
                Item.NewItem(new EntitySource_Gift(bandit), bandit.Hitbox, ItemID.SilverCoin, coinCounts[1]);
            if (coinCounts[2] > 0)
                Item.NewItem(new EntitySource_Gift(bandit), bandit.Hitbox, ItemID.GoldCoin, coinCounts[2]);
            if (coinCounts[3] > 0)
                Item.NewItem(new EntitySource_Gift(bandit), bandit.Hitbox, ItemID.PlatinumCoin, coinCounts[3]);

            CalamityWorld.MoneyStolenByBandit = 0;
            CalamityWorld.Reforges = 0;
            CalamityNetcode.SyncWorld();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Main.LocalPlayer.Calamity().trippy)
                return false;

            var something = NPC.direction == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            spriteBatch.Draw(BirthdayParty.PartyIsUp ? AltTexture.Value : TextureAssets.Npc[NPC.type].Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY) - new Vector2(0f, 6f), NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() / 2, NPC.scale, something, 0);
            return false;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28");
            button2 = this.GetLocalizedValue("RefundButton"); ;
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shopName)
        {
            if (firstButton)
            {
                shopName = "Shop";
            }
            else
            {
                Main.npcChatText = Refund();
            }
        }
        public override void AddShops()
        {
            Condition potionSells = CalamityConditions.PotionSellingConfig;
            Condition downedCalclone = CalamityConditions.DownedCalamitasClone;
            Condition downedDoG = CalamityConditions.DownedDevourerOfGods;
            Condition downedYharon = CalamityConditions.DownedYharon;

            NPCShop shop = new(Type);
            shop.AddWithCustomValue(ModContent.ItemType<Cinquedea>(), Item.buyPrice(gold: 9))
                .AddWithCustomValue(ModContent.ItemType<Glaive>(), Item.buyPrice(gold: 9))
                .AddWithCustomValue(ModContent.ItemType<Kylie>(), Item.buyPrice(gold: 9))
                .AddWithCustomValue(ModContent.ItemType<OldDie>(), Item.buyPrice(gold: 40))
                .Add(ItemID.TigerClimbingGear)
                .AddWithCustomValue(ItemID.InvisibilityPotion, Item.buyPrice(silver: 25), potionSells, Condition.HappyEnough)
                .AddWithCustomValue(ItemID.NightOwlPotion, Item.buyPrice(silver: 25), potionSells, Condition.HappyEnough)
                .AddWithCustomValue(ModContent.ItemType<SlickCane>(), Item.buyPrice(gold: 25))
                .Add(ModContent.ItemType<ThiefsDime>(), Condition.DownedPirates)
                .AddWithCustomValue(ModContent.ItemType<MomentumCapacitor>(), Item.buyPrice(gold: 60), Condition.DownedMechBossAll)
                .Add(ModContent.ItemType<DeepWounder>(), downedCalclone)
                .Add(ModContent.ItemType<MonkeyDarts>(), Condition.DownedPlantera)
                .Add(ModContent.ItemType<GloveOfPrecision>(), Condition.DownedPlantera)
                .Add(ModContent.ItemType<GloveOfRecklessness>(), Condition.DownedPlantera)
                .AddWithCustomValue(ModContent.ItemType<EtherealExtorter>(), Item.buyPrice(1), Condition.DownedGolem)
                .AddWithCustomValue(ModContent.ItemType<CelestialReaper>(), Item.buyPrice(2), Condition.DownedMoonLord)
                .AddWithCustomValue(ModContent.ItemType<VeneratedLocket>(), Item.buyPrice(25), downedDoG)
                .AddWithCustomValue(ModContent.ItemType<DragonScales>(), Item.buyPrice(40), downedYharon)
                .Add(ModContent.ItemType<BearsEye>()) //:BearWatchingYou:
                .Register();
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Bandit").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Bandit2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Bandit3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Bandit4").Type, 1f);
                }
            }
        }

        // Make this Town NPC teleport to the Queen or King statue when triggered.
        public override bool CanGoToStatue(bool toKingStatue) => true;

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 50;
            knockback = 2f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 180;
            randExtraCooldown = 60;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ModContent.ProjectileType<CinquedeaProj>();
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 6f;
        }
    }
}
