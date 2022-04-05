using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class HallowedRune : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hallowed Rune");
            Tooltip.SetDefault("Whenever your minions hit an enemy you will gain a random buff, does not stack with downgrades\n" +
                "These buffs will either boost your defense, summon damage, or life regen for a while\n" +
                "If you have the offensive boost, enemies hit by minions will sometimes be hit by stars");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().hallowedRune = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SpiritGenerator>()
                .AddIngredient(ItemID.HallowedBar, 18)
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddIngredient(ItemID.SoulofSight, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
