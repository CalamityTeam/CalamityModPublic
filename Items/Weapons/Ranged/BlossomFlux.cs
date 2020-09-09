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
            Tooltip.SetDefault("Legendary Drop\n" +
                "Fires a stream of leaves\n" +
                "Right click to fire a spore orb that explodes into a cloud of spore gas\n" +
                "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.damage = 22;
            item.ranged = true;
            item.width = 40;
            item.height = 62;
            item.useTime = 4;
            item.useAnimation = 16;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 0.15f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
            item.UseSound = SoundID.Item5;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<LeafArrow>();
            item.shootSpeed = 10f;
            item.useAmmo = AmmoID.Arrow;
            item.Calamity().customRarity = CalamityRarity.ItemSpecific;
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
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<SporeBomb>(), (int)((double)damage * 4.0), knockBack * 60f, player.whoAmI, 0.0f, 0.0f);
            }
            else
            {
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<LeafArrow>(), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
            }
            return false;
        }
    }
}
