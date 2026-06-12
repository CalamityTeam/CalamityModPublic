using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.NPCs;
using CalamityMod.Packets.Entities;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class TheGift : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Tools";
        public static readonly SoundStyle BadBuzzer = new SoundStyle("CalamityMod/Sounds/Custom/BadGiftBuzzer") with { Volume = 0.85f };

        public static bool CanApplyToNPC(NPC npc)
        {
            return npc.isLikeATownNPC && !NPCID.Sets.NoTownNPCHappiness[npc.type] && !NPCID.Sets.IsTownPet[npc.type] && !npc.GetGlobalNPC<CalamityGlobalTownNPC>().TheGiftStatus.HasValue;
        }

        public static void ApplyGiftEffects(NPC npc, bool positive)
        {
            Color color = positive ? Color.Green : Color.Red;
            SoundEngine.PlaySound(positive ? SoundID.ResearchComplete : BadBuzzer, npc.Center);

            // nothing ever happens....
            for (int i = 0; i < 6; i++)
            {
                Vector2 spawnPos = npc.Center + Main.rand.NextVector2Circular(30f, 30f);
                HealingPlus sparkle = new(spawnPos, 0.5f, (spawnPos - npc.Center).SafeNormalize(Vector2.Zero) * 0.65f, color, color * 0.2f, 25)
                {
                    Rotation = positive ? 0f : MathHelper.PiOver4
                };
                GeneralParticleHandler.SpawnParticle(sparkle);
            }

            for (int j = 0; j < 4; j++)
            {
                Vector2 spawnPos = npc.Center + Main.rand.NextVector2Circular(50f, 50f);
                Vector2 arrowVelocity = -Vector2.UnitY * 0.6f * positive.ToDirectionInt();
                StatChangeArrow arrow = new(spawnPos, arrowVelocity, -MathHelper.PiOver2 * positive.ToDirectionInt(), color, color * 0.1f, 0.7f, 40);
                GeneralParticleHandler.SpawnParticle(arrow);
            }
        }

        public static bool TryApplyGift(Item item, NPC npc, bool? forcedOutcome = null)
        {
            if (!item.active || item.type != ModContent.ItemType<TheGift>() || Main.remixWorld || !CanApplyToNPC(npc))
                return false;

            bool positive = forcedOutcome ?? Main.rand.NextBool();
            var townNPC = npc.GetGlobalNPC<CalamityGlobalTownNPC>();
            townNPC.TheGiftStatus = positive;
            townNPC.TheGiftReset = 0.0;
            ApplyGiftEffects(npc, positive);

            item.stack--;
            if (item.stack <= 0)
            {
                item.active = false;
                item.type = ItemID.None;
            }

            if (Main.netMode == NetmodeID.Server)
            {
                npc.netUpdate = true;
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item.whoAmI);
                TheGiftEffectsPacket.Send(npc, positive);
            }

            return true;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.useTime = Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.noMelee = true;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(gold: 5);  // Sold by Shady Salesman
            Item.rare = ItemRarityID.Green;
        }

        public override void Load()
        {
            On_ShopHelper.ProcessMood += TheGiftHappiness;
        }

        public override void Update(ref float gravity, ref float maxFallSpeed)
        {
            // Controls actually applying the effect to an NPC
            // None of this should work in Remix because Remix disables happiness
            bool localGiftCollisionCheck = Main.netMode == NetmodeID.SinglePlayer || Item.playerIndexTheItemIsReservedFor == Main.myPlayer;
            if (Item.noGrabDelay <= 0 && !Main.remixWorld && localGiftCollisionCheck)
            {
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (!CanApplyToNPC(n))
                        continue;

                    if (Item.Hitbox.Intersects(n.Hitbox))
                    {
                        if (Main.netMode == NetmodeID.SinglePlayer)
                            TryApplyGift(Item, n);
                        else if (Main.netMode == NetmodeID.MultiplayerClient)
                            RequestApplyTheGiftPacket.Send(Item, n);

                        // Make sure it can't apply to multiple NPCs at once
                        break;
                    }
                }
            }
        }

        // Also contains logic for The Monument, so I can make sure these apply in the correct order
        private static void TheGiftHappiness(On_ShopHelper.orig_ProcessMood orig, ShopHelper self, Player player, NPC npc)
        {
            orig(self, player, npc);

            // Immediately exit early if it's Remix world, or if it's an NPC that should not be affected by happiness
            if (Main.remixWorld || !npc.isLikeATownNPC || NPCID.Sets.NoTownNPCHappiness[npc.type] || NPCID.Sets.IsTownPet[npc.type])
                return;

            var gtnpc = npc.GetGlobalNPC<CalamityGlobalTownNPC>();
            // The Monument lowers happiness by a fixed amount. This is not applied to the Tax Collector.
            if (npc.type != NPCID.TaxCollector && gtnpc.SearchForTheMonument(npc))
            {
                float oldHappiness = self._currentPriceAdjustment;
                self._currentPriceAdjustment += TheMonument.MonumentHappinessReduction;
                self.LimitAndRoundMultiplier(self._currentPriceAdjustment);
                string dialogueKey;
                if (npc.type < NPCID.Count)
                {
                    dialogueKey = $"Mods.CalamityMod.Vanilla.TownNPCMood.{NPCID.Search.GetName(npc.type)}.Monument";
                    // Zoologist has separate dialogue when transformed
                    if (npc.type == NPCID.BestiaryGirl && npc.ShouldBestiaryGirlBeLycantrope())
                        dialogueKey += "Transformed";
                }
                else
                {
                    var modNPC = NPCLoader.GetNPC(npc.type);
                    dialogueKey = $"Mods.{modNPC.Mod.Name}.NPCs.{modNPC.Name}.TownNPCMood.Monument";
                }

                // If the NPC is content, it should override that text because they are no longer content
                if (oldHappiness == 1f)
                    self._currentHappiness = Language.Exists(dialogueKey) ? Language.GetTextValue(dialogueKey) : CalamityUtils.GetTextValue("Vanilla.TownNPCMood.DefaultMonument") + " ";
                else
                    self._currentHappiness += Language.Exists(dialogueKey) ? Language.GetTextValue(dialogueKey) : CalamityUtils.GetTextValue("Vanilla.TownNPCMood.DefaultMonument") + " ";
            }

            // The Gift sets happiness to a fixed either extremely high or extremely low value, depending on its random state.
            bool? gift = gtnpc.TheGiftStatus;
            if (gift.HasValue)
            {
                if (gift.Value)
                    self._currentPriceAdjustment = 0.5f;
                else
                    self._currentPriceAdjustment = 1.75f;

                string locKey = gift.Value ? "GiftPositive" : "GiftNegative";
                string dialogueKey;
                if (npc.type < NPCID.Count)
                {
                    dialogueKey = $"Mods.CalamityMod.Vanilla.TownNPCMood.{NPCID.Search.GetName(npc.type)}.{locKey}";
                    // Zoologist has separate dialogue when transformed
                    if (npc.type == NPCID.BestiaryGirl && npc.ShouldBestiaryGirlBeLycantrope())
                        dialogueKey += "Transformed";
                }
                else
                {
                    var modNPC = NPCLoader.GetNPC(npc.type);
                    dialogueKey = $"Mods.{modNPC.Mod.Name}.NPCs.{modNPC.Name}.TownNPCMood.{locKey}";
                }
                // Yes, it's intentional that this completely overrides all other happiness report dialogue
                self._currentHappiness = Language.Exists(dialogueKey) ? Language.GetTextValue(dialogueKey) : CalamityUtils.GetTextValue($"Vanilla.TownNPCMood.Default{locKey}") + " ";
            }
        }
    }
}
