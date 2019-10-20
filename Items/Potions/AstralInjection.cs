using Terraria;
using Terraria.ModLoader; using CalamityMod.Items.Materials;
using Terraria.DataStructures;
using Terraria.ID;
using CalamityMod.Buffs.Potions;

namespace CalamityMod.Items.Potions
{
    public class AstralInjection : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Injection");
            Tooltip.SetDefault("Gives mana sickness and hurts you when used, but you regenerate mana extremely quickly even while moving or casting spells");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 18;
            item.useTurn = true;
            item.maxStack = 999;
            item.rare = 3;
            item.useAnimation = 17;
            item.useTime = 17;
            item.useStyle = 2;
            item.UseSound = SoundID.Item3;
            item.consumable = true;
            item.buffType = ModContent.BuffType<AstralInjectionBuff>();
            item.buffTime = 180;
            item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void OnConsumeItem(Player player)
        {
            player.AddBuff(BuffID.ManaSickness, Player.manaSickTime / 2, true);
            player.statLife -= 5;
            if (Main.myPlayer == player.whoAmI)
            {
                player.HealEffect(-5, true);
            }
            if (player.statLife <= 0)
            {
                player.KillMe(PlayerDeathReason.ByOther(10), 1000.0, 0, false);
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater, 4);
            recipe.AddIngredient(ModContent.ItemType<Stardust>(), 4);
            recipe.AddIngredient(ModContent.ItemType<AstralJelly>());
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this, 4);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.BottledWater, 4);
            recipe.AddIngredient(ModContent.ItemType<BloodOrb>(), 5);
            recipe.AddIngredient(ModContent.ItemType<AstralJelly>());
            recipe.AddTile(TileID.AlchemyTable);
            recipe.SetResult(this, 4);
            recipe.AddRecipe();
        }
    }
}
