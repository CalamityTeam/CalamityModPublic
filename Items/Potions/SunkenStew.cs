using CalamityMod.Items.Placeables;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class SunkenStew : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hadal Stew");
            Tooltip.SetDefault(@"Restores 150 mana
Only gives 50 (37 with Philosopher's Stone) seconds of Potion Sickness
Grants Well Fed");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 30;
            item.useAnimation = 17;
            item.useTime = 17;
            item.rare = 3;
            item.useStyle = ItemUseStyleID.EatingUsing;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.potion = true;
            item.buffType = BuffID.WellFed;
            item.buffTime = 216000;
            item.healLife = 120;
            item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override bool CanUseItem(Player player)
        {
            return player.FindBuffIndex(BuffID.PotionSickness) == -1;
        }

        public override bool UseItem(Player player)
        {
            player.statLife += 120;
            player.statMana += 150;
            if (player.statLife > player.statLifeMax2)
            {
                player.statLife = player.statLifeMax2;
            }
            if (player.statMana > player.statManaMax2)
            {
                player.statMana = player.statManaMax2;
            }
            player.AddBuff(BuffID.ManaSickness, Player.manaSickTime, true);
            if (Main.myPlayer == player.whoAmI)
            {
                player.HealEffect(120, true);
                player.ManaEffect(150);
            }

            // fixes hardcoded potion sickness duration from quick heal
            player.ClearBuff(BuffID.PotionSickness);
            player.AddBuff(BuffID.PotionSickness, player.pStone ? 2220 : 3000);
            player.AddBuff(BuffID.WellFed, 216000);
            return true;
        }

        // Zeroes out the hardcoded healing function from having a healLife value. The item still heals in the UseItem hook.
        public override void GetHealLife(Player player, bool quickHeal, ref int healValue)
        {
            healValue = 0;
        }

        // Forces the "Restores X life" tooltip to display the actual life restored instead of zero (due to the previous function).
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.Find(line => line.Name == "HealLife").text = "Restores " + item.healLife + " life";
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AbyssGravel>(), 10);
            recipe.AddIngredient(ModContent.ItemType<CoastalDemonfish>());
            recipe.AddIngredient(ItemID.Honeyfin);
            recipe.AddIngredient(ItemID.Bowl, 2);
            recipe.AddTile(TileID.CookingPots);
            recipe.SetResult(this, 2);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Voidstone>(), 10);
            recipe.AddIngredient(ModContent.ItemType<CoastalDemonfish>());
            recipe.AddIngredient(ItemID.Honeyfin);
            recipe.AddIngredient(ItemID.Bowl, 2);
            recipe.AddTile(TileID.CookingPots);
            recipe.SetResult(this, 2);
            recipe.AddRecipe();
        }
    }
}
