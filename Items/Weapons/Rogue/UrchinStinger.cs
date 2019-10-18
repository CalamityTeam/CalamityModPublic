using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Rogue
{
    public class UrchinStinger : RogueWeapon
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Urchin Stinger");
        }

        public override void SafeSetDefaults()
        {
            item.width = 10;
            item.damage = 17;
            item.noMelee = true;
            item.consumable = true;
            item.noUseGraphic = true;
            item.useAnimation = 14;
            item.useStyle = 1;
            item.useTime = 14;
            item.knockBack = 1.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 26;
            item.maxStack = 999;
            item.value = 200;
            item.rare = 1;
            item.shoot = ModContent.ProjectileType<Projectiles.UrchinStinger>();
            item.shootSpeed = 12f;
            item.Calamity().rogue = true;
        }
    }
}
