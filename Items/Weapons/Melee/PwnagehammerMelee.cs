using CalamityMod.Projectiles.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class PwnagehammerMelee : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pwnagehammer");
            Tooltip.SetDefault("Throws an extremely fast hammer");
        }

        public override void SetDefaults()
        {
            item.width = 68;
            item.damage = 90;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useStyle = 1;
            item.useTime = 15;
            item.knockBack = 10f;
            item.UseSound = SoundID.Item1;
            item.melee = true;
            item.height = 68;
            item.value = Item.buyPrice(0, 48, 0, 0);
            item.rare = 6;
            item.shoot = ModContent.ProjectileType<PwnagehammerMeleeProj>();
            item.shootSpeed = 12f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Pwnhammer);
            recipe.AddIngredient(ItemID.HallowedBar, 7);
            recipe.AddIngredient(ItemID.SoulofMight, 3);
            recipe.AddIngredient(ItemID.SoulofSight, 3);
            recipe.AddIngredient(ItemID.SoulofFright, 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
