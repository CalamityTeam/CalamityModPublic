using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class AngryChickenStaff : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Yharon's Kindle Staff");
            Tooltip.SetDefault("Summons the Son of Yharon to fight for you\n" +
                               "The dragon increases your life regen, defense, and movement speed while summoned\n" +
                               "Requires 4 minion slots to use");
        }

        public override void SetDefaults()
        {
            item.mana = 50;
            item.damage = 160;
            item.useStyle = 1;
            item.width = 32;
            item.height = 32;
            item.useTime = 36;
            item.useAnimation = 36;
            item.noMelee = true;
            item.knockBack = 7f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/FlareSound");
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<Projectiles.SonOfYharon>();
            item.shootSpeed = 10f;
            item.summon = true;
            item.Calamity().postMoonLordRarity = 14;
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
