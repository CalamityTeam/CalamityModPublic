using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class GodSlayerHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God Slayer Helmet");
            Tooltip.SetDefault("14% increased ranged damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 75, 0, 0);
            item.defense = 35; //96
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<GodSlayerChestplate>() && legs.type == ModContent.ItemType<GodSlayerLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.godSlayer = true;
            modPlayer.godSlayerRanged = true;
            player.setBonus = "You will survive fatal damage and will be healed 150 HP if an attack would have killed you\n" +
                "This effect can only occur once every 45 seconds\n" +
                "While the cooldown for this effect is active all life regen is halved\n" +
                "Your ranged critical hits have a chance to critically hit, causing 3 times the damage\n" +
                "You have a chance to fire a god killer shrapnel round while firing ranged weapons";
        }

        public override void UpdateEquip(Player player)
        {
            player.rangedDamage += 0.14f;
            player.rangedCrit += 14;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 14);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 8);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 8);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
