using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class FearmongerChestplate : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fearmonger Chestplate");
            Tooltip.SetDefault(@"+100 max health and permanent Well Fed buffs
Greatly increases life regeneration after striking an enemy");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.defense = 55;
            item.Calamity().postMoonLordRarity = 14;
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.onHitRegen = true;
            player.statLifeMax2 += 100;

			//well fed buffs
			if (!player.HasBuff(BuffID.WellFed))
			{
				player.wellFed = true;
				player.statDefense += 2;
				player.allDamage += 0.05f;
				modPlayer.AllCritBoost(2);
				player.minionKB += 0.5f;
				player.moveSpeed += 0.2f;
			}
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Phantoplasm>(), 16);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 8);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 8);
            recipe.AddIngredient(ItemID.SoulofFright, 12);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}