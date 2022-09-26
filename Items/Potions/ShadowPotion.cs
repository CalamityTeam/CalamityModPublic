using CalamityMod.Buffs.Potions;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Potions
{
    public class ShadowPotion : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 20;
            DisplayName.SetDefault("Shadow Potion");
            Tooltip.SetDefault("Turns the player into a shadow with glowing eyes\n" +
			"Rogue weapons spawn projectiles on hit\n" +
            "Stealth generation is increased by 8%\n" +
			"Visual effects can be disabled with the Stealth Invisibility config");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useTurn = true;
            Item.maxStack = 30;
            Item.rare = ItemRarityID.Orange;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.UseSound = SoundID.Item3;
            Item.consumable = true;
            Item.buffType = ModContent.BuffType<ShadowBuff>();
            Item.buffTime = CalamityUtils.SecondsToFrames(480f);
            Item.value = Item.buyPrice(0, 2, 0, 0);
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).
                AddIngredient(ItemID.InvisibilityPotion).
                AddIngredient<Shadowfish>().
                AddTile(TileID.Bottles).
                Register();

            CreateRecipe().
                AddIngredient(ItemID.BottledWater).
                AddIngredient<BloodOrb>(20).
                AddTile(TileID.AlchemyTable).
                Register();
        }
    }
}
