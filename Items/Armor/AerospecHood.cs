using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AerospecHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Aerospec Hood");
            Tooltip.SetDefault("8% increased ranged damage");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 5; //18
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AerospecBreastplate>() && legs.type == ModContent.ItemType<AerospecLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "5% increased movement speed and ranged critical strike chance\n" +
                "Taking over 25 damage in one hit will cause a spread of homing feathers to fall\n" +
                "Allows you to fall more quickly and disables fall damage";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.aeroSet = true;
            player.noFallDmg = true;
            player.moveSpeed += 0.05f;
            player.GetCritChance<RangedDamageClass>() += 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<RangedDamageClass>() += 0.08f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AerialiteBar>(5).
                AddIngredient(ItemID.Cloud, 3).
                AddIngredient(ItemID.RainCloud).
                AddIngredient(ItemID.Feather).
                AddTile(TileID.SkyMill).
                Register();
        }
    }
}
