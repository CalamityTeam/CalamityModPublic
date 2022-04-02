using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class LatcherMine : RogueWeapon
    {
        public const int BaseDamage = 80;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Latcher Mine");
            Tooltip.SetDefault("Sticks to enemies on hit and detonates after 3 seconds.\n" +
                               "Breaks upon hitting blocks\n" +
                               "Stealth Strike Effect: On explosion, fire and shrapnel are released\n" +
                               "Stealth strike mines can stick to the ground and last much longer when doing so");
        }

        public override void SafeSetDefaults()
        {
            item.height = 32;
            item.width = 26;
            item.damage = BaseDamage;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = item.useTime = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 6f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 0, 3, 0);
            item.rare = ItemRarityID.Pink;
            item.shoot = ModContent.ProjectileType<LatcherMineProjectile>();
            item.shootSpeed = 10f;
            item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int stealth = Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            if (stealth.WithinBounds(Main.maxProjectiles))
                Main.projectile[stealth].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }
}
