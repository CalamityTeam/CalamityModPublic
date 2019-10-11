using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.SunkenSea
{
    public class Poseidon : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Poseidon");
            Tooltip.SetDefault("Casts a poseidon typhoon");
        }

        public override void SetDefaults()
        {
            item.damage = 58;
            item.magic = true;
            item.mana = 15;
            item.width = 28;
            item.height = 32;
            item.useTime = 22;
            item.useAnimation = 22;
            item.useStyle = 5;
            item.noMelee = true;
            item.knockBack = 6f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.UseSound = SoundID.Item84;
            item.rare = 5;
            item.autoReuse = true;
            item.shoot = mod.ProjectileType("PoseidonTyphoon");
            item.shootSpeed = 10f;
        }
    }
}
