using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class DaedalusHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Headgear");
            Tooltip.SetDefault("13% increased ranged damage and 7% increased ranged critical strike chance\n" +
                "Reduces ammo usage by 20%");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 25, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.defense = 9; //39
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
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.daedalusShard = true;
            player.rangedDamage += 0.05f;
        }

        public override void UpdateEquip(Player player)
        {
            player.ammoCost80 = true;
            player.rangedDamage += 0.13f;
            player.rangedCrit += 7;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 8);
            recipe.AddIngredient(ItemID.CrystalShard, 3);
            recipe.AddIngredient(ModContent.ItemType<EssenceofEleum>());
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
