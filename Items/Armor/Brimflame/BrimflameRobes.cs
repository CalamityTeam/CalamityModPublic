using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Brimflame
{
    [AutoloadEquip(EquipType.Body)]
    public class BrimflameRobes : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Brimflame Robes");
            Tooltip.SetDefault("5% increased magic damage and critical strike chance\n" +
                "Reduces damage from touching lava");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.defense = 15;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MagicDamageClass>() += 0.05f;
            player.GetCritChance<MagicDamageClass>() += 5;
            player.lavaRose = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ObsidianRose).
                AddIngredient<CalamityDust>(8).
                AddIngredient<UnholyCore>(4).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
