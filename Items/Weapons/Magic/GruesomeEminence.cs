using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Magic
{
    public class GruesomeEminence : ModItem
    {
        public const string PoeticTooltipLine = "The spirits of the amalgam could never pass on to their desired afterlife,\n" +
            "Tainted and melded by rage as they were.";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gruesome Eminence");
            Tooltip.SetDefault("Summons spirits near the cursor\n" +
                "At first, the spirits will fly wildly. This can hurt enemies and players\n" +
                "However, over time they will begin to accumulate to create a single, controllable monstrosity\n" +
               CalamityUtils.ColorMessage(PoeticTooltipLine, CalamityGlobalItem.ExhumedTooltipColor));
        }

        public override void SetDefaults()
        {
            item.damage = 888;
            item.magic = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.mana = 8;
            item.width = 32;
            item.height = 36;
            item.useTime = 27;
            item.useAnimation = 27;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5f;
            item.shootSpeed = 9f;
            item.shoot = ModContent.ProjectileType<GruesomeEminenceHoldout>();

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
