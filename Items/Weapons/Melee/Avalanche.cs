using Terraria.DataStructures;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Avalanche : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Avalanche");
            Tooltip.SetDefault("Spawns ice bombs that explode after 3 seconds into ice shards on hit");
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 64;
            Item.scale = 1.5f;
            Item.damage = 100;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 35;
            Item.useTime = 35;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 7.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            var source = player.GetProjectileSource_Item(Item);
            int totalProjectiles = 4;
            float radians = MathHelper.TwoPi / totalProjectiles;
            int type = ModContent.ProjectileType<IceBombFriendly>();
            float velocity = 4f;
            double angleA = radians * 0.5;
            double angleB = MathHelper.ToRadians(90f) - angleA;
            float velocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
            Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity) : new Vector2(-velocityX, -velocity);
            for (int k = 0; k < totalProjectiles; k++)
            {
                Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                Projectile.NewProjectile(source, target.Center, vector255, type, (int)(Item.damage * (player.GetDamage<GenericDamageClass>().Additive + player.GetDamage(DamageClass.Melee) - 1f)), knockback, Main.myPlayer);
            }
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            var source = player.GetProjectileSource_Item(Item);
            int totalProjectiles = 4;
            float radians = MathHelper.TwoPi / totalProjectiles;
            int type = ModContent.ProjectileType<IceBombFriendly>();
            float velocity = 4f;
            double angleA = radians * 0.5;
            double angleB = MathHelper.ToRadians(90f) - angleA;
            float velocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
            Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity) : new Vector2(-velocityX, -velocity);
            for (int k = 0; k < totalProjectiles; k++)
            {
                Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                Projectile.NewProjectile(source, target.Center, vector255, type, (int)(Item.damage * (player.GetDamage<GenericDamageClass>().Additive + player.GetDamage(DamageClass.Melee) - 1f)), 0f, Main.myPlayer);
            }
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int num250 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 67, (float)(player.direction * 2), 0f, 150, default, 1.5f);
                Main.dust[num250].velocity *= 0.2f;
            }
        }
    }
}
