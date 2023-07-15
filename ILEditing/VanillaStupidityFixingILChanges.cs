using CalamityMod.Balancing;
using CalamityMod.Items.Materials;
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
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdcI4(TileID.Meteorite))) // The Meteorite Tile ID check.
            {
                LogFailure("Make Meteorite Explodable", "Could not locate the Meteorite Tile ID variable.");
                return;
            }
            cursor.Remove();
            cursor.Emit(OpCodes.Ldc_I4, TileID.HellstoneBrick); // Change to Hellstone Brick. They're made of Hellstone, so it makes sense they can't be exploded until Hardmode starts :^)
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

        #region Make Tag Damage Multiplicative
        private static void MakeTagDamageMultiplicative(ILContext il)
        {
            var cursor = new ILCursor(il);
            int damageLocalIndex = 37;

            bool replaceWithMultipler(int flagLocalIndex, float damageFactor, bool usesExtraVariableToStoreDamage = false)
            {
                // Move after the bool load and branch-if-false instruction.
                cursor.Goto(0);
                if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchLdloc(flagLocalIndex)))
                {
                    LogFailure("Making Tag Damage Multiplicative", $"Could not locate the flag local index of '{flagLocalIndex}'.");
                    return false;
                }

                // Move to the point at which a local is loaded after the boolean.
                // Ideally this would be two instructions afterwards (load bool, branch), but we cannot guarantee that this will be the case.
                // As such, a match is done instead.
                if (!cursor.TryGotoNext(MoveType.After, i => i.MatchLdloc(out _)))
                {
                    LogFailure("Making Tag Damage Multiplicative", $"Could not locate the succeeding local after the flag local index of '{flagLocalIndex}'.");
                    return false;
                }

                // OPTIONAL case for if an extra variable to store damage is used:
                // Load damage to add.
                // Store damage to add as a variable.

                // Load damage ->
                // Load damage addition ->
                // Add the two ->
                // Store damage.

                // This logic for adding damage is disabled by popped at the point at which the addition happens and replacing it with zero, resulting in x += 0.
                if (!cursor.TryGotoNext(MoveType.Before, c => c.MatchAdd()))
                {
                    LogFailure("Making Tag Damage Multiplicative", $"Could not locate the damage addition at the flag local index of '{flagLocalIndex}'.");
                    return false;
                }
                cursor.Emit(OpCodes.Pop);
                cursor.Emit(OpCodes.Ldc_I4_0);

                // After this, the following operations are done as a replacement to achieve multiplicative damage:

                // Load damage ->
                // Cast damage to float ->
                // Load the damage factor ->
                // Multiply the two ->
                // Cast the result to int, removing the fractional part ->
                // Store damage.
                cursor.Emit(OpCodes.Ldloc, damageLocalIndex);
                cursor.Emit(OpCodes.Conv_R4);
                cursor.Emit(OpCodes.Ldc_R4, damageFactor);
                cursor.Emit(OpCodes.Mul);
                cursor.Emit(OpCodes.Conv_I4);
                cursor.Emit(OpCodes.Stloc, damageLocalIndex);
                return true;
            }

            // Leather whip.
            replaceWithMultipler(50, BalancingConstants.LeatherWhipTagDamageMultiplier);

            // Durendal.
            replaceWithMultipler(51, BalancingConstants.DurendalTagDamageMultiplier);

            // Snapthorn.
            replaceWithMultipler(54, BalancingConstants.SnapthornTagDamageMultiplier);

            // Spinal Tap.
            replaceWithMultipler(55, BalancingConstants.SpinalTapTagDamageMultiplier);

            // Morning Star.
            replaceWithMultipler(56, BalancingConstants.MorningStarTagDamageMultiplier);

            // Kaleidoscope.
            replaceWithMultipler(57, BalancingConstants.KaleidoscopeTagDamageMultiplier, true);

            // SPECIAL CASE: Firecracker's damage is fucking absurd and everything needs to go.
            cursor.Goto(0);
            if (!cursor.TryGotoNext(MoveType.After, i => i.MatchStloc(64)))
            {
                LogFailure("Making Tag Damage Multiplicative", $"Could not locate the flag local index of 52.");
                return;
            }

            // Change the damage of the explosions.
            cursor.Emit(OpCodes.Ldarg_0);
            cursor.EmitDelegate<Func<Projectile, int>>(projectile =>
            {
                int damage = (int)(Main.player[projectile.owner].ActiveItem().damage * BalancingConstants.FirecrackerExplosionDamageMultiplier);
                damage = (int)Main.player[projectile.owner].GetTotalDamage<SummonDamageClass>().ApplyTo(damage);
                return damage;
            });
            cursor.Emit(OpCodes.Stloc, 64);

            // Change the x in damage += x; to zero.
            if (!cursor.TryGotoNext(MoveType.Before, i => i.MatchAdd()))
            {
                LogFailure("Making Tag Damage Multiplicative", $"Could not locate the damage additive value.");
                return;
            }
            cursor.Emit(OpCodes.Pop);
            cursor.Emit(OpCodes.Ldc_I4_0);
        }
        #endregion Make Tag Damage Multiplicative

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
            int questsDone = self.anglerQuestsFinished + Main.rand.Next(101);
            float rarityReduction = 1f;
            rarityReduction = (questsDone <= 50) ? (rarityReduction - questsDone * 0.01f) : ((questsDone <= 100) ? (0.5f - (questsDone - 50) * 0.005f) : ((questsDone > 150) ? 0.15f : (0.25f - (questsDone - 100) * 0.002f)));
            rarityReduction *= 0.9f;
            rarityReduction *= (float)(self.currentShoppingSettings.PriceAdjustment + 1.0) / 2f;

            List<Item> rewardItems = new List<Item>();

            GetItemSettings anglerRewardSettings = GetItemSettings.NPCEntityToPlayerInventorySettings;

            Item item = new Item();
            item.SetDefaults(ItemID.MasterBait);
            item.stack += 5;
            if (item.stack > 0)
                rewardItems.Add(item);

            Item item2 = self.GetItem(self.whoAmI, item, GetItemSettings.NPCEntityToPlayerInventorySettings);
            if (item2.stack > 0)
                rewardItems.Add(item2);

            item = new Item();
            item.SetDefaults(ItemID.GoldCoin);
            item.stack = 10;

            item.position = self.Center;
            item2 = self.GetItem(self.whoAmI, item, anglerRewardSettings);
            if (item2.stack > 0)
                rewardItems.Add(item2);
            
            // Golden Fishing Rod
            if (Main.rand.NextBool((int)(500f * rarityReduction)))
            {
                item = new Item();
                item.SetDefaults(ItemID.GoldenFishingRod);
                rewardItems.Add(item);
            }

            // Hotline Fishing Hook
            if (Main.rand.NextBool((int)(200f * rarityReduction)) || self.anglerQuestsFinished == 22)
            {
                item = new Item();
                item.SetDefaults(ItemID.HotlineFishingHook);
                rewardItems.Add(item);
            }

            // Angler Set
            if (Main.rand.NextBool((int)(150f * rarityReduction)))
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
            if (Main.rand.NextBool((int)(150f * rarityReduction)) || self.anglerQuestsFinished == 11)
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
            if (Main.rand.NextBool((int)(150f * rarityReduction)) || self.anglerQuestsFinished == 8)
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
            if (Main.rand.NextBool((int)(140f * rarityReduction)) && Main.hardMode)
            {
                item = new Item();
                item.SetDefaults(ItemID.FinWings);
                rewardItems.Add(item);
            }

            // Bottomless Water Bucket
            if (Main.rand.NextBool((int)(140f * rarityReduction)))
            {
                item = new Item();
                item.SetDefaults(ItemID.BottomlessBucket);
                rewardItems.Add(item);
            }

            // Bottomless Honey Bucket
            /*if (Main.rand.NextBool((int)(140f * rarityReduction)) || self.anglerQuestsFinished == 29)
            {
                item = new Item();
                item.SetDefaults(ItemID.BottomlessHoneyBucket);
                rewardItems.Add(item);
            }*/

            // Super Absorbant Sponge
            if (Main.rand.NextBool((int)(140f * rarityReduction)) || self.anglerQuestsFinished == 17)
            {
                item = new Item();
                item.SetDefaults(ItemID.SuperAbsorbantSponge);
                rewardItems.Add(item);
            }

            // Honey Absorbant Sponge
            /*if (Main.rand.NextBool((int)(140f * rarityReduction)) || self.anglerQuestsFinished == 19)
            {
                item = new Item();
                item.SetDefaults(ItemID.SuperAbsorbantSponge);
                rewardItems.Add(item);
            }*/

            // Golden Bug Net
            if (Main.rand.NextBool((int)(140f * rarityReduction)) || self.anglerQuestsFinished == 27)
            {
                item = new Item();
                item.SetDefaults(ItemID.GoldenBugNet);
                rewardItems.Add(item);
            }

            // Fish Hook
            if (Main.rand.NextBool((int)(120f * rarityReduction)) || self.anglerQuestsFinished == 4)
            {
                item = new Item();
                item.SetDefaults(ItemID.FishHook);
                rewardItems.Add(item);
            }

            // Minecarp
            if (Main.rand.NextBool((int)(120f * rarityReduction)) || self.anglerQuestsFinished == 9)
            {
                item = new Item();
                item.SetDefaults(ItemID.FishMinecart);
                rewardItems.Add(item);
            }

            // High Test Fishing Line
            if (Main.rand.NextBool((int)(80f * rarityReduction)) || self.anglerQuestsFinished == 2)
            {
                item = new Item();
                item.SetDefaults(ItemID.HighTestFishingLine);
                rewardItems.Add(item);
            }

            // Angler Earring
            if (Main.rand.NextBool((int)(80f * rarityReduction)) || self.anglerQuestsFinished == 7)
            {
                item = new Item();
                item.SetDefaults(ItemID.AnglerEarring);
                rewardItems.Add(item);
            }

            // Tackle Box
            if (Main.rand.NextBool((int)(80f * rarityReduction)) || self.anglerQuestsFinished == 12)
            {
                item = new Item();
                item.SetDefaults(ItemID.TackleBox);
                rewardItems.Add(item);
            }

            // Fisherman's Pocket Guide
            if (Main.rand.NextBool((int)(60f * rarityReduction)) || self.anglerQuestsFinished == 1)
            {
                item = new Item();
                item.SetDefaults(ItemID.FishermansGuide);
                rewardItems.Add(item);
            }

            // Weather Radio
            if (Main.rand.NextBool((int)(60f * rarityReduction)) || self.anglerQuestsFinished == 3)
            {
                item = new Item();
                item.SetDefaults(ItemID.WeatherRadio);
                rewardItems.Add(item);
            }

            // Sextant
            if (Main.rand.NextBool((int)(60f * rarityReduction)) || self.anglerQuestsFinished == 6)
            {
                item = new Item();
                item.SetDefaults(ItemID.Sextant);
                rewardItems.Add(item);
            }

            // Fishing Bobber
            /*if (Main.rand.NextBool((int)(50f * rarityReduction)) || self.anglerQuestsFinished == 13)
            {
                item = new Item();
                item.SetDefaults(ItemID.FishingBobber);
                rewardItems.Add(item);
            }*/

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
