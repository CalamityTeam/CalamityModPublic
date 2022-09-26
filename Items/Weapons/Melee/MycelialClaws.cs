using CalamityMod.Buffs.StatBuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class MycelialClaws : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mycelial Claws");
            Tooltip.SetDefault("Grants the Mushy buff for 6 seconds on enemy hits");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.damage = 21;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 7;
            Item.useTurn = true;
            Item.knockBack = 3.75f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 24;
            Item.value = CalamityGlobalItem.Rarity2BuyPrice;
            Item.rare = ItemRarityID.Green;
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit) => player.AddBuff(ModContent.BuffType<Mushy>(), 360);

        public override void OnHitPvp(Player player, Player target, int damage, bool crit) => player.AddBuff(ModContent.BuffType<Mushy>(), 360);

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 56);
            }
        }
    }
}
