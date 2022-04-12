using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Accessories
{
    public class MirageMirror : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mirage Mirror");
            Tooltip.SetDefault("Bend light around you\n" +
                "Reduces enemy aggression outside of the abyss\n" +
                "Stealth generates 30% faster when standing still and 20% faster while moving");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.stealthGenStandstill += 0.3f;
            modPlayer.stealthGenMoving += 0.2f;
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
