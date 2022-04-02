using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
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
            string hotkey = CalamityMod.GodSlayerDashHotKey.TooltipHotkeyString();
            player.setBonus = "Allows you to dash for an immense distance in 8 directions\n" +
                "Press " + hotkey + " while holding down the movement keys in the direction you want to dash\n" +
                "Enemies you dash through take massive damage\n" +
                "During the dash you are immune to most debuffs\n" +
                "The dash has a 35 second cooldown\n" +
                "You fire a god killer shrapnel round while firing ranged weapons every 2.5 seconds";

            if (modPlayer.godSlayerDashHotKeyPressed)
            {
                modPlayer.dashMod = 9;
                player.dash = 0;
            }
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
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 2);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
