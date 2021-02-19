using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class HalibutCannon : ModItem
    {
        internal const float DropChance = 1E-5f; // 1 in 100,000
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Halibut Cannon");
            Tooltip.SetDefault("Becomes more powerful as you progress\n" +
                "(Yes, it's still overpowered)\n" +
                "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.damage = 12;
            item.ranged = true;
            item.width = 108;
            item.height = 54;
            item.useTime = 10;
            item.useAnimation = 20;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.rare = ItemRarityID.Red;
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
            float damageMult = 0f +
                    (NPC.downedPlantBoss ? 0.1f : 0f) +
                    (NPC.downedGolemBoss ? 0.1f : 0f) +
                    (NPC.downedAncientCultist ? 0.2f : 0f) +
                    (NPC.downedMoonlord ? 1f : 0f) +
                    (CalamityWorld.downedProvidence ? 0.15f : 0f) +
                    (CalamityWorld.downedPolterghast ? 0.3f : 0f) +
                    (CalamityWorld.downedDoG ? 0.6f : 0f) +
                    (CalamityWorld.downedYharon ? 1f : 0f);
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
                Main.projectile[shot].timeLeft = 120;
            }
            return false;
        }
    }
}
