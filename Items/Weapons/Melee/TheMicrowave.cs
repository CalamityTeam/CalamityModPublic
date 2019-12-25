using CalamityMod.Projectiles.Melee.Yoyos;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TheMicrowave : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Microwave");
            //Tooltip.SetDefault("MMMMMMMMMMMMMM, 13th letter of the alphabet moments");
            Tooltip.SetDefault("Baking enemies in the astral infection\n\n" +
				"Summons an aura around the yoyo that damages nearby enemies");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.TheEyeOfCthulhu);
            item.damage = 80;
            item.useTime = 22;
            item.useAnimation = 22;
            item.useStyle = 5;
            item.channel = true;
            item.melee = true;
            item.knockBack = 3f;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = 9;
            item.autoReuse = true;
            item.shoot = ModContent.ProjectileType<TheMicrowaveProj>();
            ItemID.Sets.Yoyo[item.type] = true;
        }
    }
}
