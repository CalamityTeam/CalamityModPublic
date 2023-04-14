using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class FeralthornClaymore : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Feralthorn Claymore");
            // Tooltip.SetDefault("Summons thorns on enemy hits");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 68;
            Item.damage = 80;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 13;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 13;
            Item.useTurn = true;
            Item.knockBack = 7.25f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 66;
            Item.value = CalamityGlobalItem.Rarity7BuyPrice;
            Item.rare = ItemRarityID.Lime;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
                Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 44);
        }

        public override void ModifyHitNPC(Player player, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.CritDamage *= 0.5f;
        }
        public override void OnHitNPC(Player player, NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Venom, 300);

            var source = player.GetSource_ItemUse(Item);

            Projectile.NewProjectile(source, player.position.X + 40f + Main.rand.Next(0, 151), player.position.Y + 36f, 0f, -18f, ModContent.ProjectileType<ThornBase>(), (int)(Item.damage * 0.25), 0f, Main.myPlayer);
            Projectile.NewProjectile(source, player.position.X - 40f + Main.rand.Next(-150, 1), player.position.Y + 36f, 0f, -18f, ModContent.ProjectileType<ThornBase>(), (int)(Item.damage * 0.25), 0f, Main.myPlayer);
        }

        public override void OnHitPvp(Player player, Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(BuffID.Venom, 300);

            var source = player.GetSource_ItemUse(Item);

            Projectile.NewProjectile(source, player.position.X + 40f + Main.rand.Next(0, 151), player.position.Y + 36f, 0f, -18f, ModContent.ProjectileType<ThornBase>(), (int)(Item.damage * 0.25), 0f, Main.myPlayer);
            Projectile.NewProjectile(source, player.position.X - 40f + Main.rand.Next(-150, 1), player.position.Y + 36f, 0f, -18f, ModContent.ProjectileType<ThornBase>(), (int)(Item.damage * 0.25), 0f, Main.myPlayer);
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
