using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class UmbraphileHood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Umbraphile Hood");
            Tooltip.SetDefault("8% increased rogue damage and 10% increased rogue velocity");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 20;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.rare = 7;
            item.defense = 8; //36
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<UmbraphileRegalia>() && legs.type == ModContent.ItemType<UmbraphileBoots>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.umbraphileSet = true;
            modPlayer.rogueStealthMax += 1.15f;
            player.setBonus = "Rogue weapons have a chance to create explosions on hit\n" +
				"Stealth strikes always create an explosion\n" +
                "Rogue stealth builds while not attacking and not moving, up to a max of 115\n" +
                "Once you have built max stealth, you will be able to perform a Stealth Strike\n" +
                "Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
                "The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
            player.Calamity().wearingRogueArmor = true;
        }

        public override void UpdateEquip(Player player)
        {
			player.Calamity().throwingDamage += 0.08f;
			player.Calamity().throwingVelocity += 0.10f;
        }

        /*public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SolarVeil>(), 12);
			recipe.AddIngredient(ItemID.HallowedBar, 8);
			recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }*/
    }
}
