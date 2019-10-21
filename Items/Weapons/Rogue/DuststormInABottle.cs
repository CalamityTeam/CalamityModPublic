using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Rogue;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class DuststormInABottle : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Duststorm in a Bottle");
            Tooltip.SetDefault("Explodes into a dust cloud");
        }

        public override void SafeSetDefaults()
        {
            item.width = 20;
            item.damage = 47;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 25;
            item.useStyle = 1;
            item.useTime = 25;
            item.knockBack = 5f;
            item.UseSound = SoundID.Item106;
            item.autoReuse = true;
            item.height = 24;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.shoot = ModContent.ProjectileType<DuststormInABottleProj>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.HolyWater, 20);
            recipe.AddIngredient(ModContent.ItemType<GrandScale>());
            recipe.AddIngredient(ItemID.SandstorminaBottle);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
