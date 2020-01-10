using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class LanternoftheSoul : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lantern of the Soul");
            Tooltip.SetDefault("Summons a lantern turret to fight for you");
        }

        public override void SetDefaults()
        {
            item.damage = 180;
            item.summon = true;
			item.sentry = true;
            item.mana = 10;
            item.width = 42;
            item.height = 60;
            item.useTime = 36;
            item.useAnimation = 36;
            item.useStyle = 1;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<LanternSoul>();
            item.Calamity().postMoonLordRarity = 22;
            item.UseSound = SoundID.Item44;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            position = Main.MouseWorld;
            speedX = 0;
            speedY = 0;
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            player.UpdateMaxTurrets();
            return false;
        }
    }
}
