using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using static Terraria.ModLoader.ModContent;
using CalamityMod.Cooldowns;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class M1Garand : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";
        public override void SetDefaults()
        {
            Item.width = 102;
            Item.height = 22;
            Item.damage = 75;
            Item.DamageType = DamageClass.Ranged;
            Item.crit = 10;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.autoReuse = false; // Because holdout
            Item.channel = true; // Because holdout
            Item.shoot = ModContent.ProjectileType<M1GarandHoldout>();
            Item.shootSpeed = 12f;
            Item.useAmmo = AmmoID.Bullet;
            Item.noUseGraphic = true;

            Item.value = Item.buyPrice(gold: 20); // Sold by Arms Dealer
            Item.rare = ItemRarityID.Orange;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        public override void HoldItem(Player player)
        {
            if (player.Calamity().cooldowns.TryGetValue(M1GarandShots.ID, out var cooldown))
            {
                cooldown.timeLeft = 8 - player.Calamity().garandShots;
            }
            else
            {
                player.AddCooldown(M1GarandShots.ID, 8);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile holdout = Projectile.NewProjectileDirect(source, player.MountedCenter, Vector2.Zero, ProjectileType<M1GarandHoldout>(), damage, knockback, player.whoAmI);

            // Seting its velocity like this is what aims to the mouse
            holdout.velocity = (player.Calamity().mouseWorld - player.MountedCenter).SafeNormalize(Vector2.Zero);

            return false;
        }
    }
}
