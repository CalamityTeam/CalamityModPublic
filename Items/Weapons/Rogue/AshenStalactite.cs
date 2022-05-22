using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class AshenStalactite : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ashen Stalactite");
            Tooltip.SetDefault("Throws a fast, small stalactite that crumbles to dust after travelling a short distance\n" +
                "Stealth strikes cause a larger, more damaging stalagmite to be thrown which travels slower and further before crumbling to damaging dust");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 34;
            Item.damage = 37;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 20;
            Item.knockBack = 1f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Orange;
            Item.shoot = ModContent.ProjectileType<AshenStalactiteProj>();
            Item.shootSpeed = 15f;
            Item.Calamity().rogue = true;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.Calamity().StealthStrikeAvailable())
            {
                int stealthType = ModContent.ProjectileType<AshenStalagmiteProj>();
                float stealthSpeedMult = 0.6f;
                float stealthDamageMult = 1.15f;
                float stealthKnockbackMult = 2.5f;
                int p = Projectile.NewProjectile(source, position.X, position.Y, velocity.X * stealthSpeedMult, velocity.Y * stealthSpeedMult, stealthType, (int)(damage * stealthDamageMult), (int)(knockback * stealthKnockbackMult), player.whoAmI, 0f, 1f);
                if (p.WithinBounds(Main.maxProjectiles))
                    Main.projectile[p].Calamity().stealthStrike = true;
                return false;
            }
            return true;
        }
    }
}
