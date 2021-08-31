using CalamityMod.Buffs.Pets;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Pets;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class DaawnlightSpiritOrigin : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daawnlight Spirit Origin");
            Tooltip.SetDefault("Reduces critical strike chance for ranged weapons but significantly boosts their damage on hit\n" +
                "The slower a ranged weapon is, the more damage critical strikes will do\n" +
                "Causes all nearby enemies to be marked with a bullseye somewhere on their hitbox\n" +
                "If a ranged projectile manages to hit the bullseye you do much more damage than usual\n" +
                "Once the bullseye is hit it disappears and a new one appears elsewhere on the enemy's hitbox");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 38;
			item.rare = ItemRarityID.Purple;
			item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.Calamity().donorItem = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().spiritOrigin = true;

            if (hideVisual)
            {
                if (player.FindBuffIndex(ModContent.BuffType<DaawnlightSpiritOriginBuff>()) != -1)
                    player.ClearBuff(ModContent.BuffType<DaawnlightSpiritOriginBuff>());
            }
            else if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<DaawnlightSpiritOriginBuff>()) == -1)
                    player.AddBuff(ModContent.BuffType<DaawnlightSpiritOriginBuff>(), 18000, true);
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DaedalusEmblem>());
            recipe.AddIngredient(ModContent.ItemType<LeadCore>(), 3);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 15);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 15);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 4);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
