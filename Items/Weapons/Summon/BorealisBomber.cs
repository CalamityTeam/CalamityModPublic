using CalamityMod.Projectiles.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Summon
{
    public class BorealisBomber : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Borealis Bomber");
            Tooltip.SetDefault("Summons aureus bombers to fight for you\n" +
            "Aureus bombers explode on enemy impact\n" +
            "Does not consume minion slots");
            Item.staff[item.type] = true;
        }

        public override void SetDefaults()
        {
            item.damage = 35;
            item.mana = 10;
            item.width = 48;
            item.height = 56;
            item.useTime = item.useAnimation = 19;
            item.useStyle = ItemUseStyleID.HoldingOut;
            item.noMelee = true;
            item.knockBack = 5f;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = ItemRarityID.Lime;
            item.UseSound = SoundID.Item44;
            //item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<AureusBomber>();
            item.shootSpeed = 10f;
            item.summon = true;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(15, 15);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld;
                speedX = 0;
                speedY = 0;
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, Main.myPlayer);
            }
            return false;
        }
    }
}
