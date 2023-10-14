using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Lazhar : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Magic";
        public override void SetDefaults()
        {
            Item.damage = 76;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 4;
            Item.width = 42;
            Item.height = 20;
            Item.useTime = 7;
            Item.useAnimation = 7;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.value = CalamityGlobalItem.Rarity10BuyPrice;
            Item.rare = ItemRarityID.Red;
            Item.UseSound = SoundID.Item12;
            Item.autoReuse = true;
            Item.shootSpeed = 15f;
            Item.shoot = ModContent.ProjectileType<LazharSolarBeam>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            velocity.X += (float)Main.rand.Next(-15, 16) * 0.05f;
            velocity.Y += (float)Main.rand.Next(-15, 16) * 0.05f;
		}

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.SpaceGun).
                AddIngredient(ItemID.HeatRay).
                AddIngredient(ItemID.FragmentSolar, 6).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
