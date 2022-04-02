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
            Tooltip.SetDefault("Slap Hand but better\n" +
            "Sends enemies straight to the stars at the speed of light");
        }

        public override void SetDefaults()
        {
            item.width = 46;
            item.height = 58;
            item.damage = 69;
            item.scale = 1.75f;
            item.knockBack = 20f; //This number doesn't mean anything, but it's not 9001f because that caused bugs.
            item.useAnimation = item.useTime = 12;
            item.melee = true;
            item.useTurn = true;
            item.autoReuse = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.UseSound = SoundID.Item1;
            item.rare = ItemRarityID.Lime;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
        }

        // Boosting crit in SetDefaults along with knockback seemed to severely inflate the reforging price. Guaranteed crits for more knockback.
        public override void GetWeaponCrit(Player player, ref int crit) => crit = 100;

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
            YeetEnemies(player, target, crit);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 300);
        }

        private void YeetEnemies(Player player, NPC target, bool crit)
        {
            // Manually doing knockback because it is capped in vanilla. This lets Titan Arm reach its full potential. =D
            // This is modified vanilla code from StrikeNPC method in NPC.cs
            // Extra Note: Will cause an out of bounds error on enemies that don't despawn and are affected. See Blue Cultist Archer.
            // Extra (extra) Note: Fails in ModifyHitNPC if the enemy has too much health due to velocity clamping if you don't do enough damage.
            float kbAmt = player.GetWeaponKnockback(item, item.knockBack) * 9001f * target.knockBackResist; //That obligatory over 9000 reference
            if (crit)
                kbAmt *= 1.4f;
            float kbAmtY = target.noGravity ? kbAmt * -0.5f : kbAmt * -0.75f;
            target.velocity.Y += kbAmtY;
            target.velocity.X += kbAmt * player.direction;
        }
    }
}
