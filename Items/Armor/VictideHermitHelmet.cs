using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class VictideHermitHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Victide Hermit Helmet");
            Tooltip.SetDefault("5% increased magic damage");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.defense = 2; //9
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<VictideBreastplate>() && legs.type == ModContent.ItemType<VictideGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+3 life regen and 10% increased magic damage while submerged in liquid\n" +
                    "When using any weapon you have a 10% chance to throw a returning seashell projectile\n" +
                    "This seashell does true damage and does not benefit from any damage class\n" +
                    "Provides increased underwater mobility and slightly reduces breath loss in the abyss";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.victideSet = true;
            player.ignoreWater = true;
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                player.GetDamage<MagicDamageClass>() += 0.1f;
                player.lifeRegen += 3;
            }
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MagicDamageClass>() += 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SeaRemains>(3).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
