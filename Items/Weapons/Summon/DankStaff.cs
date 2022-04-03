using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class DankStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dank Staff");
            Tooltip.SetDefault("Summons a dank creeper to fight for you");
        }

        public override void SetDefaults()
        {
            Item.damage = 7;
            Item.mana = 10;
            Item.width = 58;
            Item.height = 58;
            Item.useTime = Item.useAnimation = 30;
            Item.scale = 0.85f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2.25f;
            Item.value = Item.buyPrice(0, 4, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DankCreeperMinion>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.RottenChunk, 3).AddIngredient(ItemID.DemoniteBar, 8).AddIngredient(ModContent.ItemType<TrueShadowScale>(), 7).AddTile(TileID.DemonAltar).Register();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld;
                speedX = 0;
                speedY = 0;
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}
