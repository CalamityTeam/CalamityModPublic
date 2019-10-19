using Terraria;
using Terraria.ModLoader; using CalamityMod.Items.Materials;
using Terraria.ID;
using CalamityMod.Projectiles.Magic;

namespace CalamityMod.Items.Weapons.Magic
{
    public class NightsRay : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Night's Ray");
            Tooltip.SetDefault("Fires a dark ray that splits if enemies are near it");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 24;
            item.magic = true;
            item.mana = 10;
            item.width = 50;
            item.height = 50;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 3.25f;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.rare = 4;
            item.UseSound = SoundID.Item72;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<NightsRayBeam>();
            item.shootSpeed = 6f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.WandofSparking);
            recipe.AddIngredient(ItemID.Vilethorn);
            recipe.AddIngredient(ItemID.AmberStaff);
            recipe.AddIngredient(ItemID.MagicMissile);
            recipe.AddIngredient(ModContent.ItemType<TrueShadowScale>(), 15);
            recipe.AddIngredient(ModContent.ItemType<PurifiedGel>(), 10);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
