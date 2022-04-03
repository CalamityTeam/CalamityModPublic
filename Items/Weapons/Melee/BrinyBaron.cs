using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class BrinyBaron : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Briny Baron");
            Tooltip.SetDefault("Striking an enemy with the blade causes a briny typhoon to appear\n" +
                "Right click to fire a razorwind aqua blade");
        }

        public override void SetDefaults()
        {
            Item.damage = 124;
            Item.knockBack = 4f;
            Item.useAnimation = Item.useTime = 15;
            Item.DamageType = DamageClass.Melee;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.shootSpeed = 4f;

            Item.width = 100;
            Item.height = 102;
            Item.scale = 1.5f;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;

            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.noMelee = true;
                Item.noUseGraphic = true;
                Item.UseSound = SoundID.Item84;
                Item.shoot = ModContent.ProjectileType<Razorwind>();
            }
            else
            {
                Item.noMelee = false;
                Item.noUseGraphic = false;
                Item.UseSound = SoundID.Item1;
                Item.shoot = ProjectileID.None;
            }
            return base.CanUseItem(player);
        }

        public override float UseTimeMultiplier    (Player player)
        {
            if (player.altFunctionUse == 2)
                return 1f;
            return 0.75f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Projectile.NewProjectile(position.X, position.Y, speedX, speedY, ModContent.ProjectileType<Razorwind>(), (int)(damage * 0.43), knockBack, player.whoAmI);
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 187, 0f, 0f, 100, new Color(53, Main.DiscoG, 255));
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            if (crit)
                damage /= 2;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<BrinySpout>()] == 0)
                Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<BrinyTyphoonBubble>(), damage, knockback, player.whoAmI);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            if (crit)
                damage /= 2;

            if (player.ownedProjectileCounts[ModContent.ProjectileType<BrinySpout>()] == 0)
                Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<BrinyTyphoonBubble>(), damage, Item.knockBack, player.whoAmI);
        }
    }
}
