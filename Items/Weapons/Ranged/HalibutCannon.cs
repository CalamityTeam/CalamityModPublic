using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class HalibutCannon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Halibut Cannon");
            Tooltip.SetDefault("Becomes more powerful as you progress\n" +
                "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.damage = 1;
            item.ranged = true;
            item.width = 108;
            item.height = 54;
            item.useTime = 10;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = 10;
            item.noMelee = true;
            item.knockBack = 1f;
            item.value = Item.buyPrice(1, 0, 0, 0);
            item.UseSound = SoundID.Item38;
            item.autoReuse = true;
            item.shoot = ProjectileID.Bullet;
            item.shootSpeed = 12f;
            item.useAmmo = AmmoID.Bullet;
        }

        public override Vector2? HoldoutOffset() => new Vector2(-15, 0);

        public override void ModifyWeaponDamage(Player player, ref float add, ref float mult, ref float flat)
		{
			float damageMult = 0f + // 1
					(NPC.downedBoss1 ? 0.5f : 0f) +
					(NPC.downedBoss2 ? 0.5f : 0f) +
					(NPC.downedBoss3 ? 0.5f : 0f) +
					(Main.hardMode ? 0.5f : 0f) +
					(NPC.downedMechBossAny ? 0.5f : 0f) +
					(NPC.downedPlantBoss ? 0.5f : 0f) +
					(NPC.downedGolemBoss ? 0.5f : 0f) +
					(NPC.downedAncientCultist ? 0.5f : 0f) +
					(NPC.downedMoonlord ? 3f : 0f) +
					(CalamityWorld.downedProvidence ? 1.5f : 0f) +
					(CalamityWorld.downedPolterghast ? 3f : 0f) +
					(CalamityWorld.downedDoG ? 6f : 0f) +
					(CalamityWorld.downedYharon ? 10f : 0f);
			mult += damageMult;
		}

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            int bulletAmt = Main.rand.Next(25, 36);
            for (int index = 0; index < bulletAmt; ++index)
            {
                float SpeedX = speedX + Main.rand.Next(-10, 11) * 0.05f;
                float SpeedY = speedY + Main.rand.Next(-10, 11) * 0.05f;
                int shot = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
                Main.projectile[shot].timeLeft = 180;
            }
            return false;
        }
    }
}
