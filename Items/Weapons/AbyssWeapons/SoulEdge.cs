using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.AbyssWeapons
{
    public class SoulEdge : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Soul Edge");
            Tooltip.SetDefault("Fires the ghastly souls of long-deceased abyss dwellers");
        }

        public override void SetDefaults()
        {
            item.width = 88;
            item.damage = 365;
            item.melee = true;
            item.useAnimation = 18;
            item.useStyle = 1;
            item.useTime = 18;
            item.useTurn = true;
            item.knockBack = 5.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 88;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shoot = mod.ProjectileType("GhastlySoulLarge");
            item.shootSpeed = 12f;
            item.Calamity().postMoonLordRarity = 13;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            float SpeedA = speedX;
            float SpeedB = speedY;
            int num6 = Main.rand.Next(2, 4);
            for (int index = 0; index < num6; ++index)
            {
                float num7 = speedX;
                float num8 = speedY;
                float SpeedX = speedX + (float)Main.rand.Next(-40, 41) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-40, 41) * 0.05f;
                float ai1 = (Main.rand.NextFloat() + 0.5f);
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, Main.rand.Next(type, type + 3), (int)((double)damage * 0.75), knockBack, player.whoAmI, 0.0f, ai1);
            }
            return false;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(mod.BuffType("CrushDepth"), 600);
        }
    }
}
