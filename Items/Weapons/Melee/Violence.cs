using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Melee.Spears;
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
            item.height = item.width = 142;
            item.damage = 1450;
            item.melee = true;
            item.noMelee = true;
            item.useTurn = true;
            item.noUseGraphic = true;
            item.useAnimation = 25;
            item.useTime = 25;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 9f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.channel = true;
            item.shoot = ModContent.ProjectileType<ViolenceThrownProjectile>();
            item.shootSpeed = 15f;
        }

        public override bool CanUseItem(Player player) => player.altFunctionUse == 2 || player.ownedProjectileCounts[item.shoot] <= 0;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 shootVelocity = new Vector2(speedX, speedY);
            Projectile.NewProjectile(position, shootVelocity, type, damage, knockBack, player.whoAmI, 0f, shootVelocity.ToRotation());
            return false;
        }
    }
}
