using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class ReaverMask : ModItem
    {
		//Damage and Crit Helm
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaver Mask");
            Tooltip.SetDefault("5% increased damage and 10% increased critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 22;
            item.value = Item.buyPrice(0, 30, 0, 0);
            item.rare = 7;
            item.defense = 3; //36
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<ReaverScaleMail>() && legs.type == ModContent.ItemType<ReaverCuisses>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.reaverDamage = true;
            player.setBonus = "15% increased damage and 5% increased critical strike chance\n" +
				"20% chance to not consume ammo or rogue weapons\n" +
				"+80 max mana and 10% increased melee speed\n" +
				"+1 max minions and max sentries\n" +
				"However, damage taken is increased by 10% and healing potions are 10% less effective";
            player.statManaMax2 += 80;
			player.maxMinions += 1;
			player.maxTurrets += 1;
			modPlayer.throwingAmmoCost *= 0.8f;
			modPlayer.rangedAmmoCost *= 0.8f;
            modPlayer.AllCritBoost(5);
			player.allDamage += 0.15f;
            modPlayer.wearingRogueArmor = true;
        }

        public override void UpdateEquip(Player player)
        {
            player.Calamity().AllCritBoost(10);
			player.allDamage += 0.05f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 6);
			recipe.AddIngredient(ItemID.JungleSpores, 4);
			recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>());
			recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
