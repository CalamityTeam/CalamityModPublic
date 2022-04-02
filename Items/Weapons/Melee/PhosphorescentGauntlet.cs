using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class PhosphorescentGauntlet : ModItem
    {
        public const int OnHitIFrames = 15;
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Phosphorescent Gauntlet");
            Tooltip.SetDefault("Releases rapid sulphurous punches\n" +
                "On use, you lunge in the direction of the mouse\n" +
                "On collision, you are knocked back");
        }

        public override void SetDefaults()
        {
            item.width = item.height = 40;
            item.damage = 2705;
            item.melee = true;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.channel = true;
            item.useAnimation = item.useTime = 40;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 9f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            item.shoot = ModContent.ProjectileType<PhosphorescentGauntletPunches>();
            item.shootSpeed = 1f;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.Calamity().trueMelee = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults.
        public override void GetWeaponCrit(Player player, ref int crit) => crit += 10;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, new Vector2(speedX, speedY), type, damage, knockBack, player.whoAmI);
            return false;
        }
    }
}
