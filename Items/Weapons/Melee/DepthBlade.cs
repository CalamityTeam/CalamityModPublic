using CalamityMod.Buffs.DamageOverTime;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class DepthBlade : ModItem
    {
        private static int HammerPower = 50;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Depth Crusher");
            Tooltip.SetDefault("Hitting enemies will inflict the crush depth debuff\n" +
                "The lower the enemies' defense, the more damage they take from this debuff\n" +
                "Right click to use without hammering down walls");
        }

        public override void SetDefaults()
        {
            item.width = 56;
            item.height = 50;
            item.damage = 36;
            item.melee = true;
            item.useAnimation = 22;
            item.useTime = 22;
            item.useTurn = true;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.knockBack = 5.25f;
            item.UseSound = SoundID.Item1;
            item.autoReuse = true;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = ItemRarityID.Green;
            item.hammer = HammerPower;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.hammer = 0;
            }
            else
            {
                item.hammer = HammerPower;
            }
            return base.CanUseItem(player);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(3))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 33);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<CrushDepth>(), 300);
        }
    }
}
