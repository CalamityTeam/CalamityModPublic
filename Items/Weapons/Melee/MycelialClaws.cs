using CalamityMod.Buffs.StatBuffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class MycelialClaws : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 22;
            Item.damage = 28;
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

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone) => player.AddBuff(ModContent.BuffType<Mushy>(), 360);

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo) => player.AddBuff(ModContent.BuffType<Mushy>(), 360);

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 56);
            }
        }
    }
}
