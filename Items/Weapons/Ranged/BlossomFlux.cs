using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class BlossomFlux : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blossom Flux");
            Tooltip.SetDefault("Fires a stream of leaves\n" +
                "Right click to fire a spore orb that explodes into a cloud of spore gas");
        }

        public override void SetDefaults()
        {
            item.damage = 20;
            item.ranged = true;
            item.width = 38;
            item.height = 68;
            item.useTime = 4;
            item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 0.15f;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<LeafArrow>();
            item.shootSpeed = 10f;
            item.useAmmo = AmmoID.Arrow;

            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = ItemRarityID.Lime;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.useTime = 25;
                item.useAnimation = 25;
                item.UseSound = SoundID.Item77;
            }
            else
            {
                item.useTime = 2;
                item.useAnimation = 16;
                item.UseSound = SoundID.Item5;
            }
            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<SporeBomb>(), (int)(damage * 4D), knockBack * 60f, player.whoAmI);
            }
            else
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<LeafArrow>(), damage, knockBack, player.whoAmI);
            }
            return false;
        }
    }
}
