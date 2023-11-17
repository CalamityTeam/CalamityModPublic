using CalamityMod.Balancing;
using CalamityMod.Items.Fishing;
using CalamityMod.Items.Materials;
using CalamityMod.Items.TreasureBags.MiscGrabBags;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.NormalNPCs;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using System;
using System.Collections.Generic;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.ILEditing
{
    public partial class ILChanges
    {
        #region Decrease Sandstorm Wind Speed Requirement
        private static void DecreaseSandstormWindSpeedRequirement(ILContext il)
        {
            // Sandstorms don't rapidly diminish unless the wind speed is less than 0.2f instead of 0.6f.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(0.6f))) // The 0.6f wind speed check.
            {
                LogFailure("Decrease Sandstorm Wind Speed Requirement", "Could not locate the wind speed variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 0.2f); // Change to 0.2f.
        }
        #endregion Decrease Sandstorm Wind Speed Requirement

        #region Reforge Requirement Relaxation
        private static void RelaxPrefixRequirements(ILContext il)
        {
            var cursor = new ILCursor(il);

            // Search for the first instance of Math.Round, which is used to round damage.
            // This one isn't edited, but hitting the Round function is the easiest way to get to the relevant part of the method.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall("System.Math", "Round")))
            {
                LogFailure("Prefix Requirements", "Could not locate the damage Math.Round call.");
                return;
            }

            // Search for the branch-if-not-equal which checks whether the damage change rounds to nothing.
            ILLabel passesDamageCheck = null;
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchBneUn(out passesDamageCheck)))
            {
                LogFailure("Prefix Requirements", "Could not locate damage prefix failure branch.");
                return;
            }

            // Emit an unconditional branch which skips the damage check failure.
            cursor.Emit(OpCodes.Br_S, passesDamageCheck);

            // Search for the branch-if-not-equal which checks whether the use time change rounds to nothing.
            // If the change rounds to nothing, then it's equal, so the branch is NOT taken.
            // The branch skips over the "fail this prefix" code.
            ILLabel passesUseTimeCheck = null;
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchBneUn(out passesUseTimeCheck)))
            {
                LogFailure("Prefix Requirements", "Could not locate use time rounding equality branch.");
                return;
            }

            // To allow use-time affecting prefixes even on super fast weapons where they would round to nothing,
            // add another branch which skips over the "fail this prefix" code, given a custom condition.

            // Load the item itself onto the stack so that it becomes an argument for the following delegate.
            cursor.Emit(OpCodes.Ldarg_0);

            // Emit a delegate which returns whether the item's use time is 2, 3, 4 or 5.
            cursor.EmitDelegate<Func<Item, bool>>((Item i) => i.useAnimation >= 2 && i.useAnimation <= 5);

            cursor.Emit(OpCodes.Brtrue_S, passesUseTimeCheck);

            // Search for the branch-if-not-equal which checks whether the mana change rounds to nothing.
            ILLabel passesManaCheck = null;
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchBneUn(out passesManaCheck)))
            {
                LogFailure("Prefix Requirements", "Could not locate mana prefix failure branch.");
                return;
            }

            // Emit an unconditional branch which skips the mana check failure.
            cursor.Emit(OpCodes.Br_S, passesManaCheck);

            // Search for the instance field load which retrieves the item's knockback.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdfld<Item>("knockBack")))
            {
                LogFailure("Prefix Requirements", "Could not locate knockback load instruction.");
                return;
            }

            // Search for the immediately-following constant load which pulls in 0.0.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(0f)))
            {
                LogFailure("Prefix Requirements", "Could not locate zero knockback comparison constant.");
                return;
            }

            // Completely nullify the knockback computation by replacing the check against 0 with a check against negative one million.
            // If you absolutely need to block knockback reforges for some reason, you can set your knockback to this value.
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, -1000000f);
        }
        #endregion Reforge Requirement Relaxation

        #region Prevention of Slime Rain Spawns When Near Bosses
        private static void PreventBossSlimeRainSpawns(Terraria.On_NPC.orig_SlimeRainSpawns orig, int plr)
        {
            if (!Main.player[plr].Calamity().isNearbyBoss && CalamityConfig.Instance.BossZen)
                orig(plr);
        }
        #endregion Prevention of Slime Rain Spawns When Near Bosses

        #region Remove Feral Bite Random Debuffs
        private static void RemoveFeralBiteRandomDebuffs(ILContext il)
        {
            var cursor = new ILCursor(il);

            // Find the random debuff duration multiplier for the debuffs inflicted by Feral Bite.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcR4(0.01f))) // The 0.01f random debuff duration multiplier.
            {
                LogFailure("Remove Feral Bite Random Debuffs", "Could not locate the Feral Bite random debuff duration multiplier.");
                return;
            }

            // Remove and change to 0f, this makes the random debuffs from Feral Bite have 0 duration.
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 0f);
        }
        #endregion

        #region Disabling of Lava Slime Lava Creation
        private static void RemoveLavaDropsFromExpertLavaSlimes(ILContext il)
        {
            // Prevent Lava Slimes from dropping lava.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchCallOrCallvirt<WorldGen>("SquareTileFrame"))) // The only SquareTileFrame call in HitEffect.
            {
                LogFailure("Remove Lava Drops From Expert Lava Slimes", "Could not locate the SquareTileFrame function call.");
                return;
            }
            if (!cursor.TryGotoPrev(MoveType.Before, i => i.MatchLdcI4(NPCID.LavaSlime))) // The ID of Lava Slimes.
            {
                LogFailure("Remove Lava Drops From Expert Lava Slimes", "Could not locate the Lava Slime ID variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, 0); // Change to an impossible scenario.
        }
        #endregion Disabling of Lava Slime Lava Creation

        #region Make Meteorite Explodable
        private static void MakeMeteoriteExplodable(ILContext il)
        {
            // Find the Tile ID of Meteorite and change it to something that doesn't matter.
            var cursor = new ILCursor(il);

            // There are two checks for the Meteorite Tile ID. The first one is required for the switch cases to function properly, so we need to move past it.
            ILLabel label = null; // pointless label for MatchBeq
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchBeq(out label)))
            {
                LogFailure("Make Meteorite Explodable", "Could not locate the branching instruction.");
                return;
            }

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(TileID.Meteorite))) // The Meteorite Tile ID check.
            {
                LogFailure("Make Meteorite Explodable", "Could not locate the Meteorite Tile ID variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, TileID.HellstoneBrick); // This won't actually do anything since the ID is above Meteorite's and thus unreachable
        }
        #endregion

        #region Make Windy Day Music Play Less Often
        private static void MakeWindyDayMusicPlayLessOften(ILContext il)
        {
            // Make windy day theme only play when the wind speed is over 0.5f instead of 0.4f and make it stop when the wind dies down to below 0.44f instead of 0.34f.
            var cursor = new ILCursor(il);

            FieldInfo _minWindField = typeof(Main).GetField("_minWind", BindingFlags.NonPublic | BindingFlags.Static);

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdsfld(_minWindField))) // The min wind speed check that stops the windy day theme when the wind dies down enough.
            {
                LogFailure("Make Windy Day Music Play Less Often", "Could not locate the _minWind variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 0.44f); // Change to 0.44f.

            FieldInfo _maxWindField = typeof(Main).GetField("_maxWind", BindingFlags.NonPublic | BindingFlags.Static);

            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdsfld(_maxWindField))) // The max wind speed check that causes the windy day theme to play.
            {
                LogFailure("Make Windy Day Music Play Less Often", "Could not locate the _maxWind variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_R4, 0.5f); // Change to 0.5f.
        }
        #endregion Make Windy Day Music Play Less Often

        #region Change Blood Moon Max HP Requirements
        private static void BloodMoonsRequire200MaxLife(ILContext il)
        {
            // Blood Moons only happen when the player has over 200 max life.
            var cursor = new ILCursor(il);
            // Find the moon phase check which will forward the cursor around the Blood Moon portion
            if (!cursor.TryGotoNext(MoveType.After, c => c.MatchLdsfld<Main>("moonPhase")))
            {
                LogFailure("Make Blood Moons Require 200 Max Life", "Could not locate the moon phase check.");
                return;
            }
            // Find the player check itself
            if (!cursor.TryGotoNext(MoveType.After, c => c.MatchCallOrCallvirt<Player>("get_ConsumedLifeCrystals")))
            {
                LogFailure("Make Blood Moons Require 200 Max Life", "Could not locate the Life Crystal check.");
                return;
            }
            // Find the >1 Life Crystal requirement
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(1)))
            {
                LogFailure("Make Blood Moons Require 200 Max Life", "Could not locate the Life Crystal requirement.");
                return;
            }
            cursor.Remove();
            // Change it to >4 Life Crystals, which effectively allows a Blood Moon at 200 natural health.
            cursor.Emit(OpCodes.Ldc_I4, 4);
        }
        #endregion Change Blood Moon Max HP Requirements

        #region Prevent Fossil Shattering
        private static void PreventFossilShattering(ILContext il)
        {
            // Find the Tile ID of Desert Fossil and change it to something that doesn't matter.
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(TileID.DesertFossil))) // The Desert Fossil Tile ID check.
            {
                LogFailure("Prevent Fossil Shattering", "Could not locate the Desert Fossil Tile ID variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, TileID.PixelBox); // Change to Pixel Box because it cannot be obtained in-game without cheating.
        }
        #endregion

        #region Remove Hellforge Pickaxe Requirement
        private static int RemoveHellforgePickaxeRequirement(Terraria.On_Player.orig_GetPickaxeDamage orig, Player self, int x, int y, int pickPower, int hitBufferIndex, Tile tileTarget)
        {
            if (tileTarget.TileType == TileID.Hellforge)
                pickPower = 65;

            return orig(self, x, y, pickPower, hitBufferIndex, tileTarget);
        }
        #endregion

        #region Fix Chlorophyte Crystal Attacking Where it Shouldn't
        // TODO -- Finish this
        #endregion Fix Chlorophyte Crystal Attacking Where it Shouldn't

        #region Color Blighted Gel
        private static void ColorBlightedGel(Terraria.GameContent.ItemDropRules.On_CommonCode.orig_ModifyItemDropFromNPC orig, NPC npc, int itemIndex)
        {
            orig(npc, itemIndex);

            Item item = Main.item[itemIndex];
            int itemID = item.type;
            bool colorWasChanged = false;

            if (itemID == ModContent.ItemType<BlightedGel>() && npc.type == ModContent.NPCType<CrimulanBlightSlime>())
            {
                item.color = new Color(1f, 0f, 0.16f, 0.6f);
                colorWasChanged = true;
            }
            if (itemID == ItemID.SharkFin && npc.type == ModContent.NPCType<Mauler>())
            {
                item.color = new Color(151, 115, 57, 255);
                colorWasChanged = true;
            }

            // Sync the color changes.
            if (colorWasChanged)
                NetMessage.SendData(MessageID.ItemTweaker, -1, -1, null, itemID, 1f);
        }
        #endregion Color Blighted Gel

        #region Improve Angler Quest Rewards
        private static void ImproveAnglerRewards(Terraria.On_Player.orig_GetAnglerReward orig, Player self, NPC angler, int questItemType)
        {
            orig(self, angler, questItemType);

            EntitySource_Gift source = new EntitySource_Gift(angler);
            int questsDone = self.anglerQuestsFinished;
            float rarityReduction = 1f;
            rarityReduction = (questsDone <= 50) ? (rarityReduction - questsDone * 0.01f) : ((questsDone <= 100) ? (0.5f - (questsDone - 50) * 0.005f) : ((questsDone > 150) ? 0.15f : (0.25f - (questsDone - 100) * 0.002f)));
            rarityReduction *= 0.9f;
            rarityReduction *= (float)(self.currentShoppingSettings.PriceAdjustment + 1.0) / 2f;

            if (rarityReduction < 0.1f)
                rarityReduction = 0.1f;

            List<Item> rewardItems = new List<Item>();

            GetItemSettings anglerRewardSettings = GetItemSettings.NPCEntityToPlayerInventorySettings;

            Item item = new Item();

            // GUARANTEED REWARDS

            // BAIT
            switch (questsDone)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    item = new Item();
                    item.SetDefaults(ItemID.Stinkbug);
                    item.stack = Main.rand.Next(2, 6);
                    break;

                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    item = new Item();
                    item.SetDefaults(ItemID.ApprenticeBait);
                    item.stack = Main.rand.Next(2, 6);
                    break;

                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                case 15:
                    item = new Item();
                    item.SetDefaults(Main.rand.NextBool() ? ItemID.Worm : ItemID.Maggot);
                    item.stack = Main.rand.Next(2, 6);
                    break;

                case 16:
                case 17:
                case 18:
                case 19:
                case 20:
                    item = new Item();
                    item.SetDefaults(ItemID.JourneymanBait);
                    item.stack = Main.rand.Next(2, 6);
                    break;

                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                    item = new Item();
                    item.SetDefaults(Main.rand.NextBool() ? ItemID.EnchantedNightcrawler : ItemID.Buggy);
                    item.stack = Main.rand.Next(2, 6);
                    break;

                case 27:
                case 28:
                case 29:
                case 30:
                    item = new Item();
                    item.SetDefaults(ItemID.MasterBait);
                    item.stack = Main.rand.Next(2, 6);
                    break;

                default:
                    item = new Item();
                    item.SetDefaults(ModContent.ItemType<GrandMarquisBait>());
                    item.stack = Main.rand.Next(2, 6);
                    break;
            }

            item.position = self.Center;
            Item item2 = self.GetItem(self.whoAmI, item, anglerRewardSettings);
            rewardItems.Add(item2);

            // COINS
            switch (questsDone)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                case 4:
                    item = new Item();
                    item.SetDefaults(ItemID.GoldCoin);
                    break;

                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                    item = new Item();
                    item.SetDefaults(ItemID.GoldCoin);
                    item.stack = 2;
                    item = new Item();
                    item.SetDefaults(ItemID.SilverCoin);
                    item.stack = 50;
                    break;

                case 10:
                case 11:
                case 12:
                case 13:
                case 14:
                    item = new Item();
                    item.SetDefaults(ItemID.GoldCoin);
                    item.stack = 4;
                    break;

                case 15:
                case 16:
                case 17:
                case 18:
                case 19:
                    item = new Item();
                    item.SetDefaults(ItemID.GoldCoin);
                    item.stack = 6;
                    break;

                case 20:
                case 21:
                case 22:
                case 23:
                case 24:
                case 25:
                case 26:
                case 27:
                case 28:
                case 29:
                    item = new Item();
                    item.SetDefaults(ItemID.GoldCoin);
                    item.stack = 8;
                    break;

                default:
                    item = new Item();
                    item.SetDefaults(ItemID.GoldCoin);
                    item.stack = 10;
                    break;
            }

            item.position = self.Center;
            item2 = self.GetItem(self.whoAmI, item, anglerRewardSettings);
            rewardItems.Add(item2);

            // PRIMARY ITEMS
            switch (questsDone)
            {
                case 0:
                case 1:
                    item = new Item();
                    item.SetDefaults(ModContent.ItemType<Spadefish>());
                    rewardItems.Add(item);
                    break;

                case 2:
                    item = new Item();
                    item.SetDefaults(ModContent.ItemType<StuffedFish>());
                    item.stack = Main.rand.Next(4, 10);
                    rewardItems.Add(item);
                    break;

                case 3:
                    item = new Item();
                    item.SetDefaults(ItemID.HighTestFishingLine);
                    rewardItems.Add(item);
                    break;

                case 4:
                    item = new Item();
                    item.SetDefaults(ItemID.FishHook);
                    rewardItems.Add(item);
                    break;

                case 5:
                    item = new Item();
                    item.SetDefaults(ItemID.FuzzyCarrot);
                    rewardItems.Add(item);
                    break;

                case 6:
                    item = new Item();
                    item.SetDefaults(ItemID.FishermansGuide);
                    rewardItems.Add(item);
                    break;

                case 7:
                    item = new Item();
                    item.SetDefaults(ItemID.FishCostumeMask);
                    rewardItems.Add(item);
                    item = new Item();
                    item.SetDefaults(ItemID.FishCostumeShirt);
                    rewardItems.Add(item);
                    item = new Item();
                    item.SetDefaults(ItemID.FishCostumeFinskirt);
                    rewardItems.Add(item);
                    item = new Item();
                    item.SetDefaults(ModContent.ItemType<SandyAnglingKit>());
                    rewardItems.Add(item);
                    break;

                case 8:
                    item = new Item();
                    item.SetDefaults(ItemID.FishMinecart);
                    rewardItems.Add(item);
                    break;

                case 9:
                    item = new Item();
                    item.SetDefaults(ItemID.SailfishBoots);
                    rewardItems.Add(item);
                    break;

                case 10:
                    item = new Item();
                    item.SetDefaults(ItemID.AnglerHat);
                    rewardItems.Add(item);
                    item = new Item();
                    item.SetDefaults(ItemID.AnglerVest);
                    rewardItems.Add(item);
                    item = new Item();
                    item.SetDefaults(ItemID.AnglerPants);
                    rewardItems.Add(item);
                    break;

                case 11:
                    item = new Item();
                    item.SetDefaults(ItemID.WeatherRadio);
                    rewardItems.Add(item);
                    break;

                case 12:
                    item = new Item();
                    item.SetDefaults(ItemID.FishingBobber);
                    rewardItems.Add(item);
                    break;

                case 13:
                    item = new Item();
                    item.SetDefaults(ItemID.SeashellHairpin);
                    rewardItems.Add(item);
                    item = new Item();
                    item.SetDefaults(ItemID.MermaidAdornment);
                    rewardItems.Add(item);
                    item = new Item();
                    item.SetDefaults(ItemID.MermaidTail);
                    rewardItems.Add(item);
                    item = new Item();
                    item.SetDefaults(ModContent.ItemType<SandyAnglingKit>());
                    rewardItems.Add(item);
                    break;

                case 14:
                    item = new Item();
                    item.SetDefaults(ItemID.Sextant);
                    rewardItems.Add(item);
                    break;

                case 15:
                    item = new Item();
                    item.SetDefaults(ItemID.TackleBox);
                    rewardItems.Add(item);
                    break;

                case 16:
                    item = new Item();
                    item.SetDefaults(ItemID.SuperAbsorbantSponge);
                    rewardItems.Add(item);
                    break;

                case 17:
                    item = new Item();
                    item.SetDefaults(ItemID.HoneyAbsorbantSponge);
                    rewardItems.Add(item);
                    break;

                case 18:
                    item = new Item();
                    item.SetDefaults(ItemID.MagicConch);
                    rewardItems.Add(item);
                    break;

                case 19:
                    item = new Item();
                    item.SetDefaults(ItemID.DemonConch);
                    rewardItems.Add(item);
                    break;

                case 20:
                    item = new Item();
                    item.SetDefaults(ItemID.AnglerEarring);
                    rewardItems.Add(item);
                    break;

                case 21:
                    item = new Item();
                    item.SetDefaults(ItemID.LavaFishingHook);
                    rewardItems.Add(item);
                    break;

                case 22:
                    item = new Item();
                    item.SetDefaults(ItemID.HotlineFishingHook);
                    rewardItems.Add(item);
                    break;

                case 23:
                    item = new Item();
                    item.SetDefaults(ItemID.FrogLeg);
                    rewardItems.Add(item);
                    break;

                case 24:
                    item = new Item();
                    item.SetDefaults(ItemID.SuperheatedBlood);
                    rewardItems.Add(item);
                    break;

                case 25:
                    item = new Item();
                    item.SetDefaults(ItemID.BottomlessBucket);
                    rewardItems.Add(item);
                    break;

                case 26:
                    item = new Item();
                    item.SetDefaults(ItemID.Sundial);
                    rewardItems.Add(item);
                    break;

                case 27:
                    item = new Item();
                    item.SetDefaults(ItemID.BottomlessHoneyBucket);
                    rewardItems.Add(item);
                    break;

                case 28:
                    item = new Item();
                    item.SetDefaults(ItemID.GoldenBugNet);
                    rewardItems.Add(item);
                    break;

                case 29:
                    item = new Item();
                    item.SetDefaults(ItemID.BottomlessLavaBucket);
                    rewardItems.Add(item);
                    break;

                case 30:
                    item = new Item();
                    item.SetDefaults(ItemID.GoldenFishingRod);
                    rewardItems.Add(item);
                    break;
            }

            // RANDOM DROPS

            // Angling Kits
            if (Main.rand.NextBool((int)(12f * rarityReduction)) && questsDone > 30)
            {
                item = new Item();
                item.SetDefaults(Main.hardMode ? ModContent.ItemType<BleachedAnglingKit>() : ModContent.ItemType<SandyAnglingKit>());
                rewardItems.Add(item);
            }

            // Golden Fishing Rod
            if (Main.rand.NextBool((int)(500f * rarityReduction)) && questsDone > 30)
            {
                item = new Item();
                item.SetDefaults(ItemID.GoldenFishingRod);
                rewardItems.Add(item);
            }

            // Hotline Fishing Hook
            if (Main.rand.NextBool((int)(200f * rarityReduction)) && questsDone > 22)
            {
                item = new Item();
                item.SetDefaults(ItemID.HotlineFishingHook);
                rewardItems.Add(item);
            }

            // Angler Set
            if (Main.rand.NextBool((int)(150f * rarityReduction)) && questsDone > 10)
            {
                item = new Item();
                item.SetDefaults(ItemID.AnglerHat);
                rewardItems.Add(item);
                item = new Item();
                item.SetDefaults(ItemID.AnglerVest);
                rewardItems.Add(item);
                item = new Item();
                item.SetDefaults(ItemID.AnglerPants);
                rewardItems.Add(item);
            }

            // Mermaid Set
            if (Main.rand.NextBool((int)(150f * rarityReduction)) && questsDone > 13)
            {
                item = new Item();
                item.SetDefaults(ItemID.SeashellHairpin);
                rewardItems.Add(item);
                item = new Item();
                item.SetDefaults(ItemID.MermaidAdornment);
                rewardItems.Add(item);
                item = new Item();
                item.SetDefaults(ItemID.MermaidTail);
                rewardItems.Add(item);
            }

            // Fish Set
            if (Main.rand.NextBool((int)(150f * rarityReduction)) && questsDone > 7)
            {
                item = new Item();
                item.SetDefaults(ItemID.FishCostumeMask);
                rewardItems.Add(item);
                item = new Item();
                item.SetDefaults(ItemID.FishCostumeShirt);
                rewardItems.Add(item);
                item = new Item();
                item.SetDefaults(ItemID.FishCostumeFinskirt);
                rewardItems.Add(item);
            }

            // Fin Wings
            if (Main.rand.NextBool((int)(140f * rarityReduction)) && Main.hardMode && questsDone > 10)
            {
                item = new Item();
                item.SetDefaults(ItemID.FinWings);
                rewardItems.Add(item);
            }

            // Bottomless Water Bucket
            if (Main.rand.NextBool((int)(140f * rarityReduction)) && questsDone > 25)
            {
                item = new Item();
                item.SetDefaults(ItemID.BottomlessBucket);
                rewardItems.Add(item);
            }

            // Bottomless Honey Bucket
            if (Main.rand.NextBool((int)(140f * rarityReduction)) && questsDone > 27)
            {
                item = new Item();
                item.SetDefaults(ItemID.BottomlessHoneyBucket);
                rewardItems.Add(item);
            }

            // Bottomless Lava Bucket
            if (Main.rand.NextBool((int)(140f * rarityReduction)) && questsDone > 29)
            {
                item = new Item();
                item.SetDefaults(ItemID.BottomlessLavaBucket);
                rewardItems.Add(item);
            }

            // Magic Conch
            if (Main.rand.NextBool((int)(140f * rarityReduction)) && questsDone > 18)
            {
                item = new Item();
                item.SetDefaults(ItemID.MagicConch);
                rewardItems.Add(item);
            }

            // Demon Conch
            if (Main.rand.NextBool((int)(140f * rarityReduction)) && questsDone > 19)
            {
                item = new Item();
                item.SetDefaults(ItemID.DemonConch);
                rewardItems.Add(item);
            }

            // Super Absorbant Sponge
            if (Main.rand.NextBool((int)(140f * rarityReduction)) && questsDone > 16)
            {
                item = new Item();
                item.SetDefaults(ItemID.SuperAbsorbantSponge);
                rewardItems.Add(item);
            }

            // Honey Absorbant Sponge
            if (Main.rand.NextBool((int)(140f * rarityReduction)) && questsDone > 17)
            {
                item = new Item();
                item.SetDefaults(ItemID.SuperAbsorbantSponge);
                rewardItems.Add(item);
            }

            // Golden Bug Net
            if (Main.rand.NextBool((int)(140f * rarityReduction)) && questsDone > 28)
            {
                item = new Item();
                item.SetDefaults(ItemID.GoldenBugNet);
                rewardItems.Add(item);
            }

            // Fish Hook
            if (Main.rand.NextBool((int)(120f * rarityReduction)) && questsDone > 4)
            {
                item = new Item();
                item.SetDefaults(ItemID.FishHook);
                rewardItems.Add(item);
            }

            // Minecarp
            if (Main.rand.NextBool((int)(120f * rarityReduction)) && questsDone > 8)
            {
                item = new Item();
                item.SetDefaults(ItemID.FishMinecart);
                rewardItems.Add(item);
            }

            // Lava Shark
            if (Main.rand.NextBool((int)(120f * rarityReduction)) && questsDone > 24)
            {
                item = new Item();
                item.SetDefaults(ItemID.SuperheatedBlood);
                rewardItems.Add(item);
            }

            // High Test Fishing Line
            if (Main.rand.NextBool((int)(80f * rarityReduction)) && questsDone > 3)
            {
                item = new Item();
                item.SetDefaults(ItemID.HighTestFishingLine);
                rewardItems.Add(item);
            }

            // Angler Earring
            if (Main.rand.NextBool((int)(80f * rarityReduction)) && questsDone > 20)
            {
                item = new Item();
                item.SetDefaults(ItemID.AnglerEarring);
                rewardItems.Add(item);
            }

            // Lavaproof Fishing Hook
            if (Main.rand.NextBool((int)(80f * rarityReduction)) && questsDone > 21)
            {
                item = new Item();
                item.SetDefaults(ItemID.LavaFishingHook);
                rewardItems.Add(item);
            }

            // Tackle Box
            if (Main.rand.NextBool((int)(80f * rarityReduction)) && questsDone > 15)
            {
                item = new Item();
                item.SetDefaults(ItemID.TackleBox);
                rewardItems.Add(item);
            }

            // Fisherman's Pocket Guide
            if (Main.rand.NextBool((int)(60f * rarityReduction)) && questsDone > 6)
            {
                item = new Item();
                item.SetDefaults(ItemID.FishermansGuide);
                rewardItems.Add(item);
            }

            // Weather Radio
            if (Main.rand.NextBool((int)(60f * rarityReduction)) && questsDone > 11)
            {
                item = new Item();
                item.SetDefaults(ItemID.WeatherRadio);
                rewardItems.Add(item);
            }

            // Sextant
            if (Main.rand.NextBool((int)(60f * rarityReduction)) && questsDone > 14)
            {
                item = new Item();
                item.SetDefaults(ItemID.Sextant);
                rewardItems.Add(item);
            }

            // Fishing Bobber
            if (Main.rand.NextBool((int)(50f * rarityReduction)) && questsDone > 12)
            {
                item = new Item();
                item.SetDefaults(ItemID.FishingBobber);
                rewardItems.Add(item);
            }

            PlayerLoader.AnglerQuestReward(self, rarityReduction, rewardItems);

            foreach (Item rewardItem in rewardItems)
            {
                rewardItem.position = self.Center;

                Item getItem = self.GetItem(self.whoAmI, rewardItem, GetItemSettings.NPCEntityToPlayerInventorySettings);

                if (getItem.stack > 0)
                {
                    int number = Item.NewItem(source, (int)self.position.X, (int)self.position.Y, self.width, self.height, getItem.type, getItem.stack, noBroadcast: false, 0, noGrabDelay: true);

                    if (Main.netMode == NetmodeID.MultiplayerClient)
                        NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number, 1f);
                }
            }
        }
        #endregion
    }
}
