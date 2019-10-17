using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class AdamantiteThrowingAxe : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Adamantite Throwing Axe");
        }

        public override void SafeSetDefaults()
        {
            item.width = 26;
            item.damage = 44;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 12;
            item.useStyle = 1;
            item.useTime = 12;
            item.knockBack = 3.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 30;
            item.maxStack = 999;
            item.value = 1600;
            item.rare = 4;
            item.shoot = ModContent.ProjectileType<AdamantiteThrowingAxeProjectile>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.AdamantiteBar);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 25);
            recipe.AddRecipe();
        }
    }
}
