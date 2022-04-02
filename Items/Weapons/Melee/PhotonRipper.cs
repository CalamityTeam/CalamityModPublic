using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class PhotonRipper : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Photon Ripper");
            Tooltip.SetDefault("Projects a directed stream of hardlight teeth at ultra high velocity\n" +
                "This weapon and its projectiles function as a chainsaw");
        }

        public override void SetDefaults()
        {
            item.damage = 3725;
            item.knockBack = 12f;
            item.useTime = 5;
            item.useAnimation = 25;
            // In-game, the displayed axe power is 5x the value set here.
            // This corrects for trees having 500% hardness internally.
            // So that the axe power in the code looks like the axe power you see on screen, divide by 5.
            item.axe = 3000 / 5;
            // Photon Ripper's axe power is entirely for show. Its projectiles instantly one shot trees.

            item.height = 134;
            item.width = 54;
            item.melee = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.autoReuse = false;
            item.shoot = ModContent.ProjectileType<PhotonRipperProjectile>();
            item.shootSpeed = 1f;

            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.value = CalamityGlobalItem.RarityVioletBuyPrice;
        }

        public override void GetWeaponCrit(Player player, ref int crit) => crit += 18;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
