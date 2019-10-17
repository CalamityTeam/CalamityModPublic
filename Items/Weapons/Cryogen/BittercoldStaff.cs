using Microsoft.Xna.Framework;
using System;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class BittercoldStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bittercold Staff");
            Tooltip.SetDefault("Fires a spread of homing ice spikes");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 40;
            item.magic = true;
            item.mana = 17;
            item.width = 52;
            item.height = 52;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 5;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = SoundID.Item46;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Projectiles.IceSpike>();
            item.shootSpeed = 10f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "CryoBar", 8);
            recipe.AddTile(TileID.IceMachine);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            Vector2 vector2 = player.RotatedRelativePoint(player.MountedCenter, true);
            int i = Main.myPlayer;
            float num72 = item.shootSpeed;
            int num73 = damage;
            float num74 = knockBack;
            float num78 = (float)Main.mouseX + Main.screenPosition.X - vector2.X;
            float num79 = (float)Main.mouseY + Main.screenPosition.Y - vector2.Y;
            float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
            int num130 = 2;
            if (Main.rand.NextBool(3))
            {
                num130++;
            }
            if (Main.rand.NextBool(4))
            {
                num130++;
            }
            if (Main.rand.NextBool(5))
            {
                num130++;
            }
            for (int num131 = 0; num131 < num130; num131++)
            {
                float num132 = num78;
                float num133 = num79;
                float num134 = 0.05f * (float)num131;
                num132 += (float)Main.rand.Next(-100, 100) * num134;
                num133 += (float)Main.rand.Next(-100, 100) * num134;
                num80 = (float)Math.Sqrt((double)(num132 * num132 + num133 * num133));
                num80 = num72 / num80;
                num132 *= num80;
                num133 *= num80;
                float x2 = vector2.X;
                float y2 = vector2.Y;
                Projectile.NewProjectile(x2, y2, num132, num133, ModContent.ProjectileType<Projectiles.IceSpike>(), num73, num74, i, 0f, 0f);
            }
            return false;
        }
    }
}
