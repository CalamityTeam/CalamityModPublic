using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    [LegacyName("TyrantYharimsUltisword")]
    public class BlightedCleaver : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetDefaults()
        {
            Item.width = 88;
            Item.damage = 64;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 26;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 26;
            Item.useTurn = true;
            Item.knockBack = 5.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 88;
            Item.value = CalamityGlobalItem.Rarity8BuyPrice;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<BlazingPhantomBlade>();
            Item.shootSpeed = 12f;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 106);
            }
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            player.AddBuff(ModContent.BuffType<BrutalCarnage>(), 180);
            target.AddBuff(BuffID.Venom, 150);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            player.AddBuff(ModContent.BuffType<BrutalCarnage>(), 180);
            target.AddBuff(BuffID.Venom, 150);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TrueCausticEdge>().
                AddIngredient(ItemID.BrokenHeroSword).
                AddIngredient(ItemID.FlaskofVenom, 5).
                AddIngredient(ItemID.ChlorophyteBar, 15).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
