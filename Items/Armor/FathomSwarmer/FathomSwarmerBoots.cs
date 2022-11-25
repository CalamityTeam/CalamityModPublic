using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.FathomSwarmer
{
    [AutoloadEquip(EquipType.Legs)]
    public class FathomSwarmerBoots : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Fathom Swarmer Greaves");
            Tooltip.SetDefault("4% increased minion damage\n" +
                "Grants the ability to swim\n" +
                "Movement speed increased by 40% while submerged in liquid");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
            Item.defense = 15;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.04f;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                player.moveSpeed += 0.4f;
            }
            player.ignoreWater = true;
            if (player.wingTime <= 0) //ignore flippers while the player can fly
                player.accFlipper = true;
        }


        

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SeaRemains>(9).
                AddIngredient<PlantyMush>(8).
                AddIngredient<DepthCells>(4).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
