using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons.Melee
{
    public class PrismaticBreaker : ModItem
    {
        internal static readonly Color[] colors = new Color[]
        {
            new Color(255, 0, 0, 50), //Red
            new Color(255, 128, 0, 50), //Orange
            new Color(255, 255, 0, 50), //Yellow
            new Color(128, 255, 0, 50), //Lime
            new Color(0, 255, 0, 50), //Green
            new Color(0, 255, 128, 50), //Turquoise
            new Color(0, 255, 255, 50), //Cyan
            new Color(0, 128, 255, 50), //Light Blue
            new Color(0, 0, 255, 50), //Blue
            new Color(128, 0, 255, 50), //Purple
            new Color(255, 0, 255, 50), //Fuschia
            new Color(255, 0, 128, 50) //Hot Pink
        };

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismatic Breaker");
            Tooltip.SetDefault("Seems to belong to a certain magical girl. Radiates with intense cosmic energy.\n" +
                "Fire to charge for a powerful rainbow laser\n" +
                "Right click to instead swing the sword and fire rainbow colored waves\n" +
                "The sword is boosted by both melee and ranged damage");
            Item.staff[Item.type] = true;
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.damage = 662;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = Item.useAnimation = 13;
            Item.useTurn = true;
            Item.DamageType = DamageClass.Melee;
            Item.knockBack = 7f;
            Item.UseSound = SoundID.Item1;
            Item.autoReuse = true;
            Item.width = 50;
            Item.height = 50;
            Item.shoot = ModContent.ProjectileType<PrismaticBeam>();
            Item.shootSpeed = 14f;
            Item.value = CalamityGlobalItem.Rarity14BuyPrice;
            Item.Calamity().customRarity = CalamityRarity.DarkBlue;
            Item.Calamity().donorItem = true;
        }

        // Terraria seems to really dislike high crit values in SetDefaults
        public override void ModifyWeaponCrit(Player player, ref float crit) => crit += 8;

        //Cancel out normal melee damage boosts and replace it with the average of melee and ranged damage boosts
        //all damage boosts should still apply
        public override void ModifyWeaponDamage(Player player, ref StatModifier damage)
        {
            damage = player.GetDamage<MeleeDamageClass>().CombineWith(player.GetDamage<RangedDamageClass>()) * 0.5f;
        }

        public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
        {
            Item.DrawItemGlowmaskSingleFrame(spriteBatch, rotation, ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/PrismaticBreakerGlow").Value);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X, velocity.Y, ModContent.ProjectileType<PrismaticWave>(), damage, knockback, player.whoAmI);
            }
            else
            {
                Projectile.NewProjectile(source, position.X, position.Y, velocity.X * 0.5f, velocity.Y * 0.5f, type, damage, knockback, player.whoAmI);
            }
            return false;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                Item.UseSound = SoundID.Item1;
                Item.useStyle = ItemUseStyleID.Swing;
                Item.useTurn = true;
                Item.autoReuse = true;
                Item.noMelee = false;
                Item.channel = false;
            }
            else
            {
                Item.UseSound = SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Item/CrystylCharge");
                Item.useStyle = ItemUseStyleID.Shoot;
                Item.useTurn = false;
                Item.autoReuse = false;
                Item.noMelee = true;
                Item.channel = true;
            }
            return base.CanUseItem(player);
        }

        public override void MeleeEffects(Player player, Rectangle hitbox)
        {
            if (Main.rand.NextBool(4))
            {
                Dust rainbow = Main.dust[Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 267, 0f, 0f, 50, Main.rand.Next(colors), 0.8f)];
                rainbow.noGravity = true;
            }
        }

        public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 300);
            target.AddBuff(BuffID.Daybreak, 300);
        }

        public override void OnHitPvp(Player player, Player target, int damage, bool crit)
        {
            target.AddBuff(ModContent.BuffType<Nightwither>(), 300);
            target.AddBuff(BuffID.Daybreak, 300);
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CosmicRainbow>().
                AddIngredient<SolsticeClaymore>().
                AddIngredient<BarofLife>(3).
                AddIngredient<CosmiliteBar>(8).
                AddIngredient<EndothermicEnergy>(20).
                AddTile(ModContent.TileType<CosmicAnvil>()).
                Register();
        }
    }
}
