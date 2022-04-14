using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AerospecHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Aerospec Hat");
            Tooltip.SetDefault("8% increased magic damage and +20 max mana");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.defense = 3; //15
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
            player.setBonus = "5% increased movement speed and magic critical strike chance\n" +
                    "Taking over 25 damage in one hit will cause a spread of homing feathers to fall\n" +
                    "Allows you to fall more quickly and disables fall damage";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.aeroSet = true;
            player.noFallDmg = true;
            player.moveSpeed += 0.05f;
            player.GetCritChance(DamageClass.Magic) += 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.08f;
            player.statManaMax2 += 20;
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
