using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class StatigelCap : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Statigel Cap");
            Tooltip.SetDefault("10% increased magic damage and 10% decreased mana cost\n" +
                "7% increased magic critical strike chance and +30 max mana");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 5; //22
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<StatigelArmor>() && legs.type == ModContent.ItemType<StatigelGreaves>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "When you take over 100 damage in one hit you become immune to damage for an extended period of time\n" +
                    "Grants an extra jump and increased jump height\n" +
                    "12% increased jump speed";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.statigelSet = true;
            modPlayer.statigelJump = true;
            Player.jumpHeight += 5;
            player.jumpSpeedBoost += 0.6f;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<MagicDamageClass>() += 0.1f;
            player.GetCritChance<MagicDamageClass>() += 7;
            player.manaCost *= 0.9f;
            player.statManaMax2 += 30;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PurifiedGel>(5).
                AddIngredient(ItemID.HellstoneBar, 9).
                AddTile<StaticRefiner>().
                Register();
        }
    }
}
