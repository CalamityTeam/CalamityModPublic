using Terraria.DataStructures;
using CalamityMod.Projectiles.Rogue;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class ToxicantTwister : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Toxicant Twister");
            Tooltip.SetDefault("Throws a slow moving boomerang\n" +
                "After a few moments, the boomerang chooses a target and rapidly homes in\n" +
                "Stealth strikes home in faster and rapidly release sand");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 46;
            Item.damage = 323;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.useAnimation = Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ToxicantTwisterTwoPointZero>();
            Item.shootSpeed = 18f;
            Item.DamageType = RogueDamageClass.Instance;

            Item.value = CalamityGlobalItem.Rarity13BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int boomer = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            if (boomer.WithinBounds(Main.maxProjectiles))
                Main.projectile[boomer].Calamity().stealthStrike = player.Calamity().StealthStrikeAvailable();
            return false;
        }
    }
}
