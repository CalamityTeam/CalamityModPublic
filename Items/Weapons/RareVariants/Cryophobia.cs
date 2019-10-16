using Microsoft.Xna.Framework;
using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class Cryophobia : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryophobia");
            Tooltip.SetDefault("Chill");
        }

        public override void SetDefaults()
        {
            item.damage = 24;
            item.magic = true;
            item.mana = 18;
            item.width = 56;
            item.height = 34;
            item.useTime = 22;
            item.useAnimation = 22;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 1.5f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.UseSound = SoundID.Item117;
            item.autoReuse = true;
            item.shootSpeed = 12f;
            item.shoot = ModContent.ProjectileType<CryoBlast>();
            item.Calamity().postMoonLordRarity = 22;
        }

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-10, 0);
        }
    }
}
