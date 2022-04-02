using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Zapper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lazinator");
            Tooltip.SetDefault("Zap");
        }

        public override void SetDefaults()
        {
            item.damage = 55;
            item.magic = true;
            item.mana = 4;
            item.width = 46;
            item.height = 22;
            item.useTime = 7;
            item.useAnimation = 7;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 48, 0, 0);
            item.rare = ItemRarityID.LightPurple;
            item.UseSound = SoundID.Item12;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurpleLaser;
            item.shootSpeed = 20f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (Main.rand.NextBool(2))
            {
                type = ProjectileID.GreenLaser;
            }
            int laser = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
            Main.projectile[laser].usesLocalNPCImmunity = true;
            Main.projectile[laser].localNPCHitCooldown = 10;
            return false;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.SpaceGun);
            recipe.AddIngredient(ItemID.LaserRifle);
            recipe.AddIngredient(ModContent.ItemType<VictoryShard>(), 5);
            recipe.AddIngredient(ItemID.SoulofSight);
            recipe.AddIngredient(ItemID.SoulofMight);
            recipe.AddIngredient(ItemID.SoulofFright);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
