using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Bloodflare;
using CalamityMod.Items.Armor.Silva;
using CalamityMod.Items.Armor.Tarragon;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Auric
{
    [AutoloadEquip(EquipType.Head)]
    public class AuricTeslaWireHemmedVisage : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Armor.PostMoonLord";
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.defense = 24; //132
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
            modPlayer.tarraMage = true;
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareMage = true;
            modPlayer.silvaSet = true;
            modPlayer.silvaMage = true;
            modPlayer.auricSet = true;
            player.thorns += 3f;
            player.ignoreWater = true;
            player.crimsonRegen = true;
        }

        public override void UpdateEquip(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.auricBoost = true;
            player.manaCost *= 0.8f;
            player.GetDamage<MagicDamageClass>() += 0.3f;
            player.GetCritChance<MagicDamageClass>() += 20;
            player.statManaMax2 += 100;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<SilvaHeadMagic>().
                AddIngredient<BloodflareHeadMagic>().
                AddIngredient<TarragonHeadMagic>().
                AddIngredient<AuricBar>(12).
                AddTile<CosmicAnvil>().
                Register();
        }
    }
}
