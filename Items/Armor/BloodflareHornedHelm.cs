using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.World;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class BloodflareHornedHelm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodflare Horned Helm");
            Tooltip.SetDefault("You can move freely through liquids and have temporary immunity to lava\n" +
                "10% increased ranged damage and critical strike chance");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.defense = 34; //85
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void ModifyTooltips(List<TooltipLine> list)
        {
			if (CalamityWorld.death)
			{
				foreach (TooltipLine line2 in list)
				{
					if (line2.mod == "Terraria" && line2.Name == "Tooltip1")
					{
						line2.text = "10% increased ranged damage and critical strike chance\n" +
						"Provides heat protection in Death Mode";
					}
				}
			}
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
            modPlayer.bloodflareRanged = true;
            string hotkey = CalamityMod.TarraHotKey.TooltipHotkeyString();
            player.setBonus = "Greatly increases life regen\n" +
                "Enemies below 50% life drop a heart when struck\n" +
				"This effect has a 5 second cooldown\n" +
                "Enemies killed during a Blood Moon have a much higher chance to drop Blood Orbs\n" +
                "Press " + hotkey + " to unleash the lost souls of polterghast to destroy your enemies\n" +
                "This effect has a 30 second cooldown\n" +
                "Ranged weapons fire bloodsplosion orbs every 2.5 seconds";
            player.crimsonRegen = true;

			if (modPlayer.bloodflareSoulTimer == 1) //sound when ready to use again
			{
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/BloodflareRangerRecharge"), player.Center);
			}
        }

        public override void UpdateEquip(Player player)
        {
            player.lavaMax += 240;
            player.ignoreWater = true;
            player.rangedDamage += 0.1f;
            player.rangedCrit += 10;
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
