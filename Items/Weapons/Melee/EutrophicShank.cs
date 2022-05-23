using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class EutrophicShank : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eutrophic Shank");
            Tooltip.SetDefault("Shoots electric sparks");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Thrust;
            Item.useTurn = false;
            Item.useAnimation = 14;
            Item.useTime = 14;
            Item.width = 42;
            Item.height = 38;
            Item.damage = 35;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 4f;
            Item.UseSound = SoundID.Item1;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 2, 0, 0);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ModContent.ProjectileType<EutrophicSpark>();
            Item.shootSpeed = 10f;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 206, 0f, 0f, 100, default, 1f);
            }
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            int num6 = Main.rand.Next(1, 2);
            for (int index = 0; index < num6; ++index)
            {
                float SpeedX = velocity.X + (float)Main.rand.Next(-60, 61) * 0.05f;
                float SpeedY = velocity.Y + (float)Main.rand.Next(-60, 61) * 0.05f;
                int projectile = Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, (int)((double)damage * 0.4), knockback, player.whoAmI, 0.0f, 0.0f);
            }
            return false;
        }
    }
}
