using Terraria.DataStructures;
using Terraria.DataStructures;
using Terraria.DataStructures;
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
            SacrificeTotal = 1;
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

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 18;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
