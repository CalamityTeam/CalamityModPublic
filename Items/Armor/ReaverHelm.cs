using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class ReaverHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Helm");
            Tooltip.SetDefault("15% increased melee damage, 10% increased melee speed, and 5% increased melee critical strike chance\n" +
                "10% increased movement speed and can move freely through liquids");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 30, 0, 0);
			item.rare = 7;
            item.defense = 25; //58
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("ReaverScaleMail") && legs.type == mod.ItemType("ReaverCuisses");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.GetCalamityPlayer();
            player.thorns += 0.33f;
            modPlayer.reaverBlast = true;
            player.setBonus = "5% increased melee damage\n" +
                "Melee projectiles explode on hit\n" +
                "Reaver thorns\n" +
                "Rage activates when you are damaged";
            player.meleeDamage += 0.05f;
        }

        public override void UpdateEquip(Player player)
        {
            player.ignoreWater = true;
            player.meleeDamage += 0.15f;
            player.meleeCrit += 5;
            player.meleeSpeed += 0.1f;
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DraedonBar", 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
