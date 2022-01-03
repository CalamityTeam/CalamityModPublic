using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Magic
{
    public class CrystalFlareStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Flare Staff");
            Tooltip.SetDefault("Fires blue frost flames that explode");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 50;
            item.magic = true;
            item.mana = 15;
            item.width = 44;
            item.height = 46;
            item.useTime = 10;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5.25f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.UseSound = SoundID.Item20;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<SpiritFlameCurse>();
            item.shootSpeed = 14f;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FrostStaff);
            recipe.AddIngredient(ItemID.FrostCore);
            recipe.AddIngredient(ModContent.ItemType<CoreofEleum>(), 3);
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 8);
            recipe.AddIngredient(ModContent.ItemType<SeaPrism>(), 10);
            recipe.AddTile(TileID.IceMachine);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
