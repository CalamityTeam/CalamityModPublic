using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class ThornBlossom : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thorn Blossom");
            Tooltip.SetDefault("Every rose has its thorn");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 45;
            item.magic = true;
            item.mana = 10;
            item.width = 66;
            item.height = 68;
            item.useTime = 23;
            item.useAnimation = 23;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = 9;
            item.UseSound = SoundID.Item109;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<BeamingBolt>();
            item.shootSpeed = 20f;
            item.Calamity().postMoonLordRarity = 22;
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            player.statLife -= 5;
            if (player.statLife <= 0)
            {
                player.KillMe(PlayerDeathReason.ByOther(10), 1000.0, 0, false);
            }
            for (int index = 0; index < 3; ++index)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-120, 121) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-120, 121) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX * 1.5f, SpeedY * 1.5f, 150, (int)((double)damage * 1.5), knockBack, player.whoAmI, 0f, 0f);
            }
            Projectile.NewProjectile(position.X, position.Y, speedX * 0.66f, speedY * 0.66f, type, damage, knockBack, player.whoAmI, 0f, 0f);
            return false;
        }
    }
}
