using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class MeteorFist : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Meteor Fist");
            Tooltip.SetDefault("Fires a fist that explodes");
        }

        public override void SafeSetDefaults()
        {
            item.width = 22;
            item.damage = 15;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 30;
            item.useStyle = 5;
            item.useTime = 30;
            item.knockBack = 5.75f;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.height = 28;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.shoot = ModContent.ProjectileType<Projectiles.MeteorFist>();
            item.shootSpeed = 10f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.MeteoriteBar, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
