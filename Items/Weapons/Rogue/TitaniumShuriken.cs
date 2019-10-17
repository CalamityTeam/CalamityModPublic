using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class TitaniumShuriken : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titanium Shuriken");
        }

        public override void SafeSetDefaults()
        {
            item.width = 38;
            item.damage = 37;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 9;
            item.crit = 10;
            item.useStyle = 1;
            item.useTime = 9;
            item.knockBack = 3f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 38;
            item.maxStack = 999;
            item.value = 2000;
            item.rare = 4;
            item.shoot = ModContent.ProjectileType<Projectiles.TitaniumShurikenProjectile>();
            item.shootSpeed = 16f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.TitaniumBar);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 30);
            recipe.AddRecipe();
        }
    }
}
