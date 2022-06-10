using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    [LegacyName("VictideLeggings")]
    public class VictideGreaves : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Victide Greaves");
            Tooltip.SetDefault("Movement speed increased by 8%\n" +
                "Movement speed increased by 30% while submerged in liquid");

            if (Main.netMode != NetmodeID.Server)
            {
                int equipSlot = EquipLoader.GetEquipSlot(Mod, Name, EquipType.Legs);
                ArmorIDs.Legs.Sets.HidesBottomSkin[equipSlot] = true;
            }
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Green;
            Item.defense = 4; //9
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += Collision.DrownCollision(player.position, player.width, player.height, player.gravDir) ? 0.3f : 0.08f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SeaRemains>(4).
                AddTile(TileID.Anvils).
                Register();
        }
    }
}
