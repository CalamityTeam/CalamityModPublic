using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class BloodflareHornedMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodflare Hydra Hood");
            Tooltip.SetDefault("You can move freely through liquids and have temporary immunity to lava\n" +
                "20% increased magic damage, 10% increased magic critical strike chance, +100 max mana and 17% reduced mana usage");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.defense = 22; //85
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
            modPlayer.bloodflareMage = true;
            player.setBonus = "Greatly increases life regen\n" +
                "Enemies below 50% life drop a heart when struck\n" +
                "This effect has a 5 second cooldown\n" +
                "Enemies killed during a Blood Moon have a much higher chance to drop Blood Orbs\n" +
                "Magic weapons fire ghostly bolts every 1.67 seconds\n" +
                "Magic critical strikes cause flame explosions every 2 seconds";
            player.crimsonRegen = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.manaCost *= 0.83f;
            player.lavaMax += 240;
            player.ignoreWater = true;
            player.magicDamage += 0.2f;
            player.magicCrit += 10;
            player.statManaMax2 += 100;
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
