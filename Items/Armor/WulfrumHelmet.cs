using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class WulfrumHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Helmet");
            Tooltip.SetDefault("10% increased minion damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 0, 75, 0);
            item.rare = ItemRarityID.Blue;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<WulfrumArmor>() && legs.type == ModContent.ItemType<WulfrumLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+3 defense and +1 max minion\n" +
                "+5 defense when below 50% life";
            player.statDefense += 3; //8
            player.maxMinions++;
            if (player.statLife <= (int)(player.statLifeMax2 * 0.5))
            {
                player.statDefense += 5; //13
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.minionDamage += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<WulfrumShard>(), 5);
            recipe.AddIngredient(ModContent.ItemType<EnergyCore>());
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
