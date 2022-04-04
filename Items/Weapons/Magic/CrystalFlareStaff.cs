using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class CrystalFlareStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crystal Flare Staff");
            Tooltip.SetDefault("Fires blue frost flames that explode");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 50;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 15;
            Item.width = 44;
            Item.height = 46;
            Item.useTime = 10;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.25f;
            Item.value = Item.buyPrice(0, 60, 0, 0);
            Item.rare = ItemRarityID.Lime;
            Item.UseSound = SoundID.Item20;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<SpiritFlameCurse>();
            Item.shootSpeed = 14f;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(15, 15);

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.FrostStaff).AddIngredient(ItemID.FrostCore).AddIngredient(ModContent.ItemType<CoreofEleum>(), 3).AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 8).AddIngredient(ModContent.ItemType<SeaPrism>(), 10).AddTile(TileID.IceMachine).Register();
        }
    }
}
