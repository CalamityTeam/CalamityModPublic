using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class BloodflareMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodflare Ram Mask");
            Tooltip.SetDefault("You can move freely through liquids and have temporary immunity to lava\n" +
                "10% increased melee damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.defense = 49; //85
            item.Calamity().customRarity = CalamityRarity.PureGreen;
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
            modPlayer.bloodflareMelee = true;
            player.setBonus = "Greatly increases life regen\n" +
                "Enemies are more likely to target you\n" +
                "Enemies below 50% life drop a heart when struck\n" +
                "This effect has a 5 second cooldown\n" +
                "Enemies killed during a Blood Moon have a much higher chance to drop Blood Orbs\n" +
                "True melee strikes will heal you\n" +
                "After striking an enemy 15 times with true melee you will enter a blood frenzy for 5 seconds\n" +
                "During this you will gain 25% increased melee damage, critical strike chance, and contact damage is halved\n" +
                "This effect has a 30 second cooldown";
            player.crimsonRegen = true;
            player.aggro += 900;
        }

        public override void UpdateEquip(Player player)
        {
            player.lavaMax += 240;
            player.ignoreWater = true;
            player.meleeDamage += 0.1f;
            player.meleeCrit += 10;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BloodstoneCore>(), 11);
            recipe.AddIngredient(ModContent.ItemType<RuinousSoul>(), 2);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
