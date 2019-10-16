using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class BlackHawkRemote : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Black Hawk Remote");
            Tooltip.SetDefault("Summons a Black Hawk jet to fight for you");
        }

        public override void SetDefaults()
        {
            item.damage = 30;
            item.mana = 10;
            item.width = 46;
            item.height = 28;
            item.useTime = 36;
            item.useAnimation = 36;
            item.useStyle = 4;
            item.noMelee = true;
            item.knockBack = 1f;
            item.value = Item.buyPrice(0, 12, 0, 0);
            item.UseSound = SoundID.Item15; //phaseblade sound effect
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<BlackHawkSummon>();
            item.shootSpeed = 10f;
            item.summon = true;
            item.rare = 4;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                position = Main.MouseWorld;
                speedX = 0;
                speedY = 0;
                Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, player.whoAmI);
            }
            return false;
        }

        public override bool UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                player.MinionNPCTargetAim();
            }
            return base.UseItem(player);
        }
    }
}
