using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Placeables.PlaceableTurrets;
using CalamityMod.Rarities;
using CalamityMod.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.UI.DraedonLogs;

namespace CalamityMod.Items.DraedonMisc
{
    public class EncryptedSchematicPlanetoid : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.DraedonItems";
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
                    line.Text = CalamityUtils.GetTextValue($"{LocalizationCategory}.SchematicUnlocked");
                int insertIndex = list.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
                if (insertIndex != -1)
                {
                    int meleeItem = ModContent.ItemType<HydraulicVoltCrasher>();
                    TooltipLine meleeDisplay = new TooltipLine(this.Mod, "CalamityMod:MeleeDisplay", $"[i:{meleeItem}] {CalamityUtils.GetItemName(meleeItem)}");
                    meleeDisplay.OverrideColor = new Color(31, 242, 245);
                    list.Insert(insertIndex + 1, meleeDisplay);

                    int rangedItem = ModContent.ItemType<MatterModulator>();
                    TooltipLine rangedDisplay = new TooltipLine(this.Mod, "CalamityMod:RangedDisplay", $"[i:{rangedItem}] {CalamityUtils.GetItemName(rangedItem)}");
                    rangedDisplay.OverrideColor = new Color(149, 243, 43);
                    list.Insert(insertIndex + 2, rangedDisplay);

                    int mageItem = ModContent.ItemType<GaussPistol>();
                    TooltipLine mageDisplay = new TooltipLine(this.Mod, "CalamityMod:MageDisplay", $"[i:{mageItem}] {CalamityUtils.GetItemName(mageItem)}");
                    mageDisplay.OverrideColor = new Color(236, 255, 31);
                    list.Insert(insertIndex + 3, mageDisplay);

                    int summonItem = ModContent.ItemType<MountedScanner>();
                    TooltipLine summonDisplay = new TooltipLine(this.Mod, "CalamityMod:SummonDisplay", $"[i:{summonItem}] {CalamityUtils.GetItemName(summonItem)}");
                    summonDisplay.OverrideColor = new Color(255, 64, 31);
                    list.Insert(insertIndex + 4, summonDisplay);

                    int rogueItem = ModContent.ItemType<FrequencyManipulator>();
                    TooltipLine rogueDisplay = new TooltipLine(this.Mod, "CalamityMod:RogueDisplay", $"[i:{rogueItem}] {CalamityUtils.GetItemName(rogueItem)}");
                    rogueDisplay.OverrideColor = new Color(201, 41, 255);
                    list.Insert(insertIndex + 5, rogueDisplay);

                    int turretFireItem = ModContent.ItemType<FireTurret>();
                    TooltipLine turretFireDisplay = new TooltipLine(this.Mod, "CalamityMod:CodeDisplay", $"[i:{turretFireItem}] {CalamityUtils.GetItemName(turretFireItem)}");
                    turretFireDisplay.OverrideColor = new Color(165, 118, 104);
                    list.Insert(insertIndex + 6, turretFireDisplay);

                    int turretIceItem = ModContent.ItemType<IceTurret>();
                    TooltipLine turretIceDisplay = new TooltipLine(this.Mod, "CalamityMod:CodeDisplay", $"[i:{turretIceItem}] {CalamityUtils.GetItemName(turretIceItem)}");
                    turretIceDisplay.OverrideColor = new Color(165, 118, 104);
                    list.Insert(insertIndex + 7, turretIceDisplay);

                    int turretLaserItem = ModContent.ItemType<LaserTurret>();
                    TooltipLine turretLaserDisplay = new TooltipLine(this.Mod, "CalamityMod:CodeDisplay", $"[i:{turretLaserItem}] {CalamityUtils.GetItemName(turretLaserItem)}");
                    turretLaserDisplay.OverrideColor = new Color(165, 118, 104);
                    list.Insert(insertIndex + 8, turretLaserDisplay);

                    int codeItem = ModContent.ItemType<LongRangedSensorArray>();
                    TooltipLine machineDisplay = new TooltipLine(this.Mod, "CalamityMod:CodeDisplay", $"[i:{codeItem}] {CalamityUtils.GetItemName(codeItem)}");
                    machineDisplay.OverrideColor = new Color(165, 118, 104);
                    list.Insert(insertIndex + 9, machineDisplay);
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(10).
                AddIngredient<DubiousPlating>(10).
                AddIngredient(ItemID.Glass, 50).
                AddCondition(SchematicRecipe.ConstructRecipeCondition("Planetoid", out Func<bool> condition), condition).
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
