using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class TeardropCleaver : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Teardrop Cleaver");
            Tooltip.SetDefault("Makes your enemies cry");
        }

        public override void SetDefaults()
        {
            item.width = 52;
            item.damage = 18;
            item.melee = true;
            item.useAnimation = 24;
            item.useStyle = 1;
            item.useTime = 24;
            item.useTurn = true;
            item.knockBack = 4.5f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.height = 56;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 2;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(mod.BuffType("TemporalSadness"), 120);
        }
    }
}
