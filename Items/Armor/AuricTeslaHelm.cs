using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer.Dashes;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class AuricTeslaHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Auric Tesla Royal Helm");
            Tooltip.SetDefault("20% increased melee damage and critical strike chance\n" +
                               "Not moving boosts all damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(1, 80, 0, 0);
            Item.defense = 54; //132
            Item.Calamity().customRarity = CalamityRarity.Violet;
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
            player.setBonus = "Melee Tarragon, Bloodflare and God Slayer effects\n" +
                "Enemies are more likely to target you\n" +
                "All projectiles spawn healing auric orbs on enemy hits";
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.tarraSet = true;
            modPlayer.tarraMelee = true;
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareMelee = true;
            modPlayer.godSlayer = true;
            modPlayer.godSlayerDamage = true;
            modPlayer.auricSet = true;
            player.thorns += 3f;
            player.lavaMax += 240;
            player.ignoreWater = true;
            player.crimsonRegen = true;
            player.aggro += 1200;

            if (modPlayer.godSlayerDashHotKeyPressed)
                modPlayer.DashID = GodslayerArmorDash.ID;
        }

        public override void UpdateEquip(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.auricBoost = true;
            player.GetDamage(DamageClass.Melee) += 0.2f;
            player.GetCritChance(DamageClass.Melee) += 20;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GodSlayerHelm>().
                AddIngredient<BloodflareMask>().
                AddIngredient<TarragonHelm>().
                AddIngredient<PsychoticAmulet>().
                AddIngredient<AuricBar>(12).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
