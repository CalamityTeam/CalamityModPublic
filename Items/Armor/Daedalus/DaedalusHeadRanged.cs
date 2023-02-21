using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Daedalus
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("DaedalusHelmet")]
    public class DaedalusHeadRanged : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Daedalus Headgear");
            Tooltip.SetDefault("13% increased ranged damage and 7% increased ranged critical strike chance\n" +
                "Reduces ammo usage by 20%");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity5BuyPrice;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 9; //39
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DaedalusBreastplate>() && legs.type == ModContent.ItemType<DaedalusLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "5% increased ranged damage\n" +
                "Getting hit causes you to emit a blast of crystal shards";
            var modPlayer = player.Calamity();
            modPlayer.daedalusShard = true;
            player.GetDamage<RangedDamageClass>() += 0.05f;
        }

        public override void UpdateEquip(Player player)
        {
            player.ammoCost80 = true;
            player.GetDamage<RangedDamageClass>() += 0.13f;
            player.GetCritChance<RangedDamageClass>() += 7;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CryonicBar>(7).
                AddIngredient<EssenceofEleum>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
