using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

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
        }

        public override void SetDefaults()
        {
            item.damage = 444;
            item.magic = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.mana = 10;
            item.width = 66;
            item.height = 82;
            item.useTime = 27;
            item.useAnimation = 27;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5f;
            item.shootSpeed = 9f;
            item.shoot = ModContent.ProjectileType<RancorHoldout>();

            item.value = CalamityGlobalItem.Rarity15BuyPrice;
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        // This weapon uses a holdout projectile.
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, item.shoot, damage, knockBack, player.whoAmI);
            return false;
        }
    }
}
