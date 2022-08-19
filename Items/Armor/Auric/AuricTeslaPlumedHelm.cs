using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer.Dashes;
using CalamityMod.Items.Armor.Bloodflare;
using CalamityMod.Items.Armor.GodSlayer;
using CalamityMod.Items.Armor.Tarragon;

namespace CalamityMod.Items.Armor.Auric
{
    [AutoloadEquip(EquipType.Head)]
    public class AuricTeslaPlumedHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Auric Tesla Plumed Helm");
            Tooltip.SetDefault("20% increased rogue damage, critical strike chance and 5% increased movement speed");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(1, 80, 0, 0);
            Item.defense = 34; //132
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
            player.setBonus = "Rogue Tarragon, Bloodflare and God Slayer armor effects\n" +
                "All projectiles spawn healing auric orbs on enemy hits\n" +
				"+130 maximum stealth";
            var modPlayer = player.Calamity();
            modPlayer.tarraSet = true;
            modPlayer.tarraThrowing = true;
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareThrowing = true;
            modPlayer.godSlayer = true;
            modPlayer.godSlayerThrowing = true;
            modPlayer.auricSet = true;
            modPlayer.rogueStealthMax += 1.3f;
            modPlayer.wearingRogueArmor = true;
            player.thorns += 3f;
            player.lavaMax += 240;
            player.ignoreWater = true;
            player.crimsonRegen = true;

            if (modPlayer.godSlayerDashHotKeyPressed || (player.dashDelay != 0 && modPlayer.LastUsedDashID == GodslayerArmorDash.ID))
                modPlayer.DeferredDashID = GodslayerArmorDash.ID;
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.auricBoost = true;
            player.GetDamage<ThrowingDamageClass>() += 0.2f;
            player.GetCritChance<ThrowingDamageClass>() += 20;
            player.moveSpeed += 0.05f;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GodSlayerHeadRogue>().
                AddIngredient<BloodflareHeadRogue>().
                AddIngredient<TarragonHeadRogue>().
                AddIngredient<AuricBar>(12).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
