using CalamityMod.CustomRecipes;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using CalamityMod.Items.DraedonMisc;
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
    public class EncryptedSchematicHell : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Encrypted Schematic (Underworld)");
            Tooltip.SetDefault("Requires a Codebreaker with a sophisticated display to decrypt");
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
            if (Main.netMode != NetmodeID.MultiplayerClient && !RecipeUnlockHandler.HasFoundHellSchematic)
            {
                RecipeUnlockHandler.HasFoundHellSchematic = true;
                CalamityNetcode.SyncWorld();
            }
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");
            if (RecipeUnlockHandler.HasUnlockedT4ArsenalRecipes)
            {
                if (line != null)
                    line.Text = "Has already been decrypted.\n" +
                        "Click to view its contents.\n" +
                        "Unlocked recipes:";
                int insertIndex = list.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
                if (insertIndex != -1)
                {
                    TooltipLine meleeDisplay = new TooltipLine(this.Mod, "CalamityMod:MeleeDisplay", $"[i:{ModContent.ItemType<PulseDragon>()}] Pulse Dragon");
                    meleeDisplay.OverrideColor = new Color(201, 41, 255);
                    list.Insert(insertIndex + 1, meleeDisplay);

                    TooltipLine rangedDisplay = new TooltipLine(this.Mod, "CalamityMod:RangedDisplay", $"[i:{ModContent.ItemType<HeavyLaserRifle>()}] Heavy Laser Rifle");
                    rangedDisplay.OverrideColor = new Color(255, 64, 31);
                    list.Insert(insertIndex + 2, rangedDisplay);

                    TooltipLine mageDisplay = new TooltipLine(this.Mod, "CalamityMod:MageDisplay", $"[i:{ModContent.ItemType<PlasmaCaster>()}] Plasma Caster");
                    mageDisplay.OverrideColor = new Color(149, 243, 43);
                    list.Insert(insertIndex + 3, mageDisplay);

                    TooltipLine summonDisplay = new TooltipLine(this.Mod, "CalamityMod:SummonDisplay", $"[i:{ModContent.ItemType<SnakeEyes>()}] Snake Eyes");
                    summonDisplay.OverrideColor = new Color(31, 242, 245);
                    list.Insert(insertIndex + 4, summonDisplay);

                    TooltipLine rogueDisplay = new TooltipLine(this.Mod, "CalamityMod:RogueDisplay", $"[i:{ModContent.ItemType<WavePounder>()}] Wave Pounder");
                    rogueDisplay.OverrideColor = new Color(236, 255, 31);
                    list.Insert(insertIndex + 5, rogueDisplay);

                    TooltipLine machineDisplay = new TooltipLine(this.Mod, "CalamityMod:CodeDisplay", $"[i:{ModContent.ItemType<VoltageRegulationSystem>()}] Voltage Regulation System");
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
                AddCondition(SchematicRecipe.ConstructRecipeCondition("Hell", out Predicate<Recipe> condition), condition).
                AddTile(TileID.Anvils).
                Register();
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI && RecipeUnlockHandler.HasUnlockedT4ArsenalRecipes)
            {
                PopupGUIManager.FlipActivityOfGUIWithType(typeof(DraedonSchematicHellGUI));
            }
            return true;
        }
    }
}
