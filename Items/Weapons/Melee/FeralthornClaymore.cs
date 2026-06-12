using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Systems.Collections;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class FeralthornClaymore : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";
        public override void SetStaticDefaults()
        {
            CalamityItemSets.ExtraDebuffTooltip_Enemy[Type] = [BuffID.Venom];
        }
        public override void SetDefaults()
        {
            Item.width = 68;
            Item.height = 66;
            Item.damage = 109;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 13;
            Item.useTurn = true;
            Item.knockBack = 7.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.value = CalamityGlobalItem.RarityLimeBuyPrice;
            Item.rare = ItemRarityID.Lime;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, DustID.JungleSpore);
        }

        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 300);

            var source = player.GetSource_ItemUse(Item);

            Projectile.NewProjectile(source, target.Center.X, target.Center.Y, Main.rand.NextFloat(-18f, 18f), Main.rand.NextFloat(-18f, 18f), ModContent.ProjectileType<ThornBase>(), (int)(Item.damage * 0.5), 0f, Main.myPlayer);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Venom, 300);

            var source = player.GetSource_ItemUse(Item);

            Projectile.NewProjectile(source, target.Center.X, target.Center.Y, Main.rand.NextFloat(-18f, 18f), Main.rand.NextFloat(-18f, 18f), ModContent.ProjectileType<ThornBase>(), (int)(Item.damage * 0.5), 0f, Main.myPlayer);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PerennialBar>(12).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
