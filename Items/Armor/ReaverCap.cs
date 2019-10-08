using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.CalamityCustomThrowingDamage;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class ReaverCap : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Headgear");
            Tooltip.SetDefault("15% increased rogue damage, 5% increased rogue velocity and critical strike chance\n" +
                "20% increased movement speed and can move freely through liquids");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 22;
			item.value = Item.buyPrice(0, 30, 0, 0);
			item.rare = 7;
            item.defense = 10; //43
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("ReaverScaleMail") && legs.type == mod.ItemType("ReaverCuisses");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.GetCalamityPlayer();
            modPlayer.reaverSpore = true;
			modPlayer.rogueStealthMax = 1.15f;
			player.setBonus = "5% increased rogue damage\n" +
                "You emit a cloud of spores when you are hit\n" +
				"Rogue stealth builds while not attacking and not moving, up to a max of 115\n" +
				"Rogue stealth only reduces when you attack, it does not reduce while moving\n" +
				"The higher your rogue stealth the higher your rogue damage, crit, and movement speed";
            player.GetCalamityPlayer().throwingDamage += 0.05f;
            player.GetCalamityPlayer().wearingRogueArmor = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.ignoreWater = true;
            player.GetCalamityPlayer().throwingDamage += 0.15f;
            player.GetCalamityPlayer().throwingCrit += 5;
            player.GetCalamityPlayer().throwingVelocity += 0.05f;
            player.moveSpeed += 0.2f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "DraedonBar", 8);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
