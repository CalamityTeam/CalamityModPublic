using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class AstralInjection : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Potions";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 30;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 9999;
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
                player.KillMe(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.AstralInjection" + Main.rand.Next(1, 2 + 1)).Format(player.name)), 1000.0, 0, false);
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
                Register()
                .DisableDecraft();
        }
    }
}
