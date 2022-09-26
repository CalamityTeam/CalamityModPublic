using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.UI;
using CalamityMod.Rarities;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.DraedonMisc
{
    public class EncryptedSchematicSunkenSea : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Schematic (Sunken Sea)");
            Tooltip.SetDefault("Finely detailed diagrams of numerous devices and weaponry dance across the holographic screen.\n" +
                "Picking up this item or holding it in your inventory permanently unlocks new recipes.\n" +
                "Click to view its contents.");
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
            // Since no decrypting is necessary for this item simply placing it in your inventory is sufficient enough to "learn"
            // from it.
            if (Main.netMode != NetmodeID.MultiplayerClient && (!RecipeUnlockHandler.HasUnlockedT1ArsenalRecipes || !RecipeUnlockHandler.HasFoundSunkenSeaSchematic))
            {
                RecipeUnlockHandler.HasFoundSunkenSeaSchematic = true;
                RecipeUnlockHandler.HasUnlockedT1ArsenalRecipes = true;
                CalamityNetcode.SyncWorld();
            }
        }

        // Recipe exists for posierity.
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<MysteriousCircuitry>(10).
                AddIngredient<DubiousPlating>(10).
                AddIngredient(ItemID.Glass, 50).
                AddCondition(SchematicRecipe.ConstructRecipeCondition("Sunken Sea", out Predicate<Recipe> condition), condition).
                AddTile(TileID.Anvils).
                Register();
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");
            if (RecipeUnlockHandler.HasUnlockedT1ArsenalRecipes)
            {
                int insertIndex = list.FindIndex(x => x.Name == "Tooltip2" && x.Mod == "Terraria");
                if (insertIndex != -1)
                {
                    TooltipLine meleeDisplay = new TooltipLine(this.Mod, "CalamityMod:MeleeDisplay", $"[i:{ModContent.ItemType<GaussDagger>()}] Gauss Dagger");
                    meleeDisplay.OverrideColor = new Color(236, 255, 31);
                    list.Insert(insertIndex + 1, meleeDisplay);

                    TooltipLine rangedDisplay = new TooltipLine(this.Mod, "CalamityMod:RangedDisplay", $"[i:{ModContent.ItemType<Taser>()}] Taser");
                    rangedDisplay.OverrideColor = new Color(31, 242, 245);
                    list.Insert(insertIndex + 2, rangedDisplay);

                    TooltipLine mageDisplay = new TooltipLine(this.Mod, "CalamityMod:MageDisplay", $"[i:{ModContent.ItemType<PulsePistol>()}] Pulse Pistol");
                    mageDisplay.OverrideColor = new Color(201, 41, 255);
                    list.Insert(insertIndex + 3, mageDisplay);

                    TooltipLine summonDisplay = new TooltipLine(this.Mod, "CalamityMod:SummonDisplay", $"[i:{ModContent.ItemType<StarSwallowerContainmentUnit>()}] Star Swallower Containment Unit");
                    summonDisplay.OverrideColor = new Color(149, 243, 43);
                    list.Insert(insertIndex + 4, summonDisplay);

                    TooltipLine rogueDisplay = new TooltipLine(this.Mod, "CalamityMod:RogueDisplay", $"[i:{ModContent.ItemType<TrackingDisk>()}] Tracking Disk");
                    rogueDisplay.OverrideColor = new Color(255, 64, 31);
                    list.Insert(insertIndex + 5, rogueDisplay);

                    TooltipLine machineDisplay = new TooltipLine(this.Mod, "CalamityMod:CodeDisplay", $"[i:{ModContent.ItemType<DecryptionComputer>()}] Decryption Computer");
                    machineDisplay.OverrideColor = new Color(165, 118, 104);
                    list.Insert(insertIndex + 6, machineDisplay);

                }
            }

        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI && RecipeUnlockHandler.HasUnlockedT1ArsenalRecipes)
            {
                PopupGUIManager.FlipActivityOfGUIWithType(typeof(DraedonSchematicSunkenSeaGUI));
            }
            return true;
        }
    }
}
