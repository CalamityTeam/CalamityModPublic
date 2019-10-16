using Terraria; using CalamityMod.Projectiles; using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class TheReaper : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Reaper");
            Tooltip.SetDefault("Slice 'n dice");
        }

        public override void SafeSetDefaults()
        {
            item.width = 80;
            item.damage = 325;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.useAnimation = 22;
            item.useTime = 22;
            item.useStyle = 1;
            item.knockBack = 4f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 64;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<ReaperProjectile>();
            item.shootSpeed = 20f;
            item.Calamity().rogue = true;
            item.Calamity().postMoonLordRarity = 22;
        }
    }
}
