using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class BloodflareHelmet : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Bloodflare Wyvern Helm");
            Tooltip.SetDefault("You can move freely through liquids and have temporary immunity to lava\n" +
                "5% increased minion damage");
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.defense = 16; //85
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
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
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareSummon = true;
            modPlayer.WearingPostMLSummonerSet = true;
            player.setBonus = "50% increased minion damage and +3 max minions\n" +
                "Greatly increases life regen\n" +
                "Enemies below 50% life drop a heart when struck\n" +
                "This effect has a 5 second cooldown\n" +
                "Enemies killed during a Blood Moon have a much higher chance to drop Blood Orbs\n" +
                "Summons polterghast mines to circle you\n" +
                "At 90% life and above you gain 10% increased minion damage\n" +
                "At 50% life and below you gain 20 defense and 2 life regen";
            player.crimsonRegen = true;
            player.GetDamage(DamageClass.Summon) += 0.5f;
            player.maxMinions += 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.lavaMax += 240;
            player.ignoreWater = true;
            player.GetDamage(DamageClass.Summon) += 0.05f;
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
