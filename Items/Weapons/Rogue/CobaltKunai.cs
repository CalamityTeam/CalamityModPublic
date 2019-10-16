using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class CobaltKunai : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cobalt Kunai");
        }

        public override void SafeSetDefaults()
        {
            item.width = 18;
            item.damage = 36;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 12;
            item.useStyle = 1;
            item.useTime = 12;
            item.knockBack = 2.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 40;
            item.maxStack = 999;
            item.value = 900;
            item.rare = 4;
            item.shoot = ModContent.ProjectileType<CobaltKunaiProjectile>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CobaltBar);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this, 30);
            recipe.AddRecipe();
        }
    }
}
