using Terraria.DataStructures;
using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

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
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = Item.height = 40;
            Item.damage = 2705;
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.useAnimation = Item.useTime = 40;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 9f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityPureGreenBuyPrice;
            Item.shoot = ModContent.ProjectileType<PhosphorescentGauntletPunches>();
            Item.shootSpeed = 1f;
            Item.Calamity().customRarity = CalamityRarity.PureGreen;
            Item.Calamity().trueMelee = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults.
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 10;

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }
    }
}
