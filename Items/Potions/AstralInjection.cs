using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class AstralInjection : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 20;
            DisplayName.SetDefault("Astral Injection");
            Tooltip.SetDefault("Gives mana sickness and hurts you when used, but you regenerate mana extremely quickly even while moving or casting spells");
            SacrificeTotal = 30;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Lime;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<AstralInjectionBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(5f);
            Item.value = Item.buyPrice(0, 2, 0, 0);
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
                player.KillMe(PlayerDeathReason.ByCustomReason(player.name + "'s blood vessels burst from drug overdose."), 1000.0, 0, false);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe(15).
                AddIngredient(ItemID.BottledWater, 15).
                AddIngredient<Stardust>(4).
                AddIngredient<AureusCell>().
                AddTile(TileID.AlchemyTable).
				AddConsumeItemCallback(Recipe.ConsumptionRules.Alchemy).
                Register();

            CreateRecipe(15).
                AddIngredient(ItemID.BottledWater, 15).
                AddIngredient<BloodOrb>(5).
                AddIngredient<AureusCell>().
                AddTile(TileID.AlchemyTable).
                Register();
        }
    }
}
