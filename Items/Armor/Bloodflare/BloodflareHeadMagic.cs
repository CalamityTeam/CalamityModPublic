using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor.Bloodflare
{
    [AutoloadEquip(EquipType.Head)]
    [LegacyName("BloodflareHornedMask")]
    public class BloodflareHeadMagic : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.defense = 22; //85
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<BloodflareBodyArmor>() && legs.type == ModContent.ItemType<BloodflareCuisses>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            var modPlayer = player.Calamity();
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareMage = true;
            player.setBonus = "Greatly increases life regen\n" +
                "Enemies below 50% life drop a heart when struck\n" +
                "This effect has a 5 second cooldown\n" +
                "Magic weapons fire ghostly bolts every 1.67 seconds\n" +
                "Magic critical strikes cause flame explosions every 2 seconds";
            player.crimsonRegen = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.manaCost *= 0.83f;
            player.GetDamage<MagicDamageClass>() += 0.2f;
            player.GetCritChance<MagicDamageClass>() += 10;
            player.statManaMax2 += 100;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BloodstoneCore>(11).
                AddIngredient<RuinousSoul>(2).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
