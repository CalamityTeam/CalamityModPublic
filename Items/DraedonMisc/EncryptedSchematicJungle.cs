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
using CalamityMod.UI.DraedonLogs;

namespace CalamityMod.Items.DraedonMisc
{
    public class EncryptedSchematicJungle : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Encrypted Schematic (Jungle)");
            Tooltip.SetDefault("Requires a Codebreaker with a fine tuned, long range sensor to decrypt");
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
                    line.Text = "Has already been decrypted.\n" +
                        "Click to view its contents.\n" +
                        "Unlocked recipes:";
                int insertIndex = list.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
                if (insertIndex != -1)
                {
                    TooltipLine meleeDisplay = new TooltipLine(this.Mod, "CalamityMod:MeleeDisplay", $"[i:{ModContent.ItemType<GalvanizingGlaive>()}] Galvanizing Glaive");
                    meleeDisplay.OverrideColor = new Color(149, 243, 43);
                    list.Insert(insertIndex + 1, meleeDisplay);

                    TooltipLine rangedDisplay = new TooltipLine(this.Mod, "CalamityMod:RangedDisplay", $"[i:{ModContent.ItemType<GaussRifle>()}] Gauss Rifle");
                    rangedDisplay.OverrideColor = new Color(236, 255, 31);
                    list.Insert(insertIndex + 2, rangedDisplay);

                    TooltipLine mageDisplay = new TooltipLine(this.Mod, "CalamityMod:MageDisplay", $"[i:{ModContent.ItemType<GatlingLaser>()}] Gatling Laser");
                    mageDisplay.OverrideColor = new Color(255, 64, 31);
                    list.Insert(insertIndex + 3, mageDisplay);

                    TooltipLine summonDisplay = new TooltipLine(this.Mod, "CalamityMod:SummonDisplay", $"[i:{ModContent.ItemType<PulseTurretRemote>()}] Pulse Turret Remote");
                    summonDisplay.OverrideColor = new Color(201, 41, 255);
                    list.Insert(insertIndex + 4, summonDisplay);

                    TooltipLine rogueDisplay = new TooltipLine(this.Mod, "CalamityMod:RogueDisplay", $"[i:{ModContent.ItemType<SystemBane>()}] System Bane");
                    rogueDisplay.OverrideColor = new Color(31, 242, 245);
                    list.Insert(insertIndex + 5, rogueDisplay);

                    TooltipLine machineDisplay = new TooltipLine(this.Mod, "CalamityMod:CodeDisplay", $"[i:{ModContent.ItemType<AdvancedDisplay>()}] Advanced Display");
                    machineDisplay.OverrideColor = new Color(165, 118, 104);
                    list.Insert(insertIndex + 6, machineDisplay);

                }
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(10).
                AddIngredient<DubiousPlating>(10).
                AddIngredient(ItemID.Glass, 50).
                AddCondition(SchematicRecipe.ConstructRecipeCondition("Jungle", out Predicate<Recipe> condition), condition).
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
