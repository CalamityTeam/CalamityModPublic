using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class YharimsInsignia : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 38;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.accessory = true;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.yInsignia = true;
            player.GetAttackSpeed<MeleeDamageClass>() += 0.1f;
            player.GetDamage<TrueMeleeDamageClass>() += 0.1f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WarriorEmblem).
                AddIngredient<NecklaceofVexation>().
                AddIngredient<CoreofSunlight>(5).
                AddIngredient<DivineGeode>(5).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
