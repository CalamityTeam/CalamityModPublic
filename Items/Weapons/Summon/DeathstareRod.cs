using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class DeathstareRod : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deathstare Rod");
            Tooltip.SetDefault("Summons an eye above your head that watches you and shoots at enemies\n" +
            "There can only be one eye");
        }

        public override void SetDefaults()
        {
            item.damage = 33;
            item.mana = 10;
            item.width = item.height = 42;
            item.useTime = item.useAnimation = 35;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = CalamityGlobalItem.Rarity2BuyPrice;
            item.rare = ItemRarityID.Green;
            item.UseSound = SoundID.NPCHit8;
            item.shoot = ModContent.ProjectileType<DeathstareEyeball>();
            item.shootSpeed = 10f;
            item.summon = true;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[item.shoot] <= 0;
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityUtils.KillShootProjectiles(true, type, player);
            Projectile.NewProjectile(player.Center, Vector2.Zero, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
