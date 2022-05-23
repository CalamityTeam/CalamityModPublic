using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
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
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = BaseDamage;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 41;
            Item.width = 44;
            Item.height = 44;
            Item.useTime = Item.useAnimation = 43;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.noMelee = true;
            Item.knockBack = 5.75f;
            Item.UseSound = SoundID.Item73;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<VehemenceHoldout>();
            Item.shootSpeed = 16f;

            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(25, 25);

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Item v = player.ActiveItem();
            int chargeTime = 2 * v.useAnimation;
            Projectile.NewProjectile(source, position, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, chargeTime);
            return false;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
    }
}
