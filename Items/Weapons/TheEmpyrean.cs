using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class TheEmpyrean : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Empyrean");
            Tooltip.SetDefault("70% chance to not consume gel");
        }

        public override void SetDefaults()
        {
            item.damage = 92;
            item.ranged = true;
            item.width = 70;
            item.height = 24;
            item.useTime = 5;
            item.useAnimation = 15;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 3.5f;
            item.UseSound = SoundID.Item34;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = 9;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<CosmicFire>();
            item.shootSpeed = 7.5f;
            item.useAmmo = 23;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 70)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "MeldiateBar", 12);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
