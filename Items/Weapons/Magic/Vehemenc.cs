using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
    public class Vehemenc : ModItem
    {
        public const int BaseDamage = 5185;
        public const float SkullRatio = 0.08f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vehemence");
            Tooltip.SetDefault("Casts intense bolts of hellish power that explode into skulls\n" +
                "Direct hits inflict Demon Flames for an extended period of time");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = BaseDamage;
            item.magic = true;
            item.mana = 41;
            item.width = 44;
            item.height = 44;
            item.useTime = item.useAnimation = 43;
            item.noUseGraphic = true;
            item.channel = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5.75f;
            item.UseSound = SoundID.Item73;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<VehemenceHoldout>();
            item.shootSpeed = 16f;

            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.value = CalamityGlobalItem.Rarity15BuyPrice;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(25, 25);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Item v = player.ActiveItem();
            int chargeTime = 2 * v.useAnimation;
            Projectile.NewProjectile(position, Vector2.Zero, type, damage, knockBack, player.whoAmI, 0f, chargeTime);
            return false;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;
    }
}
