using Terraria.DataStructures;
using CalamityMod.Buffs.StatBuffs;
using CalamityMod.Items.Materials;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class TrueTyrantYharimsUltisword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Tyrant's Ultisword");
            Tooltip.SetDefault("Fires blazing, hyper, and sunlight blades\n" +
                "Gives the player the tyrant's fury buff on enemy hits\n" +
                "This buff increases melee damage by 30% and melee crit chance by 10%");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 102;
            Item.damage = 112;
            Item.DamageType = DamageClass.Melee;
            Item.useAnimation = 18;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 18;
            Item.useTurn = true;
            Item.knockBack = 7.5f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.height = 102;
            Item.value = CalamityGlobalItem.Rarity12BuyPrice;
            Item.rare = ItemRarityID.Purple;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
            Item.shoot = ModContent.ProjectileType<BlazingPhantomBlade>();
            Item.shootSpeed = 12f;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            switch (Main.rand.Next(3))
            {
                case 0:
                    type = ModContent.ProjectileType<BlazingPhantomBlade>();
                    break;
                case 1:
                    type = ModContent.ProjectileType<HyperBlade>();
                    break;
                case 2:
                    type = ModContent.ProjectileType<SunlightBlade>();
                    break;
                default:
                    break;
            }
            int proj = Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, type, damage, knockback, Main.myPlayer);
            Main.projectile[proj].extraUpdates += 1;
            return false;
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(5))
            {
                int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 106);
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            player.AddBuff(ModContent.BuffType<TyrantsFury>(), 300);
            target.AddBuff(BuffID.Poisoned, 300);
            target.AddBuff(BuffID.Venom, 150);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<TyrantsFury>(), 300);
            target.AddBuff(BuffID.Poisoned, 300);
            target.AddBuff(BuffID.Venom, 150);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<TyrantYharimsUltisword>().
                AddIngredient<CoreofCalamity>().
                AddIngredient<UeliaceBar>(15).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
