using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing
{
    public class PolarisParrotfish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Polaris Parrotfish");
            Tooltip.SetDefault("It carries the mark of the Northern Star");
            Item.staff[item.type] = true; //so it doesn't look weird af when holding it
        }

        public override void SetDefaults()
        {
            item.damage = 40;
            item.ranged = true;
            item.width = 38;
            item.height = 34;
            item.useTime = 20;
            item.useAnimation = 20;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 2.25f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/LaserCannon"); //pew pew
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("PolarStar");
            item.shootSpeed = 15f;
        }

        public override Vector2? HoldoutOrigin()
        {
            return new Vector2(10, 10);
        }
    }
}
