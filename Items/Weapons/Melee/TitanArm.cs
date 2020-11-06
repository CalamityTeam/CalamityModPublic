using CalamityMod.Buffs.DamageOverTime;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TitanArm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titan Arm");
            Tooltip.SetDefault("Slap Hand but better");
        }

        public override void SetDefaults()
        {
            item.width = 46;
            item.height = 58;
			item.scale = 1.5f;
            item.damage = 60;
            item.crit += 96; //more knockback huehue
            item.melee = true;
            item.useTurn = true;
            item.useAnimation = 12;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTime = 12;
            item.knockBack = 9001f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 60, 0, 0);
            item.rare = 7;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 120);
        }
    }
}
