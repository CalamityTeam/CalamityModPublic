using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class CryogenicStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryogenic Staff");
            Tooltip.SetDefault(@"Summons an animated ice construct to protect you
Fire rate and range increase the longer it targets an enemy");
        }

        public override void SetDefaults()
        {
            item.damage = 50;
            item.mana = 10;
            item.summon = true;
            item.sentry = true;
            item.width = 82;
            item.height = 84;
            item.useTime = item.useAnimation = 25;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.autoReuse = true;
            item.knockBack = 4f;

            item.value = CalamityGlobalItem.Rarity6BuyPrice;
            item.rare = ItemRarityID.LightPurple;
            item.Calamity().devItem = true;

            item.UseSound = SoundID.Item78;
            item.shoot = ModContent.ProjectileType<IceSentry>();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            player.UpdateMaxTurrets();
            return false;
        }
    }
}
