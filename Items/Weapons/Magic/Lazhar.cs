using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Lazhar : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lazhar");
            Tooltip.SetDefault("Fires a bouncing laser that explodes on enemy hits");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 100;
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
            Item.shoot = ModContent.ProjectileType<SolarBeam2>();
        }

        public override Vector2? HoldoutOffset() => new Vector2(-5, 0);

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{
            velocity.X += (float)Main.rand.Next(-15, 16) * 0.05f;
            velocity.Y += (float)Main.rand.Next(-15, 16) * 0.05f;
		}

        public override void AddRecipes()
        {
            CreateRecipe(1).
                AddIngredient(ItemID.HeatRay).
                AddIngredient<Lazinator>().
                AddIngredient(ItemID.FragmentSolar, 10).
                AddIngredient(ItemID.ChlorophyteBar, 6).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
