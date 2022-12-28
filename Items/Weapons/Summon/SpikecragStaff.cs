using Terraria.DataStructures;
using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class SpikecragStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spikecrag Staff");
            Tooltip.SetDefault("Summons a spikecrag to protect you");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 56;
            Item.mana = 10;
            Item.DamageType = DamageClass.Summon;
            Item.sentry = true;
            Item.width = 50;
            Item.height = 50;
            Item.useTime = Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.autoReuse = true;
            Item.shootSpeed = 20f;
            Item.UseSound = SoundID.Item78;
            Item.shoot = ModContent.ProjectileType<Spikecrag>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            //CalamityUtils.OnlyOneSentry(player, type);
            int p = Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 120f, 0f);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].originalDamage = Item.damage;
            player.UpdateMaxTurrets();
            return false;
        }
    }
}
