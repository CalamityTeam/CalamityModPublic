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
            Item.damage = 3725;
            Item.knockBack = 12f;
            Item.useTime = 5;
            Item.useAnimation = 25;
            // In-game, the displayed axe power is 5x the value set here.
            // This corrects for trees having 500% hardness internally.
            // So that the axe power in the code looks like the axe power you see on screen, divide by 5.
            Item.axe = 3000 / 5;
            // Photon Ripper's axe power is entirely for show. Its projectiles instantly one shot trees.

            Item.height = 134;
            Item.width = 54;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
            Item.shoot = ModContent.ProjectileType<PhotonRipperProjectile>();
            Item.shootSpeed = 1f;

            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
        }

        public override void GetWeaponCrit(Player player, ref int crit) => crit += 18;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
