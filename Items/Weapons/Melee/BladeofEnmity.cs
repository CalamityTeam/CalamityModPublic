using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Buffs.DamageOverTime;

namespace CalamityMod.Items.Weapons.Melee
{
    public class BladeofEnmity : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blade of Enmity");
        }

        public override void SetDefaults()
        {
            item.width = 60;
            item.damage = 158;
            item.melee = true;
            item.useAnimation = 10;
            item.useStyle = 1;
            item.useTime = 10;
            item.useTurn = true;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 68;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "BarofLife", 5);
            recipe.AddIngredient(null, "CoreofCalamity", 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 600);
        }
    }
}
