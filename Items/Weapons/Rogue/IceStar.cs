using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using Terraria;
namespace CalamityMod.Items.Weapons.Rogue
{
    public class IceStar : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ice Star");
            Tooltip.SetDefault("Throws homing ice stars\n" +
                "Ice Stars are too brittle to be recovered after being thrown");
        }

        public override void SafeSetDefaults()
        {
            item.width = 62;
            item.damage = 45;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 12;
            item.crit = 7;
            item.useStyle = 1;
            item.useTime = 12;
            item.knockBack = 2.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 62;
            item.maxStack = 999;
            item.value = Item.sellPrice(0, 0, 1, 20);
            item.rare = 5;
            item.shoot = ModContent.ProjectileType<IceStarProjectile>();
            item.shootSpeed = 14f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<CryoBar>());
            recipe.AddTile(TileID.IceMachine);
            recipe.SetResult(this, 50);
            recipe.AddRecipe();
        }
    }
}
