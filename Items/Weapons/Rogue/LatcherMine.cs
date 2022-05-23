using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class LatcherMine : ModItem
    {
        public const int BaseDamage = 80;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Latcher Mine");
            Tooltip.SetDefault("Sticks to enemies on hit and detonates after 3 seconds.\n" +
                               "Breaks upon hitting blocks\n" +
                               "Stealth Strike Effect: On explosion, fire and shrapnel are released\n" +
                               "Stealth strike mines can stick to the ground and last much longer when doing so");
            SacrificeTotal = 99;
        }

        public override void SetDefaults()
        {
            Item.height = 32;
            Item.width = 26;
            Item.damage = BaseDamage;
            Item.noMelee = true;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 6f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 0, 3, 0);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<LatcherMineProjectile>();
            Item.shootSpeed = 10f;
            Item.DamageType = RogueDamageClass.Instance;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int stealth = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (stealth.WithinBounds(Main.maxProjectiles))
                Main.projectile[stealth].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }
}
