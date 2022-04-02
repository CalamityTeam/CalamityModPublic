using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AuricTeslaHoodedFacemask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Tesla Hooded Facemask");
            Tooltip.SetDefault("30% increased ranged damage and critical strike chance\n" +
                               "Not moving boosts all damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.defense = 40; //132
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AuricTeslaBodyArmor>() && legs.type == ModContent.ItemType<AuricTeslaCuisses>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "Ranged Tarragon, Bloodflare and God Slayer armor effects\n" +
                "All projectiles spawn healing auric orbs on enemy hits";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.tarraSet = true;
            modPlayer.tarraRanged = true;
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareRanged = true;
            modPlayer.godSlayer = true;
            modPlayer.godSlayerRanged = true;
            modPlayer.auricSet = true;
            player.thorns += 3f;
            player.lavaMax += 240;
            player.ignoreWater = true;
            player.crimsonRegen = true;
            if (player.lavaWet)
            {
                player.statDefense += 30;
                player.lifeRegen += 10;
            }

            if (modPlayer.godSlayerDashHotKeyPressed)
                modPlayer.dashMod = 9;
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.auricBoost = true;
            player.rangedDamage += 0.3f;
            player.rangedCrit += 30;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<GodSlayerHelmet>());
            recipe.AddIngredient(ModContent.ItemType<BloodflareHornedHelm>());
            recipe.AddIngredient(ModContent.ItemType<TarragonVisage>());
            recipe.AddIngredient(ModContent.ItemType<PsychoticAmulet>());
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 12);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
