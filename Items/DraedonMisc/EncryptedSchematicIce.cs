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
    public class EncryptedSchematicIce : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Encrypted Schematic (Ice)");
            Tooltip.SetDefault("Requires a Codebreaker with a complex voltage regulation system to decrypt");
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
            if (Main.netMode != NetmodeID.MultiplayerClient && !RecipeUnlockHandler.HasFoundIceSchematic)
            {
                RecipeUnlockHandler.HasFoundIceSchematic = true;
                CalamityNetcode.SyncWorld();
            }
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            TooltipLine line = list.FirstOrDefault(x => x.Mod == "Terraria" && x.Name == "Tooltip0");
            if (RecipeUnlockHandler.HasUnlockedT5ArsenalRecipes)
            {
                if (line != null)
                    line.Text = "Has already been decrypted.\n" +
                        "Click to view its contents.\n" +
                        "Unlocked recipes:";
                int insertIndex = list.FindIndex(x => x.Name == "Tooltip0" && x.Mod == "Terraria");
                if (insertIndex != -1)
                {
                    TooltipLine meleeDisplay = new TooltipLine(this.Mod, "CalamityMod:MeleeDisplay", $"[i:{ModContent.ItemType<Phaseslayer>()}] Phaseslayer");
                    meleeDisplay.OverrideColor = new Color(255, 64, 31);
                    list.Insert(insertIndex + 1, meleeDisplay);

                    TooltipLine rangedDisplay = new TooltipLine(this.Mod, "CalamityMod:RangedDisplay", $"[i:{ModContent.ItemType<PulseRifle>()}] Pulse Rifle");
                    rangedDisplay.OverrideColor = new Color(201, 41, 255);
                    list.Insert(insertIndex + 2, rangedDisplay);

                    TooltipLine mageDisplay = new TooltipLine(this.Mod, "CalamityMod:MageDisplay", $"[i:{ModContent.ItemType<TeslaCannon>()}] Tesla Cannon");
                    mageDisplay.OverrideColor = new Color(31, 242, 245);
                    list.Insert(insertIndex + 3, mageDisplay);

                    TooltipLine summonDisplay = new TooltipLine(this.Mod, "CalamityMod:SummonDisplay", $"[i:{ModContent.ItemType<PoleWarper>()}] Pole Warper");
                    summonDisplay.OverrideColor = new Color(236, 255, 31);
                    list.Insert(insertIndex + 4, summonDisplay);

                    TooltipLine rogueDisplay = new TooltipLine(this.Mod, "CalamityMod:RogueDisplay", $"[i:{ModContent.ItemType<PlasmaGrenade>()}] Plasma Grenade");
                    rogueDisplay.OverrideColor = new Color(149, 243, 43);
                    list.Insert(insertIndex + 5, rogueDisplay);

                    TooltipLine machineDisplay = new TooltipLine(this.Mod, "CalamityMod:CodeDisplay", $"[i:{ModContent.ItemType<AuricQuantumCoolingCell>()}] Auric Quantum Cooling Cell");
                    machineDisplay.OverrideColor = new Color(255, 215, 0);
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
                AddCondition(SchematicRecipe.ConstructRecipeCondition("Ice", out Predicate<Recipe> condition), condition).
                AddTile(TileID.Anvils).
                Register();
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI && RecipeUnlockHandler.HasUnlockedT5ArsenalRecipes)
            {
                PopupGUIManager.FlipActivityOfGUIWithType(typeof(DraedonSchematicIceGUI));
            }
            return true;
        }
    }
}
