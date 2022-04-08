using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Violence : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Violence");
            Tooltip.SetDefault("Releases a blazing fork which stays near the mouse and shreds enemies");
        }

        public override void SetDefaults()
        {
            Item.height = Item.width = 142;
            Item.damage = 404;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 25;
            Item.useTime = 25;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 9f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.channel = true;
            Item.shoot = ModContent.ProjectileType<ViolenceThrownProjectile>();
            Item.shootSpeed = 15f;
        }

        public override bool CanUseItem(Player player) => player.altFunctionUse == 2 || player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, velocity.ToRotation());
            return false;
        }
    }
}
