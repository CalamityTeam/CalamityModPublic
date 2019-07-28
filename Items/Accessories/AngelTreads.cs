using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    [AutoloadEquip(EquipType.Shoes)]
    public class AngelTreads : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Angel Treads");
            Tooltip.SetDefault("Extreme speed!\n" +
                               "Greater mobility on ice\n" +
                               "Water and lava walking\n" +
                               "Temporary immunity to lava");
        }

        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 32;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 8;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.harpyRing = true;
            player.accRunSpeed = 8f;
            player.rocketBoots = 3;
            player.moveSpeed += 0.16f;
            player.iceSkate = true;
            player.waterWalk = true;
            player.fireWalk = true;
            player.lavaMax += 240;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FrostsparkBoots);
            recipe.AddIngredient(ItemID.LavaWaders);
            recipe.AddIngredient(null, "HarpyRing");
            recipe.AddIngredient(null, "EssenceofCinder", 5);
            recipe.AddIngredient(null, "AerialiteBar", 20);
            recipe.AddIngredient(ItemID.SoulofMight);
            recipe.AddIngredient(ItemID.SoulofSight);
            recipe.AddIngredient(ItemID.SoulofFright);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
