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
        }

        public override void SetDefaults()
        {
            item.width = 44;
            item.damage = 50;
            item.melee = true;
            item.useAnimation = 18;
            item.useTime = 18;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 4.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 54;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = ItemRarityID.Pink;
            item.shoot = ProjectileID.RubyBolt;
            item.shootSpeed = 11f;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            type = Utils.SelectRandom(Main.rand, new int[]
            {
                ProjectileID.RubyBolt,
                ProjectileID.SapphireBolt,
                ProjectileID.AmethystBolt
            });
            for (int projectiles = 0; projectiles <= 3; projectiles++)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-30, 31) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-30, 31) * 0.05f;
                int proj = Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, (int)(damage * 0.6), knockBack, Main.myPlayer);
                if (proj.WithinBounds(Main.maxProjectiles))
                {
                    Main.projectile[proj].Calamity().forceMelee = true;
                    Main.projectile[proj].penetrate = 1;
                }
            }
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<VerstaltiteBar>(), 9);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 73);
            }
        }
    }
}
