using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
namespace CalamityMod.Items.Weapons.Rogue
{
    public class BouncingBetty : RogueWeapon
    {
        public const int BaseDamage = 52;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bouncing Betty");
            Tooltip.SetDefault("Throws a grenade which bounces after exploding\n" +
                               "Stealth strikes explode into a violent blast of fire and shrapnel when it bounces");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SafeSetDefaults()
        {
            Item.damage = BaseDamage;
            Item.noMelee = true;
            Item.consumable = true;
            Item.noUseGraphic = true;
            Item.useAnimation = 24;
            Item.useTime = 24;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 3f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.width = 16;
            Item.height = 22;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 0, 4, 0);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ModContent.ProjectileType<BouncingBettyProjectile>();
            Item.shootSpeed = 16f;
            Item.Calamity().rogue = true;
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {

            int projectileIndex = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (player.Calamity().StealthStrikeAvailable() && projectileIndex.WithinBounds(Main.maxProjectiles))
            {
                Main.projectile[projectileIndex].Calamity().stealthStrike = true;
            }
            return false;
        }
    }
}
