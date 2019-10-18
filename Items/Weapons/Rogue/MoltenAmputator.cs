using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class MoltenAmputator : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Molten Amputator");
            Tooltip.SetDefault("Throws a scythe that emits molten globs on enemy hits");
        }

        public override void SafeSetDefaults()
        {
            item.width = 60;
            item.damage = 250;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.autoReuse = true;
            item.useAnimation = 18;
            item.useStyle = 1;
            item.useTime = 18;
            item.knockBack = 9f;
            item.UseSound = SoundID.Item1;
            item.height = 60;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.shoot = ModContent.ProjectileType<Projectiles.MoltenAmputator>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
            item.Calamity().postMoonLordRarity = 12;
        }
    }
}
