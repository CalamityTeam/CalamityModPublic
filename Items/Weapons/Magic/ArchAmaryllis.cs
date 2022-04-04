using CalamityMod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Magic
{
    public class ArchAmaryllis : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arch Amaryllis");
            Tooltip.SetDefault("Casts a beaming flower that explodes into homing petals");
            Item.staff[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.damage = 75;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.width = 66;
            Item.height = 68;
            Item.useTime = 23;
            Item.useAnimation = 23;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 7.5f;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item109;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<BeamingBolt>();
            Item.shootSpeed = 20f;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(15, 15);

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<GleamingMagnolia>()).AddIngredient(ItemID.FragmentNebula, 10).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
