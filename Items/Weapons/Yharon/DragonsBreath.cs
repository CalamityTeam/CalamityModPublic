using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class DragonsBreath : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragon's Breath");
            Tooltip.SetDefault("80% chance to not consume ammo\n" +
                "Shoots a spread of exploding fire bullets");
        }

        public override void SetDefaults()
        {
            item.damage = 320;
            item.ranged = true;
            item.width = 64;
            item.height = 28;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 6.5f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item36;
            item.autoReuse = true;
            item.shoot = 10;
            item.shootSpeed = 12f;
            item.useAmmo = 97;
            item.Calamity().postMoonLordRarity = 14;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-5, 10);
        }

        public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            for (int i = 0; i < 13; i++)
            {
                float SpeedX = speedX + (float)Main.rand.Next(-20, 21) * 0.05f;
                float SpeedY = speedY + (float)Main.rand.Next(-20, 21) * 0.05f;
                Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, ModContent.ProjectileType<DragonBurst>(), damage, knockBack, player.whoAmI, 0f, 0f);
            }
            return false;
        }

        public override bool ConsumeAmmo(Player player)
        {
            if (Main.rand.Next(0, 100) < 80)
                return false;
            return true;
        }
    }
}
