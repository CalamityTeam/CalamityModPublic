using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AuricTeslaWireHemmedVisage : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Tesla Wire-Hemmed Visage");
            Tooltip.SetDefault("30% increased magic damage, 20% increased magic critical strike chance, +100 max mana and 20% reduced mana usage\n" +
                               "Not moving boosts all damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.defense = 24; //132
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
            player.setBonus = "Magic Tarragon, Bloodflare and Silva armor effects\n" +
                "All projectiles spawn healing auric orbs on enemy hits\n" +
                "Max run speed and acceleration boosted by 5%";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.tarraSet = true;
            modPlayer.tarraMage = true;
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareMage = true;
            modPlayer.silvaSet = true;
            modPlayer.silvaMage = true;
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
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.auricBoost = true;
            player.manaCost *= 0.8f;
            player.magicDamage += 0.3f;
            player.magicCrit += 20;
            player.statManaMax2 += 100;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SilvaMaskedCap>());
            recipe.AddIngredient(ModContent.ItemType<BloodflareHornedMask>());
            recipe.AddIngredient(ModContent.ItemType<TarragonMask>());
            recipe.AddIngredient(ModContent.ItemType<PsychoticAmulet>());
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 12);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
