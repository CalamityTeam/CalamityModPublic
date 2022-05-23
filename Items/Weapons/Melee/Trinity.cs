using Terraria.DataStructures;
using Terraria.DataStructures;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class Trinity : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Trinity");
            Tooltip.SetDefault("Fires a spread of energy bolts");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 44;
            Item.damage = 50;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 18;
            Item.useTime = 18;
            Item.useTurn = true;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.knockBack = 4.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 54;
            Item.value = Item.buyPrice(0, 36, 0, 0);
            Item.rare = ItemRarityID.Pink;
            Item.shoot = ProjectileID.RubyBolt;
            Item.shootSpeed = 11f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            type = Utils.SelectRandom(Main.rand, new int[]
            {
                ProjectileID.RubyBolt,
                ProjectileID.SapphireBolt,
                ProjectileID.AmethystBolt
            });
            for (int projectiles = 0; projectiles <= 3; projectiles++)
            {
                float SpeedX = velocity.X + (float)Main.rand.Next(-30, 31) * 0.05f;
                float SpeedY = velocity.Y + (float)Main.rand.Next(-30, 31) * 0.05f;
                int proj = Projectile.NewProjectile(source, position.X, position.Y, SpeedX, SpeedY, type, (int)(damage * 0.6), knockback, Main.myPlayer);
                if (proj.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[proj].Calamity().forceMelee = true;
                    Main.projectile[proj].penetrate = 1;
                }
            }
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 73);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VerstaltiteBar>(9).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
