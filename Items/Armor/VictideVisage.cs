using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class VictideVisage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Victide Visage");
            Tooltip.SetDefault("5% increased ranged damage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 1, 50, 0);
			item.rare = 2;
            item.defense = 3; //10
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("VictideBreastplate") && legs.type == mod.ItemType("VictideLeggings");
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Increased life regen and ranged damage while submerged in liquid\n" +
                   "When using any weapon you have a 10% chance to throw a returning seashell projectile\n" +
                   "This seashell does true damage and does not benefit from any damage class\n" +
                   "Slightly reduces breath loss in the abyss";
            CalamityPlayer modPlayer = player.GetCalamityPlayer();
            modPlayer.victideSet = true;
            player.ignoreWater = true;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                player.rangedDamage += 0.1f;
                player.lifeRegen += 3;
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.rangedDamage += 0.05f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "VictideBar", 3);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
