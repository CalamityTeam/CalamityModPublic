using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
    public class Grax : ModItem
    {
        private const int HammerPower = 110;
        private const int AxePower = 180 / 5;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grax");
            Tooltip.SetDefault("Hitting an enemy will greatly boost your defense, melee damage and melee crit for a short time\n" +
                "Right click to use without hammering down walls or chopping down trees");
        }

        public override void SetDefaults()
        {
            item.damage = 472;
            item.knockBack = 8f;
            item.useTime = 4;
            item.useAnimation = 16;
            item.hammer = HammerPower;
            item.axe = AxePower;
            item.tileBoost += 5;

            item.width = 62;
            item.height = 62;
            item.scale = 1.5f;
            item.melee = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.axe = 0;
                item.hammer = 0;
            }
            else
            {
                item.axe = AxePower;
                item.hammer = HammerPower;
            }
            return base.CanUseItem(player);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<InfernaCutter>());
            recipe.AddRecipeGroup("LunarHamaxe");
            recipe.AddIngredient(ModContent.ItemType<MolluskHusk>(), 10);
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 5);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            player.AddBuff(ModContent.BuffType<GraxDefense>(), 600);
        }
    }
}
