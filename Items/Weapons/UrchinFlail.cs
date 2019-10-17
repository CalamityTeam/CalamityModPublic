using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Items
{
    public class UrchinFlail : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Urchin Flail");
            Tooltip.SetDefault("Launch an urchin ball, which shoots a spike on contact with an enemy");
        }

        public override void SetDefaults()
        {
            item.damage = 20;
            item.melee = true;
            item.width = 44;
            item.height = 36;
            item.useTime = 25;
            item.useAnimation = 25;
            item.useStyle = 5;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.knockBack = 6f;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.channel = true;
            item.shoot = ModContent.ProjectileType<UrchinBall>();
            item.shootSpeed = 12f;
        }
    }
}
