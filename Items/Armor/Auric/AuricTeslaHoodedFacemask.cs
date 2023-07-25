using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
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
    public class AuricTeslaHoodedFacemask : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.defense = 40; //132
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<AuricTeslaBodyArmor>() && legs.type == ModContent.ItemType<AuricTeslaCuisses>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = this.GetLocalizedValue("SetBonus");
            var modPlayer = player.Calamity();
            modPlayer.tarraSet = true;
            modPlayer.tarraRanged = true;
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareRanged = true;
            modPlayer.godSlayer = true;
            modPlayer.godSlayerRanged = true;
            modPlayer.auricSet = true;
            player.thorns += 3f;
            player.ignoreWater = true;
            player.crimsonRegen = true;

            if (modPlayer.godSlayerDashHotKeyPressed || (player.dashDelay != 0 && modPlayer.LastUsedDashID == GodslayerArmorDash.ID))
                modPlayer.DeferredDashID = GodslayerArmorDash.ID;
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.auricBoost = true;
            player.GetDamage<RangedDamageClass>() += 0.3f;
            player.GetCritChance<RangedDamageClass>() += 30;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<GodSlayerHeadRanged>().
                AddIngredient<BloodflareHeadRanged>().
                AddIngredient<TarragonHeadRanged>().
                AddIngredient<AuricBar>(12).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
