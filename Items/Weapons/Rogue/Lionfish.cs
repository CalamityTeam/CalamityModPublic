using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class Lionfish : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lionfish");
            Tooltip.SetDefault("Sticks to enemies and injects a potent toxin");
        }

        public override void SafeSetDefaults()
        {
            item.width = 40;
            item.damage = 54;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 22;
            item.useStyle = 1;
            item.useTime = 22;
            item.knockBack = 2.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 40;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
            item.shoot = ModContent.ProjectileType<Projectiles.Lionfish>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }
    }
}
