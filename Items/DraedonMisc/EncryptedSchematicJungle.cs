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
    public class EncryptedSchematicJungle : ModItem, ILocalizedModType
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
            if (Main.netMode != NetmodeID.MultiplayerClient && !RecipeUnlockHandler.HasFoundJungleSchematic)
            {
                RecipeUnlockHandler.HasFoundJungleSchematic = true;
                CalamityNetcode.SyncWorld();
            }
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");
            if (RecipeUnlockHandler.HasUnlockedT3ArsenalRecipes)
            {
                if (line != null)
                    line.Text = CalamityUtils.GetTextValue($"{LocalizationCategory}.SchematicUnlocked");
                int insertIndex = list.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
                if (insertIndex != -1)
                {
                    int meleeItem = ModContent.ItemType<GalvanizingGlaive>();
                    TooltipLine meleeDisplay = new TooltipLine(this.Mod, "CalamityMod:MeleeDisplay", $"[i:{meleeItem}] {CalamityUtils.GetItemName(meleeItem)}");
                    meleeDisplay.OverrideColor = new Color(149, 243, 43);
                    list.Insert(insertIndex + 1, meleeDisplay);

                    int rangedItem = ModContent.ItemType<GaussRifle>();
                    TooltipLine rangedDisplay = new TooltipLine(this.Mod, "CalamityMod:RangedDisplay", $"[i:{rangedItem}] {CalamityUtils.GetItemName(rangedItem)}");
                    rangedDisplay.OverrideColor = new Color(236, 255, 31);
                    list.Insert(insertIndex + 2, rangedDisplay);

                    int mageItem = ModContent.ItemType<GatlingLaser>();
                    TooltipLine mageDisplay = new TooltipLine(this.Mod, "CalamityMod:MageDisplay", $"[i:{mageItem}] {CalamityUtils.GetItemName(mageItem)}");
                    mageDisplay.OverrideColor = new Color(255, 64, 31);
                    list.Insert(insertIndex + 3, mageDisplay);

                    int summonItem = ModContent.ItemType<PulseTurretRemote>();
                    TooltipLine summonDisplay = new TooltipLine(this.Mod, "CalamityMod:SummonDisplay", $"[i:{summonItem}] {CalamityUtils.GetItemName(summonItem)}");
                    summonDisplay.OverrideColor = new Color(201, 41, 255);
                    list.Insert(insertIndex + 4, summonDisplay);

                    int rogueItem = ModContent.ItemType<SystemBane>();
                    TooltipLine rogueDisplay = new TooltipLine(this.Mod, "CalamityMod:RogueDisplay", $"[i:{rogueItem}] {CalamityUtils.GetItemName(rogueItem)}");
                    rogueDisplay.OverrideColor = new Color(31, 242, 245);
                    list.Insert(insertIndex + 5, rogueDisplay);

                    int turretPlagueItem = ModContent.ItemType<PlagueTurret>();
                    TooltipLine turretPlagueDisplay = new TooltipLine(this.Mod, "CalamityMod:CodeDisplay", $"[i:{turretPlagueItem}] {CalamityUtils.GetItemName(turretPlagueItem)}");
                    turretPlagueDisplay.OverrideColor = new Color(165, 118, 104);
                    list.Insert(insertIndex + 6, turretPlagueDisplay);

                    int codeItem = ModContent.ItemType<AdvancedDisplay>();
                    TooltipLine machineDisplay = new TooltipLine(this.Mod, "CalamityMod:CodeDisplay", $"[i:{codeItem}] {CalamityUtils.GetItemName(codeItem)}");
                    machineDisplay.OverrideColor = new Color(165, 118, 104);
                    list.Insert(insertIndex + 7, machineDisplay);
                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(10).
                AddIngredient<DubiousPlating>(10).
                AddIngredient(ItemID.Glass, 50).
                AddCondition(SchematicRecipe.ConstructRecipeCondition("Jungle", out Func<bool> condition), condition).
                AddTile(TileID.Anvils).
                Register();
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI && RecipeUnlockHandler.HasUnlockedT3ArsenalRecipes)
            {
                PopupGUIManager.FlipActivityOfGUIWithType(typeof(DraedonSchematicJungleGUI));
            }
            return true;
        }
    }
}
