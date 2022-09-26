using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Rarities;
using CalamityMod.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.DraedonMisc
{
    public class EncryptedSchematicPlanetoid : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Encrypted Schematic (Planetoid)");
            Tooltip.SetDefault("Requires a Codebreaker with a basic decryption computer to decrypt");
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ModContent.RarityType<DarkOrange>();
            Item.maxStack = 1;
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override void UpdateInventory(Player player)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && !RecipeUnlockHandler.HasFoundPlanetoidSchematic)
            {
                RecipeUnlockHandler.HasFoundPlanetoidSchematic = true;
                CalamityNetcode.SyncWorld();
            }
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");
            if (RecipeUnlockHandler.HasUnlockedT2ArsenalRecipes)
            {
                if (line != null)
                    line.Text = "Has already been decrypted.\n" +
                        "Click to view its contents.\n" +
                        "Unlocked recipes:";
                int insertIndex = list.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
                if (insertIndex != -1)
                {
                    TooltipLine meleeDisplay = new TooltipLine(this.Mod, "CalamityMod:MeleeDisplay", $"[i:{ModContent.ItemType<HydraulicVoltCrasher>()}] Hydraulic Volt Crasher");
                    meleeDisplay.OverrideColor = new Color(31, 242, 245);
                    list.Insert(insertIndex + 1, meleeDisplay);

                    TooltipLine rangedDisplay = new TooltipLine(this.Mod, "CalamityMod:RangedDisplay", $"[i:{ModContent.ItemType<MatterModulator>()}] Matter Modulator");
                    rangedDisplay.OverrideColor = new Color(149, 243, 43);
                    list.Insert(insertIndex + 2, rangedDisplay);

                    TooltipLine mageDisplay = new TooltipLine(this.Mod, "CalamityMod:MageDisplay", $"[i:{ModContent.ItemType<GaussPistol>()}] Gauss Pistol");
                    mageDisplay.OverrideColor = new Color(236, 255, 31);
                    list.Insert(insertIndex + 3, mageDisplay);

                    TooltipLine summonDisplay = new TooltipLine(this.Mod, "CalamityMod:SummonDisplay", $"[i:{ModContent.ItemType<MountedScanner>()}] Mounted Scanner");
                    summonDisplay.OverrideColor = new Color(255, 64, 31);
                    list.Insert(insertIndex + 4, summonDisplay);

                    TooltipLine rogueDisplay = new TooltipLine(this.Mod, "CalamityMod:RogueDisplay", $"[i:{ModContent.ItemType<FrequencyManipulator>()}] Frequency Manipulator");
                    rogueDisplay.OverrideColor = new Color(201, 41, 255);
                    list.Insert(insertIndex + 5, rogueDisplay);

                    TooltipLine machineDisplay = new TooltipLine(this.Mod, "CalamityMod:CodeDisplay", $"[i:{ModContent.ItemType<LongRangedSensorArray>()}] Long Ranged Sensor Array");
                    machineDisplay.OverrideColor = new Color(165, 118, 104);
                    list.Insert(insertIndex + 6, machineDisplay);

                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 10).
                AddIngredient(ModContent.ItemType<DubiousPlating>(), 10).
                AddIngredient(ItemID.Glass, 50).
                AddCondition(SchematicRecipe.ConstructRecipeCondition("Planetoid", out Predicate<Recipe> condition), condition).
                AddTile(TileID.Anvils).
                Register();
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI && RecipeUnlockHandler.HasUnlockedT2ArsenalRecipes)
            {
                PopupGUIManager.FlipActivityOfGUIWithType(typeof(DraedonSchematicPlanetoidGUI));
            }
            return true;
        }
    }
}
