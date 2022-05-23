using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class PolypLauncher : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Polyp Launcher");
            Tooltip.SetDefault("Summons a polyp bunch that launches coral chunks at nearby enemies");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 12;
            Item.mana = 10;
            Item.width = 10;
            Item.height = 32;
            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 0f;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = 2;
            Item.UseSound = SoundID.Item44;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<PolypLauncherSentry>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //CalamityUtils.OnlyOneSentry(player, type);
            int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 20f);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            player.UpdateMaxTurrets();
            return false;
        }
    }
}
