using CalamityMod.Projectiles.Magic;
using CalamityMod.Rarities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Rancor : ModItem
    {
        public const string PoeticTooltipLine = "Forgiveness can only heal so much,\n" +
            "If the recipient has not yet forgiven themselves.";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rancor");
            Tooltip.SetDefault("Casts a magic circle that charges energy and eventually releases a powerful laser burst of controlled maleficent magic\n" +
                "The laser causes arms and searing lava to appear on surfaces it touches which harm both you and enemies\n" +
                CalamityUtils.ColorMessage(PoeticTooltipLine, CalamityGlobalItem.ExhumedTooltipColor));
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 444;
            Item.DamageType = DamageClass.Magic;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.mana = 10;
            Item.width = 66;
            Item.height = 82;
            Item.useTime = 27;
            Item.useAnimation = 27;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5f;
            Item.shootSpeed = 9f;
            Item.shoot = ModContent.ProjectileType<RancorHoldout>();

            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
        }

        // This weapon uses a holdout projectile.
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
