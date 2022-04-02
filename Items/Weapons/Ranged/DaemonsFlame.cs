using CalamityMod.Projectiles.Ranged;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class DaemonsFlame : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daemon's Flame");
            Tooltip.SetDefault("Shoots daemon flame fireballs as well as 4 regular arrows");
        }

        public override void SetDefaults()
        {
            item.damage = 135;
            item.width = 62;
            item.height = 128;
            item.useTime = 12;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item5;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.ranged = true;
            item.channel = true;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<DaemonsFlameBow>();
            item.shootSpeed = 20f;
            item.useAmmo = AmmoID.Arrow;

            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.Calamity().canFirePointBlankShots = true;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.GetTexture("CalamityMod/Items/Weapons/Ranged/DaemonsFlameGlow"));
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<DaemonsFlameBow>(), damage, knockBack, player.whoAmI);
            return false;
        }
    }
}
