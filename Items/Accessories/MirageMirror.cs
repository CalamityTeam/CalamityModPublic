using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class MirageMirror : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Mirage Mirror");
            Tooltip.SetDefault("Bend light around you\n" +
                "Reduces enemy aggression outside of the abyss\n" +
                "Stealth generates 25% faster when standing still and 12% faster while moving");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.stealthGenStandstill += 0.25f;
            modPlayer.stealthGenMoving += 0.12f;
            player.aggro -= 200;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.MagicMirror).
                AddIngredient(ItemID.BlackLens).
                AddIngredient(ItemID.Bone, 50).
                AddTile(TileID.TinkerersWorkbench).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.IceMirror).
                AddIngredient(ItemID.BlackLens).
                AddIngredient(ItemID.Bone, 50).
                AddTile(TileID.TinkerersWorkbench).
                Register();
        }
    }
}
