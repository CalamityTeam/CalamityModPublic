using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.Projectiles.Rogue;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class SlickCane : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slick Cane");
            Tooltip.SetDefault("Swipes a cane that steals money from enemies.\n" +
                               "Stealth strike effect: 1 in 15 chance for hit enemies to drop 1-3 gold coins");
        }

        public override void SafeSetDefaults()
        {
            item.width = 42;
            item.height = 36;
            item.damage = 27;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.melee = true;
            item.useAnimation = 26;
            item.useTime = 26;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5f;
            item.UseSound = SoundID.DD2_GhastlyGlaivePierce;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.rare = 4;
            item.shoot = ModContent.ProjectileType<SlickCaneProjectile>();
            item.shootSpeed = 42f;
        }

        public override bool CanUseItem(Player player)
        {
            for (int i = 0; i < 1000; ++i)
            {
                if (Main.projectile[i].active && Main.projectile[i].owner == Main.myPlayer && Main.projectile[i].type == item.shoot)
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float ai0 = Main.rand.NextFloat() * item.shootSpeed * 0.75f * (float)player.direction;
            int projectileIndex = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI, ai0, 0f);
            Main.projectile[projectileIndex].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }
}
