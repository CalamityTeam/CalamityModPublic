using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Magic
{
	public class Vehemenc : ModItem
    {
        public const int VehemenceSkullDamage = 1250;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vehemence");
            Tooltip.SetDefault("Casts an intense energy blast\n" +
                               "If an enemy has full HP it will inflict Demon Flames for an extremely long time");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 1412;
            item.magic = true;
            item.mana = 41;
            item.width = 44;
            item.height = 44;
            item.useTime = item.useAnimation = 50;
            item.noUseGraphic = true;
            item.channel = true;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5.75f;
            item.value = Item.buyPrice(1, 0, 0, 0);
            item.rare = ItemRarityID.Red;
            item.UseSound = SoundID.Item73;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<VehemenceHoldout>();
            item.shootSpeed = 16f;
        }

        public override Vector2? HoldoutOrigin() => new Vector2(25, 25);

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position, Vector2.Zero, type, damage, knockBack, player.whoAmI);
            player.AddBuff(BuffID.ManaSickness, 600, true);
            return false;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;
    }
}
