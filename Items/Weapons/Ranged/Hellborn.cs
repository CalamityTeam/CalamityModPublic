using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Items.Weapons.Ranged
{
    public class Hellborn : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hellborn");
            Tooltip.SetDefault("Converts all bullets to explosive rounds\n" +
                "Enemies that touch the gun while it's being fired take massive damage");
        }

        public override void SetDefaults()
        {
            item.damage = 21;
            item.ranged = true;
            item.width = 50;
            item.height = 24;
            item.useTime = 9;
            item.useAnimation = 9;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.knockBack = 1f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = ItemRarityID.Yellow;
            item.UseSound = SoundID.Item11;
            item.autoReuse = true;
            item.shoot = ProjectileID.PurificationPowder;
            item.shootSpeed = 14f;
            item.useAmmo = AmmoID.Bullet;
            item.Calamity().customRarity = CalamityRarity.RareVariant;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int index = 0; index < 3; ++index)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-15, 16) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-15, 16) * 0.05f;
                int bullet = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ProjectileID.ExplosiveBullet, damage, knockBack, player.whoAmI, 0f, 0f);
				Main.projectile[bullet].usesLocalNPCImmunity = true;
				Main.projectile[bullet].localNPCHitCooldown = 10;
            }
            return false;
        }

        public override void ModifyHitNPC(Player player, NPC target, ref int damage, ref float knockBack, ref bool crit)
        {
            damage *= 15;
        }

        public override void ModifyHitPvp(Player player, Player target, ref int damage, ref bool crit)
        {
            damage *= 15;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 360);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 360);
        }
    }
}
