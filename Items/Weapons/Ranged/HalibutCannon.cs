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
					(NPC.downedBoss1 ? 1f : 0f) + // 2
					(NPC.downedBoss2 ? 1f : 0f) + // 3
					(NPC.downedBoss3 ? 1f : 0f) + // 4
					(Main.hardMode ? 1f : 0f) + // 5
					(NPC.downedMechBossAny ? 1f : 0f) + // 6
					(NPC.downedPlantBoss ? 1f : 0f) + // 7
					(NPC.downedGolemBoss ? 1f : 0f) + // 8
					(NPC.downedAncientCultist ? 1f : 0f) + // 9
					(NPC.downedMoonlord ? 6f : 0f) + // 15
					(CalamityWorld.downedProvidence ? 3f : 0f) + // 18
					(CalamityWorld.downedPolterghast ? 6f : 0f) + // 24
					(CalamityWorld.downedDoG ? 12f : 0f) + // 36
					(CalamityWorld.downedYharon ? 20f : 0f); // 56
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
