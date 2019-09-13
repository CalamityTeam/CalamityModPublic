using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.CalamityCustomThrowingDamage;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class BloodflareHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodflare Helm");
            Tooltip.SetDefault("You can move freely through liquids and have temporary immunity to lava\n" +
                "10% increased rogue damage and critical strike chance, 15% increased movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
			item.value = Item.buyPrice(0, 60, 0, 0);
			item.defense = 28; //85
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("BloodflareBodyArmor") && legs.type == mod.ItemType("BloodflareCuisses");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.bloodflareSet = true;
            modPlayer.bloodflareThrowing = true;
			modPlayer.rogueStealthMax = 1.35f;
			player.setBonus = "Greatly increases life regen\n" +
                "Enemies below 50% life have a chance to drop hearts when struck\n" +
                "Enemies above 50% life have a chance to drop mana stars when struck\n" +
                "Enemies killed during a Blood Moon have a much higher chance to drop Blood Orbs\n" +
                "Being over 80% life boosts your defense by 30 and rogue crit by 5%\n" +
                "Being below 80% life boosts your rogue damage by 10%\n" +
                "Rogue critical strikes have a 50% chance to heal you\n" +
				"Rogue stealth builds while not attacking and not moving, up to a max of 135\n" +
				"Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
				"The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
            player.crimsonRegen = true;
        }

        public override void UpdateEquip(Player player)
        {
			player.lavaMax += 240;
			player.ignoreWater = true;
            player.GetCalamityPlayer().throwingDamage += 0.1f;
            player.GetCalamityPlayer().throwingCrit += 10;
			player.moveSpeed += 0.15f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BloodstoneCore", 11);
            recipe.AddIngredient(null, "RuinousSoul", 2);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
