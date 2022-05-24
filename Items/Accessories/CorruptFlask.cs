using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class CorruptFlask : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Corrupt Flask");
            Tooltip.SetDefault("4% increased damage reduction and +6 defense while in the corruption\n" +
                "Grants immunity to the Cursed Inferno debuff");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.buffImmune[BuffID.CursedInferno] = true;
            if (player.ZoneCorrupt)
            {
                player.statDefense += 6;
                player.endurance += 0.04f;
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.VilePowder, 15).
                AddIngredient(ItemID.RottenChunk, 10).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
