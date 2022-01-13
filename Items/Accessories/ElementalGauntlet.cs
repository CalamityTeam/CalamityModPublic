using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
	[AutoloadEquip(new EquipType[] { EquipType.HandsOn, EquipType.HandsOff } )]
    public class ElementalGauntlet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Elemental Gauntlet");
            Tooltip.SetDefault("Melee attacks and projectiles inflict on fire, frostburn and holy flames\n" +
                "15% increased melee speed, damage, and 5% increased melee critical strike chance\n" +
                "20% increased true melee damage\n" +
                "Temporary immunity to lava\n" +
                "Increased melee knockback");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 38;
            item.value = CalamityGlobalItem.Rarity14BuyPrice;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
            if (CalamityWorld.death)
            {
                foreach (TooltipLine line2 in list)
                {
                    if (line2.mod == "Terraria" && line2.Name == "Tooltip4")
                    {
                        line2.text += "\nProvides heat and cold protection in Death Mode";
                    }
                }
            }
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.eGauntlet = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FireGauntlet);
            recipe.AddIngredient(ModContent.ItemType<YharimsInsignia>());
            recipe.AddIngredient(ItemID.LunarBar, 8);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 4);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 4);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
