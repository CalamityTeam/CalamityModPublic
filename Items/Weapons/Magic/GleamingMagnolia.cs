using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class GleamingMagnolia : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gleaming Magnolia");
            Tooltip.SetDefault("Casts a gleaming flower that explodes into petals");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 53;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 11;
            Item.width = 52;
            Item.height = 54;
            Item.useTime = 27;
            Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.5f;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item109;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<GleamingBolt>();
            Item.shootSpeed = 14f;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(15, 15);

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ManaRose>().
                AddIngredient(ItemID.HallowedBar, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
