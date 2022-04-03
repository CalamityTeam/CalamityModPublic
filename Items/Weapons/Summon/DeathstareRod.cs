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
            Item.damage = 33;
            Item.mana = 10;
            Item.width = Item.height = 42;
            Item.useTime = Item.useAnimation = 35;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.noMelee = true;
            Item.knockBack = 2f;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
            Item.UseSound = SoundID.NPCHit8;
            Item.shoot = ModContent.ProjectileType<DeathstareEyeball>();
            Item.shootSpeed = 10f;
            Item.DamageType = DamageClass.Summon;
        }
        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            CalamityUtils.KillShootProjectiles(true, type, player);
            Projectile.NewProjectile(player.Center, Vector2.Zero, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
