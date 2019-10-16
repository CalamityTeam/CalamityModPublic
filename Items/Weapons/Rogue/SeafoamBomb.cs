using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class SeafoamBomb : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Seafoam Bomb");
            Tooltip.SetDefault("Throws a bomb that explodes into a bubble which deals extra damage to enemies");
        }

        public override void SafeSetDefaults()
        {
            item.width = 26;
            item.height = 44;
            item.damage = 16;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 22;
            item.useStyle = 1;
            item.useTime = 22;
            item.knockBack = 8f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.shoot = ModContent.ProjectileType<Projectiles.SeafoamBomb>();
            item.shootSpeed = 8f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Bomb, 25);
            recipe.AddIngredient(null, "SeaPrism", 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
