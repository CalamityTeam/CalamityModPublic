using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class DaedalusHat : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daedalus Hood");
            Tooltip.SetDefault("13% increased magic damage and 7% increased magic critical strike chance\n" +
                "10% decreased mana usage and +60 max mana");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 25, 0, 0);
            item.rare = 5;
            item.defense = 5; //35
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
            player.setBonus = "5% increased magic damage\n" +
                "You have a 10% chance to absorb physical attacks and projectiles when hit\n" +
                "If you absorb an attack you are healed for 1/2 of that attack's damage";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.daedalusAbsorb = true;
            player.magicDamage += 0.05f;
        }

        public override void UpdateEquip(Player player)
        {
            player.manaCost *= 0.9f;
            player.magicDamage += 0.13f;
            player.magicCrit += 7;
            player.statManaMax2 += 60;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
