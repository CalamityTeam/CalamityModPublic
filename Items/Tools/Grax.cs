using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Tools
{
    public class Grax : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grax");
            Tooltip.SetDefault("Hitting an enemy will greatly boost your defense and melee stats for a short time");
        }

        public override void SetDefaults()
        {
            item.width = 62;
            item.height = 62;
			item.scale = 1.5f;
            item.damage = 500;
            item.melee = true;
            item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 4;
            item.useTurn = true;
            item.axe = 50;
            item.hammer = 200;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.tileBoost += 5;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<InfernaCutter>());
            recipe.AddIngredient(ModContent.ItemType<DraedonBar>(), 5);
            recipe.AddIngredient(ModContent.ItemType<UeliaceBar>(), 5);
            recipe.AddRecipeGroup("LunarHamaxe");
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
