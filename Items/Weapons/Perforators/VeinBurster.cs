using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class VeinBurster : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vein Burster");
            Tooltip.SetDefault("Fires a blood ball that sticks to tiles and explodes");
        }

        public override void SetDefaults()
        {
            item.width = 52;
            item.damage = 22;
            item.melee = true;
            item.useAnimation = 25;
            item.useStyle = 1;
            item.useTime = 25;
            item.useTurn = true;
            item.knockBack = 4.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 50;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
            item.shoot = ModContent.ProjectileType<BloodBall>();
            item.shootSpeed = 5f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Vertebrae, 5);
            recipe.AddIngredient(ItemID.CrimtaneBar, 5);
            recipe.AddIngredient(null, "BloodSample", 15);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
