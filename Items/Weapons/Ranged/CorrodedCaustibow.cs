using CalamityMod.Projectiles.Ranged;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class CorrodedCaustibow : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Corroded Caustibow");
            Tooltip.SetDefault("Shoots slow, powerful shells that trail an irradiated aura");
        }

        public override void SetDefaults()
        {
            item.damage = 80;
            item.ranged = true;
            item.width = 30;
            item.height = 38;
            item.crit += 20;
            item.useTime = 45;
            item.useAnimation = 45;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 6f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Shell>();
            item.shootSpeed = 5f;
            item.useAmmo = AmmoID.Arrow;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<CorrodedShell>(), damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.AddIngredient(ModContent.ItemType<Shellshooter>());
            r.AddIngredient(ModContent.ItemType<Toxibow>());
            r.AddIngredient(ModContent.ItemType<CorrodedFossil>(), 10);
            r.AddTile(TileID.MythrilAnvil);
            r.SetResult(this);
            r.AddRecipe();
        }
    }
}
