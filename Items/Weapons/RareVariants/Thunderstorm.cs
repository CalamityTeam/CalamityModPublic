using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class Thunderstorm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thunderstorm");
            Tooltip.SetDefault("Make it rain");
        }

        public override void SetDefaults()
        {
            item.damage = 360;
            item.mana = 50;
            item.magic = true;
            item.width = 48;
            item.height = 22;
            item.useTime = 24;
            item.useAnimation = 24;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 2f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/PlasmaBlast");
            item.autoReuse = true;
            item.shootSpeed = 6f;
            item.shoot = ModContent.ProjectileType<Projectiles.ThunderstormShot>();
            item.Calamity().postMoonLordRarity = 22;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }
    }
}
