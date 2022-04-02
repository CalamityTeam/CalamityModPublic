using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Summon
{
    public class EnergyStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Staff");
            Tooltip.SetDefault("Summons a profaned energy turret to fight for you");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 128;
            item.summon = true;
            item.sentry = true;
            item.mana = 10;
            item.width = 66;
            item.height = 68;
            item.useTime = item.useAnimation = 14;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = CalamityGlobalItem.Rarity11BuyPrice;
            item.rare = ItemRarityID.Purple;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<ProfanedEnergy>();
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            //CalamityUtils.OnlyOneSentry(player, type);
            Projectile.NewProjectile(Main.MouseWorld, Vector2.Zero, type, damage, knockBack, player.whoAmI, 16f);
            player.UpdateMaxTurrets();
            return false;
        }
    }
}
