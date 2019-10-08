using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Armor
{
    [AutoloadEquip(EquipType.Head)]
    public class OmegaBlueHelmet : ModItem
	{
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Omega Blue Helmet");
            Tooltip.SetDefault(@"You can move freely through liquids
12% increased damage and 8% increased critical strike chance
+2 max minions");
		}

		public override void SetDefaults()
		{
            item.width = 18;
            item.height = 18;
			item.value = Item.sellPrice(0, 35, 0, 0);
			item.rare = 10;
			item.defense = 19;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}

        public override void UpdateEquip(Player player)
        {
            player.ignoreWater = true;

			player.allDamage += 0.12f;
			player.GetModPlayer<CalamityPlayer>().AllCritBoost(8);

			player.maxMinions += 2;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == mod.ItemType("OmegaBlueChestplate") && legs.type == mod.ItemType("OmegaBlueLeggings");
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = @"Increases armor penetration by 50
10% increased damage and critical strike chance
Short-ranged tentacles heal you by sucking enemy life
Press Y to activate abyssal madness for 5 seconds
Abyssal madness increases damage, critical strike chance, and tentacle aggression/range
This effect has a 30 second cooldown";

            player.armorPenetration += 50;
            player.GetCalamityPlayer().wearingRogueArmor = true;

            //raise rev caps
            player.GetModPlayer<CalamityPlayer>().omegaBlueSet = true;

            if (player.GetModPlayer<CalamityPlayer>().omegaBlueCooldown > 0)
            {
                if (player.GetModPlayer<CalamityPlayer>().omegaBlueCooldown == 1) //dust when ready to use again
                {
                    for (int i = 0; i < 66; i++)
                    {
                        int d = Dust.NewDust(player.position, player.width, player.height, 20, 0, 0, 100, Color.Transparent, 2.6f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].fadeIn = 1f;
                        Main.dust[d].velocity *= 6.6f;
                    }
                }
                player.GetModPlayer<CalamityPlayer>().omegaBlueCooldown--;
            }

            if (player.GetModPlayer<CalamityPlayer>().omegaBlueCooldown > 1500)
            {
                player.GetModPlayer<CalamityPlayer>().omegaBlueHentai = true;

                int d = Dust.NewDust(player.position, player.width, player.height, 20, 0, 0, 100, Color.Transparent, 1.6f);
                Main.dust[d].noGravity = true;
                Main.dust[d].noLight = true;
                Main.dust[d].fadeIn = 1f;
                Main.dust[d].velocity *= 3f;
            }
        }

        public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(mod.ItemType("ReaperTooth"), 11);
            recipe.AddIngredient(mod.ItemType("Lumenite"), 5);
            recipe.AddIngredient(mod.ItemType("Tenebris"), 5);
            recipe.AddIngredient(mod.ItemType("RuinousSoul"), 2);
            recipe.AddTile(TileID.LunarCraftingStation);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
