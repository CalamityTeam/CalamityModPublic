using System;
using System.Collections.Generic;
using System.Reflection;
using CalamityMod.Events;
using CalamityMod.Items.Fishing;
using CalamityMod.Items.Materials;
using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Walls;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Drawing;
using Terraria.GameContent.ItemDropRules;
using Terraria.GameInput;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.ILEditing
{
    public partial class ILChanges
    {
        [Obsolete("Use 'Main.instance.TilesRenderer.Wind' Instead. This property is included in the Calamity source code only for historic value.", error: true)]
        public static WindGrid Windgrid
        {
            get;
            internal set;
        }

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
        private static void PreventBossSlimeRainSpawns(On_NPC.orig_SlimeRainSpawns orig, int plr)
        {
            if (CalamityServerConfig.Instance.BossZen && Main.player[plr].Calamity().isNearbyBoss)
                return;

            orig(plr);
        }
        #endregion Prevention of Slime Rain Spawns When Near Bosses

        #region Remove Expert Brain of Cthulhu Random Debuffs
        private static void RemoveExpertBrainRandomDebuffs(ILContext il)
        {
            // Remove Expert+ Brain of Cthulhu and Creeper random debuffs on hit.
            var cursor = new ILCursor(il);

            // Go to the check for Expert Mode.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchCall<Main>("get_expertMode")))
            {
                LogFailure("Remove Expert Brain Random Debuffs", "Could not locate the Expert Mode check.");
                return;
            }

            // AND with 0, so that Expert Mode is never considered active.
            // Calamity implements something more sinister in GFB :)
            cursor.Emit(OpCodes.Ldc_I4_0);
            cursor.Emit(OpCodes.And);
        }
        #endregion

        #region Prevent Lava Slime Dropping Lava
        private static void PreventLavaSlimeLavaDrop(On_NPC.orig_HitEffect_HitInfo orig, NPC self, NPC.HitInfo hit)
        {
            if (self.type != NPCID.LavaSlime || !CalamityServerConfig.Instance.RemoveLavaDropsFromLavaSlimes)
            {
                orig(self, hit);
                return;
            }

            int tileX = (int)(self.Center.X / 16);
            int tileY = (int)(self.Center.Y / 16);
            Tile tile = Framing.GetTileSafely(tileX, tileY);
            if (tile.LiquidAmount != 0)
            {
                orig(self, hit);
                return;
            }

            byte originalAmount = tile.LiquidAmount;
            tile.LiquidAmount = 255;
            orig(self, hit);
            tile.LiquidAmount = originalAmount;
        }
        #endregion

        #region Disable Detonating Bubble StrikeNPC Hardcoded Override
        private static void LetDetonatingBubblesTakeDamage(ILContext il)
        {
            // In vanilla's StrikeNPC function, Detonating Bubbles have a hardcoded type check which sets the damage of the strike to 0.
            // This IL edit disables that type check in Death Mode.
            var cursor = new ILCursor(il);

            // Go to the point after the check for the Detonating Bubble NPC ID.
            if (!cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdcR8(0.0)))
            {
                LogFailure("Let Detonating Bubbles Take Damage in Death", "Could not move after the NPC type check.");
                return;
            }

            // Define the label.
            var label = il.DefineLabel();

            // Add a branch if it is Death Mode.
            cursor.Emit(OpCodes.Ldsfld, typeof(CalamityWorld).GetField("death"));
            cursor.Emit(OpCodes.Brtrue, label);

            // Move to the point after Detonating Bubble changes are implemented to place the branch label.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStfld<NPC>("dontTakeDamage")))
            {
                LogFailure("Let Detonating Bubbles Take Damage in Death", "Could not move to after the Detonating Bubble logic.");
                return;
            }
            if (!cursor.TryGotoNext(MoveType.AfterLabel, i => i.MatchLdarg0()))
            {
                LogFailure("Let Detonating Bubbles Take Damage in Death", "Could not move to after the Detonating Bubble logic.");
                return;
            }
            cursor.MarkLabel(label);
        }
        #endregion

        #region Make PunchCameraModifier Affected by Screenshake Config
        private static void PunchCameraUsesScreenshakeConfig(ILContext il)
        {
            // Allow the screenshake from PunchCameraModifier to scale based on our screenshake power config.
            var cursor = new ILCursor(il);

            // There are 3 local variables that control the strength of the screenshake in separate ways, but they all get multiplied together at the end.
            // Thus it doesn't matter at all which one is multiplied to. Here I chose num2.
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStloc2()))
            {
                LogFailure("Make PunchCameraModifier Affected by Screenshake Config", "Could not move to the location to inject code.");
                return;
            }

            // Emit a delegate which grabs the value of the screenshake config. Then multiply the local variable by it.
            cursor.Emit(OpCodes.Ldloc_1);
            cursor.EmitDelegate<Func<float>>(() => CalamityClientConfig.Instance.ScreenshakePower);
            cursor.Emit(OpCodes.Mul);
            cursor.Emit(OpCodes.Stloc_1);
        }
        #endregion

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

            // Remove this value and replace it with a large number that will never be a valid tile ID.
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, 40000);
        }
        #endregion

        #region Remove Hellforge Pickaxe Requirement
        private static int RemoveHellforgePickaxeRequirement(On_Player.orig_GetPickaxeDamage orig, Player self, int x, int y, int pickPower, int hitBufferIndex, Tile tileTarget)
        {
            if (tileTarget.TileType == TileID.Hellforge)
                pickPower = 65;

            return orig(self, x, y, pickPower, hitBufferIndex, tileTarget);
        }
        #endregion

        #region Prevent UFO Mount from Dismounting in Water
        private static void PreventUFODismountInWater(ILContext il)
        {
            // Prevent the Cosmic Car Key's UFO mount from dismounting when the player is in water.
            var cursor = new ILCursor(il);

            // Unfortunately, the code responsible for this is 4000 lines into Player.Update, meaning that reaching it is far from simple.
            // The following method was the easiest way I could find to reach it:
            // Move to the third call of Mount.Dismount.
            for (int i = 0; i < 3; i++)
            {
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchCallvirt<Mount>("Dismount")))
                {
                    LogFailure("Prevent UFO Dismounting in Water", "Could not reach the Dismount instruction.");
                    return;
                }
            }
            // Move the cursor backwards to place it right after the instruction which loads Main.myPlayer onto the stack.
            if (!cursor.TryGotoPrev(MoveType.After, i => i.MatchLdsfld<Main>("myPlayer")))
            {
                LogFailure("Prevent UFO Dismounting in Water", "Could not locate the myPlayer check.");
                return;
            }

            // Remove the instruction and replace it with the integer limit. The next instruction checks if this value is equal to Player.whoAmI.
            // Player.whoAmI will never be the integer limit, so the check will always fail and the UFO will not dismount.
            cursor.EmitPop();
            cursor.Emit(OpCodes.Ldc_I4, int.MaxValue);
        }
        #endregion Prevent UFO Mount from Dismounting in Water

        #region Color Blighted Gel
        private static void ColorBlightedGel(On_CommonCode.orig_ModifyItemDropFromNPC orig, NPC npc, int itemIndex)
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
                NetMessage.SendData(MessageID.ItemTweaker, -1, -1, null, itemIndex, 1f);
        }
        #endregion Color Blighted Gel

        #region Improve Angler Quest Rewards
        private static void AddMoreGuaranteedAnglerRewards(On_Player.orig_GetAnglerReward_MainReward orig, Player self, List<Item> rewardItems, IEntitySource source, int questsDone, float rarityReduction, int questItemType, ref GetItemSettings anglerRewardSettings)
        {
            // Adds several new guaranteed rewards for specific quests.
            // These will replace the item that would have dropped via vanilla logic.
            Item item = new Item();
            item.type = ItemID.None;
            List<int> checkingList = [];

            switch (questsDone)
            {
                case 3: // High Test Fishing Line
                    checkingList.Add(ItemID.HighTestFishingLine);
                    if (self.DropAnglerAccByMissing(checkingList, 1f, out _, out int highTest))
                        item.SetDefaults(highTest);
                    break;
                case 6: // Fisherman's Pocket Guide
                    checkingList.Add(ItemID.FishermansGuide);
                    if (self.DropAnglerAccByMissing(checkingList, 1f, out _, out int fishGuide))
                        item.SetDefaults(fishGuide);
                    break;
                case 10: // Angler armor
                    Item vest = new Item(ItemID.AnglerVest); // Vanilla has a guaranteed reward for Angler Hat here. Calamity makes the other two pieces drop as well.
                    rewardItems.Add(vest);
                    Item pants = new Item(ItemID.AnglerPants); // This intentionally does not set the item variable, to allow vanilla logic to drop the Angler Hat.
                    rewardItems.Add(pants);
                    break;
                case 11: // Weather Radio
                    checkingList.Add(ItemID.WeatherRadio);
                    if (self.DropAnglerAccByMissing(checkingList, 1f, out _, out int radio))
                        item.SetDefaults(radio);
                    break;
                case 14: // Sextant
                    checkingList.Add(ItemID.Sextant);
                    if (self.DropAnglerAccByMissing(checkingList, 1f, out _, out int sextant))
                        item.SetDefaults(sextant);
                    break;
                case 15: // Tackle Box
                    checkingList.Add(ItemID.TackleBox);
                    if (self.DropAnglerAccByMissing(checkingList, 1f, out _, out int tackle))
                        item.SetDefaults(tackle);
                    break;
                case 20: // Angler Earring
                    checkingList.Add(ItemID.AnglerEarring);
                    if (self.DropAnglerAccByMissing(checkingList, 1f, out _, out int earring))
                        item.SetDefaults(earring);
                    break;
                case 26: // Enchanted Sundial
                    item.SetDefaults(ItemID.Sundial);
                    break;
                case 28: // Golden Bug Net
                    item.SetDefaults(ItemID.GoldenBugNet);
                    break;
                default:
                    break;
            }

            // If the quest is of a number that gives a guaranteed reward from Calamity, add it to the reward pool and skip vanilla logic.
            if (item.type > ItemID.None)
                rewardItems.Add(item);
            else
                orig(self, rewardItems, source, questsDone, rarityReduction, questItemType, ref anglerRewardSettings);
        }

        private static void ImproveAnglerBaitReward(On_Player.orig_GetAnglerReward_Bait orig, Player self, List<Item> rewardItems, IEntitySource source, int questsDone, float rarityReduction, ref GetItemSettings anglerRewardSettings)
        {
            // Improves the bait reward given for Angler quests in three ways:
            // 1. Makes bait reward be guaranteed.
            // 2. Adds the ability for Grand Marquis Bait to be dropped.
            // 3. Increases the amount of bait dropped.
            // This entirely replaces the vanilla logic.

            Item bait = new Item();
            if (Main.rand.NextBool((int)(15f * rarityReduction)))
                bait.SetDefaults(ModContent.ItemType<GrandMarquisBait>());
            else if (Main.rand.NextBool((int)(10f * rarityReduction)))
                bait.SetDefaults(ItemID.MasterBait);
            else if (Main.rand.NextBool((int)(5f * rarityReduction)))
                bait.SetDefaults(ItemID.JourneymanBait);
            else
                bait.SetDefaults(ItemID.ApprenticeBait);

            bait.stack = 2;
            if (Main.rand.Next(10) <= questsDone)
                bait.stack++;
            if (Main.rand.Next(20) <= questsDone)
                bait.stack++;
            if (Main.rand.Next(30) <= questsDone)
                bait.stack++;
            if (Main.rand.Next(40) <= questsDone)
                bait.stack++;
            if (Main.rand.Next(50) <= questsDone)
                bait.stack++;

            rewardItems.Add(bait);
        }

        private static void ImproveAnglerMoneyReward(On_Player.orig_GetAnglerReward_Money orig, Player self, List<Item> rewardItems, IEntitySource source, int questsDone, float rarityReduction, ref GetItemSettings anglerRewardSettings)
        {
            // Improves the logic for giving money to the player from Angler quests.
            // This is accomplished via a higher starting amount, reduced variance, and not truncating off Silver Coins if giving Gold Coins.
            // This entirely replaces the vanilla logic.

            int moneyDrop = (questsDone + 70) / 2; // Vanilla uses 50
            moneyDrop = (int)(moneyDrop * Main.rand.NextFloat(1f, 2f)); // Vanilla is 0.75x-3x
            moneyDrop = (int)(moneyDrop * 1.5f); // Vanilla then arbitrarily multiplies the value by 1.5x
            if (Main.hardMode)
                moneyDrop *= 2;
            if (Main.expertMode)
                moneyDrop *= 2;
            if (moneyDrop > 1000)
                moneyDrop = 1000; // Vanilla has a cap of 10 Gold Coins, or 1000 Silver Coins.

            // moneyDrop now contains the number of SILVER Coins to drop.
            // Determine the number of Gold Coins to drop, if any.
            if (moneyDrop >= 100)
            {
                int goldDrop = moneyDrop / 100;
                Item gold = new Item();
                gold.SetDefaults(ItemID.GoldCoin);
                gold.stack = goldDrop;
                rewardItems.Add(gold);
            }
            // Now the Silver Coins to drop.
            int silverDrop = moneyDrop % 100;
            Item silver = new Item();
            silver.SetDefaults(ItemID.SilverCoin);
            silver.stack = silverDrop;
            rewardItems.Add(silver);
        }
        #endregion

        #region Render Special Map Colors
        private static void UseVisibleThroughWaterMapTile(ILContext il)
        {
            var c = new ILCursor(il);

            if (!c.TryGotoNext(x => x.MatchCall<Tilemap>("get_Item")))
            {
                LogFailure("Use VisibleThroughWater Map Tile", "Could not locate call to Terraria.Map.TileMap::get_Item.");
                return;
            }

            int tileIndex = -1;
            if (!c.TryGotoNext(x => x.MatchStloc(out tileIndex)) || tileIndex == -1)
            {
                LogFailure("Use VisibleThroughWater Map Tile", "Could not determine the local variable index tile is pushed to.");
                return;
            }

            if (!c.TryGotoNext(x => x.MatchCall<Tile>("liquidType")))
            {
                LogFailure("Use VisibleThroughWater Map Tile", "Could not locate call to Terraria.Tile::liquidType.");
                return;
            }

            int liquidTypeIndex = -1;
            if (!c.TryGotoNext(x => x.MatchStloc(out liquidTypeIndex)) || liquidTypeIndex == -1)
            {
                LogFailure("Use VisibleThroughWater Map Tile", "Could not determine the local variable index liquidType is pushed to.");
                return;
            }

            int relativeMapTypeIndex = -1;
            if (!c.TryGotoNext(MoveType.After, x => x.MatchStloc(out relativeMapTypeIndex)) || relativeMapTypeIndex == -1)
            {
                LogFailure("Use VisibleThroughWater Map Tile", "Could not determine the local variable index of the relative map type.");
                return;
            }

            c.Emit(OpCodes.Ldloc_0);
            c.Emit(OpCodes.Ldloc, relativeMapTypeIndex);
            c.Emit(OpCodes.Ldloc, liquidTypeIndex);
            c.EmitDelegate(
                (Tile tile, int relativeMapType, int liquidType) =>
                {
                    if (liquidType != LiquidID.Water)
                        return relativeMapType;

                    if (WallLoader.GetWall(tile.WallType) is IVisibleThroughWater visibleThroughWater)
                        return visibleThroughWater.WaterMapEntry;

                    return relativeMapType;
                }
            );
            c.Emit(OpCodes.Stloc, relativeMapTypeIndex);
        }
        #endregion

        #region Make Magma Stone & Fire Gauntlet Dust Toggleable
        private static void MakeMagmaStoneFireGauntletDustToggleable(ILContext il)
        {
            // Allows Magma Stone and Fire Gauntlet's obnoxious dust on melee swings to be toggled off with visbility
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("magmaStone"))) // Flag for if Magma Stone is equipped. Fire Gauntlet also uses this.
            {
                LogFailure("Make Magma Stone & Fire Gauntlet Dust Toggleable", "Could not locate the Magma Stone variable.");
                return;
            }
            // Load the player itself onto the stack so that it becomes an argument for the following delegate.
            cursor.Emit(OpCodes.Ldarg_0);

            // Emit a delegate which places whether the player has their Magma Stone visuals enabled onto the stack.
            cursor.EmitDelegate<Func<Player, bool>>(MagmaStoneVisualsEnabled);
            cursor.Emit(OpCodes.And);
        }

        private static readonly Func<Player, bool> MagmaStoneVisualsEnabled = (Player p) => p.Calamity().magmaStoneVisuals;

        private static void MakeMagmaStoneFireGauntletProjectileDustToggleable(ILContext il)
        {
            // Allows Magma Stone and Fire Gauntlet's obnoxious dust on projectiles to be toggled off with visbility
            var cursor = new ILCursor(il);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdfld<Player>("magmaStone"))) // Flag for if Magma Stone is equipped. Fire Gauntlet also uses this.
            {
                LogFailure("Make Magma Stone & Fire Gauntlet Projectile Dust Toggleable", "Could not locate the magma stone variable.");
                return;
            }
            // Load the player itself onto the stack so that it becomes an argument for the following delegate.
            cursor.Emit(OpCodes.Ldloc_0);

            // Emit a delegate which places whether the player has their Magma Stone visuals enabled onto the stack.
            cursor.EmitDelegate<Func<Player, bool>>(MagmaStoneVisualsEnabled);
            cursor.Emit(OpCodes.And);
        }

        #endregion Make Magma Stone & Fire Gauntlet Dust Toggleable

        #region Vanilla Non-Linearity Fixes
        private static void RemovePowerCellPlanteraLock(ILContext il)
        {
            // Remove the check requiring Plantera to be defeated to use Lihzahrd Power Cells at the Altar.
            var cursor = new ILCursor(il);

            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdsfld<NPC>("downedPlantBoss")))
            {
                LogFailure("Remove Power Cell Plantera Lock", "Could not locate the downed Plantera bool.");
                return;
            }

            // Remove the instruction and replace with 1 (true). This effectively removes the requirement for defeating Plantera.
            // The only requirements for summoning Golem with Power Cells are now: 1) Golem is not alive, and 2) The world is in Hardmode.
            cursor.EmitPop();
            cursor.Emit(OpCodes.Ldc_I4_1);
        }

        private static bool RemoveUseLocks(On_Player.orig_ItemCheck_CheckCanUse orig, Player self, Item sItem)
        {
            if (sItem.type == ItemID.CelestialSigil)
                return !NPC.AnyNPCs(NPCID.MoonLordCore) && !BossRushEvent.BossRushActive;
            if (sItem.type == ItemID.SolarTablet)
                return Main.dayTime && !Main.eclipse && (Main.hardMode || NPC.downedMechBossAny || NPC.downedPlantBoss);

            return orig(self, sItem);
        }

        private static void ApplyCelestialSigilChanges(On_Player.orig_ItemCheck_UseEventItems orig, Player self, Item sItem)
        {
            if (self.ItemTimeIsZero && self.itemAnimation > 0 && sItem.type == ItemID.CelestialSigil)
            {
                if (NPC.AnyNPCs(NPCID.MoonLordCore) || BossRushEvent.BossRushActive)
                    return;

                SoundEngine.PlaySound(SoundID.Roar, self.Center);
                self.ApplyItemTime(sItem);

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(self.whoAmI, NPCID.MoonLordCore);
                else
                    NetMessage.SendData(MessageID.SpawnBossUseLicenseStartEvent, -1, -1, null, self.whoAmI, NPCID.MoonLordCore);
            }
            else
                orig(self, sItem);
        }
        #endregion

        #region Remove NPC.damage Condition from Radar
        private static void RemoveDamageConditionFromRadar(ILContext il)
        {
            var cursor = new ILCursor(il);

            Func<Instruction, bool>[] searchFor =
            [
                (x => x.MatchLdfld<NPC>(nameof(NPC.damage))),
                (x => x.MatchLdcI4(out var comp) && comp == 0),
                (x => x.MatchBle(out _)) //ble.s
            ];

            if (!cursor.TryGotoNext(MoveType.After, searchFor))
            {
                LogFailure("Radar Condition", "Unable to locate condition for NPC.damage > 0");
                return;
            }

            // Branch is used for exit condition. So setting ble.s opcode to nop will remove the condition
            cursor.Prev.OpCode = OpCodes.Nop;

            // After that we pop NPC.damage and 0 from stack
            cursor.EmitPop();
            cursor.EmitPop();
        }
        #endregion

        #region Multiple NPC Happiness support 
        // Currently unused as the one NPC who used it was removed. However it is very likely it'll be used again in the future, so this code is being kept.
        /*private static void AllowMultipleLikedNPCs(On_ShopHelper.orig_ApplyNpcRelationshipEffect orig, ShopHelper self, int npcType, AffectionLevel affectionLevel)
        {
            FieldInfo npcTalkField = typeof(ShopHelper).GetField("_currentNPCBeingTalkedTo", BindingFlags.Instance | BindingFlags.NonPublic);
            NPC talkedNPC = (NPC)npcTalkField.GetValue(self);

            int npcTypee = 0;

            // Allow the given NPC to have things to say about multiple NPCs with the same happiness level
            if (talkedNPC.type == npcTypee)
            {
                MethodInfo addReportField = typeof(ShopHelper).GetMethod("AddHappinessReportText", BindingFlags.Instance | BindingFlags.NonPublic);

                FieldInfo happinessField = typeof(ShopHelper).GetField("_currentPriceAdjustment", BindingFlags.Instance | BindingFlags.NonPublic);
                float currentPriceAdjustment = (float)happinessField.GetValue(self);

                if (affectionLevel != 0 && Enum.IsDefined(affectionLevel))
                {
                    // Add a suffix to the localization key which specifies the NPC's name
                    addReportField.Invoke(self, [ $"{affectionLevel}NPC_" + NPCID.Search.GetName(npcType),  new
                    {
                        NPCName = NPC.GetFullnameByID(npcType)
                    }, 0]);
                    currentPriceAdjustment *= NPCHappiness.AffectionLevelToPriceMultiplier[affectionLevel];
                    happinessField.SetValue(self, currentPriceAdjustment);
                }
            }
            else
            {
                orig(self, npcType, affectionLevel);
            }
        }*/
        #endregion

        #region Allow Disabling Gravity Swap Visual and Allow Gravity Keybind
        private static void DelayGravity(On_Player.orig_UpdateControlHolds orig, Player Player)
        {
            var cplay = Player.Calamity();
            if (CalamityKeybinds.SwitchGravityHotkey.GetAssignedKeysOrEmpty().Count != 0 && (Player.gravControl || Player.gravControl2) && !Player.mount.Active)
            {
                if (Player.controlUp && Player.releaseUp)
                {
                    Player.gravDir *= -1;
                }
                if (CalamityKeybinds.SwitchGravityHotkey.JustPressed)
                {
                    Player.gravDir *= -1;
                    Player.fallStart = (int)(Player.position.Y / 16f);
                    Player.jump = 0;
                    SoundEngine.PlaySound(SoundID.Item8, Player.position);
                }

                if (Player.forcedGravity > 0)
                {
                    Player.gravDir = -1f;
                }
            }

            if (cplay.justChangedGravity)
            {
                Player.gravDir = cplay.oldGravDir;
            }
            cplay.justChangedGravity = cplay.oldGravDir != Player.gravDir;

            cplay.oldGravDir = Player.gravDir;
            if (Main.netMode != NetmodeID.Server && !Main.gameMenu && CalamityClientConfig.Instance.DisableGravityScreenSwap)
            {
                if (Player.gravDir == -1)
                {
                    if (!Filters.Scene["CalamityMod:FlipScreen"].IsActive())
                    {
                        Filters.Scene.Activate("CalamityMod:FlipScreen");
                        Filters.Scene["CalamityMod:FlipScreen"].Opacity = 1f;

                    }
                }
                else
                {
                    if (Filters.Scene["CalamityMod:FlipScreen"].IsActive())
                    {
                        Filters.Scene["CalamityMod:FlipScreen"].Opacity = 0f;
                        Filters.Scene.Deactivate("CalamityMod:FlipScreen");

                    }
                }
            }
            if (cplay.justChangedGravity)
            {
                Player.gravDir *= -1;
            }
            orig(Player);
        }

        private static void GravityMouse(On_PlayerInput.orig_SetZoom_MouseInWorld orig)
        {
            orig();
            if (!Main.gameMenu && Filters.Scene["CalamityMod:FlipScreen"].IsActive())//((Main.LocalPlayer.gravDir == -1 && !Main.LocalPlayer.Calamity().justChangedGravity) || (Main.LocalPlayer.Calamity().oldGravDir == -1 && Main.LocalPlayer.Calamity().justChangedGravity))
            {
                var center = Main.screenHeight / 2;
                Main.mouseY = center - (Main.mouseY - center);
            }
            ;
        }
        private static void UI_Unflip_Start(On_Main.orig_DrawPlayerChatBubbles orig, Main self)
        {
            if (!Main.gameMenu && (Filters.Scene["CalamityMod:FlipScreen"].IsActive() || Main.LocalPlayer.Calamity().justChangedGravity))
            {
                Main.LocalPlayer.Calamity().tempGravDir = Main.LocalPlayer.gravDir;
                Main.LocalPlayer.gravDir = 1;
            }
            orig(self);
        }

        private static void UI_Unflip_End(On_Main.orig_DrawInterface orig, Main self, GameTime gameTime)
        {
            orig(self, gameTime);
            if (!Main.gameMenu && Filters.Scene["CalamityMod:FlipScreen"].IsActive())
            {
                Main.LocalPlayer.gravDir = Main.LocalPlayer.Calamity().tempGravDir;
            }
        }
        #endregion

        // 02JUN2024: Ozzatron: The below code is being kept in its initial state for historic value.
        #region Store The Stupid Fucking Private Wind Map In Public Property
        [/*TotallyNot*/Obsolete("This function serves no purpose and is included in the Calamity source code for historic value.", error: true)]
        private static void StoreWindGrid(On_TileDrawing.orig_Update orig, TileDrawing self)
        {
            orig(self);

            // FUCK YOU FUCK YOU FUCK YOU FUCK YOU FUCK YOU FUCK YOU FUCK YOU FUCK YOU FUCK YOU FUCK
            if (Windgrid is null)
                Windgrid = typeof(TileDrawing).GetField("_windGrid", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(self) as WindGrid;
        }
        #endregion Store The Stupid Fucking Private Wind Map In Public Property
    }
}
