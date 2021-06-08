using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class GodSlayerMask : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("God Slayer Mask");
            Tooltip.SetDefault("14% increased rogue damage and critical strike chance, 18% increased movement speed");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 75, 0, 0);
            item.defense = 29; //96
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<GodSlayerChestplate>() && legs.type == ModContent.ItemType<GodSlayerLeggings>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.godSlayer = true;
            modPlayer.godSlayerThrowing = true;
            modPlayer.rogueStealthMax += 1.2f;
            modPlayer.wearingRogueArmor = true;
            player.setBonus = "Allows you to dash for an immense distance\n" +
				"Enemies you dash through take massive damage\n" +
				"This dash has a 30 second cooldown\n" +
				"While at full HP all of your rogue stats are boosted by 10%\n" +
                "If you take over 80 damage in one hit you will be given extra immunity frames\n" +
                "Rogue stealth builds while not attacking and slower while moving, up to a max of 120\n" +
                "Once you have built max stealth, you will be able to perform a Stealth Strike\n" +
                "Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                "The higher your rogue stealth the higher your rogue damage, crit, and movement speed";

			if (!modPlayer.godSlayerCooldown)
				modPlayer.dashMod = 9;
		}

        public override void UpdateEquip(Player player)
        {
            player.Calamity().throwingDamage += 0.14f;
            player.Calamity().throwingCrit += 14;
            player.moveSpeed += 0.1f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 14);
            recipe.AddIngredient(ModContent.ItemType<NightmareFuel>(), 8);
            recipe.AddIngredient(ModContent.ItemType<EndothermicEnergy>(), 8);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
