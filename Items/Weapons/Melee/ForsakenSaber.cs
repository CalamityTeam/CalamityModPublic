using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class ForsakenSaber : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Forsaken Saber");
            Tooltip.SetDefault("Shoots three sand blades that alter their velocity as they travel");
        }

        public override void SetDefaults()
        {
            item.width = 46;
            item.damage = 65;
            item.melee = true;
            item.useAnimation = 18;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 18;
            item.useTurn = true;
            item.knockBack = 6;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 56;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<SandBlade>();
            item.shootSpeed = 15f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			for (int projectiles = 0; projectiles < 3; projectiles++)
			{
				float SpeedX = speedX + Main.rand.Next(-40, 41) * 0.05f;
				float SpeedY = speedY + Main.rand.Next(-40, 41) * 0.05f;
				Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, (int)(damage * 0.8), knockBack, player.whoAmI);
			}
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AnyAdamantiteBar", 5);
            recipe.AddIngredient(ItemID.AncientBattleArmorMaterial, 2);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 159);
            }
        }
    }
}
